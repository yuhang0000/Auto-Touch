using Auto_Touch;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static Commands.DLL;

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
        /// <summary>
        /// 帮助文档
        /// </summary>
        public static string[] helptext = { 
        "用法: ",
        "",
        "Auto Touch [OPTION]",
        "",
        "\t--help, -h      \t\t获取帮助",
        "\t--version, -ver \t\t检查版本信息",
        "\t--profile, -p <PATH|NAME>\t以指定的预设执行",
        "\t-x <INT>\t\t\t光标 X 轴坐标位置",
        "\t-y <INT>\t\t\t光标 Y 轴坐标位置",
        "\t--wheel, -w <INT>\t\t鼠标滚轮滚动距离, 缺省时为 0",
        "\t--action, -a <ACTION>   \t鼠标按键动作, 缺省时为 None",
        "\t--time, -t <INT|HH:MM:SS>\t延时运行, 单位: ms | HH:MM:SS, 缺省时为立即执行",
        "\t--debug \t\t\t调试模式, 在当前终端中打印额外信息",
        "",
        "<ACTION>",
        "\tNone, MouseLeft, MouseMiddle, MouseRight, MouseXButton1, MouseXButton2",
        "",
        "示例: ",
        "\tAuto Touch.exe -x 1920 -y 1080 --wheel 120 --time 60000",
        "\tAuto Touch.exe -x 1920 -y 1080 --action MouseLeft,MouseRight",
        "\tAuto Touch.exe -x 1920 -y 1080 --time 100 --action MouseLeft",
        "\tAuto Touch.exe -x 1920 -y 1080 --time 10:00:00",
        "\tAuto Touch.exe --profile #1"
        };
        /// <summary>
        /// 可提供的鼠标按键列表
        /// </summary>
        public static string[] MouseButtons = { "mouseleft", "mousemiddle", "mouseright", "mousexbutton1", "mousexbutton2" };
        /// <summary>
        /// ITaskbarList3 对象
        /// </summary>
        public static DLL.ITaskbarList3 ITaskbarList3;
        /// <summary>
        /// 调试模式
        /// </summary>
        public static bool IsDebug = false;
        /// <summary>
        /// 是否附加到控制台, -1 = 没有检查, 0 = 没有, 1 = 有的
        /// </summary>
        public static int IsAttachConsole = -1;
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

        /// <summary>
        /// 将相对路径转换为绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToAbsolutePath(string path)
        {
            //跳过空的
            if (path == null)
            {
                return null;
            }
            path = path.Trim();
            if (path.Length == 0)
            {
                return "";
            }

            path = path.Replace("/","\\");
            //如果有指向驱动器的话, 就直接返回
            if (path.Length > 2 && ((path[0] > 64 && path[0] < 91) || (path[0] > 96 && path[0] < 123) ) && path[1] == ':' && path[2] == '\\')
            {
                return path;
            }
            //如果有指向网页驱动器的话, 就直接返回
            if (path.Length > 1 && path[0] == '\\' && path[1] == '\\')
            {
                return path;
            }
            //根目录
            List<string> cd = Environment.CurrentDirectory.Split('\\').ToList<string>();
            string[] path_array = path.Split('\\');
            if (path[0] == '\\')
            {
                return cd[0] + path;
            }
            //循环遍历 ..\ .\
            foreach (string c in path_array)
            {
                //上一级
                if (c == "..")
                {
                    //不能再向上了
                    if (cd.Count < 2)
                    {
                        throw new Exception("超出路径范围. ");
                    }
                    else
                    {
                        cd.RemoveAt(cd.Count - 1);
                    }
                }
                //当前目录
                else if(c == ".")
                {
                    continue;
                }
                else
                {
                    cd.Add(c);
                }
            }

            return string.Join("\\", cd);
        }

        /// <summary>
        /// 打印日志, 仅当 GlobalStatus.IsDebug == true 时
        /// </summary>
        /// <param name="text"></param>
        public static void ConsoleLog(string text)
        {
            if (GlobalStatus.IsDebug == true)
            {
                Console.WriteLine(text);
            }
        }

        /// <summary>
        /// 打印文本到控制台里
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="title">标题</param>
        /// <param name="trymsgbox">失败时, 尝试使用 MessageBox</param>
        public static void ConsoleLog4CMD(string text, string title = "Auto Touch", bool trymsgbox = true)
        {
            AttachConsole();
            if (GlobalStatus.IsAttachConsole == 1)
            {
                Console.WriteLine(text);
            }
            else if (GlobalStatus.IsAttachConsole == 0 && trymsgbox == true)
            {
                MessageBox.Show(text, title);
            }
        }

        /// <summary>
        /// 尝试附加到控制台
        /// </summary>
        public static void AttachConsole()
        {
            //检查有没有附加到控制台
            if (GlobalStatus.IsAttachConsole == -1)
            {
                if (DLL.AttachConsole(-1) == true)
                {
                    GlobalStatus.IsAttachConsole = 1;
                    Console.WriteLine("");
                    Console.WriteLine(Application.ProductName + "  v" + GlobalStatus.Version + " - " + ((AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute))).Description);
                }
                else
                {
                    GlobalStatus.IsAttachConsole = 0;
                }
            }
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

        /// <summary>
        /// 将调用进程附加到指定进程的控制台作为客户端应用程序。
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/console/attachconsole">AttachConsole 函数</a>
        /// </para>
        /// </summary>
        /// <param name="dwProcessId">要使用的控制台的进程标识符. 值为 -1 时, 使用当前进程的父级的控制台</param>
        /// <returns>bool: 如果该函数成功，则返回值为非零值，反之则为零值。</returns>
        [DllImport("Kernel32")]
        public static extern bool AttachConsole(int dwProcessId);

        /// <summary>
        /// 从其控制台分离调用进程。
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/console/freeconsole">FreeConsole 函数</a>
        /// </para>
        /// </summary>\
        /// <returns>bool: 如果该函数成功，则返回值为非零值，反之则为零值。</returns>
        [DllImport("Kernel32")]
        public static extern bool FreeConsole();

        /// <summary>
        /// TIMECAPS 结构包含有关计时器分辨率的信息
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/timeapi/ns-timeapi-timecaps">TIMECAPS 结构</a>
        /// </para>
        /// </summary>
        public struct timecaps_tag
        {
            /// <summary>
            /// 支持的最小分辨率, 单位 ms
            /// </summary>
            public uint wPeriodMin;
            /// <summary>
            /// 支持的最大分辨率, 单位 ms
            /// </summary>
            public uint wPeriodMax;
        }

        /// <summary>
        /// 查询计时器设备以确定其分辨率
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/timeapi/nf-timeapi-timegetdevcaps">timeGetDevCaps 函数 (timeapi.h)</a>
        /// </para>
        /// </summary>
        /// <param name="ptc">指向 TIMECAPS 结构的指针</param>
        /// <param name="cbtc">TIMECAPS 结构的大小</param>
        /// <returns>int: 如果成功, 则返回 0</returns>
        [DllImport("Winmm")]
        public static extern int timeGetDevCaps(ref timecaps_tag ptc, uint cbtc);

        /// <summary>
        /// 设置周期计时器的最低分辨率
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/timeapi/nf-timeapi-timebeginperiod">timeBeginPeriod 函数 (timeapi.h)</a>
        /// </para>
        /// </summary>
        /// <param name="uPeriod">最低定时器分辨率, 单位 ms</param>
        /// <returns>int: 如果成功, 则返回 0</returns>
        [DllImport("Winmm")]
        public static extern int timeBeginPeriod(uint uPeriod);

        /// <summary>
        /// 清除以前设置的最小计时器分辨率
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/timeapi/nf-timeapi-timeendperiod">timeEndPeriod 函数 (timeapi.h)</a>
        /// </para>
        /// </summary>
        /// <param name="uPeriod">上次调用 timeBeginPeriod 函数时指定的最小计时器分辨率</param>
        /// <returns>int: 如果成功, 则返回 0</returns>
        [DllImport("Winmm")]
        public static extern int timeEndPeriod(uint uPeriod);

        /// <summary>
        /// 启动指定的定时器事件
        /// <para>
        /// <a href="https://learn.microsoft.com/en-us/previous-versions//dd757634(v=vs.85)">timeSetEvent 函数</a>
        /// </para>
        /// </summary>
        /// <param name="uDelay">事件循环周期延迟, 单位 ms</param>
        /// <param name="uResolution">计时器事件的分辨率, 单位 ms</param>
        /// <param name="lpTimeProc">指针指向回调函数, 每一周期调用一次</param>
        /// <param name="dwUser">用户提供的回调数据</param>
        /// <param name="fuEvent">计时器事件类型</param>
        /// <returns>如果成功，返回计时器事件的标识符，否则则返回错误。如果该函数失败且计时器事件未被创建，则返回 NULL。（该标识符也会传递给回调函数。）</returns>
        [DllImport("Winmm")]
        public static extern int timeSetEvent(uint uDelay, uint uResolution, TimeProc lpTimeProc, uint dwUser, uint fuEvent);

        /// <summary>
        /// 计时器事件类型
        /// <para>
        /// 我服了, 这里定义得从 Mmsystem.h 里找. 
        /// </para>
        /// </summary>
        public static class TimerEvents
        {
            /// <summary>
            /// 事件只触发一次
            /// </summary>
            public static uint TIME_ONESHOT = 0x0000;
            /// <summary>
            /// 事件循环触发
            /// </summary>
            public static uint TIME_PERIODIC = 0x0001;
            /// <summary>
            /// 当计时器到期时，Windows 调用 lpTimeProc 参数指向的函数。这是默认的。
            /// </summary>
            public static uint TIME_CALLBACK_FUNCTION = 0x0000;
            /// <summary>
            /// 当计时器到期时，Windows 调用 SetEvent 函数，将 lpTimeProc 参数指向的事件设置。dwUser 参数被忽略。
            /// </summary>
            public static uint TIME_CALLBACK_EVENT_SET = 0x0010;
            /// <summary>
            /// 当定时器到期时，Windows 调用 PulseEvent 函数，脉冲 lpTimeProc 参数指向的事件。dwUser 参数被忽略。
            /// </summary>
            public static uint TIME_CALLBACK_EVENT_PULSE = 0x0020;
            /// <summary>
            /// 传递该标志可以防止在调用 timeKillEvent 函数后发生事件。
            /// </summary>
            public static uint TIME_KILL_SYNCHRONOUS = 0x0100;
        }

        /// <summary>
        /// 取消指定的定时器事件
        /// <para>
        /// <a href="https://learn.microsoft.com/en-us/previous-versions//dd757630(v=vs.85)">timeKillEvent 函数</a>
        /// </para>
        /// </summary>
        /// <param name="uTimerID">用于取消定时器的事件标识符</param>
        /// <returns>int: 如果成功, 则返回 0</returns>
        [DllImport("Winmm")]
        public static extern int timeKillEvent(uint uTimerID);

        /// <summary>
        /// 适用于 timeSetEvent 的回调函数
        /// </summary>
        /// <param name="uID">定时器事件的标识符</param>
        /// <param name="uMsg">保留项, 没什么用</param>
        /// <param name="dwUser">传递给 timeSetEvent 函数 dwUser 参数的用户实例数据</param>
        /// <param name="dw1">保留项, 没什么用</param>
        /// <param name="dw2">保留项, 没什么用</param>
        /// <returns></returns>
        public delegate IntPtr TimeProc(uint uID, uint uMsg, uint dwUser, uint dw1, uint dw2);

        /// <summary>
        /// 创建或打开可等待的计时器对象，并返回对象的句柄。
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/synchapi/nf-synchapi-createwaitabletimerexw">CreateWaitableTimerExW 函数 (synchapi.h)</a>
        /// </para>
        /// </summary>
        /// <param name="lpTimerAttributes">指向 SECURITY_ATTRIBUTES 结构的指针</param>
        /// <param name="lpTimerName">计时器对象的名称</param>
        /// <param name="dwFlags">标识位</param>
        /// <param name="dwDesiredAccess">计时器对象的访问掩码</param>
        /// <returns>IntPtr: 如果函数成功，则返回值是计时器对象的句柄, 反之为 IntPtr.Zero </returns>
        [DllImport("Kernel32", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CreateWaitableTimerExW(IntPtr lpTimerAttributes, string lpTimerName, uint dwFlags, uint dwDesiredAccess);

        /// <summary>
        /// 适用于 CreateWaitableTimerExW 的标志位
        /// </summary>
        public static class WaitableTimerFlags
        {
            /// <summary>
            /// 默认
            /// </summary>
            public static uint DEFAULT = 0x00000000;
            /// <summary>
            /// 必须手动重置计时器
            /// </summary>
            public static uint CREATE_WAITABLE_TIMER_MANUAL_RESET = 0x00000001;
            /// <summary>
            /// 创建高分辨率计时器
            /// </summary>
            public static uint CREATE_WAITABLE_TIMER_HIGH_RESOLUTION = 0x00000002;
        }

        /// <summary>
        /// 枚举了适用于 dwDesiredAccess 的同步对象安全性和访问权限
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/sync/synchronization-object-security-and-access-rights">同步对象安全性和访问权限</a>
        /// </para>
        /// </summary>
        public static class DesiredAccesss
        {
            //所有对象使用的标准访问权限
            /// <summary>
            /// 需要删除对象。
            /// </summary>
            public static ulong DELETE = 0x00010000L;
            /// <summary>
            /// 在对象的安全描述符中读取信息（不包括 SACL 中的信息）所必需的。 若要读取或写入 SACL，必须请求 ACCESS_SYSTEM_SECURITY 访问权限。
            /// </summary>
            public static ulong READ_CONTROL = 0x00010000L;
            /// <summary>
            /// 使用对象进行同步的权限。
            /// </summary>
            public static ulong SYNCHRONIZE = 0x00010000L;
            /// <summary>
            /// 在对象的安全描述符中修改 DACL 所必需的。
            /// </summary>
            public static ulong WRITE_DAC = 0x00010000L;
            /// <summary>
            /// 在对象的安全描述符中更改所有者所必需的。
            /// </summary>
            public static ulong WRITE_OWNER = 0x00010000L;

            //事件对象
            /// <summary>
            /// 事件对象的所有可能访问权限
            /// </summary>
            public static uint EVENT_ALL_ACCESS = 0x1F0003;
            /// <summary>
            /// 修改 SetEvent、ResetEvent 和 PulseEvent 函数所需的状态访问
            /// </summary>
            public static uint EVENT_MODIFY_STATE = 0x0002;

            //互斥对象
            /// <summary>
            /// 互斥体对象的所有可能访问权限
            /// </summary>
            public static uint MUTEX_ALL_ACCESS = 0x1F0001;
            /// <summary>
            /// 保留以供将来使用
            /// </summary>
            public static uint MUTEX_MODIFY_STATE = 0x0001;

            //信号灯对象
            /// <summary>
            /// 信号灯对象的所有可能访问权限
            /// </summary>
            public static uint SEMAPHORE_ALL_ACCESS = 0x1F0003;
            /// <summary>
            /// 修改 ReleaseSemaphore 函数所需的状态访问
            /// </summary>
            public static uint SEMAPHORE_MODIFY_STATE = 0x0002;

            //计时器
            /// <summary>
            /// 计时器对象的所有可能访问权限
            /// </summary>
            public static uint TIMER_ALL_ACCESS = 0x1F0003;
            /// <summary>
            /// 修改 SetWaitableTimer 和 CancelWaitableTimer 函数所需的状态访问
            /// </summary>
            public static uint TIMER_MODIFY_STATE = 0x0002;
            /// <summary>
            /// 保留以供将来使用
            /// </summary>
            public static uint TIMER_QUERY_STATE = 0x0001;
        }

        /// <summary>
        /// <para>
        /// 包含一个 64 位值，该值表示自 1601 年 1 月 1 日 (UTC) 以来的 100 纳秒间隔数 <br/>
        /// 再套一个 ref 就可以当作 ulong 用了. 
        /// </para>
        /// <para>
        /// <br/>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/minwinbase/ns-minwinbase-filetime">fileTIME 结构 (minwinbase.h)</a>
        /// </para>
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct FILETIME
        {
            /// <summary>
            /// 文件时间的低序部分
            /// </summary>
            [FieldOffset(0)]public uint dwLowDateTime;
            /// <summary>
            /// 文件时间的高序部分
            /// </summary>
            [FieldOffset(4)]public uint dwHighDateTime;
            /// <summary>
            /// 像 long 一样填数字就好
            /// </summary>
            [FieldOffset(0)]public long AsLong;
        }

        /// <summary>
        /// 激活指定的计时器
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/synchapi/nf-synchapi-setwaitabletimer">SetWaitableTimer 函数 (synchapi.h)</a>
        /// </para>
        /// </summary>
        /// <param name="hTimer">计时器对象的句柄</param>
        /// <param name="lpDueTime">计时器首次 Tick 的时间, 单位 100ns</param>
        /// <param name="lPeriod">计时器的循环周期, 单位 ms</param>
        /// <param name="pfnCompletionRoutine">指向可选完成例程的指针</param>
        /// <param name="lpArgToCompletionRoutine">指向传递到完成例程的结构的指针</param>
        /// <param name="fResume">如果此参数 TRUE，则当计时器状态设置为信号时，还原处于挂起的节能模式的系统</param>
        /// <returns>bool: 如果函数成功, 则返回值为 true</returns>
        [DllImport("Kernel32", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetWaitableTimer(IntPtr hTimer, ref FILETIME lpDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutine, bool fResume);

        /// <summary>
        /// 暂停指定的可等待计时器
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/synchapi/nf-synchapi-cancelwaitabletimer">CancelWaitableTimer 函数 (synchapi.h)</a>
        /// </para>
        /// </summary>
        /// <param name="hTimer">计时器对象的句柄</param>
        /// <returns>bool: 如果该函数成功, 则返回值为 true</returns>
        [DllImport("Kernel32")]
        public static extern bool CancelWaitableTimer(IntPtr hTimer);

        /// <summary>
        /// 关闭打开的对象句柄
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/handleapi/nf-handleapi-closehandle">closeHandle 函数 (handleapi.h)</a>
        /// </para>
        /// </summary>
        /// <param name="hObject">打开对象的有效句柄</param>
        /// <returns>bool: 如果该函数成功, 则返回值为 true</returns>
        [DllImport("Kernel32")]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// 等待指定的对象处于信号状态或超时间隔过
        /// <para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/synchapi/nf-synchapi-waitforsingleobject">WaitForSingleObject 函数 (synchapi.h)</a>
        /// </para>
        /// </summary>
        /// <param name="hHandle">对象的句柄</param>
        /// <param name="dwMilliseconds">超时间隔, 单位 ms</param>
        /// <returns>uint: 函数返回的事件, 正常的话是 WAIT_OBJECT_0</returns>
        [DllImport("Kernel32")]
        public static extern ulong WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        /// <summary>
        /// 适用于 WaitForSingleObject 的函数返回的事件列表
        /// </summary>
        public static class WAIT_Event
        {
            /// <summary>
            /// 指定的对象是一个互斥体对象，该对象不是由拥有互斥体对象的线程在拥有线程终止之前释放的。
            /// </summary>
            public static ulong WAIT_ABANDONED = 0x00000080L;
            /// <summary>
            /// 指定对象的状态已正常发出信号。
            /// </summary>
            public static ulong WAIT_OBJECT_0 = 0x00000000L;
            /// <summary>
            /// 指定的对象已超时。
            /// </summary>
            public static ulong WAIT_TIMEOUT = 0x00000102L;
            /// <summary>
            /// 指定的对象执行失败了。
            /// </summary>
            public static ulong WAIT_FAILED = 0xFFFFFFFF;
        }

        /// <summary>
        /// <para>
        /// ITaskbarList3 的 COM 接口
        /// <br/>
        /// 一个个对着 ShObjIdl_core.h 抄表好麻烦
        /// <br/><br/>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nn-shobjidl_core-itaskbarlist3">ITaskbarList3 接口 (shobjidl_core.h)</a>
        /// </para>
        /// </summary>
        [ComImport]
        [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITaskbarList3
        {
            //ITaskbarList1
            uint HrInit();
            uint AddTab(IntPtr hwnd);
            uint DeleteTab(IntPtr hwnd);
            uint ActivateTab(IntPtr hwnd);
            uint SetActiveAlt(IntPtr hwnd);

            //ITaskbarList2
            uint MarkFullscreenWindow(IntPtr hwnd, bool fFullscreen);

            //ITaskbarList3
            uint SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);
            uint SetProgressState(IntPtr hwnd, uint tbpFlags);
            uint RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
            uint UnregisterTab(IntPtr hwndTab);
            uint SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
            uint SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, uint dwReserved);
            uint ThumbBarAddButtons(IntPtr hwnd, uint cButtons, IntPtr pButton);
            uint ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, IntPtr pButton);
            uint ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
            /// <summary>
            /// 在任务栏图标上显示叠加层, 就是在任务栏图标的基础上, 右上角显示小图标
            /// <para>
            /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nf-shobjidl_core-itaskbarlist3-setoverlayicon">ITaskbarList3::SetOverlayIcon 方法 (shobjidl_core.h)</a>
            /// </para>
            /// </summary>
            /// <param name="hwnd">关联任务栏图标的窗体句柄</param>
            /// <param name="hIcon">叠加层图标的句柄. 传递 IntPtr.Zero 即可清除</param>
            /// <param name="pszDescription">指向字符串的指针, 该字符串提供覆盖所传达信息的替换文字版本</param>
            /// <returns>uint: 如果该方法成功, 则返回 0x0</returns>
            uint SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, string pszDescription);
            uint SetThumbnailTooltip(IntPtr hwnd, string pszTip);
            uint SetThumbnailClip(IntPtr hwnd, IntPtr prcClip);
        }

        /// <summary>
        /// <para>
        /// ITaskbarList 的GUID. 
        /// <br/>
        /// 调用 ITaskbarList3 前, 先把 ITaskbarList 弄出来
        /// </para>
        /// </summary>
        public static Guid CLSID_TaskbarLis = new Guid("56FDF344-FD6D-11d0-958A-006097C9A090");
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

    /// <summary>
    /// 执行鼠标动作
    /// </summary>
    public class MouseSentInput
    {
        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool IsRunning = false;

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="items">鼠标动作项列表</param>
        public void Run(List<MouseActionItem> items)
        {
            this.IsRunning = true;
            Console.WriteLine("\r\n开始执行... ");

            //创建媒体计时器
            IntPtr timer = DLL.CreateWaitableTimerExW(IntPtr.Zero, null, DLL.WaitableTimerFlags.CREATE_WAITABLE_TIMER_HIGH_RESOLUTION, DLL.DesiredAccesss.TIMER_ALL_ACCESS);
            DLL.FILETIME lpDueTime = new DLL.FILETIME();
            lpDueTime.AsLong = -10*1000*10L;
            DLL.SetWaitableTimer(timer, ref lpDueTime, 1, IntPtr.Zero, IntPtr.Zero, false);

            Point lastpos = new Point();
            DLL.GetCursorPos(out lastpos);
            tagINPUT[] inputs = new tagINPUT[1];
            int taginputsize = Marshal.SizeOf(typeof(tagINPUT));
            int dx = Screen.PrimaryScreen.Bounds.Width;
            int dy = Screen.PrimaryScreen.Bounds.Height;
            long lasttime = Command.GetTimeStampMs(); //最后获取的时间
            long now = lasttime; //当前时间
            long spend = 0; // now - lasttime
            long targettime = lasttime; //目标时间
            //暂存按键状态
            int[] status = { 0, 0, 0, 0, 0 }; //改用数组来表示状态, 0 = 无, 1 = 松开, 2 = 按下, 3 = 已按下


            foreach (MouseActionItem item in items)
            {
                //延时
                //await Task.Delay(item.Delay);
                targettime = targettime + item.Delay;
                while (item.Delay > 0 && targettime > now && this.IsRunning == true)
                {
                    //DLL.DwmFlush();
                    DLL.WaitForSingleObject(timer, 15);
                    now = Command.GetTimeStampMs();
                }
                spend = now - lasttime;
                lasttime = now;

                //检查是否停止状态
                if (this.IsRunning == false)
                {
                    break;
                }

                //DLL.SetCursorPos(item.XY); 
                int x = ((ushort.MaxValue * item.X) / dx) + 1;
                int y = ((ushort.MaxValue * item.Y) / dy) + 1;

                //获取要按下哪个按键
                bool[] dumpbuttonstatus = { false, false, false, false, false }; //暂存按键是否读取过状态
                string[] actions = item.Action.ToLower().Split('|');
                foreach (string action in actions)
                {
                    //便利
                    for (int i = 0; i < GlobalStatus.MouseButtons.Length; i++)
                    {
                        //按下
                        if (action == GlobalStatus.MouseButtons[i])
                            if (string.Equals(action, GlobalStatus.MouseButtons[i], StringComparison.CurrentCultureIgnoreCase))
                            {
                                dumpbuttonstatus[i] = true;
                            }
                    }
                }
                for (int i = 0; i < dumpbuttonstatus.Length; i++)
                {
                    //按下
                    if (dumpbuttonstatus[i] == true)
                    {
                        if (status[i] != 3)
                        {
                            status[i] = 2;
                        }
                    }
                    //释放
                    else if (status[i] > 1)
                    {
                        status[i] = 1;
                    }
                }

                inputs[0].mi.dwFlags = 0x0;
                //滚轮
                if (status[3] + status[4] == 0 || status[3] + status[4] == 6 || (status[3] * status[4] == 0 && status[3] + status[4] == 3))
                {
                    inputs[0].mi.dwFlags = inputs[0].mi.dwFlags | DLL.MOUSEEVENTF.MOUSEEVENTF_WHEEL;
                    inputs[0].mi.mouseData = item.Wheel;
                }
                //鼠标侧键
                else
                {
                    //侧键1
                    inputs[0].mi.dx = x;
                    inputs[0].mi.dy = y;
                    inputs[0].mi.mouseData = DLL.MOUSEEVENTF.mouseData.XBUTTON1;
                    if (status[3] == 2)
                    {
                        inputs[0].mi.dwFlags = DLL.MOUSEEVENTF.MOUSEEVENTF_MOVE | DLL.MOUSEEVENTF.MOUSEEVENTF_ABSOLUTE | DLL.MOUSEEVENTF.MOUSEEVENTF_XDOWN;
                        status[3] = 3;
                        SendInput(1, inputs, taginputsize);
                    }
                    else if (status[3] == 1)
                    {
                        inputs[0].mi.dwFlags = DLL.MOUSEEVENTF.MOUSEEVENTF_MOVE | DLL.MOUSEEVENTF.MOUSEEVENTF_ABSOLUTE | DLL.MOUSEEVENTF.MOUSEEVENTF_XUP;
                        status[3] = 0;
                        SendInput(1, inputs, taginputsize);
                    }
                    //侧键2
                    inputs[0].mi.dx = x;
                    inputs[0].mi.dy = y;
                    inputs[0].mi.mouseData = DLL.MOUSEEVENTF.mouseData.XBUTTON2;
                    if (status[4] == 2)
                    {
                        inputs[0].mi.dwFlags = DLL.MOUSEEVENTF.MOUSEEVENTF_MOVE | DLL.MOUSEEVENTF.MOUSEEVENTF_ABSOLUTE | DLL.MOUSEEVENTF.MOUSEEVENTF_XDOWN;
                        status[4] = 3;
                        SendInput(1, inputs, taginputsize);
                    }
                    else if (status[4] == 1)
                    {
                        inputs[0].mi.dwFlags = DLL.MOUSEEVENTF.MOUSEEVENTF_MOVE | DLL.MOUSEEVENTF.MOUSEEVENTF_ABSOLUTE | DLL.MOUSEEVENTF.MOUSEEVENTF_XUP;
                        status[4] = 0;
                        SendInput(1, inputs, taginputsize);
                    }
                    //标识位复位
                    inputs[0].mi.dwFlags = 0x0;
                    inputs[0].mi.mouseData = 0;
                }
                //指定要按下哪个按键
                if (status[0] == 2)
                {
                    inputs[0].mi.dwFlags = inputs[0].mi.dwFlags | DLL.MOUSEEVENTF.MOUSEEVENTF_LEFTDOWN;
                    status[0] = 3;
                }
                if (status[0] == 1)
                {
                    inputs[0].mi.dwFlags = inputs[0].mi.dwFlags | DLL.MOUSEEVENTF.MOUSEEVENTF_LEFTUP;
                    status[0] = 0;
                }
                if (status[1] == 2)
                {
                    inputs[0].mi.dwFlags = inputs[0].mi.dwFlags | DLL.MOUSEEVENTF.MOUSEEVENTF_MIDDLEDOWN;
                    status[1] = 3;
                }
                if (status[1] == 1)
                {
                    inputs[0].mi.dwFlags = inputs[0].mi.dwFlags | DLL.MOUSEEVENTF.MOUSEEVENTF_MIDDLEUP;
                    status[1] = 0;
                }
                if (status[2] == 2)
                {
                    inputs[0].mi.dwFlags = inputs[0].mi.dwFlags | DLL.MOUSEEVENTF.MOUSEEVENTF_RIGHTDOWN;
                    status[2] = 3;
                }
                if (status[2] == 1)
                {
                    inputs[0].mi.dwFlags = inputs[0].mi.dwFlags | DLL.MOUSEEVENTF.MOUSEEVENTF_RIGHTUP;
                    status[2] = 0;
                }
                //移动
                inputs[0].mi.dx = x;
                inputs[0].mi.dy = y;
                inputs[0].mi.dwFlags = inputs[0].mi.dwFlags | DLL.MOUSEEVENTF.MOUSEEVENTF_MOVE | DLL.MOUSEEVENTF.MOUSEEVENTF_ABSOLUTE;

                //调试
                if (GlobalStatus.IsDebug == true)
                {
                    Console.WriteLine("X: " + inputs[0].mi.dx + "\tY: " + inputs[0].mi.dy + "\tDelay: " + item.Delay +
                        "ms\tStatus: " + status[0].ToString() + status[1].ToString() + status[2].ToString() + status[3].ToString() +
                        status[4].ToString() + "\tMouseData: " + inputs[0].mi.mouseData + "\tSpend: " + spend.ToString() + "ms" +
                        "\tTargetTime: " + targettime + "\tNow: " + now + "\tErr: " + (now - targettime).ToString() + "ms");
                }

                SendInput(1, inputs, taginputsize);
            }

            //完成后复位
            inputs[0].mi.dx = ((ushort.MaxValue * lastpos.X) / dx) + 1;
            inputs[0].mi.dy = ((ushort.MaxValue * lastpos.Y) / dy) + 1;
            inputs[0].mi.dwFlags = DLL.MOUSEEVENTF.MOUSEEVENTF_MOVE | DLL.MOUSEEVENTF.MOUSEEVENTF_ABSOLUTE;
            inputs[0].mi.mouseData = 0;
            SendInput(1, inputs, taginputsize);
            SendInput(1, inputs, taginputsize);
            if (status[0] > 1)
            {
                inputs[0].mi.dwFlags = DLL.MOUSEEVENTF.MOUSEEVENTF_LEFTUP;
                SendInput(1, inputs, taginputsize);
            }
            if (status[1] > 1)
            {
                inputs[0].mi.dwFlags = DLL.MOUSEEVENTF.MOUSEEVENTF_MIDDLEUP;
                SendInput(1, inputs, taginputsize);
            }
            if (status[2] > 1)
            {
                inputs[0].mi.dwFlags = DLL.MOUSEEVENTF.MOUSEEVENTF_RIGHTUP;
                SendInput(1, inputs, taginputsize);
            }
            if (status[3] > 1)
            {
                inputs[0].mi.dwFlags = DLL.MOUSEEVENTF.MOUSEEVENTF_XUP;
                inputs[0].mi.mouseData = DLL.MOUSEEVENTF.mouseData.XBUTTON1;
                SendInput(1, inputs, taginputsize);
            }
            if (status[4] > 1)
            {
                inputs[0].mi.dwFlags = DLL.MOUSEEVENTF.MOUSEEVENTF_XUP;
                inputs[0].mi.mouseData = DLL.MOUSEEVENTF.mouseData.XBUTTON2;
                SendInput(1, inputs, taginputsize);
            }

            DLL.CloseHandle(timer);
            Console.WriteLine("Done! ");
            this.IsRunning = false;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (this.IsRunning == true)
            {
                this.IsRunning = false;
            }
        }
    }
}
