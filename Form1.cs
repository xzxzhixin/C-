using myclass.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 动态壁纸
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            // 开启双级缓冲，避免界面闪烁
            //SetStyle(
            //         ControlStyles.OptimizedDoubleBuffer
            //         | ControlStyles.ResizeRedraw
            //         | ControlStyles.Selectable
            //         | ControlStyles.AllPaintingInWmPaint
            //         | ControlStyles.UserPaint
            //         | ControlStyles.SupportsTransparentBackColor,
            //         true);
            InitializeComponent();
            form2 = new Form2();
            label2.Text = "音量：" + trackBar1.Value.ToString();
            form2.vlcPlayer.SetVolume(trackBar1.Value);
        }
        

        Form2 form2;
        Form3 form3;

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            // 因为最小化不是隐藏窗口，所以为了在最小化以后双击托盘可以看到而实现
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        // 背景播放
        private void button1_Click(object sender, EventArgs e)
        {
            if (FlagClass.fileName == null)
            {
                MessageBox.Show("你没有预览文件");
                return;
            }
            this.Text = "正在拷贝文件，请耐心等待";
            FlagClass.fileName = copyFileToVideo(FlagClass.fileName);
            if (FlagClass.fileName != null)
            {
                form3.vlcPlayer.Stop();
                form3.vlcPlayer = null;
                form3.Close();
                form3 = null;
                Console.WriteLine("背景播放");
                this.Text = "动态壁纸  正在播放：" + Path.GetFileNameWithoutExtension(FlagClass.fileName); //这个就是获取文件名的
                form2.vlcPlayer.PlayFile(FlagClass.fileName);
                //form2.vlcPlayer.Play();
                checkBox3.Checked = false;
            }
            else
            {
                this.Text = "文件拷贝错误，请重试";
            }
            if (!FlagClass.form2show)
            {
                FlagClass.form2show = true;
                form2.Show();
            }
        }

        // 拷贝文件到video文件夹
        private String copyFileToVideo(String fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists)
            {

                // 获得路径目录
                string path = Application.StartupPath.ToString() + "\\video";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (fileName.Equals(path + "\\" + Path.GetFileName(fileName)))
                {
                    return fileName;
                }
                Console.WriteLine(path + "\\" + Path.GetFileName(fileName));
                fileInfo.CopyTo(path + "\\" + Path.GetFileName(fileName), true);
                panel1.Controls.Clear();
                createButton();
                return path + "\\" + Path.GetFileName(fileName);
            }
            return null;
        }


        // 选取视频
        private void button3_Click(object sender, EventArgs e)
        {
            // 取消设置为壁纸
            if (!FlagClass.setWallpaper)
            {
                button3.Text = "预览";
                form3.vlcPlayer.Stop();
                form3.vlcPlayer = null;
                form3.Close();
                form3 = null;
                form2.vlcPlayer.Play();
                checkBox3.Checked = false;
                FlagClass.setWallpaper = true;
                return;
            }
            // 创建对话框
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            // 设置过滤器，只允许 .wmv 和 mp4 格式的视频。
            openFileDialog1.Filter = "视频(*.wmv;*.mp4;*.mov)|*.wmv;*.mp4;*.mov";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FlagClass.fileName = openFileDialog1.FileName;
                if (form3 == null)
                {
                    form3 = new Form3();
                    //form3.MdiParent = this;
                    Win32.SetParent(form3.Handle, this.Handle);
                    form3.Show();
                    Console.WriteLine("newform3");
                }
                form3.vlcPlayer.PlayFile(FlagClass.fileName);
                form3.vlcPlayer.Play();
                if (FlagClass.form2show && form3 != null)
                {
                    Console.WriteLine("Pause");
                    form2.vlcPlayer.Pause();
                    checkBox3.Checked = true;
                }
                button3.Text = "取消设置壁纸";
                FlagClass.setWallpaper = false;
            }            
        }

        // 调节音量
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label2.Text = "音量：" + trackBar1.Value.ToString();
            form2.vlcPlayer.SetVolume(trackBar1.Value);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = new beginProgram().MeIsAutoStart();
            //timer1.Start();
            if (getFileNamePath() != null)
            {
                form2.vlcPlayer.PlayFile(getFileNamePath());    // 获取一个随机的播放视频进行播放
                //form2.vlcPlayer.Play();
                form2.Show();
                FlagClass.form2show = true;
                createButton();
            }
        }

        /// <summary>
        /// 列表循环播放
        /// </summary>
        private void ListLoopPlay()
        {
            if (((int)form2.vlcPlayer.Duration() - 1) <= (int)form2.vlcPlayer.GetPlayTime())
            {
                form2.vlcPlayer.PlayFile(getFileNamePath());    // 获取一个随机的播放视频进行播放
                //form2.vlcPlayer.Play();
            }
        }

        /// <summary>
        /// 创建动态按钮并添加事件
        /// </summary>
        private void createButton()
        {
            // 添加路径
            string path = Application.StartupPath.ToString() + "\\video";
            if (Directory.Exists(path))
            {
                // 获取文件夹里面所有文件的并且带有条件
                var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4") || s.EndsWith(".mov") || s.EndsWith(".wmv"));
                if (files.Count() != 0)
                {
                    int i = 0;  // 控制按钮的x轴
                    int j = 0;  // 控制按钮的y轴
                    foreach (var file in files)
                    {
                        string name = Path.GetFileNameWithoutExtension(file); //这个就是获取文件名的
                        Button btn = new Button();
                        // 添加btn样式属性
                        btn.Size = new Size(80, 60);
                        // 按钮设置为透明
                        //btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                        btn.BackColor = System.Drawing.Color.Transparent;
                        if ((i * 90 + 90) > panel1.Size.Width)
                        {
                            j++;
                            i = 0;
                        }
                        btn.Location = new Point(i * 90, j * 70);
                        i++;
                        btn.Cursor = System.Windows.Forms.Cursors.Hand;
                        btn.Text = name;
                        btn.MouseClick += (e, a) => createButtonMouseClick(file);
                        // 添加到谁的身上
                        panel1.Controls.Add(btn);
                    }
                }
                else
                {
                    MessageBox.Show("video文件夹为空");
                }
            }
            else
            {
                Directory.CreateDirectory(path);    // 创建路径
            }
        }

        /// <summary>
        /// 动态按钮的点击事件
        /// </summary>
        /// <param name="PathFileName">传入播放视频的完整路径</param>
        private void createButtonMouseClick(string PathFileName)
        {
            if (FlagClass.form2show != true)
            {
                FlagClass.form2show = true;
                form2.Show();
            }
            this.Text = "动态壁纸  正在播放：" + Path.GetFileNameWithoutExtension(PathFileName); //这个就是获取文件名的
            form2.vlcPlayer.PlayFile(PathFileName);
            //form2.vlcPlayer.Play();
            checkBox3.Checked = false;
        }

        /// <summary>
        /// 返回一个随机播放路径
        /// </summary>
        /// <returns></returns>
        private string getFileNamePath()
        {
            /// <summary>
            /// 存放完整路径
            /// </summary>
            List<string> fileNameList = new List<string>();
            // 获得路径目录
            string path = Application.StartupPath.ToString() + "\\video";
            if (Directory.Exists(path))
            {
                // 获取完整路径名字
                var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4") || s.EndsWith(".mov") || s.EndsWith(".wmv"));
                if (files.Count() != 0)
                {
                    foreach (var file in files)
                    {
                        fileNameList.Add(file);
                    }
                }
                else
                {
                    MessageBox.Show("video文件夹为空");
                }
            }
            else
            {
                Directory.CreateDirectory(path);    // 创建路径
                return null;
            }
            Random random = new Random((int)DateTime.Now.Ticks);
            if (fileNameList.Count() == 0)
            {
                return null;
            }
            var t = random.Next(fileNameList.Count());
            // 允许其他线程操作本控件
            this.Invoke(new MethodInvoker(() =>
            {
                this.Text = "动态壁纸  正在播放：" + Path.GetFileNameWithoutExtension(fileNameList[t]); //这个就是获取文件名的
            }));
            return fileNameList[t];
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            new beginProgram().SetMeAutoStart(checkBox1.Checked);
            Console.WriteLine(checkBox1.Checked);
        }

        /// <summary>
        /// 监听窗口大小改变进行相对应的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            // 窗口最小化进行窗口隐藏
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            form2.vlcPlayer.setloop(!checkBox2.Checked);
            StartListLoopPlay();
        }

        /// <summary>
        /// 循环执行判断是否列表循环播放
        /// </summary>
        private void StartListLoopPlay()
        {
            new Task(() =>
            {
                while (checkBox2.Checked)
                {
                    ListLoopPlay();
                    System.Threading.Thread.Sleep(1000);
                }
            }).Start();
        }
        ///// <summary>
        ///// 每过一分钟释放一次内存
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    Win32.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);//释放内存
        //}

        /// <summary>
        /// 控制视频的暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            timeStopAndPlayPause(checkBox3.Checked);
        }

        /// <summary>
        /// 传值判断选项是否需要壁纸暂停
        /// </summary>
        /// <param name="istrue"></param>
        private void timeStopAndPlayPause(bool istrue)
        {
            if (istrue)
            {
                form2.vlcPlayer.Pause();
                //timer1.Stop();
            }
            else
            {
                form2.vlcPlayer.Play();
                //timer1.Start();
            }
        }

        /// <summary>
        /// 开启线程判断激活窗口是否最大化并且暂停壁纸
        /// </summary>
        private void ForeGroundWindowIsMax()
        {
            new Task(() =>
            {
                // 当暂停播放没有打钩和最大化暂停播放打钩时执行
                while (checkBox4.Checked && !checkBox3.Checked)
                {
                    timeStopAndPlayPause(Win32.ExistWindowFormMax());
                    System.Threading.Thread.Sleep(2000);
                }
                Console.WriteLine("退出");
            }).Start();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            ForeGroundWindowIsMax();
        }

        /// <summary>
        /// 显示控件
        /// </summary>
        private void ControlShow()
        {
            panel1.Controls.Clear();
            panel1.Width = 625;
            createButton();
            checkBox1.Show();
            checkBox2.Show();
            //checkBox3.Show();
            //631, 169
            checkBox3.Location = new Point(631, 169);
            checkBox4.Show();
            button1.Show();
            button3.Show();
            //trackBar1.Show();            
        }

        /// <summary>
        /// 隐藏控件
        /// </summary>
        private void ControlHide()
        {
            panel1.Controls.Clear();
            panel1.Width = 105;
            createButton();
            checkBox1.Hide();
            checkBox2.Hide();
            //checkBox3.Hide();
            checkBox3.Location = new Point(105, 0);
            checkBox4.Hide();
            button1.Hide();
            button3.Hide();
            //trackBar1.Hide();
        }

        /// <summary>
        /// 双击用户显示控件和隐藏控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!FlagClass.Form1Panel1IsHide)
            {
                ControlHide();
                FlagClass.Form1Panel1IsHide = true;
            }
            else
            {
                FlagClass.Form1Panel1IsHide = false;
                ControlShow();
            }
        }

        /// <summary>
        /// 鼠标进入panel控件判断为有文件下载完成就重新加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            if (FlagClass.IHaveDownFile)
            {
                FlagClass.IHaveDownFile = false;
                panel1.Controls.Clear();
                createButton();
                MessageBox.Show("列表刷新完成，有视频：" + panel1.Controls.Count + "个");
            }
        }
    }
}
