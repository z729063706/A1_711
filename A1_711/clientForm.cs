using shardLib;

namespace client
{
    public partial class clientForm : Form
    {
        private List<Files> F;
        public int serverPort = 8088;
        public clientForm()
        {
            InitializeComponent();
            Listview1_Load(this, null);
        }
        //设定listview1中数据
        private void Listview1_Load(object sender, EventArgs e)
        {
            List<Files> F = ClientUtils.GetFileList(serverPort);
            this.listView1.Items.Clear();
            this.listView1.Columns.Clear();
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
            this.btn.Size = new Size(120,20);
            this.listView1.BeginUpdate();
            for (int i = 0; i < F.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.ImageIndex = i;
                lvi.Text = F[i].Name;                
                lvi.SubItems.Add(Files.sizeToString(F[i].Size));
                lvi.SubItems.Add(F[i].Update.ToString());
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
                this.listView1.SelectedItems[0].SubItems[3].Text = "Downloading";
                this.btn.Location = new Point(this.listView1.SelectedItems[0].SubItems[3].Bounds.Left, this.listView1.SelectedItems[0].SubItems[3].Bounds.Top);
                this.btn.Visible = true;
            }
            
        }
        private void button_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.listView1.SelectedItems[0].Index;
            Files selectedFile = this.F[selectedIndex];
            MessageBox.Show(selectedFile.Path);
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