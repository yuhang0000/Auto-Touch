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
    public partial class 捕捉 : Form
    {
        public 捕捉()
        {
            InitializeComponent();
        }

        public static class 全局变量
        {
            public static int dx = Screen.PrimaryScreen.Bounds.Width;
            public static int dy = Screen.PrimaryScreen.Bounds.Height;
            public static int XXX = 0;
            public static int YYY = 0;
        }

        [DllImport("user32")]
        public static extern int GetCursorPos(out System.Drawing.Point lpPoint);

        //素计时器欸, 实时捕捉光标位置
        private void timer1_Tick(object sender, EventArgs e)
        {
            System.Drawing.Point mp = new System.Drawing.Point();
            GetCursorPos(out mp);
            全局变量.XXX = mp.X;
            全局变量.YYY = mp.Y;
            if (mp.X > 全局变量.dx - this.Width - label1.Height * 2)
            {
                this.Left = mp.X - this.Width - label1.Height;
            }
            else
            {
                this.Left = mp.X + label1.Height;
            }
            if (mp.Y > 全局变量.dy - this.Height - label1.Height * 2)
            {
                this.Top = mp.Y - this.Height - label1.Height;
            }
            else
            {
                this.Top = mp.Y + label1.Height;
            }
            label1.Text = "X: " + mp.X + " Y: " + mp.Y;
            this.Width = label1.Width + label1.Height / 2 * 3;
            this.Height = label1.Height * 2;
            this.TopMost = true;
            //Dispose();
        }

        //初始化窗口大小和位置
        private void 捕捉_Load(object sender, EventArgs e)
        {
            System.Drawing.Point mp = new System.Drawing.Point();
            GetCursorPos(out mp);
            this.Left = mp.X;
            this.Top = mp.Y;
            label1.Top = label1.Height / 2;
            label1.Width = label1.Height / 2;
            this.Width = label1.Width + label1.Height / 2 * 3;
            this.Height = label1.Height * 2;
            this.Opacity = Auto_Touch.Form1.让我看看.Opacity;
        }

        public void 返回()
        {
            Auto_Touch.Form1.让我看看.textBox1.Text = "(" + 全局变量.XXX + "," + 全局变量.YYY + ")";
            Auto_Touch.Form1.让我看看.Activate();
            Auto_Touch.Form1.让我看看.Focus();
            Auto_Touch.Form1.让我看看.WindowState = FormWindowState.Normal;
            this.Close();
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
