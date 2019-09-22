using myclass.movie;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 动态壁纸
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            vlcPlayer.SetRenderWindow((int)this.Handle);
            vlcPlayer.SetVolume(0);
        }

        public VlcPlayer vlcPlayer = new VlcPlayer();

        private void Form3_Load(object sender, EventArgs e)
        {
            vlcPlayer.SetFullScreen(true);
        }

    }
}
