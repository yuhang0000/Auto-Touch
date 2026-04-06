using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auto_Touch
{
    public partial class Form1 : Form
    {
        //全局访问 Form1 实例
        public static Form1 让我看看 { get; private set; }

        //启动参数
        public Form1(String[] args = null)
        {
            if (args != null && args.Length > 0)
            {
                String command = "";
                foreach (var item in args)
                {
                    command = command + " " + item.ToString();
                }

                //OMG，居然可以在方法里再嵌套个新方法
                void err()
                {
                    String name = "Auto Touch.exe";
                    try
                    {
                        name = Application.ExecutablePath;
                        name = name.Replace(Application.StartupPath + "\\", "");
                    }
                    catch
                    {
                        name = "Auto Touch.exe";
                    }
                    SystemSounds.Hand.Play();
                    MessageBox.Show("参数错误: \r\n\"" + name + command + "\"", "Oops! ");
                    help();
                }

                try
                {
                    if (args[0] == "-p" || args[0] == "-P" || args[0] == "profile" || args[0] == "profiles" || 
                        args[0] == "/p" || args[0] == "/P")
                    {
                        readini(false);
                        String xy;
                        String[] ini = { };
                        String[] xys = { };
                        if (全局变量.ini.Length < 1)
                        {
                            MessageBox.Show("当前配置文档为空! ","Oops! ");
                            System.Environment.Exit(0);
                        }
                        if (args.Length < 2)
                        {
                            ini = 全局变量.ini[全局变量.ini.Length].Split(';');
                            
                        }
                        else
                        {
                            foreach(String temp in 全局变量.ini)
                            {
                                ini = temp.Split(';');
                                if (ini[0] == args[1])
                                {
                                    break;
                                }
                            }
                            if (ini[0] != args[1])
                            {
                                MessageBox.Show("找不到该项目: " +  args[1],"Oops! ");
                                System.Environment.Exit(0);
                            }
                        }
                        
                        xy = ini[1];
                        if(xy.IndexOf("(") != -1)
                        {
                            xy = xy.Replace("(", "");
                        }
                        if(xy.IndexOf(")") != -1)
                        {
                            xy = xy.Replace(")", "");
                        }
                        if(xy.IndexOf(" ") != -1)
                        {
                            xy = xy.Replace(" ", "");
                        }
                        xys = xy.Split(',');
                        Array.Resize(ref args, 3);
                        args[0] = xys[0];
                        args[1] = xys[1];
                        args[2] = ini[2];
                    }
                    if (args[0] == "ver" || args[0] == "version")
                    {
                        about();
                    }
                    else if (args[0] == "help" || args[0] == "/?" || args[0] == "-h")
                    {
                        help();
                    }
                    else
                    {
                        全局变量.XXX = int.Parse(args[0]);
                        全局变量.YYY = int.Parse(args[1]);
                        String time = "0";
                        if (args.Length > 2)    //带了时间
                        {
                            time = args[2];
                            if (time.IndexOf(":")  != -1)
                            {
                                if (HHMMSS(time) == "err") 
                                {
                                    help();
                                };
                            }
                            else
                            {
                                run(int.Parse(time) * 1000);
                            }
                        }
                        //Console.WriteLine(args.ToString());
                        else
                        {
                            run(0);
                        }
                    }
                    System.Environment.Exit(0);
                }
                catch
                {
                    err();
                    System.Environment.Exit(0);
                }
            }
            else
            {
                //help() ;
            }

            InitializeComponent();
            让我看看 = this;
        }

        [DllImport("user32")]
        public static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32")]
        public static extern int GetCursorPos(out System.Drawing.Point lpPoint);
        [DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        //计时器相关的
        [DllImport("winmm")]
        static extern uint timeGetTime();

        public static class 全局变量
        {
            public static String 版本 = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            public static DateTime 编译时间 = System.IO.File.GetLastWriteTime(typeof(全局变量).Assembly.Location);
            public static int XXX = 0;
            public static int YYY = 0;
            public static int 状态 = 1;
            public static String[] ini = new String[0];
            //public static String[] proj = { };
            public static bool 闪 = false;
            public static int comboboxindex = 0;
            public static float dpi = 1;
        }

        //读取配置文档
        public void readini(bool ui = true)
        {
            //if (File.Exists(@"./Auto Touch.ini") == true) //通过命令行运行, 没指定工作路径, 是打不开的
            if (File.Exists(Application.StartupPath + @"/Auto Touch.ini") == true)
            {
                try
                {
                    //全局变量.ini = File.ReadAllLines(@"./Auto Touch.ini");
                    全局变量.ini = File.ReadAllLines(Application.StartupPath + @"/Auto Touch.ini");
                    //Console.WriteLine(全局变量.ini[0]);
                    if (ui == true)
                    {
                        this.textBox1.Text = 全局变量.ini[全局变量.ini.Length - 1].Split(';')[1];
                    }
                    foreach (String inis in 全局变量.ini)
                    {
                        string[] ini = inis.Split(';');
                        //全局变量.proj.Append(ini[0]);
                        if(ui == true){
                            this.comboBox1.Items.Add(ini[0]);
                        }
                        //this.comboBox1.SelectedIndex = 1;
                        //this.textBox1.Text = ini[1];
                        //this.textBox2.Text = ini[2];
                    }
                }
                catch (Exception ex)
                {
                    全局变量.ini = new String[0];
                    Console.WriteLine("读取失败: " + ex.Message);
                }
            }
        }

        public void help()
        {
            MessageBox.Show("Auto Touch.exe : \r\n\r\n\thelp\t\t获取帮助。\r\n\tver\t\t检查版本信息。\r\n\t" +
                "X[int] Y[int]\t点击 (X,Y) 坐标位置。\r\n\tTime[int]\t\t延时运行, 单位: 秒。\r\n\tTime[String]" +
                "\t延时运行, 单位: HH:MM:SS。\r\n\tprofile [String]\t以指定预设项目运行。\r\n\r\n示例: " +
                "\tAuto Touch.exe 1920 1080 60\r\n\t( 等待1分钟后, 点击 (1920,1080) 所处位置。)\r\n\n" +
                "\tAuto Touch.exe 1920 1080 10:00:00\r\n\t( 等待10小时后, 点击 (1920,1080) 所处位置。)\r\n\n" +
                "\tAuto Touch.exe profile #1\r\n\t( 读取预设项目 \"#1\" 并运行。)", "帮助: ");
        }

        public void about()
        {
            MessageBox.Show("Auto Touch\r\nBy: yuhang0000\r\nBuild Time: " + 全局变量.编译时间
                + "\r\n版本: " + 全局变量.版本, "关于: ");
        }

        //一打开就运行
        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = " v"+全局变量.版本;
            toolStripStatusLabel3.Text = 全局变量.编译时间.ToShortDateString().ToString();
            this.Width = 311;
            readini();
            //获取 dpi
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiX = graphics.DpiX;
                全局变量.dpi = (dpiX / 120);
            }
            //MessageBox.Show(全局变量.dpi.ToString());
            //MessageBox.Show(this.MaximumSize.ToString() + "\r\n" + this.MinimumSize.ToString());
            /*Size max = new Size( (int)( (float)this.MaximumSize.Width * 全局变量.dpi),
                (int)((float)this.MaximumSize.Height * 全局变量.dpi) );
            Size min = new Size( (int)( (float)this.MinimumSize.Width * 全局变量.dpi),
                (int)((float)this.MinimumSize.Height * 全局变量.dpi) ); */
            Size max = new Size( (int)( 588 * 全局变量.dpi),
                (int)( (159 * 全局变量.dpi) + (1 - 全局变量.dpi) * 16 ) );
            Size min = new Size( (int)( 311 * 全局变量.dpi),
                (int)( (159 * 全局变量.dpi) + (1 - 全局变量.dpi) * 16 ) );
            //MessageBox.Show(((1 - 全局变量.dpi) * 16).ToString());
            this.MaximumSize = max;
            this.MinimumSize = min;
            this.Size = min;
            //MessageBox.Show(this.MaximumSize.ToString() + "\r\n" + this.MinimumSize.ToString());
        }

        async public void run(int time = 0)
        {
            //我服了, 为甚么这里也会溢出
            int 纠正 = 0;
            if((int)timeGetTime() < 0)
            {
                纠正 = (int)timeGetTime();
            }
            time = ( (int)timeGetTime() - 纠正) + time;
            //MessageBox.Show(time.ToString() + "\n" + (int)timeGetTime());
            
            //如果 time 為负数
            if(time < 0)
            {
                oops("你数值调太大了", this.textBox2);
                return;
            }
            while (time > ((int)timeGetTime() - 纠正) ) {
                //time = time - 1000;
                if (全局变量.状态 == 2)
                {
                    this.toolStripStatusLabel5.Text = (time - ( (int)timeGetTime() - 纠正) ).ToString() + "ms";
                    await Task.Delay(10);
                }
                else if (全局变量.状态 == 1)
                {
                    Thread.Sleep(10);
                }
                if (全局变量.状态 == 0)
                {
                    try
                    {
                        this.button3.Enabled = true;
                        this.button3.Focus();
                        this.textBox1.Enabled = true;
                        //SelectNextControl(ActiveControl, false, true, true, true);
                        //SelectNextControl(ActiveControl, false, true, true, true);
                        this.button2.Enabled = true;
                        this.button3.Text = "开始";
                    }
                    catch { }
                    return;
                }
            }
            System.Drawing.Point mp = new System.Drawing.Point();
            GetCursorPos(out mp);
            int XX = mp.X;
            int YY = mp.Y;
            SetCursorPos(全局变量.XXX, 全局变量.YYY);
            mouse_event(0x0002 | 0x0004, 0, 0, 0, 0);
            SetCursorPos(XX, YY);
            this.Close();
            //System.Environment.Exit(0);
        }

        //捕捉光标位置
        private void button2_Click(object sender, EventArgs e)
        {
            this.button3.Focus();
            this.WindowState = FormWindowState.Minimized;
            CapturePosition pos = new CapturePosition();
            pos.Show();
            pos.Activate();
            pos.Focus();
            //SelectNextControl(ActiveControl, true, true, true, true);
        }

        //关鱼
        private void statusStrip1_Click(object sender, EventArgs e)
        {
            about();
        }

        //帮助
        private void button1_Click(object sender, EventArgs e)
        {
            help();
        }

        //报错放这里
        public void oops(String text = "", object control = null)
        {
            SystemSounds.Hand.Play();
            if (text != "")
            {
                text = ", " + text;
            }
            if (control is Control)
            {
                Control con = control as Control;
                if(con == null)
                {
                    con = this.textBox1;
                }
                MessageBox.Show("输入有误" + text + ": \r\n" + con.Text, "Oops!");
            }
            else
            {
                MessageBox.Show("输入有误" + text + ": \r\n" + control, "Oops!");
            }
            if (全局变量.状态 == 2)
            {
                this.button2.Enabled = true;
                this.textBox1.Enabled = true;
                this.button3.Text = "开始";
            }
            全局变量.状态 = 0;
        }

        //是 HH:MM:SS 格式用介个
        public string HHMMSS(String posttime = "0")
        {
            String[] times;
            int time = 0;
            try
            {
                times = posttime.Split(':');
                //有没有按下 Ctrl
                if (Control.ModifierKeys == Keys.Control)   //这是到哪时截至
                {
                    SystemSounds.Beep.Play();   //不知道, 先 Beep 一下提示你按下了 ctrl
                    DateTime now = DateTime.Now;
                    if (times.Length == 2)
                    {
                        Array.Resize(ref times, times.Length + 1);
                        /*times[2] = times[1];
                        times[1] = times[0];
                        times[0] = now.Hour.ToString();*/
                        times[2] = "0";
                    }
                    else if (times.Length > 3)
                    {
                        oops("这不是正确的时间格式", posttime);
                        return "err";
                    }
                    DateTime totime = new DateTime(now.Year, now.Month, now.Day,
                        int.Parse(times[0]), int.Parse(times[1]), int.Parse(times[2]));
                    //好麻烦，时间戳单独一个方法
                    TimeSpan aaa = totime - now;
                    Console.WriteLine(now);
                    Console.WriteLine(totime);
                    time = (int)aaa.TotalMilliseconds;
                    if (aaa.TotalMilliseconds < 0)   //时间晚了就多加一天
                    {
                        time = (int)aaa.TotalMilliseconds + 3600 * 24 * 1000;
                    }
                }
                else    //这是倒计时
                {
                    if (times.Length == 2)
                    {
                        time = ((int.Parse(times[0]) * 3600) + (int.Parse(times[1]) * 60)) * 1000;
                        //time = ((int.Parse(times[0]) * 60) + int.Parse(times[1])) * 1000;
                    }
                    else if (times.Length == 3)
                    {
                        time = ((int.Parse(times[0]) * 3600) + (int.Parse(times[1]) * 60) + int.Parse(times[2])) * 1000;
                    }
                    else if (times.Length > 3)
                    {
                        oops("这不是正确的时间格式", posttime);
                        return "err";
                    }
                    if (time < 0)
                    {
                        oops("你数值调太大了", posttime);
                        return "err";
                    }
                }
                run(time);
            }
            catch
            {
                oops("这不是正确的时间格式", posttime);
                return "err";
            }
            return "ok";
        }

        //开始运行
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.button3.Text == "开始")
            {
                this.button3.Text = "停止";
                this.button2.Enabled = false;
                this.textBox1.Enabled = false;
                全局变量.状态 = 2;
                String xy = textBox1.Text;

                try
                {
                    xy = xy.Substring(0, xy.Length - 1);
                    xy = xy.Substring(1, xy.Length - 1);
                    全局变量.XXX = int.Parse(xy.Substring(0, xy.IndexOf(",")));
                    全局变量.YYY = int.Parse(xy.Substring(xy.IndexOf(",") + 1));
                    Console.WriteLine(全局变量.XXX + "," + 全局变量.YYY);
                    if(this.textBox2.Text == "")    //时间不能为空的
                    {
                        this.textBox2.Text = "0";
                    }
                    //给定时间用这个
                    if (this.textBox2.Text.IndexOf(":") > -1)
                    {
                        if(HHMMSS(this.textBox2.Text) == "err")
                        {
                            return;
                        }
                    }
                    //倒计时用这个
                    else
                    {
                        run(int.Parse(this.textBox2.Text) * 1000);
                    }
                }
                catch
                {
                    oops("",this.textBox1);
                    return;
                }
            }
            else
            {
                全局变量.状态 = 0;
                this.button3.Enabled = false;
                //this.button3.Text = "开始";
            }
        }

        //文本纠正, 为了防呆而防呆, 不要键入中文标点符号!!!
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int aaa = textBox1.SelectionStart;
            if (textBox1.Text.IndexOf(" ") > 0)
            {
                aaa--;
            }
            textBox1.Text = textBox1.Text.Replace(" ", "");
            textBox1.Text = textBox1.Text.Replace("（", "(");
            textBox1.Text = textBox1.Text.Replace("）", ")");
            textBox1.Text = textBox1.Text.Replace("，", ",");
            textBox1.SelectionStart = aaa;
        }

        //Exit
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //高级选项
        private void button5_Click(object sender, EventArgs e)
        {
            if (this.Width < 444 * 全局变量.dpi)
            {
                this.Width = (int)(588 * 全局变量.dpi);
                this.toolStripStatusLabel4.Visible = true;
                this.toolStripStatusLabel5.Visible = true;
                this.comboBox1.Visible = true;
                this.textBox2.Visible = true;
                this.label2.Visible = true;
                this.button6.Visible = true;
                this.button7.Visible = true;
                this.button4.Visible = true;
                if(this.comboBox1.SelectedIndex != 0)
                {
                    this.button7.Enabled = true;
                }
                //this.button8.Enabled = true;
            }
            else
            {
                this.Width = (int)(311 * 全局变量.dpi);
                this.toolStripStatusLabel4.Visible = false;
                this.toolStripStatusLabel5.Visible = false;
                this.comboBox1.Visible = false;
                this.textBox2.Visible = false;
                this.label2.Visible = false;
                this.button6.Visible = false;
                this.button7.Visible = false;
                this.button4.Visible = false;
                this.button7.Enabled = false;
                //this.button8.Enabled = false;
            }
        }

        //时间，仅允许数字，和 ":"
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //我的天呐，不能用双引号
            if (e.KeyChar != '\b' && e.KeyChar != ':' && e.KeyChar != ';' && e.KeyChar != '-' && e.KeyChar != ' ' 
                && Char.IsDigit(e.KeyChar) == false)
            {
                e.Handled = true;
                SystemSounds.Beep.Play();
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int start = this.textBox2.SelectionStart;
            if(this.textBox2.Text.IndexOf(" ") != -1)
            {
                this.textBox2.Text = this.textBox2.Text.Replace(" ", ":");
            }
            if(this.textBox2.Text.IndexOf(";") != -1)
            {
                this.textBox2.Text = this.textBox2.Text.Replace(";", ":");
            }
            if(this.textBox2.Text.IndexOf("-") != -1)
            {
                this.textBox2.Text = this.textBox2.Text.Replace("-", ":");
            }
            this.textBox2.SelectionStart = start;
        }

        //重命名
        public void rename(String text = null)
        {
            if(text == "新建" || text == "" || text == " " || this.textBox1.Text == "" || 
                this.textBox1.Text == " " || this.textBox2.Text == "" || this.textBox2.Text == " ")
            {
                return;
            }
            //if (this.comboBox1.SelectedIndex >= 0 && 全局变量.comboboxindex > 0)
            if (全局变量.comboboxindex > 0)
            {
                String[] ini = 全局变量.ini[全局变量.comboboxindex - 1].Split(';');
                ini[0] = this.comboBox1.Text;
                ini[1] = this.textBox1.Text;
                ini[2] = this.textBox2.Text;
                全局变量.ini[全局变量.comboboxindex - 1] = String.Join(";", ini);
                this.comboBox1.Items[全局变量.comboboxindex] = this.comboBox1.Text;
                if (text == null)
                {
                    this.comboBox1.SelectedIndex = 全局变量.comboboxindex;
                    this.comboBox1.Text = ini[0];
                }
            }
        }

        //选择框被更改
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Console.WriteLine(comboBox1.Text);
            全局变量.comboboxindex = comboBox1.SelectedIndex;
            if (comboBox1.SelectedIndex <= 0)
            {
                this.button6.Text = "保存";
                this.button7.Enabled = false;
                return;
            }
            else
            {
                this.button7.Enabled = true;
                this.button6.Text = "新建";
                String[] ini = 全局变量.ini[this.comboBox1.SelectedIndex - 1].Split(';');
                this.comboBox1.Text = ini[0];
                this.textBox1.Text = ini[1];
                this.textBox2.Text = ini[2];
                //rename();
            }
        }

        async public void 闪烁(Control aaa)
        {
            SystemSounds.Beep.Play();
            int top = aaa.Top;
            aaa.Focus();
            //SelectNextControl(ActiveControl, false, true, true, true);
            if (全局变量.闪 == false)
            {
                全局变量.闪 = true;
                aaa.Top = 65536;
                await Task.Delay(100);
                aaa.Top = top;
                await Task.Delay(100);
                aaa.Top = 65536;
                await Task.Delay(100);
                aaa.Top = top;
                await Task.Delay(100);
                aaa.Top = 65536;
                await Task.Delay(100);
                aaa.Top = top;
                await Task.Delay(100);
                aaa.Top = 65536;
                await Task.Delay(100);
                aaa.Top = top;
                await Task.Delay(100);
                aaa.Top = 65536;
                await Task.Delay(100);
                aaa.Top = top;
                await Task.Delay(100);
                全局变量.闪 = false;
            }
        }

        //保存 & 新建
        private void button6_Click(object sender, EventArgs e)
        {
            if (this.button6.Text == "新建")
            {
                //创建的时候，不想要保存当前的选项，可以把 rename() 给嘎了
                //rename();
                this.comboBox1.Focus();
                this.comboBox1.SelectedIndex = 0;
                this.comboBox1.SelectAll();
            }
            else
            {
                if(this.comboBox1.Text == "新建" || this.comboBox1.Text == "" || this.comboBox1.Text == " ")
                {
                    闪烁(this.comboBox1);
                    return;
                }
                if(this.textBox1.Text == "" || this.textBox1.Text == " ")
                {
                    闪烁(this.textBox1); 
                    return;
                }
                if(this.textBox2.Text == "" || this.textBox2.Text == " ")
                {
                    闪烁(this.textBox2); 
                    return;
                }
                Array.Resize(ref 全局变量.ini,全局变量.ini.Length + 1);
                全局变量.ini[全局变量.ini.Length - 1] = this.comboBox1.Text + ";" + this.textBox1.Text + ";" + 
                    this.textBox2.Text;
                this.comboBox1.Items.Add(this.comboBox1.Text);
                this.comboBox1.SelectedIndex = 全局变量.ini.Length;
            }
        }

        //不要在文本框输入 ";"
        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            if(this.comboBox1.Text.IndexOf(";") != -1)
            {
                int aaa = this.comboBox1.SelectionStart;
                SystemSounds.Beep.Play();
                this.comboBox1.Text = this.comboBox1.Text.Replace(";","") ;
                this.comboBox1.SelectionStart = aaa - 1;
            }
        }

        //关闭前先保存
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                rename();
                //File.WriteAllLines(@"./Auto Touch.ini", 全局变量.ini);
                if (全局变量.ini.Length > 0)
                {
                    File.WriteAllLines(Application.StartupPath + @"/Auto Touch.ini", 全局变量.ini);
                }
                else   //这里忘记修了 o(≧口≦)o
                {
                    if(File.Exists(Application.StartupPath + @"/Auto Touch.ini") == true)
                    {
                        File.Delete(Application.StartupPath + @"/Auto Touch.ini");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败哩: \r\n" + ex.Message, "(＃°Д°) !!!  失败");
            }
        }

        //删除
        private void button7_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex == 0)
            {
                return;
            }
            if(MessageBox.Show("欸，要删除 " + 全局变量.ini[this.comboBox1.SelectedIndex - 1].Split(';')[0] +
                " 吗? ", "删除",MessageBoxButtons.OKCancel,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)
                == DialogResult.OK){
                int index = 全局变量.comboboxindex;
                if(index == 1)
                {
                    this.button2.Focus();
                }
                this.comboBox1.SelectedIndex = 全局变量.comboboxindex - 1;
                this.comboBox1.Items.RemoveAt(index);
                string[] ini = { };
                Array.Resize(ref ini, 全局变量.ini.Length - 1);
                int a = 0;
                int b = 0;
                while(a < 全局变量.ini.Length)
                {
                    if(a != (index - 1))
                    {
                        ini[b] = 全局变量.ini[a];
                        b++;
                    }
                    a++;
                }
                全局变量.ini = ini;
            }
        }

        //失去焦点来重命名
        private void comboBox1_Leave(object sender, EventArgs e)
        {
            rename();
        }

        //改变选项来重命名
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            rename(this.comboBox1.Text);
        }

        //Exit * 2
        private void button8_Click(object sender, EventArgs e)
        {
            if (this.button3.Text == "停止")
            {
                全局变量.状态 = 0;
                this.button3.Enabled = false;
            }
            else {
                this.Close();
            }
        }

        //获得焦点就全选
        private void textBox2_Click(object sender, EventArgs e)
        {
            if(this.textBox2.Text == "0")
            {
                this.textBox2.SelectAll();
            }
        }
    }
}
