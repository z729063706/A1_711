using shardLib;

namespace cache
{
    public partial class cacheForm : Form
    {
        public bool isCacheRunning = false;
        public int cachePort = Configer.cachePort;
        public cacheForm()
        {
            InitializeComponent();
            if (!Directory.Exists(Configer.CachePath))
            {
                Directory.CreateDirectory(Configer.CachePath);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isCacheRunning)
            {
                try
                {
                    CacheUtils.StopServer(cachePort);
                    isCacheRunning = false;
                    button1.Text = "Start Cache";
                    label2.Text = "Stopped";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    CacheUtils.StartServer(cachePort);
                    isCacheRunning = true;
                    button1.Text = "Stop Cache";
                    label2.Text = "Running";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}