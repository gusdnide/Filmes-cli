using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using filmes_cli.Library;
namespace filmes_cli
{
    public partial class Form1 : Form
    {
        #region Global Vars
        cFilmes FilmeAPI;
        cConfig Config;
        #endregion

        #region Methods
        private void LoadPosts(cPost[] posts) {
            this.Invoke((MethodInvoker)(() => pnResultado.Controls.Clear()));
            int w = 180;
            int h = 270;
            int MaxItensX = (pnResultado.Size.Width / w);
            for(int i = 0; i < posts.Length; i++)
            {
                if (posts[i] == null || i >= Config.MaxItens)
                    return;


                int posY = 1;
                if (i > 0)
                    posY += ((i / MaxItensX) * h);

                int posX = 0;
                if (i >= MaxItensX)
                    posX = (w * (i - (MaxItensX * (i / MaxItensX))));
                else
                    posX = h * i;

                //PictureBox Imagem
                PictureBox pb = new PictureBox();
                pb.Size = new Size(w, h - 15);
                pb.SizeMode = PictureBoxSizeMode.StretchImage;                
                pb.Cursor = Cursors.Hand;
                pb.Location = new Point(posX, posY);
                pb.ImageLocation = posts[i].image;
                pb.ErrorImage = Properties.Resources.teste;
                pb.Tag = posts[i];
                pb.MouseClick += (object sender, MouseEventArgs e) =>
                {                
                    LoadFilme(((cPost)((PictureBox)sender).Tag));
                };

                //Label Nome
                Label lb = new Label();
                lb.Size = new Size(w, 15);
                lb.Font = new Font(FontFamily.GenericSansSerif, 10);
                lb.Text = posts[i].name;
                lb.Location = new Point(posX +5, posY + (h - 14));
                lb.MouseClick += (object sender, MouseEventArgs e) =>
                {
                    LoadFilme(((cPost)((PictureBox)sender).Tag));
                };

                this.Invoke((MethodInvoker) (() => pnResultado.Controls.Add(pb)));
                this.Invoke((MethodInvoker)(() => pnResultado.Controls.Add(lb)));
            }
        }
        private void Buscar()
        {
            this.Invoke((MethodInvoker)(() => pnSearch.Visible = false));
            this.Invoke((MethodInvoker)(() => pnWait.Visible = true));
            cPost[] listPost = FilmeAPI.searchFilme(txtPesquisa.Text);
            if (listPost != null && listPost.Count() > 0)
            {
                LoadPosts(listPost);
            }
            else
            {
                MessageBox.Show("Impossivel localizar este filme, tente novamente mais tarde!");
            }
            this.Invoke((MethodInvoker)(() => pnSearch.Visible = true));
            this.Invoke((MethodInvoker)(() => pnWait.Visible = false));
        }
        private void LoadFilme(cPost post)
        {
            string MagnetLink = FilmeAPI.getMagnet(post.link);
            FilmeAPI.startVLC(MagnetLink);
        }
        #endregion

        #region MethodsConfig
        public void LoadConfig()
        {
            if (!File.Exists("config.json"))
            {
                NewConfig();
            }
            else
            {
                try
                {
                    Config = JsonConvert.DeserializeObject<cConfig>(File.ReadAllText("config.json"));
                    
                }
                catch
                {
                    NewConfig();
                }
            }
            if (Config == null || Config.MaxItens > 10)
                NewConfig();
           
        }
        public void NewConfig()
        {
            Config = new cConfig();
            File.WriteAllText("config.json", JsonConvert.SerializeObject(Config));
        }
       
        public void SaveConfig()
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(Config));
        }
        #endregion

        #region FormEvents
        public Form1()
        {
            InitializeComponent();
            LoadConfig();
            AllowDrop = true;
            pnDrDrop.AllowDrop = true;
            tabPage2.AllowDrop = true;
            iTalk_ThemeContainer1.AllowDrop = true;
            FilmeAPI = new cFilmes("http://localhost/filmes-cli/");
        }
        private void pnDrDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
           
        }
        private void pnDrDrop_DragOver(object sender, DragEventArgs e)
        {
            lbMaxItens.Text = "Drop leave left  mouse button";
        }
        private void pnDrDrop_DragDrop(object sender, DragEventArgs e)
        {
            lbMaxItens.Text = "Drag and Drop here";
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            FileInfo f = new FileInfo(files[0]);
            if(f.Extension == ".torrent")
            {
                FilmeAPI.startFileVLC(files[0]);
            }
        }
        private void iTalk_TrackBar1_ValueChanged()
        {
            lbMaxItens.Text = $"{trkbMaxItens.Value}";
            Config.MaxItens = trkbMaxItens.Value;
            SaveConfig();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            pnDrDrop.Location = new Point((tabPage2.Size.Width / 2) - pnDrDrop.Width / 2, (tabPage2.Size.Height / 2) - pnDrDrop.Height / 2);
        }
        private void brnProcura_Click(object sender, EventArgs e)
        {
            new Thread(Buscar).Start();            
        }      
        private void tabPage2_Resize(object sender, EventArgs e)
        {
            //Center panel
            pnDrDrop.Location = new Point((tabPage2.Size.Width / 2) - pnDrDrop.Width/2, (tabPage2.Size.Height / 2) - pnDrDrop.Height / 2);
        }




        #endregion

       
    }
}
