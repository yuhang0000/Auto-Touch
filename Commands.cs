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
        /// <summary>
        /// 预设文件夹路径
        /// </summary>
        public static string AssumptionPath = System.Environment.CurrentDirectory + "\\Assumption\\";
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

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <param name="target">给定时间对象, 缺省时取当前时间</param>
        /// <returns>long: 时间戳</returns>
        public static long GetTimeStamp(DateTime? target = null)
        {
            DateTime now;
            if (target == null)
            {
                now = DateTime.Now;
            }
            else
            {
                now = (DateTime)target;
            }
            DateTime old = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long timestamp = (long)(now - old).TotalSeconds;

            return timestamp;
        }

        /// <summary>
        /// 获取当前时间戳 (精确到毫秒)
        /// </summary>
        /// <param name="target">给定时间对象, 缺省时取当前时间</param>
        /// <returns>long: 时间戳</returns>
        public static long GetTimeStampMs(DateTime? target = null)
        {
            DateTime now;
            if (target == null)
            {
                now = DateTime.Now;
            }
            else
            {
                now = (DateTime)target;
            }
            DateTime old = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long timestamp = (now.Ticks - old.Ticks) / 10000;

            return timestamp;
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
            /// 鼠标额外数据: 滚轮信息, 按下按键信息, 侧键信息
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
        /// KBDLLHOOKSTRUCT 结构 <br/>
        /// 包含有关低级别键盘输入事件的信息
        /// <para><a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/ns-winuser-kbdllhookstruct">KBDLLHOOKSTRUCT 结构 (winuser.h)</a></para>
        /// </summary>
        public struct tagKBDLLHOOKSTRUCT
        {
            /// <summary>
            /// 按键 KeyCode
            /// </summary>
            public int vkCode;
            /// <summary>
            /// 键盘扫描码
            /// </summary>
            public int scanCode;
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
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/inputdev/wm-rbuttondown">WM_RBUTTONDOWN消息</a><br/>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/inputdev/wm-rbuttonup">WM_RBUTTONUP消息</a><br/>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/inputdev/wm-mbuttondown">WM_MBUTTONDOWN消息</a><br/>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/inputdev/wm-mbuttonup">WM_MBUTTONUP消息</a><br/>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/inputdev/wm-xbuttondown">WM_XBUTTONDOWN消息</a><br/>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/inputdev/wm-xbuttonup">WM_XBUTTONUP消息</a><br/>
        /// </para>
        /// </summary>
        public static class WM_Mouse
        {
            /// <summary>
            /// 无
            /// </summary>
            public static int NONE = 0x0000;
            /// <summary>
            /// 鼠标左键按下
            /// </summary>
            public static int WM_LBUTTONDOWN = 0x0201;
            /// <summary>
            /// 鼠标左键松开
            /// </summary>
            public static int WM_LBUTTONUP = 0x0202;
            /// <summary>
            /// 鼠标移动
            /// </summary>
            public static int WM_MOUSEMOVE = 0x0200;
            /// <summary>
            /// 鼠标滚轮
            /// </summary>
            public static int WM_MOUSEWHEEL = 0x020A;
            /// <summary>
            /// 鼠标右键按下
            /// </summary>
            public static int WM_RBUTTONDOWN = 0x0204;
            /// <summary>
            /// 鼠标右键松开
            /// </summary>
            public static int WM_RBUTTONUP = 0x0205;
            /// <summary>
            /// 鼠标中键按下
            /// </summary>
            public static int WM_MBUTTONDOWN = 0x0207;
            /// <summary>
            /// 鼠标中键放开
            /// </summary>
            public static int WM_MBUTTONUP = 0x0208;
            /// <summary>
            /// 鼠标侧键按下
            /// </summary>
            public static int WM_XBUTTONDOWN = 0x020B;
            /// <summary>
            /// 鼠标侧键松开
            /// </summary>
            public static int WM_XBUTTONUP = 0x020C;

            /// <summary>
            /// 鼠标左键关闭
            /// </summary>
            public static int MK_LBUTTON = 0x0001;
            /// <summary>
            /// 鼠标右键关闭
            /// </summary>
            public static int MK_RBUTTON = 0x0002;
            /// <summary>
            /// Shift关闭
            /// </summary>
            public static int MK_SHIFT = 0x0004;
            /// <summary>
            /// Ctrl关闭
            /// </summary>
            public static int MK_CONTROL = 0x0008;
            /// <summary>
            /// 鼠标中键关闭
            /// </summary>
            public static int MK_MBUTTON = 0x0010;
            /// <summary>
            /// 鼠标侧键1关闭
            /// </summary>
            public static int MK_XBUTTON1 = 0x0020;
            /// <summary>
            /// 鼠标侧键2关闭
            /// </summary>
            public static int MK_XBUTTON2 = 0x0040;
        }

        /// <summary>
        /// 等待下一次的荧幕刷新, 需要 DWM
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/dwmapi/nf-dwmapi-dwmflush">dwmFlush 函数 (dwmapi.h)</a>
        /// </para>
        /// </summary>
        [DllImport("Dwmapi.dll")]
        public static extern long DwmFlush();

        /// <summary>
        /// 向指定窗体发送消息
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-sendmessage">sendMessage 函数 (winuser.h)</a>
        /// </para>
        /// </summary>
        /// <param name="hWnd">指定窗体句柄</param>
        /// <param name="Msg">要发送的消息</param>
        /// <param name="wParam">其他的消息特定信息</param>
        /// <param name="lParam">其他的消息特定信息</param>
        /// <returns>bool: 返回结果</returns>
        [DllImport("user32")]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 设置指定窗口的显示状态。
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-showwindow">ShowWindow 函数 (winuser.h)</a>
        /// </para>
        /// </summary>
        /// <param name="hWnd">窗口的句柄</param>
        /// <param name="nCmdShow">控制窗口的显示方式</param>
        /// <returns>bool: 返回状态</returns>
        [DllImport("user32")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// 将创建指定窗口的线程引入前台并激活窗口。
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-setforegroundwindow">SetForegroundWindow 函数 (winuser.h)</a>
        /// </para>
        /// </summary>
        /// <param name="hWnd">应激活并带到前台的窗口的句柄</param>
        /// <returns>bool: 返回状态</returns>
        [DllImport("user32")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 合成键击、鼠标动作和按钮单击。
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-sendinput">sendInput 函数 (winuser.h)</a><br/>
        /// </para>
        /// </summary>
        /// <param name="cInputs">pInputs 数组中的数量</param>
        /// <param name="pInputs">INPUT 结构的数组</param>
        /// <param name="cbSize">INPUT 结构的大小 (以字节为单位)</param>
        /// <returns>uint: 函数返回成功插入键盘或鼠标输入流的事件数。</returns>
        [DllImport("user32")]
        public static extern uint SendInput(uint cInputs, tagINPUT[] pInputs, int cbSize);

        /// <summary>
        /// 输入结构, 由 SendInput 用于存储输入信息的结构
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/ns-winuser-input">INPUT 结构 (winuser.h)</a>
        /// </para>
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct tagINPUT
        {
            /// <summary>
            /// 输入事件的类型
            /// </summary>
            [FieldOffset(0)] public uint type;
            /// <summary>
            /// 有关模拟鼠标事件的信息
            /// </summary>
            [FieldOffset(4)] public tagMOUSEINPUT mi;
            /// <summary>
            /// 有关模拟键盘事件的信息
            /// </summary>
            [FieldOffset(4)] public tagKEYBDINPUT ki;
            /// <summary>
            /// 有关模拟硬件事件的信息
            /// </summary>
            [FieldOffset(4)] public tagHARDWAREINPUT hi;
        }
        /// <summary>
        /// 包含有关模拟鼠标事件的信息
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/ns-winuser-mouseinput">MOUSEINPUT 结构 (winuser.h)</a>
        /// </para>
        /// </summary>
        public struct tagMOUSEINPUT
        {
            /// <summary>
            /// 鼠标的绝对位置, X 轴坐标
            /// </summary>
            public int dx;
            /// <summary>
            /// 鼠标的绝对位置, Y 轴坐标
            /// </summary>
            public int dy;
            /// <summary>
            /// 如果 dwFlags 包含 MOUSEEVENTF_WHEEL, 则 mouseData 为鼠标滚轮移动量; <br/>
            /// 如果 dwFlags 包含 MOUSEEVENTF_HWHEEL, 则 mouseData 为水平方向的鼠标滚轮移动量; <br/>
            /// 如果 dwFlags 包含 MOUSEEVENTF_XDOWN 或 MOUSEEVENTF_XUP, 则 mouseData 为按下的指定鼠标侧键. <br/>
            /// </summary>
            public int mouseData;
            /// <summary>
            /// 指定鼠标的标识位
            /// </summary>
            public uint dwFlags;
            /// <summary>
            /// 事件的时间戳, 值为 0 时系统将提供自己的时间戳
            /// </summary>
            public uint time;
            /// <summary>
            /// 与鼠标关联的附加值
            /// </summary>
            public UIntPtr dwExtraInfo;
        }
        /// <summary>
        /// 包含有关模拟键盘事件的信息
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/ns-winuser-keybdinput">KEYBDINPUT 结构 (winuser.h)</a>
        /// </para>
        /// </summary>
        public struct tagKEYBDINPUT
        {
            /// <summary>
            /// 按键代码
            /// </summary>
            public ushort wVk;
            /// <summary>
            /// 按键的硬件扫描代码. 如果 dwFlags 指定为 KEYEVENTF_UNICODE, wScan 将指定要发送到前台应用程序的 Unicode 字符
            /// </summary>
            public ushort wScan;
            /// <summary>
            /// 指定按键的标识位
            /// </summary>
            public uint dwFlags;
            /// <summary>
            /// 事件的时间戳, 值为 0 时系统将提供自己的时间戳
            /// </summary>
            public uint time;
            /// <summary>
            /// 与按键关联的附加值
            /// </summary>
            public UIntPtr dwExtraInfo;
        }
        /// <summary>
        /// 包含有关由键盘或鼠标以外的输入设备生成的模拟消息的信息
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/ns-winuser-hardwareinput">HARDWAREINPUT 结构 (winuser.h)</a>
        /// </para>
        /// </summary>
        public struct tagHARDWAREINPUT
        {
            /// <summary>
            /// 输入硬件生成的消息
            /// </summary>
            public uint uMsg;
            /// <summary>
            /// uMsg 的 lParam 参数的低序字
            /// </summary>
            public ushort wParamL;
            /// <summary>
            /// uMsg 的 lParam 参数的高序字
            /// </summary>
            public ushort wParamH;
        }
        /// <summary>
        /// 适用于 MOUSEINPUT 结构的标识位置
        /// </summary>
        public static class MOUSEEVENTF
        {
            /// <summary>
            /// 鼠标移动
            /// </summary>
            public static uint MOUSEEVENTF_MOVE = 0x0001;
            /// <summary>
            /// 鼠标左键按下
            /// </summary>
            public static uint MOUSEEVENTF_LEFTDOWN = 0x0002;
            /// <summary>
            /// 鼠标左键松开
            /// </summary>
            public static uint MOUSEEVENTF_LEFTUP = 0x0004;
            /// <summary>
            /// 鼠标右键按下
            /// </summary>
            public static uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
            /// <summary>
            /// 鼠标右键松开
            /// </summary>
            public static uint MOUSEEVENTF_RIGHTUP = 0x0010;
            /// <summary>
            /// 鼠标中键按下
            /// </summary>
            public static uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
            /// <summary>
            /// 鼠标中键松开
            /// </summary>
            public static uint MOUSEEVENTF_MIDDLEUP = 0x0040;
            /// <summary>
            /// 鼠标侧键按下
            /// </summary>
            public static uint MOUSEEVENTF_XDOWN = 0x0080;
            /// <summary>
            /// 鼠标侧键松开
            /// </summary>
            public static uint MOUSEEVENTF_XUP = 0x0100;
            /// <summary>
            /// 鼠标滚轮
            /// </summary>
            public static uint MOUSEEVENTF_WHEEL = 0x0800;
            /// <summary>
            /// 鼠标滚轮, 水平方向的
            /// </summary>
            public static uint MOUSEEVENTF_HWHEEL = 0x1000;
            /// <summary>
            /// 不合并 WM_MOUSEMOVE(鼠标移动) 消息
            /// </summary>
            public static uint MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000;
            /// <summary>
            /// 将坐标映射到整个桌面, 必须与 MOUSEEVENTF_ABSOLUTE一起使用
            /// </summary>
            public static uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
            /// <summary>
            /// 设置为绝对坐标. 未设定此值时, 默认为相对坐标, 即设置的坐标相对上一个光标位置做偏移
            /// </summary>
            public static uint MOUSEEVENTF_ABSOLUTE = 0x8000;
            /// <summary>
            /// 指定鼠标侧键, 使用时, 请将 dwFlags 设为 MOUSEEVENTF_XDOWN 或 MOUSEEVENTF_XUP
            /// </summary>
            public static class mouseData
            {
                /// <summary>
                /// 鼠标侧键1
                /// </summary>
                public static int XBUTTON1 = 0x0001;
                /// <summary>
                /// 鼠标侧键2
                /// </summary>
                public static int XBUTTON2 = 0x0002;
            }
        }
        /// <summary>
        /// 适用于 KEYBDINPUT 结构的标识位置
        /// </summary>
        public static class KEYEVENTF
        {
            /// <summary>
            /// 如果指定, wScan 扫描代码由两个字节序列组成, 其中第一个字节的值为0xE0
            /// </summary>
            public static uint KEYEVENTF_EXTENDEDKEY = 0x0001;
            /// <summary>
            /// 松开键盘按键, 未指定时默认为按下键盘按键
            /// </summary>
            public static uint KEYEVENTF_KEYUP = 0x0002;
            /// <summary>
            /// 如果指定, 那么 wVk 将替换为 wScan, 作为按键代码, 并且忽略 wVk
            /// </summary>
            public static uint KEYEVENTF_SCANCODE = 0x0008;
            /// <summary>
            /// 如果指定，那么可以在 wScan 中指定 Unicode 字符并发送出去, 请配合 KEYEVENTF_KEYUP 使用
            /// </summary>
            public static uint KEYEVENTF_UNICODE = 0x0004;
        }
        /// <summary>
        /// 适用于 SendInputType 的输入事件的类型
        /// </summary>
        public static class SendInputType
        {
            /// <summary>
            /// 鼠标事件
            /// </summary>
            public static uint INPUT_MOUSE = 0;
            /// <summary>
            /// 键盘事件
            /// </summary>
            public static uint INPUT_KEYBOARD = 1;
            /// <summary>
            /// 硬件消息事件
            /// </summary>
            public static uint INPUT_HARDWARE = 2;
        }

    }

    /// <summary>
    /// 鼠标动作项
    /// </summary>
    public class MouseActionItem
    {
        public Point XY;
        public int X;
        public int Y;
        public int Delay = 0;
        public int Wheel = 0;
        public string Action = "None";

        public MouseActionItem(int X, int Y)
        {
            this.XY = new Point(X, Y);
            this.X = X;
            this.Y = Y;
        }
    }
}
