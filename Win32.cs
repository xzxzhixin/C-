/**
*┌──────────────────────────────────────────────────────────────┐
*│　描   述：                                                    
*│　作   者：致心
*│　创建时间：1/31/2019 4:07:39 PM                             
*└──────────────────────────────────────────────────────────────┘
*┌──────────────────────────────────────────────────────────────┐
*│　命名空间: myclass.util                                   
*│　类   名：use32                                      
*└──────────────────────────────────────────────────────────────┘
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace myclass.util
{
    public static class Win32
    {
        /// <summary>
        ///  查找顶级窗口句柄
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="winName">标题</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string winName);

        //[DllImport("user32.dll", EntryPoint = "GetWindow")]
        //public static extern IntPtr GetWindow(IntPtr hwnd,int wCmd);
        //public const int GW_CHILD = 5;//定义窗体关系

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageTimeout(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, uint fuFlage, uint timeout, IntPtr result);

        ///// <summary>
        ///// 获得焦点窗口的句柄
        ///// </summary>
        ///// <returns></returns>
        //[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        //private static extern IntPtr GetForegroundWindow();

        ///// <summary>
        ///// 对获得焦点的窗口最大化判断进行封装
        ///// </summary>
        ///// <returns></returns>
        //public static bool ForeGroundWindowIsMax()
        //{
        //    return IsZoomed(GetForegroundWindow());
        //}

        /// <summary>
        /// 判断桌面是否存在有最大化窗口
        /// </summary>
        /// <returns></returns>
        public static bool ExistWindowFormMax()
        {
            bool find = false;
            EnumWindows((hwnd, lParam) =>
            {
                if (hwnd != FindWindow("ApplicationFrameWindow", "电影和电视"))
                {
                    if (IsZoomed(hwnd))
                    {
                        find = true;
                        return false;
                    }
                }
                return true;
            }, IntPtr.Zero);
            return find;
        }

        /// <summary>
        /// 判断窗口是否最大化
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool IsZoomed(IntPtr hWnd);

        ///// <summary>
        ///// 判断窗口是否最小化
        ///// </summary>
        ///// <param name="hWnd"></param>
        ///// <returns></returns>
        //[DllImport("user32.dll")]
        //public static extern bool IsIconic(IntPtr hWnd);

        //[DllImport("user32.dll",EntryPoint ="SendMessage")]
        //public static extern IntPtr SendMessage(IntPtr hWnd,uint Msg,IntPtr wParam,IntPtr lParam);

        /// <summary>
        /// 该函数枚举所有屏幕上的顶层窗口,并将窗口句柄传送给应用程序定义的回调函数,如果 callback 返回的是true，则会继续枚举，否则就会终止枚举
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc callback, IntPtr lParam);

        ///// <summary>
        ///// 该函数枚举所有window上的子窗口,并将窗口句柄传送给应用程序定义的回调函数,如果 callback 返回的是true，则会继续枚举，否则就会终止枚举
        ///// </summary>
        ///// <param name="window"></param>
        ///// <param name="callback"></param>
        ///// <param name="i"></param>
        ///// <returns></returns>
        //[DllImport("user32.dll")]
        //private static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, IntPtr i);

        /// <summary>
        /// 自动接收所有顶层窗口
        /// </summary>
        /// <param name="hwnd">找到的窗口句柄</param>
        /// <param name="lParam">EnumWindows 传给的参数; 因为它是指针, 可传入, 但一般用作传出数据</param>
        /// <returns></returns>
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        ///    查找子窗口句柄
        /// </summary>
        /// <param name="hwndParent">要查找窗口的父句柄</param>
        /// <param name="hwndChildAfter">从这个窗口后开始查找，是 0, 查找从 hwndParent 的第一个子窗口开始</param>
        /// <param name="className">窗口类名</param>
        /// <param name="winName">窗口标题</param>
        /// <returns>找到返回窗口句柄，没找到返回0</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string className, string winName);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        /// <summary>
        /// 改变指定子窗口的父窗口
        /// </summary>
        /// <param name="hwndChild">子窗口句柄</param>
        /// <param name="newParent">新的父窗口句柄 如果该参数是NULL，则桌面窗口就成为新的父窗口</param>
        /// <returns>如果函数成功，返回值为子窗口的原父窗口句柄；如果函数失败，返回值为NULL</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hwndChild, IntPtr newParent);  // 在同一个程序里使用，但是对于桌面可以使用

        //[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        ////import the native SetWindowPos function--导入SetWindowPos这个函数
        //public static extern bool SetWindowPos(IntPtr hWnd, // window handle
        //                                    int hWndInsertAfter, // placement-order handle
        //                                    int X, // horizontal position
        //                                    int Y, // vertical position
        //                                    int cx, // width
        //                                    int cy, // height
        //                                    uint uFlags); // window positioning flags
        //public const int HWND_BOTTOM = 0x1;
        //public const uint SWP_NOSIZE = 0x1;
        //public const uint SWP_NOMOVE = 0x2;
        //public const uint SWP_SHOWWINDOW = 0x40;
        //public const int SE_SHUTDOWN_PRIVILEGE = 0x13;

        ///// <summary>
        ///// 保存消息发送状态
        ///// </summary>
        //private static bool sentMessageOut = false;

        /// <summary>
        /// 设置窗口为桌面
        /// </summary>
        /// <param name="Handle">窗口的句柄</param>
        public static bool SetToDeskBackground(IntPtr Handle)
        {
            bool succeed = false;
            bool find = false;
            int chances = 10;
            while (!find && chances != 0)
            {
                try
                {
                    if (Environment.OSVersion.Version.Major < 6)
                    {
                        IntPtr hWndNewParent = FindWindow("Progman", "Program Manager");
                        if (hWndNewParent == IntPtr.Zero)
                        {
                            Console.WriteLine("获取句柄失败");
                            return succeed;
                        }
                        if (IntPtr.Zero != SetParent(Handle, hWndNewParent))
                        {
                            succeed = true;
                        }
                    }
                    else
                    {
                        // 找到桌面窗口句柄
                        IntPtr hWnd = FindWindow("Progman", "Program Manager");
                        // 判断是否获取到
                        if (hWnd == IntPtr.Zero)
                        {
                            Console.WriteLine("获取句柄失败");
                            return succeed;
                        }
                        // 向 Program Manager 窗口发送 0x52c 的一个消息，超时设置为0x3e8（1秒）,消息会生成两个WorkerW 顶级窗口
                        SendMessageTimeout(hWnd, 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 0x3e8, IntPtr.Zero);
                        // 遍历顶级窗口
                        EnumWindows((hwnd, lParam) =>
                        {
                            // 枚举不包含“SHELLDLL_DefView”这个的 WorkerW 窗口 隐藏掉
                            if (FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                            {
                                // 找到当前 WorkerW 窗口的后一个 WorkerW 窗口。 
                                IntPtr tempHwnd = FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);
                                if (tempHwnd != IntPtr.Zero)
                                {
                                    Console.WriteLine("查找成功！！！！:句柄："+ Convert.ToString((int)tempHwnd, 16));                            
                                    find = true;
                                    //显示这个窗口
                                    ShowWindow(tempHwnd, 8);
                                    int chance = 10;
                                    // 设置为桌面窗口的子窗体
                                    while (chance != 0)
                                    {
                                        if (IntPtr.Zero != SetParent(Handle, tempHwnd))
                                        {
                                            Console.WriteLine("设置成功！！！！");
                                            succeed = true;
                                            return true;
                                        }
                                        System.Threading.Thread.Sleep(100);
                                        chance--;
                                    }
                                }
                                return false;
                            }
                            return true;
                        }, IntPtr.Zero);
                        // 再向 Program Manager 窗口发送 0x52c 的一个消息，显示窗口
                        SendMessageTimeout(hWnd, 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 0x3e8, IntPtr.Zero);
                    }
                }
                catch (ApplicationException exx)
                {
                    Console.WriteLine(exx.HelpLink);
                }
                chances--;
            }            
            return succeed;
        }

        ///// <summary>
        ///// 查找窗口是不是桌面窗口的子窗口
        ///// </summary>
        ///// <param name="handle">窗口的句柄</param>
        ///// <returns></returns>
        //public static bool FindDeskChildForm(IntPtr handle)
        //{
        //    bool isfind = false;
        //    // 找到桌面窗口句柄
        //    IntPtr hWnd = FindWindow("Progman", "Program Manager");
        //    // 枚举子窗口的句柄
        //    EnumChildWindows(hWnd, (hwnd, lParam) =>
        //    {
        //        // 存在一个子窗口的句柄==handle退出枚举
        //        if (hwnd == handle)
        //        {
        //            isfind = true;
        //            return false;
        //        }
        //        return true;
        //    }, IntPtr.Zero);
        //    return isfind;
        //}

        ///// <summary>
        ///// 保存worker窗口句柄
        ///// </summary>
        //private static IntPtr tempHwnd;

        ///// <summary>
        ///// 释放内存
        ///// </summary>
        ///// <param name="process"></param>
        ///// <param name="minSize"></param>
        ///// <param name="maxSize"></param>
        ///// <returns></returns>
        //[DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        //public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        ///// <summary>
        ///// 通过句柄判断窗口是否被隐藏
        ///// </summary>
        ///// <param name="hWnd"></param>
        ///// <returns>隐藏返回false</returns>
        //[DllImport("user32.dll")]
        //private static extern bool IsWindowVisible(IntPtr hWnd);

        ///// <summary>
        ///// 判断worker窗口是否隐藏
        ///// </summary>
        ///// <returns></returns>
        //public static bool WorkerIsNotHide()
        //{
        //    return IsWindowVisible(tempHwnd);
        //}

        ///// <summary>
        ///// 将窗口定义为分层窗体
        ///// </summary>
        ///// <param name="hwnd"></param>
        ///// <param name="nIndex"></param>
        ///// <param name="dwNewLong"></param>
        ///// <returns></returns>
        //[DllImport("user32", EntryPoint = "SetWindowLong")]
        //private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        ///// <summary>
        ///// 设置透明
        ///// </summary>
        ///// <param name="hwnd"></param>
        ///// <param name="crKey"></param>
        ///// <param name="bAlpha"></param>
        ///// <param name="dwFlags"></param>
        ///// <returns></returns>
        //[DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        //private static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

        ///// <summary>
        ///// 设置窗体为鼠标可以穿透
        ///// </summary>
        ///// <param name="handle"></param>
        //public static void SetFormIsPierce(IntPtr handle)
        //{
        //    SetWindowLong(handle, -20, 0x80000);
        //    SetLayeredWindowAttributes(handle, 65280, 255, 1);
        //}
    }
}
