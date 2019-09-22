/**
*┌──────────────────────────────────────────────────────────────┐
*│　描   述：调用驱动类，使用vlc开源编码，进行封装。使用时plugins文件
*|           夹和liblv.dll以及libclcore.dll要放在exe文件目录下面                           
*│　作   者：致心
*│　创建时间：1/30/2019 9:17:37 PM                             
*└──────────────────────────────────────────────────────────────┘
*┌──────────────────────────────────────────────────────────────┐
*│　命名空间: myclass.movie                                   
*│　类   名：VlcPlayer                                      
*└──────────────────────────────────────────────────────────────┘
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace myclass.movie
{
    public class VlcPlayer
    {
        private IntPtr libvlc_instance_;
        private IntPtr libvlc_media_player_;
        private double duration_;
        private string filepath;
        private bool loop = false;
        private bool pause = true;
        public VlcPlayer()
        {
            string plugin_arg = "--plugin-path=plugins";//插件路径
            string[] arguments = { "-I", "dummy", "--ignore-config", "--no-video-title", plugin_arg };
            libvlc_instance_ = LibVlcAPI.libvlc_new(arguments);
            libvlc_media_player_ = LibVlcAPI.libvlc_media_player_new(libvlc_instance_);
        }
        
        /// <summary>
        /// 开启循环播放的函数
        /// </summary>
        private void loopFunction()
        {
            new Task(() =>
            {
                while (this.loop)
                {
                    if (((int)Duration() - 1) <= (int)GetPlayTime())
                    {
                        PlayFile(filepath);
                        //Play();
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }).Start();
        }
        // 设置循环播放
        public void setloop(bool loop)
        {
            this.loop = loop;
            loopFunction();
        }
        /// <summary>
        /// 设置显示在那个窗口，没有设置自行一个窗口
        /// </summary>
        /// <param name="wndHandle">父窗口句柄</param>
        public void SetRenderWindow(int wndHandle)
        {
            if (libvlc_instance_ != IntPtr.Zero && wndHandle != 0)
            {
                LibVlcAPI.libvlc_media_player_set_hwnd(libvlc_media_player_, wndHandle);
            }
        }
        public void PlayFile(string filePath)
        {
            this.filepath = filePath;
            IntPtr libvlc_media = LibVlcAPI.libvlc_media_new_path(libvlc_instance_, filePath);
            if (libvlc_media != IntPtr.Zero)
            {
                LibVlcAPI.libvlc_media_parse(libvlc_media);
                duration_ = LibVlcAPI.libvlc_media_get_duration(libvlc_media) / 1000.0;
                LibVlcAPI.libvlc_media_player_set_media(libvlc_media_player_, libvlc_media);
                LibVlcAPI.libvlc_media_release(libvlc_media);
                LibVlcAPI.libvlc_media_player_play(libvlc_media_player_);
                pause = false;
            }
        }
        public void Pause()
        {
            if (libvlc_media_player_ != IntPtr.Zero && !pause)
            {
                pause = true;
                LibVlcAPI.libvlc_media_player_pause(libvlc_media_player_);
            }
        }
        public void Play()
        {
            if (libvlc_media_player_ != IntPtr.Zero && pause)
            {
                pause = false;
                LibVlcAPI.libvlc_media_player_play(libvlc_media_player_);
                //  LibVlcAPI.libvlc_media_player_pause(libvlc_media_player_);
            }
        }
        public void Stop()
        {
            if (libvlc_media_player_ != IntPtr.Zero)
            {
                pause = true;
                LibVlcAPI.libvlc_media_player_stop(libvlc_media_player_);
            }
        }
        //  public void FastForward()
        // {
        //    if (libvlc_media_player_ != IntPtr.Zero)
        //   {
        //      LibVlcAPI.libvlc_media_player_fastforward(libvlc_media_player_);
        // }
        // }
        // 获得当前播放时长
        public double GetPlayTime()
        {
            return LibVlcAPI.libvlc_media_player_get_time(libvlc_media_player_) / 1000.0;
        }
        public void SetPlayTime(double seekTime)
        {
            LibVlcAPI.libvlc_media_player_set_time(libvlc_media_player_, (Int64)(seekTime * 1000));
        }
        public int GetVolume()
        {
            return LibVlcAPI.libvlc_audio_get_volume(libvlc_media_player_);
        }
        public void SetVolume(int volume)
        {
            LibVlcAPI.libvlc_audio_set_volume(libvlc_media_player_, volume);
        }
        public void SetFullScreen(bool istrue)
        {
            LibVlcAPI.libvlc_set_fullscreen(libvlc_media_player_, istrue ? 1 : 0);
        }

        // 返回视频总长度
        public double Duration()
        {
            return duration_;
        }
        public string Version()
        {
            return LibVlcAPI.libvlc_get_version();
        }
    }
    internal static class LibVlcAPI
    {
        internal struct PointerToArrayOfPointerHelper
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public IntPtr[] pointers;
        }
        public static IntPtr libvlc_new(string[] arguments)
        {
            PointerToArrayOfPointerHelper argv = new PointerToArrayOfPointerHelper();
            argv.pointers = new IntPtr[11];
            for (int i = 0; i < arguments.Length; i++)
            {
                argv.pointers[i] = Marshal.StringToHGlobalAnsi(arguments[i]);
            }
            IntPtr argvPtr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(typeof(PointerToArrayOfPointerHelper));
                argvPtr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(argv, argvPtr, false);
                return libvlc_new(arguments.Length, argvPtr);
            }
            finally
            {
                for (int i = 0; i < arguments.Length + 1; i++)
                {
                    if (argv.pointers[i] != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(argv.pointers[i]);
                    }
                }
                if (argvPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(argvPtr);
                }
            }
        }
        public static IntPtr libvlc_media_new_path(IntPtr libvlc_instance, string path)
        {
            IntPtr pMrl = IntPtr.Zero;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(path);
                pMrl = Marshal.AllocHGlobal(bytes.Length + 1);
                Marshal.Copy(bytes, 0, pMrl, bytes.Length);
                Marshal.WriteByte(pMrl, bytes.Length, 0);
                return libvlc_media_new_path(libvlc_instance, pMrl);
            }
            finally
            {
                if (pMrl != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pMrl);
                }
            }
        }
        public static IntPtr libvlc_media_new_location(IntPtr libvlc_instance, string path)
        {
            IntPtr pMrl = IntPtr.Zero;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(path);
                pMrl = Marshal.AllocHGlobal(bytes.Length + 1);
                Marshal.Copy(bytes, 0, pMrl, bytes.Length);
                Marshal.WriteByte(pMrl, bytes.Length, 0);
                return libvlc_media_new_path(libvlc_instance, pMrl);
            }
            finally
            {
                if (pMrl != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pMrl);
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        // 以下是libvlc.dll导出函数

        // 创建一个libvlc实例，它是引用计数的
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr libvlc_new(int argc, IntPtr argv);

        // 释放libvlc实例
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_release(IntPtr libvlc_instance);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern String libvlc_get_version();

        // 从视频来源(例如Url)构建一个libvlc_meida
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr libvlc_media_new_location(IntPtr libvlc_instance, IntPtr path);

        // 从本地文件路径构建一个libvlc_media
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr libvlc_media_new_path(IntPtr libvlc_instance, IntPtr path);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_release(IntPtr libvlc_media_inst);

        // 创建libvlc_media_player(播放核心)
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern IntPtr libvlc_media_player_new(IntPtr libvlc_instance);

        // 将视频(libvlc_media)绑定到播放器上
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_set_media(IntPtr libvlc_media_player, IntPtr libvlc_media);

        // 设置图像输出的窗口
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_set_hwnd(IntPtr libvlc_mediaplayer, Int32 drawable);

        // 使用播放句柄控制播放器播放
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_play(IntPtr libvlc_mediaplayer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="libvlc_mediaplayer"></param>
        //[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        //[SuppressUnmanagedCodeSecurity]
        // public static extern void libvlc_media_player_fastforward(IntPtr libvlc_mediaplayer);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_pause(IntPtr libvlc_mediaplayer);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_stop(IntPtr libvlc_mediaplayer);

        // 解析视频资源的媒体信息(如时长等)
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_parse(IntPtr libvlc_media);

        // 返回视频的时长(必须先调用libvlc_media_parse之后，该函数才会生效)
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern Int64 libvlc_media_get_duration(IntPtr libvlc_media);

        // 当前播放的时间
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern Int64 libvlc_media_player_get_time(IntPtr libvlc_mediaplayer);

        // 设置播放位置(拖动)
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_set_time(IntPtr libvlc_mediaplayer, Int64 time);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_release(IntPtr libvlc_mediaplayer);

        // 获取和设置音量
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int libvlc_audio_get_volume(IntPtr libvlc_media_player);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_audio_set_volume(IntPtr libvlc_media_player, int volume);

        // 设置全屏
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_set_fullscreen(IntPtr libvlc_media_player, int isFullScreen);

        ///************************************************************************************
        // * 下面用来提取视频每一帧到图片
        // * 自定义改变播放窗口，会使用到上面的函数
        // * 这种方法进行回调很影响播放性能并且渲染速度比较慢
        // * *********************************************************************************/
        //[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        //[SuppressUnmanagedCodeSecurity]
        //public static extern void libvlc_video_set_callbacks(IntPtr mediaPlayInstance, VideoLockCB lockCB, VideoUnlockCB unlockCB, VideoDisplayCB displayCB, IntPtr opaque);
        //[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        //[SuppressUnmanagedCodeSecurity]
        //public static extern void libvlc_video_set_format(IntPtr mediaPlayerInstance, IntPtr chroma, UInt32 width, UInt32 height, UInt32 pitch);

        ///// <summary>
        ///// 预先设置的变量
        ///// </summary>
        //private const int _width = 1920;
        //private const int _height = 1080;
        //private const int _pixelBytes = 4;
        //private const int _pitch = _width * _pixelBytes;
        //private static IntPtr _buff = IntPtr.Zero;

        ///// <summary>
        ///// 初始化三个委托
        ///// </summary>
        //private static VideoLockCB _videoLockCBnew = new VideoLockCB(VideoLockCallBack);
        //private static VideoUnlockCB _videoUnlockCB = new VideoUnlockCB(VideoUnlockCallBack);
        //private static VideoDisplayCB _videoDisplayCB = new VideoDisplayCB(VideoDiplayCallBack);
        //private delegate IntPtr VideoLockCB(IntPtr opaque, IntPtr planes);
        ////解锁一个图片缓冲区
        //private delegate void VideoUnlockCB(IntPtr opaque, IntPtr picture, IntPtr planes);
        ////显示图片
        //private delegate void VideoDisplayCB(IntPtr opaque, IntPtr picture);
        //// 锁定一个图片缓冲区时先锁定，然后初始化这个缓冲区。
        //private static IntPtr VideoLockCallBack(IntPtr opaque, IntPtr planes)
        //{
        //    Lock();
        //    _buff = Marshal.AllocHGlobal(_pitch * _height);
        //    Marshal.WriteIntPtr(planes, _buff);//初始化
        //    return IntPtr.Zero;
        //}

        ///// <summary>
        ///// VideoDiplayCallBack委托中用到了一个winapi函数
        ///// </summary>
        ///// <param name="Destination"></param>
        ///// <param name="Source"></param>
        ///// <param name="Length"></param>
        //[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        //private static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

        //private static void VideoDiplayCallBack(IntPtr opaque, IntPtr picture)
        //{
        //    if (Islock())
        //    {
        //        using (Bitmap bmp = new Bitmap(_width, _height, PixelFormat.Format32bppPArgb))
        //        {
        //            Rectangle rect = new Rectangle(0, 0, _width, _height);
        //            BitmapData bp = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
        //            CopyMemory(bp.Scan0, _buff, (uint)(_height * _pitch));
        //            bmp.UnlockBits(bp);
        //            //bmp.Save("c:\\vlc.bmp");
        //            // 储存图片在bitmap
        //            Bitmap bitmap = (Bitmap)bmp.Clone();
        //        }
        //    }
        //}

        ////解锁图片缓冲区
        //private static void VideoUnlockCallBack(IntPtr opaque, IntPtr picture, IntPtr planes)
        //{
        //    Marshal.FreeHGlobal(_buff);//释放缓冲区
        //    Unlock();
        //}


        //static bool obj = false;
        //private static void Lock()
        //{
        //    obj = true;
        //}
        //private static void Unlock()
        //{
        //    obj = false;
        //}
        //private static bool Islock()
        //{
        //    return obj;
        //}

    }
}

