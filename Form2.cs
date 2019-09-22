using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using myclass.movie;
using myclass.util;

namespace 动态壁纸
{
    public partial class Form2 : Form
    {
        public VlcPlayer vlcPlayer = new VlcPlayer();

        public Form2()
        {
            InitializeComponent();
            this.MaximizedBounds = Screen.PrimaryScreen.Bounds;//在窗体初始化后添加一句代码  
            vlcPlayer.SetRenderWindow((int)this.Handle);
            if (!Win32.SetToDeskBackground(this.Handle))
            {
                MessageBox.Show("发生致命错误，程序退出");
                System.Environment.Exit(0);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            vlcPlayer.SetFullScreen(true);
            vlcPlayer.setloop(true);
            //timer1.Start();
        }

        ///// <summary>
        ///// 判断是否全屏
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    // worker窗口没有隐藏，不是桌面的子窗口   执行
        //    if (Win32.WorkerIsNotHide() || !(Win32.FindDeskChildForm(this.Handle)))
        //    {
        //        SetToDeskBackground();
        //        this.WindowState = FormWindowState.Maximized;
        //        this.MaximizedBounds = Screen.PrimaryScreen.Bounds;//在窗体初始化后添加一句代码
        //    }
        //}
    }
}
