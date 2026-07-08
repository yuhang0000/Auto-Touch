using Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auto_Touch
{
    public partial class CapturePosition : Form
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="RunOnce">只捕捉单个点</param>
        public CapturePosition(bool RunOnce)
        {
            InitializeComponent();
            this.sss.RunOnce = RunOnce;
            //准备回调函数
            this.sss.HookProc = this.LLMouseProc;
            this.sss.kbHookProc = this.LLKBProc;
            //部署消息钩子
            this.sss.LLMouseHook = DLL.SetWindowsHookExA(DLL.IdHook.WH_MOUSE_LL, this.sss.HookProc, IntPtr.Zero, 0); //是调用全局变量里的回调函数, 不然他会GC垃圾回收的
            this.sss.LLKBHook = DLL.SetWindowsHookExA(DLL.IdHook.WH_KEYBOARD_LL, this.sss.kbHookProc, IntPtr.Zero, 0); //是调用全局变量里的回调函数, 不然他会GC垃圾回收的
        }

        /// <summary>
        /// WH_MOUSE_LL 的回调函数
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public IntPtr LLMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode >= 0 && GlobalStatus.capturePosition != null && GlobalStatus.capturePosition.IsHandleCreated == true)
            {
                GlobalStatus.capturePosition.Invoke(new MethodInvoker( () =>
                {
                    GlobalStatus.capturePosition.SetMouseInfo(wParam, lParam );
                }));
            }
            return DLL.CallNextHookEx(this.sss.LLMouseHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// WH_KEYBOARD_LL 的回调函数
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public IntPtr LLKBProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode >= 0 && GlobalStatus.capturePosition != null && GlobalStatus.capturePosition.IsHandleCreated == true)
            {
                if((int)wParam == 0x0100) //WM_KEYDOWN = 0x0100
                {
                    this.sss.tagKBDLLHOOKSTRUCT = Marshal.PtrToStructure<DLL.tagKBDLLHOOKSTRUCT>(lParam);
                    //Console.WriteLine(this.sss.tagKBDLLHOOKSTRUCT.vkCode);
                    if (this.sss.tagKBDLLHOOKSTRUCT.vkCode == (int)Keys.Escape)
                    {
                        GlobalStatus.capturePosition.返回();
                    }
                }
            }

            return DLL.CallNextHookEx(this.sss.LLKBHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// 全局变量
        /// </summary>
        public class SSS
        {
            public SSS()
            {
                this.mousebuttonstatus = new MouseButtonStatus();
                this.mousebuttonstatus.Action = new StringBuilder();
            }

            /// <summary>
            /// 荧幕宽度
            /// </summary>
            public int dx = Screen.PrimaryScreen.Bounds.Width;
            /// <summary>
            /// 荧幕高度
            /// </summary>
            public int dy = Screen.PrimaryScreen.Bounds.Height;
            /// <summary>
            /// X 坐标
            /// </summary>
            public int X = 0;
            /// <summary>
            /// Y 坐标
            /// </summary>
            public int Y = 0;
            /// <summary>
            /// 延时
            /// </summary>
            public int delay = 0;
            /// <summary>
            /// 上一次记录的时间戳
            /// </summary>
            public int LastTick = -1;
            /// <summary>
            /// 滚轮位置
            /// </summary>
            public short Wheel = 0;
            /// <summary>
            /// 鼠标动作
            /// </summary>
            public string MouseAction = "None";
            /// <summary>
            /// 只捕捉单个点
            /// </summary>
            public bool RunOnce = true;
            /// <summary>
            /// 是否触发关闭事件
            /// </summary>
            public bool IsClose = false;
            /// <summary>
            /// 捕获到按键状态了吗?
            /// </summary>
            public bool Actioned = true;
            /// <summary>
            /// 暂存轨迹信息, 给轨迹捕捉模式用的
            /// </summary>
            public List<ListViewItem> Items = new List<ListViewItem>();

            /// <summary>
            /// 暂存鼠标按键状态
            /// </summary>
            public class MouseButtonStatus
            {
                /// <summary>
                /// 暂存鼠标动作
                /// </summary>
                public StringBuilder Action;
                /// <summary>
                /// 鼠标左键
                /// </summary>
                public bool MouseLeft = false;
                /// <summary>
                /// 鼠标中间
                /// </summary>
                public bool MouseMiddle = false;
                /// <summary>
                /// 鼠标右键
                /// </summary>
                public bool MouseRight = false;
                /// <summary>
                /// 鼠标滚轮
                /// </summary>
                public bool MouseWheel = false;
                /// <summary>
                /// 鼠标侧键1
                /// </summary>
                public bool MouseXButton1 = false;
                /// <summary>
                /// 鼠标侧键2
                /// </summary>
                public bool MouseXButton2 = false;
            }
            public MouseButtonStatus mousebuttonstatus;

            /// <summary>
            /// 创建委托, 给LLMouseHook
            /// </summary>
            public DLL.HookProc HookProc;
            /// <summary>
            /// 保留消息钩子的句柄
            /// </summary>
            public IntPtr LLMouseHook;
            /// <summary>
            /// 创建委托, 给LLKBHook
            /// </summary>
            public DLL.HookProc kbHookProc;
            /// <summary>
            /// 保留消息钩子的句柄
            /// </summary>
            public IntPtr LLKBHook;
            /// <summary>
            /// tagMSLLHOOKSTRUCT 结构体
            /// </summary>
            public DLL.tagMSLLHOOKSTRUCT tagMSLLHOOKSTRUCT;
            /// <summary>
            /// tagKBDLLHOOKSTRUCT 结构体
            /// </summary>
            public DLL.tagKBDLLHOOKSTRUCT tagKBDLLHOOKSTRUCT;
        }
        public SSS sss = new SSS();

        //素计时器欸, 实时捕捉光标位置 (弃用)
        /*private void timer1_Tick(object sender, EventArgs e)
        {
            System.Drawing.Point mp = new System.Drawing.Point();
            DLL.GetCursorPos(out mp);
            SetForm(mp.X, mp.Y, 0, 0);
        }*/

        /// <summary>
        /// 处理鼠标信息
        /// </summary>
        /// <param name="wParam">鼠标按键事件</param>
        /// <param name="lParam">鼠标拓展信息</param>
        public void SetMouseInfo(IntPtr wParam, IntPtr lParam)
        {
            this.sss.tagMSLLHOOKSTRUCT = Marshal.PtrToStructure<DLL.tagMSLLHOOKSTRUCT>(lParam);

            this.sss.Actioned = true; 
            int TimeStamp = (int)Command.GetTimeStampMs(); //获取当前时间, 毫秒
            this.sss.X = this.sss.tagMSLLHOOKSTRUCT.pt.X;
            this.sss.Y = this.sss.tagMSLLHOOKSTRUCT.pt.Y;
            this.sss.Wheel = 0;
            switch ((int)wParam)
            {
                case 0x0201: //鼠标左键按下
                    this.sss.MouseAction = "MouseLeft";
                    this.sss.mousebuttonstatus.MouseLeft = true;
                    break;
                case 0x0204: //鼠标右键按下
                    this.sss.MouseAction = "MouseRight";
                    this.sss.mousebuttonstatus.MouseRight = true;
                    break;
                case 0x0207: //鼠标中间按下
                    this.sss.MouseAction = "MouseMiddle";
                    this.sss.mousebuttonstatus.MouseMiddle = true;
                    break;
                case 0x020B: //鼠标侧键按下
                    int xbuttondown = (short)(this.sss.tagMSLLHOOKSTRUCT.mouseData >> 16);
                    if(xbuttondown == 0x0001)
                    {
                        this.sss.MouseAction = "MouseXButton1";
                    this.sss.mousebuttonstatus.MouseXButton1 = true;
                    }
                    else if(xbuttondown == 0x0002)
                    {
                        this.sss.MouseAction = "MouseXButton2";
                    this.sss.mousebuttonstatus.MouseXButton2 = true;
                    }
                    break;
                case 0x0202: //鼠标左键松开
                    this.sss.mousebuttonstatus.MouseLeft = false;
                    break;
                case 0x0205: //鼠标右键松开
                    this.sss.mousebuttonstatus.MouseRight = false;
                    break;
                case 0x0208: //鼠标中键松开
                    this.sss.mousebuttonstatus.MouseMiddle = false;
                    break;
                case 0x020C: //鼠标侧键按下
                    int xbuttonup = (short)(this.sss.tagMSLLHOOKSTRUCT.mouseData >> 16);
                    if (xbuttonup == 0x0001)
                    {
                        this.sss.MouseAction = "MouseXButton1";
                        this.sss.mousebuttonstatus.MouseXButton1 = false;
                    }
                    else if (xbuttonup == 0x0002)
                    {
                        this.sss.MouseAction = "MouseXButton2";
                        this.sss.mousebuttonstatus.MouseXButton2 = false;
                    }
                    break;
                case 0x020A: //鼠标滚轮
                    //this.sss.MouseAction = "MouseWheel";
                    //this.sss.mousebuttonstatus.Action.Append("MouseWheel|");
                    this.sss.Actioned = false;
                    this.sss.Wheel = (short)(this.sss.tagMSLLHOOKSTRUCT.mouseData >> 16);
                    break;
                default: //什么也不是
                    this.sss.MouseAction = "None";
                    this.sss.Actioned = false;
                    break;
            }
            if(this.sss.LastTick == -1)
            {
                this.sss.LastTick = TimeStamp;
            }
            this.sss.delay = TimeStamp - this.sss.LastTick;
            this.sss.LastTick = TimeStamp;
            //Console.WriteLine("X: " + X + "\tY: " + Y + "\tWheel: " + Wheel + "\tAction: " + WM_Mouse + "\tTime: " + TimeStamp + "(" + this.sss.delay + ")");

            //设置窗体位置
            if (this.sss.RunOnce == true)
            {
                SetFormSize(this.sss.X, this.sss.Y);
            }

            //捕捉到了按键状态并且处于单点模式就返回
            if (this.sss.RunOnce == true && this.sss.Actioned == true)
            {
                返回();
            }
            //轨迹模式
            else if(this.sss.RunOnce == false)
            {
                //查询当前鼠标按键状态
                if(this.sss.mousebuttonstatus.MouseLeft == true)
                {
                    this.sss.mousebuttonstatus.Action.Append("MouseLeft|");
                }
                if(this.sss.mousebuttonstatus.MouseRight == true)
                {
                    this.sss.mousebuttonstatus.Action.Append("MouseRight|");
                }
                if(this.sss.mousebuttonstatus.MouseMiddle == true)
                {
                    this.sss.mousebuttonstatus.Action.Append("MouseMiddle|");
                }
                if(this.sss.mousebuttonstatus.MouseXButton1 == true)
                {
                    this.sss.mousebuttonstatus.Action.Append("MouseXButton1|");
                }
                if(this.sss.mousebuttonstatus.MouseXButton2 == true)
                {
                    this.sss.mousebuttonstatus.Action.Append("MouseXButton2|");
                }
                if(sss.mousebuttonstatus.Action.Length > 0)
                {
                    sss.mousebuttonstatus.Action.Remove(sss.mousebuttonstatus.Action.Length - 1, 1);
                }
                else
                {
                    sss.mousebuttonstatus.Action.Append("None");
                }

                ListViewItem item = new ListViewItem();
                item.Text = this.sss.Items.Count.ToString();
                item.SubItems.Add(this.sss.X + "," + this.sss.Y); //坐标
                item.SubItems.Add(this.sss.delay.ToString() + "ms"); //延时
                item.SubItems.Add(this.sss.Wheel.ToString()); //滚轮
                item.SubItems.Add(this.sss.mousebuttonstatus.Action.ToString()); //按键动作
                this.sss.Items.Add(item);

                sss.mousebuttonstatus.Action.Clear();
            }
        }

        /// <summary>
        /// 处理窗体位置和尺寸
        /// </summary>
        public void SetFormSize(int X, int Y)
        {
            
            if (X > this.sss.dx - this.Width - label1.Height * 2)
            {
                this.Left = X - this.Width - label1.Height;
            }
            else
            {
                this.Left = X + label1.Height;
            }
            if (Y > this.sss.dy - this.Height - label1.Height * 2)
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
            if (this.sss.RunOnce == true)
            {
                System.Drawing.Point mp = new System.Drawing.Point();
                DLL.GetCursorPos(out mp);
                //SetForm(mp.X, mp.Y, 0, 0);
                SetFormSize(mp.X, mp.Y);
                this.Opacity = 0.8;
            }
            else
            {
                this.Hide();
            }
        }

        //返回已捕捉的坐标
        public void 返回()
        {
            if(this.sss.IsClose == false)
            {
                this.sss.IsClose = true;
            }
            else
            {
                return;
            }

            //卸载钩子
            DLL.UnhookWindowsHookEx(this.sss.LLMouseHook);
            DLL.UnhookWindowsHookEx(this.sss.LLKBHook);
            //单点捕捉
            if (this.sss.RunOnce == true)
            {
                GlobalStatus.main.ComboBoxAction.Text = this.sss.MouseAction;
                GlobalStatus.main.TextBoxPosition.Text = this.sss.X + "," + this.sss.Y;
            }
            //轨迹捕捉
            else
            {
                GlobalStatus.main.listView1.Items.AddRange(this.sss.Items.ToArray());
                /*foreach (ListViewItem item in this.sss.Items)
                {
                    GlobalStatus.main.listView1.Items.Add(item);
                }*/
                GlobalStatus.main.UpdateItemIndex();
            }
            GlobalStatus.main.WindowState = FormWindowState.Normal;
            //GlobalStatus.main.Activate();
            GlobalStatus.main.Focus();
            GlobalStatus.main.TextBoxPosition.Focus();
            GlobalStatus.main.TextBoxPosition_Leave(null, null);
            GlobalStatus.main.BtnCapturePosition.Enabled = true;
            GlobalStatus.main.BtnCaptureTrajectory.Enabled = true;
            GlobalStatus.main.Disable_listView1_ItemSelectionChanged = false;
            GlobalStatus.main.StatusBarText.Text = "就绪";
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
