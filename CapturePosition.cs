using Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auto_Touch
{
    public partial class CapturePosition : Form
    {
        public CapturePosition()
        {
            InitializeComponent();
            //部署消息钩子
            SSS.LLMouseHook = DLL.SetWindowsHookExA(DLL.IdHook.WH_MOUSE_LL, SSS.HookProc, IntPtr.Zero, 0); //是调用全局变量里的回调函数, 不然他会GC垃圾回收的

        }

        /// <summary>
        /// WH_MOUSE_LL 的回调函数
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public static IntPtr LLMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode >= 0 && GlobalStatus.capturePosition != null && GlobalStatus.capturePosition.IsHandleCreated == true)
            {
                SSS.tagMSLLHOOKSTRUCT = Marshal.PtrToStructure<DLL.tagMSLLHOOKSTRUCT>(lParam);
                Console.WriteLine("X: " + SSS.tagMSLLHOOKSTRUCT.pt.X.ToString() + "\tY: " + SSS.tagMSLLHOOKSTRUCT.pt.Y.ToString() + "\tTime: " + SSS.tagMSLLHOOKSTRUCT.time.ToString());
                GlobalStatus.capturePosition.Invoke(new MethodInvoker( () =>
                {
                    GlobalStatus.capturePosition.SetForm(SSS.tagMSLLHOOKSTRUCT.pt.X, SSS.tagMSLLHOOKSTRUCT.pt.Y);
                }));
            }
            return DLL.CallNextHookEx(SSS.LLMouseHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// 全局变量
        /// </summary>
        public class SSS
        {
            /// <summary>
            /// 荧幕宽度
            /// </summary>
            public static int dx = Screen.PrimaryScreen.Bounds.Width;
            /// <summary>
            /// 荧幕高度
            /// </summary>
            public static int dy = Screen.PrimaryScreen.Bounds.Height;
            /// <summary>
            /// X 坐标
            /// </summary>
            public static int X = 0;
            /// <summary>
            /// Y 坐标
            /// </summary>
            public static int Y = 0;
            /// <summary>
            /// 显示保存的回调函数. 
            /// <para>我的天, 还得单独把这个回调函数存起来</para>
            /// </summary>
            public static DLL.HookProc HookProc = LLMouseProc;
            /// <summary>
            /// 消息钩子的句柄
            /// </summary>
            public static IntPtr LLMouseHook;
            /// <summary>
            /// tagMSLLHOOKSTRUCT 结构体
            /// </summary>
            public static DLL.tagMSLLHOOKSTRUCT tagMSLLHOOKSTRUCT;
        }

        //素计时器欸, 实时捕捉光标位置 (弃用)
        private void timer1_Tick(object sender, EventArgs e)
        {
            System.Drawing.Point mp = new System.Drawing.Point();
            DLL.GetCursorPos(out mp);
            SetForm(mp.X, mp.Y);
            //Dispose();
        }

        /// <summary>
        /// 设置窗体位置, 并储存坐标
        /// </summary>
        /// <param name="X">X 坐标</param>
        /// <param name="Y">Y 坐标</param>
        public void SetForm(int X, int Y)
        {
            SSS.X = X;
            SSS.Y = Y;
            if (X > SSS.dx - this.Width - label1.Height * 2)
            {
                this.Left = X - this.Width - label1.Height;
            }
            else
            {
                this.Left = X + label1.Height;
            }
            if (Y > SSS.dy - this.Height - label1.Height * 2)
            {
                this.Top = Y - this.Height - label1.Height;
            }
            else
            {
                this.Top = Y + label1.Height;
            }
            label1.Text = "X: " + X + " Y: " + Y;
            this.Width = label1.Width + label1.Height / 2 * 3;
            this.Height = label1.Height * 2;
            this.TopMost = true;
        }

        //初始化窗口大小和位置
        private void 捕捉_Load(object sender, EventArgs e)
        {
            System.Drawing.Point mp = new System.Drawing.Point();
            DLL.GetCursorPos(out mp);
            SetForm(mp.X, mp.Y);
            //this.Opacity = Auto_Touch.Form1.让我看看.Opacity;
            this.Opacity = 0.8;
        }

        //返回已捕捉的坐标
        public void 返回()
        {
            /*
            Auto_Touch.Form1.让我看看.textBox1.Text = "(" + 全局变量.XXX + "," + 全局变量.YYY + ")";
            Auto_Touch.Form1.让我看看.Activate();
            Auto_Touch.Form1.让我看看.Focus();
            Auto_Touch.Form1.让我看看.WindowState = FormWindowState.Normal;
            */
            DLL.UnhookWindowsHookEx(SSS.LLMouseHook); //卸载钩子
            GlobalStatus.main.TextBoxPosition.Text = SSS.X + "," + SSS.Y;
            GlobalStatus.main.WindowState = FormWindowState.Normal;
            //GlobalStatus.main.Activate();
            GlobalStatus.main.Focus();
            GlobalStatus.main.TextBoxPosition.Focus();
            GlobalStatus.main.TextBoxPosition_Leave(null, null);
            this.Close();
            this.Dispose();
            GlobalStatus.capturePosition = null;
        }

        //失去焦点
        private void 捕捉_Deactivate(object sender, EventArgs e)
        {
            返回();
        }

        //你按了 Esc 对吧?
        private void 捕捉_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                返回();
            }
        }
    }
}
