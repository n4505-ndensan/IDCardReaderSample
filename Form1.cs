namespace IDCardReaderSample
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            setStatusLabel("初期化中");

            IDCardReader reader = new IDCardReader();
            reader.Start();
            setStatusLabel("カードを待機中");

            reader.CardReceived += (id) =>
            {
                setStatusLabel("カードが読まれました");
                reader.Stop();

                Invoke(new Action(() =>
                {
                    // idを表示する
                    resultLabel.Text = id;
                }));
            };
        }

        private void setStatusLabel(string msg)
        {
            readStatusLabel.Text = msg;
        }
    }
}
