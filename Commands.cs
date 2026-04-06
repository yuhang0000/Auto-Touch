using Auto_Touch;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Commands
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class GlobalStatus
    {
        /// <summary>
        /// 主窗体
        /// </summary>
        public static Main main;
        /// <summary>
        /// 单点捕捉窗体
        /// </summary>
        public static CapturePosition capturePosition;
        /// <summary>
        /// 版本号
        /// </summary>
        public static string Version;
        /// <summary>
        /// 编译时间
        /// </summary>
        public static DateTime BuildTime;
    }

    /// <summary>
    /// 工具集
    /// </summary>
    public static class Command
    {
        /// <summary>
        /// 关于
        /// </summary>
        public static void About()
        {
            string[] text =
            {
                Application.ProductName, 
                "By: " + Application.CompanyName,
                "BuildTime: " + GlobalStatus.BuildTime.ToString(),
                "Version: " + GlobalStatus.Version,
            };
            MessageBox.Show(string.Join("\r\n", text), "关于");
        }
    }

    /// <summary>
    /// 外部函数库
    /// </summary>
    public static class DLL
    {
        /// <summary>
        /// 捕捉滑鼠坐标
        /// </summary>
        /// <param name="point">Point 对象</param>
        /// <returns>int: 如果成功, 则返回非零值, 否则返回零. ; out Point: 返回滑鼠坐标</returns>
        [DllImport("user32")]
        public static extern int GetCursorPos(out Point point);

        /// <summary>
        /// 设置滑鼠坐标
        /// </summary>
        /// <param name="point">Point 对象</param>
        /// <returns>int: 如果成功, 则返回非零值, 否则返回零</returns>
        [DllImport("user32")]
        public static extern int SetCursorPos(Point point);

        /// <summary>
        /// 回调函数的指针 (这是模板, 要自己单独写回调函数, 传递结构得和这个一样
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nc-winuser-hookproc">HOOKPROC 回调函数 (winuser.h)</a>
        /// </para>
        /// </summary>
        /// <param name="code">不知道是啥</param>
        /// <param name="wParam">指定消息是否由当前进程发送. 如果消息由当前进程发送, 则为非零; 否则为 NULL. </param>
        /// <param name="lParam">指向 CWPRETSTRUCT 结构的指针, 该结构包含有关消息的详细信息. </param>
        /// <returns></returns>
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 设置消息钩子
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-setwindowshookexa">SetWindowsHookExA 函数 （winuser.h）</a>
        /// </para>
        /// </summary>
        /// <returns>IntPtr: 返回消息钩子句柄</returns>
        [DllImport("user32")]
        public static extern IntPtr SetWindowsHookExA(int idHook, HookProc lpfn, IntPtr hmod, int dwThreadId); //这里的 HookProc, 是要自己写的回调函数, 函数结构得写的和它一样

        /// <summary>
        /// 移除消息钩子
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-unhookwindowshookex">UnhookWindowsHookEx 函数 (winuser.h)</a>
        /// </para>
        /// </summary>
        /// <param name="idHook">要移除消息钩子的句柄</param>
        /// <returns>bool: 如果该函数成功, 则返回值为 true. </returns>
        [DllImport("user32")]
        public static extern bool UnhookWindowsHookEx(IntPtr idHook);

        /// <summary>
        /// 继续运行下一个钩子 (其实是把钩子消息传递给下一个程序)
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-callnexthookex">CallNextHookEx 函数 (winuser.h)</a>
        /// </para>
        /// </summary>
        /// <param name="idHook">消息钩子句柄</param>
        /// <param name="nCode">传递给当前消息钩子的代码</param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// WH_MOUSE_LL 的回调函数 (不用这个, 搞半天原来是要自己写一个)
        /// <para><a href="https://learn.microsoft.com/zh-cn/windows/win32/winmsg/lowlevelmouseproc">LowLevelMouseProc 函数</a></para>
        /// </summary>
        /// <param name="nCode">挂钩过程用于确定如何处理消息的代码</param>
        /// <param name="wParam">鼠标消息的标识符</param>
        /// <param name="lParam">指向 MSLLHOOKSTRUCT 结构的指针</param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// POINT 结构体
        /// <para><a href="https://learn.microsoft.com/zh-cn/windows/win32/api/windef/ns-windef-point">POINT 结构 (windef.h)</a></para>
        /// </summary>
        public struct tagPOINT
        {
            /// <summary>
            /// X 坐标
            /// </summary>
            public int X;
            /// <summary>
            /// Y 坐标
            /// </summary>
            public int Y;
        }

        /// <summary>
        /// MSLLHOOKSTRUCT 结构体 <br/>
        /// 包含有关低级别鼠标输入事件的信息
        /// <para><a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/ns-winuser-msllhookstruct">MSLLHOOKSTRUCT 结构 (winuser.h)</a></para>
        /// </summary>
        public struct tagMSLLHOOKSTRUCT
        {
            /// <summary>
            /// 光标的 XY 坐标
            /// </summary>
            public tagPOINT pt;
            /// <summary>
            /// 鼠标额外数据: 滚轮信息, 按下按键信息
            /// </summary>
            public int mouseData;
            /// <summary>
            /// 事件注入的标志
            /// </summary>
            public int flags;
            /// <summary>
            /// 此消息的时间戳
            /// </summary>
            public int time;
            /// <summary>
            /// 与消息关联的其他信息
            /// </summary>
            public uint dwExtraInfo;
        }

        /// <summary>
        /// 消息钩子类型
        /// </summary>
        public static class IdHook
        {
            /// <summary>
            /// 监听键盘的
            /// </summary>
            public static int WH_KEYBOARD = 2;
            /// <summary>
            /// 监听低级别键盘的
            /// </summary>
            public static int WH_KEYBOARD_LL = 13;
            /// <summary>
            /// 监听滑鼠的
            /// </summary>
            public static int WH_MOUSE = 7;
            /// <summary>
            /// 监听低级别滑鼠的
            /// </summary>
            public static int WH_MOUSE_LL = 14;
        }

        /// <summary>
        /// WM_Mouse消息
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/inputdev/wm-lbuttondown">WM_LBUTTONDOWN消息</a><br/>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/inputdev/wm-lbuttonup">WM_LBUTTONUP消息</a><br/>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/inputdev/wm-mousemove">WM_MOUSEMOVE消息</a><br/>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/inputdev/wm-mousewheel">WM_MOUSEWHEEL消息</a><br/>
        /// </para>
        /// </summary>
        public static class WM_Mouse
        {
            /// <summary>
            /// 鼠标左键
            /// </summary>
            public static int MK_LBUTTON = 0x0001;
            /// <summary>
            /// 鼠标右键
            /// </summary>
            public static int MK_RBUTTON = 0x0002;
            /// <summary>
            /// Shift
            /// </summary>
            public static int MK_SHIFT = 0x0004;
            /// <summary>
            /// Ctrl
            /// </summary>
            public static int MK_CONTROL = 0x0008;
            /// <summary>
            /// 鼠标中键
            /// </summary>
            public static int MK_MBUTTON = 0x0010;
            /// <summary>
            /// 鼠标侧键1
            /// </summary>
            public static int MK_XBUTTON1 = 0x0020;
            /// <summary>
            /// 鼠标侧键2
            /// </summary>
            public static int MK_XBUTTON2 = 0x0040;
        }
    }
}
