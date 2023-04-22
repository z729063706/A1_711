using shardLib;
using System.Diagnostics;

namespace client
{
    public partial class clientForm : Form
    {
        private List<Files> F;
        public int cachePort = Configer.cachePort;
        public string cacheIP = Configer.cacheIP;
        public string clientPath = Configer.ClientPath;

        public clientForm()
        {
            InitializeComponent();
            if (!Directory.Exists(Configer.ClientPath))
            {
                Directory.CreateDirectory(Configer.ClientPath);
            }
        }
        //设定listview1中数据
        private void Listview1_Load(object sender, EventArgs e)
        {
            List<Files> F = SocketUtils.GetFileList(cacheIP, cachePort);
            this.F = F;
            this.listView1.Items.Clear();
            this.listView1.Columns.Clear();
            this.imageList1.Images.Clear();
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 200);
            listView1.SmallImageList = imgList;
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.Columns.Add("Name", 250, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Size", 120, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Update", 160, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Download Status", 120, HorizontalAlignment.Left);
            this.listView1.View = System.Windows.Forms.View.Details;
            btn.Visible = false;
            btn.Text = "Download";
            btn.Click += this.button_Click;
            this.listView1.Controls.Add(btn);
            this.btn.Size = new Size(120,30);
            this.listView1.BeginUpdate();
            for (int i = 0; i < F.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.ImageIndex = i;
                lvi.Text = F[i].Name;                
                lvi.SubItems.Add(Files.sizeToString(F[i].Size));
                lvi.SubItems.Add(F[i].Update.ToString());
                if (File.Exists(Configer.ClientPath + @"\" + F[i].Name))
                {
                    F[i].DownloadStatus = "Downloaded";
                }
                lvi.SubItems.Add(F[i].DownloadStatus);
                imageList1.Images.Add(Files.ByteArrayToImage(F[i].Tb));
                this.listView1.Items.Add(lvi);
            }

            this.listView1.EndUpdate();
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                this.btn.Location = new Point(this.listView1.SelectedItems[0].SubItems[3].Bounds.Left, this.listView1.SelectedItems[0].SubItems[3].Bounds.Top);
                this.btn.Visible = true;
                if (this.listView1.SelectedItems[0].SubItems[3].Text == "Downloaded")
                {
                    this.btn.Text = "Open";
                }
                else
                {
                    this.btn.Text = "Download";
                }
            }
            
        }
        private void button_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.listView1.SelectedItems[0].Index;
            Files selectedFile = this.F[selectedIndex];
            if (this.btn.Text == "Download")
            {
                List<string> Splits = ClientUtils.GetSplites(selectedFile.Path);
                ClientUtils.DownloadFile(Splits, selectedFile.Path);
                this.listView1.SelectedItems[0].SubItems[3].Text = "Downloaded";
                this.btn.Text = "Open";
            }
            else
            {
                var openProcess = new ProcessStartInfo(Configer.ClientPath + @"\" + selectedFile.Name)
                {
                    UseShellExecute = true
                };
                Process.Start(openProcess);
            }
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Listview1_Load(sender, e);
        }
    }
}