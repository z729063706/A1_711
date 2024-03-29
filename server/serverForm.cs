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
        private static serverForm myFormInstance;
        public static serverForm GetInstance()
        {
            if (myFormInstance == null || myFormInstance.IsDisposed)
            {
                myFormInstance = new serverForm();
            }
            return myFormInstance;
        }
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
                    ServerUtils.StopServer(serverPort);
                    isServerRunning = false;
                    button1.Text = "Start Server";
                    label2.Text = "Stopped";
                    addLog("Server Stopped");
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
                    ServerUtils.StartServer(serverPort);
                    button2_Click(sender, e);
                    isServerRunning = true;
                    button1.Text = "Stop Server";
                    label2.Text = "Running";
                    addLog("Server Started");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return;
        }//start or stop
        public void addLog(string message)
        {
            LogUtil.Log("server",message);
            textBox1.AppendText(message+ Environment.NewLine);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            addLog("Refreshing File List");
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

            ServerUtils.SpiltInit();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void serverForm_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(Configer.ServerPath))
            {
                Directory.CreateDirectory(Configer.ServerPath);                
            }
            if (!Directory.Exists(Configer.ServerPath + @"\splits"))
            {
                Directory.CreateDirectory(Configer.ServerPath + @"\splits");
            }
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
            if (File.Exists(newpath))
            {
                addLog("File exists, will be overwritten");
                File.Delete(newpath);
                ServerUtils.deleteSplite(newpath);
            }
            File.Copy(path, newpath);
            button2_Click(sender, e);
            addLog("File Uploading and Splitting");
        }
    }
}