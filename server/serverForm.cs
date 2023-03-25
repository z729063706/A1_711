using shardLib;
using System.Windows.Forms;

namespace server
{
   
    public partial class serverForm : Form
    {
        public bool isServerRunning = false;
        public int serverPort = Configer.serverPort;
        public static string ServerPath = Configer.ServerPath;
        public static List<Files> F = null;
        public static DateTime T = DateTime.Now;
        public serverForm()
        {
            InitializeComponent();
        }
        public static List<Files> GetFiles()
        {
            return F;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (isServerRunning)
            {
                try
                {
                    SocketUtils.StopServer(serverPort);
                    isServerRunning = false;
                    button1.Text = "Start Server";
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
                    SocketUtils.StartServer(serverPort);
                    button2_Click(sender, e);
                    isServerRunning = true;
                    button1.Text = "Stop Server";
                    label2.Text = "Running";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return;
        }//start or stop

        private void button2_Click(object sender, EventArgs e)
        {
            T = DateTime.Now;
            F = Files.getFiles(ServerPath);
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 200);
            listView1.SmallImageList = imgList;
            this.listView1.Clear();
            this.listView1.Columns.Clear();
            this.imageList1.Images.Clear();
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.Columns.Add("Name", 250, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Size", 120, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Update", 160, HorizontalAlignment.Left);
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.BeginUpdate();            
            for (int i = 0; i < F.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.ImageIndex = i;
                lvi.Text = F[i].Name;
                lvi.SubItems.Add(Files.sizeToString(F[i].Size));
                lvi.SubItems.Add(F[i].Update.ToString());
                imageList1.Images.Add(Files.ByteArrayToImage(F[i].Tb));
                this.listView1.Items.Add(lvi);
            }

            this.listView1.EndUpdate();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void serverForm_Load(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select a File";
            openFileDialog1.ShowDialog();
            openFileDialog1.InitialDirectory = "C:\\";
            string path = openFileDialog1.FileName;
            string filename = path.Split('\\')[path.Split('\\').Length - 1];
            string newpath = ServerPath + "\\" + filename;
            File.Copy(path, newpath);
            button2_Click(sender, e);
        }
    }
}