using PCSC;
using PCSC.Exceptions;
using PCSC.Iso7816;


namespace IDCardReaderSample
{
    internal class IDCardReader
    {
        private CancellationTokenSource _cts;
        private Task _pollingTask;
        private string _lastId; //最終読込ID

        public bool IsEnabled { get; private set; } = false;　//有効・無効

        public event Action<string> CardReceived;

        public void Start()
        {
            if (IsEnabled) return;

            IsEnabled = true;
            _lastId = null;
            _cts = new CancellationTokenSource();
            _pollingTask = Task.Run(() => PollLoop(_cts.Token));
        }

        public void Stop()
        {
            IsEnabled = false;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private void PollLoop(CancellationToken token)
        {
            // リーダーの機器情報を取得
            try
            {
                using (var context = ContextFactory.Instance.Establish(SCardScope.System))
                {
                    var readerNames = context.GetReaders();
                    if (readerNames == null || readerNames.Length == 0)
                    {
                        Console.WriteLine("スマートカードリーダが見つかりません");
                        return;
                    }
                    string readerName = readerNames[0];
                    Console.WriteLine("reader: " + readerName);

                    while (!token.IsCancellationRequested)
                    {
                        string result = GetData(readerName);
                        if (result != null)
                        {
                            // 同じカードを載せ続けている間の重複通知を抑止
                            if (result != _lastId)
                            {
                                _lastId = result;
                                Console.WriteLine(result);
                                CardReceived?.Invoke(result);
                            }
                        }
                        else
                        {
                            _lastId = null;
                        }

                    }
                }
            }
            catch (NoServiceException)
            {
                Console.WriteLine("スマートカードリソースマネージャが稼働していません");
            }
        }

        private static string GetData(string readerName)
        {

            try
            {
                using (var context = ContextFactory.Instance.Establish(SCardScope.System))
                {
                    using (var rfidReader = context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any))
                    {
                        // APDUコマンドの作成
                        var apdu = new CommandApdu(IsoCase.Case2Short, rfidReader.Protocol)
                        {
                            CLA = 0xFF,
                            Instruction = InstructionCode.GetData,
                            P1 = 0x00,
                            P2 = 0x00,
                            Le = 0 // We don't know the ID tag size
                        };
                        // 読み取りコマンド送信
                        using (rfidReader.Transaction(SCardReaderDisposition.Leave))
                        {
                            var sendPci = SCardPCI.GetPci(rfidReader.Protocol);
                            var receivePci = new SCardPCI(); // IO returned protocol control information.

                            var receiveBuffer = new byte[256];
                            var command = apdu.ToArray();

                            var bytesReceived = rfidReader.Transmit(
                                sendPci, // Protocol Control Information (T0, T1 or Raw)
                                command, // command APDU
                                command.Length,
                                receivePci, // returning Protocol Control Information
                                receiveBuffer,
                                receiveBuffer.Length); // data buffer

                            var responseApdu = new ResponseApdu(receiveBuffer, bytesReceived, IsoCase.Case2Short, rfidReader.Protocol);
                            if (responseApdu.HasData)
                            {
                                // バイナリ文字列の整形
                                string id = BitConverter.ToString(responseApdu.GetData()).Replace("-", "");
                                Console.WriteLine(id);
                                return id;
                            }
                            else
                            {
                                Console.WriteLine("エラー: このカードではIDを取得できません");
                            }
                        }
                    }
                }
            }
            catch (RemovedCardException)
            {
                Console.WriteLine("エラー: スマートカードが取り外されたため、通信できません");
            }
            catch (PCSCException ex)
            {
                Console.WriteLine("スマートカードエラー; " + ex.Message);
            }

            return null;
        }

    }
}

