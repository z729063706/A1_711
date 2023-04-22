using shardLib;
using System.Windows.Forms;

namespace cache
{
    public partial class cacheForm : Form
    {
        public bool isCacheRunning = false;
        public int cachePort = Configer.cachePort;
        private static cacheForm myFormInstance;
        public int cacheCount = 0;
        public static cacheForm GetInstance()
        {
            if (myFormInstance == null || myFormInstance.IsDisposed)
            {
                myFormInstance = new cacheForm();
            }
            return myFormInstance;
        }
        public cacheForm()
        {
            InitializeComponent();
            myFormInstance = this;
            if (!Directory.Exists(Configer.CachePath))
            {
                Directory.CreateDirectory(Configer.CachePath);
            }
            foreach (string file in Directory.GetFiles(Configer.CachePath))
            {
                addFile(file.ToString().Substring(Configer.CachePath.Length + 1));
                addCount();
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
        public void addLog(string message)
        {
            LogUtil.Log("cache", message);
            textBox1.AppendText(message + Environment.NewLine);
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo(Configer.CachePath);
            directory.Delete(true);
            Directory.CreateDirectory(Configer.CachePath);
            dataGridView1.Rows.Clear();
            addLog("Cache cleared");
            textBox2.Text = "Select Cache to Preview";
            cacheCount = 0;
            label6.Text = cacheCount.ToString();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        public void addFile(string name)
        {
            dataGridView1.Rows.Add(name);
        }
        public void addCount()
        {
            cacheCount = cacheCount + 1;
            label6.Text = cacheCount.ToString();
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int columnIndex = e.ColumnIndex;            
            DataGridViewCell cell = dataGridView1.Rows[rowIndex].Cells[columnIndex];
            string value = cell.Value.ToString();
            string filePath = Configer.CachePath + "\\" + value;
            textBox2.Text = File.ReadAllText(filePath);
        }
    }
}