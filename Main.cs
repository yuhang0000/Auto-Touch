using Commands;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Commands.DLL;

namespace Auto_Touch
{
    public partial class Main : Form
    {
        /// <summary>
        /// 是否新建过
        /// </summary>
        public bool IsNew
        {
            get
            {
                return this._IsNew;
            }
            set
            { 
                if(value == true)
                {
                    this.BtnAssumptionNew.Enabled = false;
                    this.BtnAssumptionDel.Enabled = false;
                }
                else
                {
                    this.BtnAssumptionNew.Enabled = true;
                    this.BtnAssumptionDel.Enabled = true;
                }
                _IsNew = value;
            }
        }
        /// <summary>
        /// 是否编辑过
        /// </summary>
        public bool IsEdit
        {
            get
            {
                return this._IsEdit;
            }
            set
            {
                this.BtnAssumptionSave.Enabled = value;
                this._IsEdit = value;
            }
        }
        bool _IsNew = false;
        bool _IsEdit = false;
        /// <summary>
        /// 是否附加到控制台, -1 = 没有检查, 0 = 没有, 1 = 有的
        /// </summary>
        int IsAttachConsole = -1;

        /// <summary>
        /// 打印文本到控制台里
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="title">标题</param>
        /// <param name="trymsgbox">失败时, 尝试使用 MessageBox</param>
        public void ConsoleLog4CMD(string text, string title = "Auto Touch", bool trymsgbox = true)
        {
            //检查有没有附加到控制台
            if(this.IsAttachConsole == -1)
            {
                if(DLL.AttachConsole(-1) == true)
                {
                    this.IsAttachConsole = 1;
                    Console.WriteLine("");
                    Console.WriteLine(Application.ProductName + "  v" + GlobalStatus.Version + " - " + ((AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute))).Description);
                }
                else
                {
                    this.IsAttachConsole = 0;
                }
            }

            if (this.IsAttachConsole == 1)
            {
                Console.WriteLine(text);
            }
            else if(this.IsAttachConsole == 0 && trymsgbox == true)
            {
                MessageBox.Show(text, title);
            }
        }

        /// <summary>
        /// 构造函数, 处理启动参数, GUI 初始化也在这
        /// </summary>
        /// <param name="args">启动参数</param>
        public Main(string[] args = null)
        {
            //接受到启动参数
            if (args != null && args.Length > 0)
            {
                Point nowxy;
                DLL.GetCursorPos(out nowxy);
                bool isMouseActionTask = false;
                int x = nowxy.X;
                int y = nowxy.Y;
                int delay = 0;
                int wheel = 0;
                List<string> action = new List<string>();
                List<MouseActionItem> items = new List<MouseActionItem>();

                //ConsoleLog4CMD("测试", "标头");

                //错误的命令
                void errcomm()
                {
                    ConsoleLog4CMD("未知指令.");
                    ConsoleLog4CMD("", trymsgbox: false);
                    ConsoleLog4CMD(string.Join("\r\n", GlobalStatus.helptext), "帮助");
                    isMouseActionTask = false;
                    items.Clear();
                }

                //尝试理解数字
                bool tryparse(string key, string value, out int num)
                {
                    if (int.TryParse(value, out num) == false)
                    {
                        unknowtype(key, value);
                        return false; ;
                    }
                    return true;
                }
                //该重载为非 type 类型
                void unknowtype(string key, string value, string type = "INT")
                {
                    string text = key + " " + value + "\r\n";
                    for (int ii = 0; ii < key.Length + 1; ii++)
                    {
                        text = text + " ";
                    }
                    text = text + "^";
                    for (int ii = 0; ii < value.Length - 1; ii++) //像 GCC 一样
                    {
                        text = text + "~";
                    }
                    text = text + "\r\n该重载为非 " + type + " 类型";

                    ConsoleLog4CMD("", trymsgbox: false);
                    ConsoleLog4CMD(text, "语法错误");
                    isMouseActionTask = false;
                }

                //便利启动参数
                for (int i = 0; i < args.Length; i++)
                {
                    //帮助
                    if (string.Equals(args[i], "-h", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--help", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "/?", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        ConsoleLog4CMD("", trymsgbox: false);
                        ConsoleLog4CMD(string.Join("\r\n", GlobalStatus.helptext), "帮助");
                        isMouseActionTask = false;
                        items.Clear();
                        break;
                    }
                    //版本
                    else if (string.Equals(args[i], "-v", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "-ver", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--version", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        ConsoleLog4CMD("Build Time: " + GlobalStatus.BuildTime, trymsgbox:false);
                        isMouseActionTask = false;
                        items.Clear();
                        break; //前面 ConsoleLog4CMD 初始化时, 已经打印版本信息了. 
                    }
                    //从预设启动
                    else if (string.Equals(args[i], "-p", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--profile", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        if(i + 1 < args.Length)
                        {
                            string path = GlobalStatus.AssumptionPath + args[i + 1] + ".txt";
                            if (File.Exists(path) == true)
                            {
                                isMouseActionTask = false;
                                items.Clear();
                                try
                                {
                                    string[] file = File.ReadAllLines(path);
                                    foreach (string line in file)
                                    {
                                        if (line.Trim().Length == 0)
                                        {
                                            continue;
                                        }

                                        //行拆字
                                        string[] word = line.Trim().Split(';');
                                        string[] pos = word[1].Split(',');
                                        MouseActionItem item = new MouseActionItem(int.Parse(pos[0].Trim()), int.Parse(pos[1].Trim()));
                                        item.Delay = int.Parse(word[2].Substring(0, word[2].Length - 2));
                                        item.Wheel = int.Parse(word[3]);
                                        item.Action = word[4];
                                        items.Add(item);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ConsoleLog4CMD("载入预设时出错了, 原因是: \r\n" + ex.ToString(), "Oops! ");
                                }

                                //i++;
                                break;
                            }
                            else
                            {
                                ConsoleLog4CMD("找不到改预设: " + args[i + 1]);
                                isMouseActionTask = false;
                                items.Clear();
                                break;
                            }
                        }
                        //越界了
                        else
                        {
                            errcomm();
                            break;
                        }
                    }
                    //X 轴
                    else if (string.Equals(args[i], "-x", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        isMouseActionTask = true;
                        if (i + 1 < args.Length)
                        {
                            if (tryparse(args[i], args[i + 1], out x) == false)
                            {
                                break;
                            }
                            i++;
                            continue;
                        }
                        //越界了
                        else
                        {
                            errcomm();
                            break;
                        }
                    }
                    //Y 轴
                    else if (string.Equals(args[i], "-y", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        isMouseActionTask = true;
                        if (i + 1 < args.Length)
                        {
                            if (tryparse(args[i], args[i + 1], out y) == false)
                            {
                                break;
                            }
                            i++;
                            continue;
                        }
                        //越界了
                        else
                        {
                            errcomm();
                            break;
                        }
                    }
                    //滚轮
                    else if (string.Equals(args[i], "-w", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--wheel", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        isMouseActionTask = true;
                        if (i + 1 < args.Length)
                        {
                            if (tryparse(args[i], args[i + 1], out wheel) == false)
                            {
                                break;
                            }
                            i++;
                            continue;
                        }
                        //越界了
                        else
                        {
                            errcomm();
                            break;
                        }
                    }
                    //时间
                    else if (string.Equals(args[i], "-t", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--time", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--delay", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        isMouseActionTask = true;
                        if (i + 1 < args.Length)
                        {
                            string t = args[i + 1];
                            //HH:MM:SS
                            if (t.IndexOf(":") != -1)
                            {
                                string[] tt = t.Split(':');
                                //超过了三个冒号或者没有
                                if (tt.Length > 3 || tt.Length == 0)
                                {
                                    unknowtype(args[i], t, "HH:MM:SS");
                                    break;
                                }
                                //挨个转
                                bool err = false;
                                for (int ii = 0; ii < tt.Length; ii++)
                                {
                                    int num = 0;
                                    if (tt[ii].Length == 0 || int.TryParse(tt[ii], out num) == false)
                                    {
                                        unknowtype(args[i], t, "HH:MM:SS");
                                        err = true;
                                        break;
                                    }
                                    //检查时间有没有超过60 的
                                    if (tt.Length > 1 && ii > 0 && num > 59)
                                    {
                                        unknowtype(args[i], t, "HH:MM:SS");
                                        err = true;
                                        break;
                                    }
                                    else
                                    {
                                        delay = delay + (int)Math.Pow(60, -1 + tt.Length - ii) * num ;
                                    }
                                }
                                //完成了吗?
                                if (err == true)
                                {
                                    break;
                                }
                                delay = delay * 1000;
                                //ConsoleLog4CMD(delay.ToString());
                            }
                            //假如是纯数字
                            else if(tryparse(args[i], args[i + 1], out delay) == false)
                            {
                                break;
                            }
                            i++;
                            continue;
                        }
                        //越界了
                        else
                        {
                            errcomm();
                            break;
                        }
                    }
                    //动作
                    else if (string.Equals(args[i], "-a", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--action", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--button", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        isMouseActionTask = true;
                        if (i + 1 < args.Length)
                        {
                            bool err = false;
                            //便利动作组
                            string[] acts = args[i + 1].ToLower().Split(',');
                            for (int ii = 0; ii < acts.Length; ii++)
                            {
                                err = false;
                                bool err2 = false;
                                string actss = acts[ii];
                                foreach (string mb in GlobalStatus.MouseButtons)
                                {
                                    if (mb == actss)
                                    {
                                        err2 = true;
                                        break;
                                    }
                                }
                                if (err2 == false && actss != "none")
                                {
                                    unknowtype(args[i], args[i + 1], "ACTION");
                                    err = true;
                                    break;
                                }
                                action.Add(actss);
                            }
                            if (err == true)
                            {
                                break;
                            }
                            else
                            {
                                i++;
                                continue;
                            }
                        }
                        //越界了
                        else
                        {
                            errcomm();
                            break;
                        }
                    }
                    //未知命令
                    else
                    {
                        errcomm();
                        break;
                    }
                }

                //如果是鼠标动作
                if(isMouseActionTask == true)
                {
                    MouseActionItem item = new MouseActionItem(nowxy.X, nowxy.Y);
                    items.Add(item);
                    item = new MouseActionItem(x, y);
                    item.Delay = delay;
                    item.Wheel = wheel;
                    item.Action = string.Join("|", action);
                    items.Add(item);
                }

                //执行
                if (items.Count > 0)
                {
                    RunStart(items);
                }

                //关闭
                this.Hide();
                this.ShowInTaskbar = false;
                DLL.FreeConsole();
                this.Load += new EventHandler( (object sender, EventArgs e) => {
                    this.Close();
                });
            }
            //正常打开
            else
            {
                InitializeComponent();
                this.Text = Application.ProductName;
                this.StatusBarVersion.Text = "v" + GlobalStatus.Version;
                //注册消息过滤器
                Application.AddMessageFilter(new MsgFilter());
                //尝试让下拉框设定只读
                //DLL.SendMessage(this.ComboBoxAction.Handle, 0x00CF, IntPtr.Zero, IntPtr.Zero);
                //設定状态栏文本计时器
                this.StatusBarTipsTimer.Interval = 5000;
                this.StatusBarTipsTimer.Tick += new EventHandler((obj, e) => {
                    if (this.StatusBarTipsWait == false)
                    {
                        this.StatusBarTips.Text = "";
                    }
                    this.StatusBarTipsWait = false;
                    this.StatusBarTipsTimer.Stop();
                });
                //尝试为每个控件设置提示文本
                this.BtnAssumptionDel.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnAssumptionRename.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnAssumptionSave.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnAssumptionNew.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnCapturePosition.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnCaptureTrajectory.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnExit.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnStart.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnExport.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnImport.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnHelp.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnListDel.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnListDown.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnListUp.MouseEnter += new EventHandler(Control_MouseEnter);
                this.BtnListNew.MouseEnter += new EventHandler(Control_MouseEnter);
                this.ComboBoxAssumption.MouseEnter += new EventHandler(Control_MouseEnter);
                this.ComboBoxAction.MouseEnter += new EventHandler(Control_MouseEnter);
                this.NumDelay.MouseEnter += new EventHandler(Control_MouseEnter);
                this.NumWheel.MouseEnter += new EventHandler(Control_MouseEnter);
                this.TextBoxPosition.MouseEnter += new EventHandler(Control_MouseEnter);

                //加载预设列表
                RefleshAssumption();
                NewAssumption();
            }
            this.MinimumSize = this.Size;
        }

        //启动时运行
        private void Main_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 加载/刷新预设列表
        /// </summary>
        public void RefleshAssumption()
        {
            if (Directory.Exists(GlobalStatus.AssumptionPath) == true)
            {
                this.ComboBoxAssumption.Items.Clear();
                foreach (string assu in Directory.GetFiles(GlobalStatus.AssumptionPath))
                {
                    //跳过非 TXT 文件
                    if (assu.Substring(assu.LastIndexOf(".") + 1, assu.Length - assu.LastIndexOf(".") - 1) != "txt")
                    {
                        continue;
                    }

                    string file = assu.Substring(assu.LastIndexOf("\\") + 1, assu.Length - assu.LastIndexOf("\\") - 1);
                    file = file.Substring(0, file.LastIndexOf("."));
                    this.ComboBoxAssumption.Items.Add(file);
                }
            }
            else
            {
                NewAssumption();
            }
        }

        /// <summary>
        /// 创建新预设
        /// </summary>
        public void NewAssumption()
        {
            this.IsNew = true;
            this.ComboBoxAssumption.Text = "";
            this.listView1.Items.Clear();
            NewItem();
        }

        /// <summary>
        /// 创建新动作
        /// </summary>
        public void NewItem()
        {
            this.listView1.BeginUpdate();
            ListViewItem list = new ListViewItem();
            list.Text = "0";
            list.SubItems.Add("0,0");
            list.SubItems.Add("1000ms");
            list.SubItems.Add("0");
            list.SubItems.Add("None");
            //插入
            this.listView1.Items.Add(list);
            if (this.listView1.CheckedItems.Count == 1)
            {

            }
            this.listView1.EndUpdate();
            UpdateItemIndex();
            this.listView1.SelectedItems.Clear();
            this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
        }

        /// <summary>
        /// 移除动作
        /// </summary>
        public void DelItem()
        {
            if (this.listView1.SelectedItems.Count == 0)
            {
                return;
            }

            this.listView1.BeginUpdate();
            int lastfocus = -1;
            for (int i = this.listView1.SelectedItems.Count - 1; i > -1; i--)
            {
                lastfocus = this.listView1.SelectedItems[i].Index;
                this.listView1.Items.RemoveAt(lastfocus);
            }
            this.listView1.EndUpdate();
            UpdateItemIndex();
            //当全部清空时, 新建一个
            if (this.listView1.Items.Count == 0)
            {
                NewItem();
            }
            //把焦点转移至上一个位置
            else if(lastfocus > -1)
            {
                if(lastfocus > this.listView1.Items.Count - 1)
                {
                    lastfocus = this.listView1.Items.Count - 1;
                }
                this.listView1.Items[lastfocus].Selected = true;
            }
        }

        /// <summary>
        /// 列表项上移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnListUp_Click(object sender, EventArgs e)
        {
            if(this.listView1.SelectedItems.Count == 0 || this.listView1.SelectedItems[0].Index == 0)
            {
                SystemSounds.Beep.Play();
                return;
            }

            this.listView1.BeginUpdate();
            this.Disable_listView1_ItemSelectionChanged = true;

            //先暂存选中列表项
            int insindex = this.listView1.SelectedItems[0].Index - 1; //记录插入位置
            List<ListViewItem> selectlists = new List<ListViewItem>();
            for (int i = this.listView1.SelectedItems.Count - 1; i > -1; i--)
            {
                ListViewItem item = this.listView1.SelectedItems[i];
                selectlists.Add(item);
                item.Remove();
            }

            //然后插入
            foreach (ListViewItem item in selectlists)
            {
                this.listView1.Items.Insert(insindex, item);
                insindex++;
            }

            this.listView1.EndUpdate();
            this.Disable_listView1_ItemSelectionChanged = false;
            UpdateItemIndex();
        }

        /// <summary>
        /// 列表项下移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnListDown_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0 || this.listView1.SelectedItems[this.listView1.SelectedItems.Count - 1].Index == this.listView1.Items.Count - 1)
            {
                SystemSounds.Beep.Play();
                return;
            }

            this.listView1.BeginUpdate();
            this.Disable_listView1_ItemSelectionChanged = true;

            //先暂存选中列表项
            int insindex = this.listView1.SelectedItems[0].Index + 1; //记录插入位置
            List<ListViewItem> selectlists = new List<ListViewItem>();
            for (int i = this.listView1.SelectedItems.Count - 1; i > -1; i--)
            {
                ListViewItem item = this.listView1.SelectedItems[i];
                selectlists.Add(item);
                item.Remove();
            }

            //然后插入
            foreach (ListViewItem item in selectlists)
            {
                this.listView1.Items.Insert(insindex, item);
                insindex++;
            }

            this.listView1.EndUpdate();
            this.Disable_listView1_ItemSelectionChanged = false;
            UpdateItemIndex();
        }

        /// <summary>
        /// 更新列表序号
        /// </summary>
        public void UpdateItemIndex()
        {
            this.IsEdit = true;

            this.listView1.BeginUpdate();
            int index = 0;
            foreach (ListViewItem i in this.listView1.Items)
            {
                i.Text = index.ToString();
                index++;
            }
            this.listView1.EndUpdate();
        }

        /// <summary>
        /// 激活编辑栏
        /// </summary>
        public void EnableEditor(bool enable)
        {
            this.TextBoxPosition.Enabled = enable;
            this.NumDelay.Enabled = enable;
            this.ComboBoxAction.Enabled = enable;
            this.NumWheel.Enabled = enable;
        }

        /// <summary>
        /// 控件闪烁
        /// </summary>
        async public void Blinking(Control control)
        {
            if (control == null)
            {
                return;
            }
            control.Visible = false;
            await Task.Delay(100);
            control.Visible = true;
            await Task.Delay(100);
            control.Visible = false;
            await Task.Delay(100);
            control.Visible = true;
            await Task.Delay(100);
            control.Visible = false;
            await Task.Delay(100);
            control.Visible = true;
        }

        private void BtnListNew_Click(object sender, EventArgs e)
        {
            NewItem();
        }

        private void BtnListDel_Click(object sender, EventArgs e)
        {
            DelItem();
        }

        //关于
        private void StatusBarVersion_Click(object sender, EventArgs e)
        {
            Command.About();
        }

        //退出
        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //列表选择项变动时
        /// <summary>
        /// 暂时禁用 "列表选择项变动" 事件
        /// </summary>
        public bool Disable_listView1_ItemSelectionChanged = false;
        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (this.Disable_listView1_ItemSelectionChanged == true)
            {
                return;
            }
            if (this.listView1.SelectedItems.Count == 1)
            {
                ListViewItem list = this.listView1.SelectedItems[0];
                EnableEditor(true);
                this.TextBoxPosition.Text = list.SubItems[1].Text;
                this.NumDelay.Value = decimal.Parse(list.SubItems[2].Text.Substring(0, list.SubItems[2].Text.Length - 2));
                this.NumWheel.Value = decimal.Parse(list.SubItems[3].Text);

                //拆分动作
                this.Disable_CheckBoxMouse_CheckedChanged_TextChange = true;
                this.CheckBoxMouseLeft.Checked = false;
                this.CheckBoxMouseMiddle.Checked = false;
                this.CheckBoxMouseRight.Checked = false;
                this.CheckBoxMouseXButton1.Checked = false;
                this.CheckBoxMouseXButton2.Checked = false;
                this.Disable_CheckBoxMouse_CheckedChanged_TextChange = false;
                //this.ComboBoxAction.Text = list.SubItems[4].Text;  //复选框变更时会更新文本的
                string[] mouseactionlist = list.SubItems[4].Text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries); //看起来很怪，我只想要不保留空子项，但必须这样写
                foreach (string mouseaction in mouseactionlist)
                {
                    switch (mouseaction)
                    {
                        case "None":
                            //前面就已经全灭了
                            this.ComboBoxAction.Text = "None";
                            break;
                        case "MouseLeft":
                            this.CheckBoxMouseLeft.Checked = true;
                            break;
                        case "MouseMiddle":
                            this.CheckBoxMouseMiddle.Checked = true;
                            break;
                        case "MouseRight":
                            this.CheckBoxMouseRight.Checked = true;
                            break;
                        case "MouseXButton1":
                            this.CheckBoxMouseXButton1.Checked = true;
                            break;
                        case "MouseXButton2":
                            this.CheckBoxMouseXButton2.Checked = true;
                            break;
                    }
                }

            }
            else
            {
                EnableEditor(false);
            }
        }

        //"延时" 数值选择器
        private void NumDelay_ValueChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                this.IsEdit = true;
                ListViewItem list = this.listView1.SelectedItems[0];
                list.SubItems[2].Text = this.NumDelay.Value.ToString() + "ms";
            }
        }
        //"滚轮" 数值选择器
        private void NumWheel_ValueChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                this.IsEdit = true;
                ListViewItem list = this.listView1.SelectedItems[0];
                list.SubItems[3].Text = this.NumWheel.Value.ToString();
            }
        }
        //"坐标" 文本框离开焦点事件
        public void TextBoxPosition_Leave(object sender, EventArgs e)
        {
            //检查格式是否正确
            string[] array = this.TextBoxPosition.Text.Split(',');
            if(array.Length != 2)
            {
                StatusBarTipsShow("这不是一个有效的坐标值. ", true);
                SystemSounds.Hand.Play();
                Blinking(this.TextBoxPosition);
                return;
            }
            int test;
            for (int i = 0; i < array.Length; i++)
            {
                string str = array[i].Trim();
                if(int.TryParse(str, out test) == false)
                {
                    StatusBarTipsShow("这不是一个有效的坐标值. ", true);
                    SystemSounds.Hand.Play();
                    Blinking(this.TextBoxPosition);
                    return;
                };
                //这里是把 001 转成 1;
                array[i] = test.ToString();
            }
            //更新列表数据
            this.TextBoxPosition.Text = array[0] + "," + array[1];
            this.TextBoxPosition.SelectionStart = this.TextBoxPosition.Text.Length;
            if (this.listView1.SelectedItems.Count == 1)
            {
                this.IsEdit = true;
                ListViewItem list = this.listView1.SelectedItems[0];
                list.SubItems[1].Text = this.TextBoxPosition.Text;
            }
        }
        //"坐标" 文本框键盘监听事件
        private void TextBoxPosition_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                TextBoxPosition_Leave(null, null);
            }
        }
        //列表框键盘监听事件
        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            //Ctrl + Shift + A
            if(e.Control == true && e.Shift == true && e.KeyCode == Keys.A)
            {
                this.listView1.SelectedItems.Clear();
            }
            //Ctrl + A
            else if(e.Control == true && e.KeyCode == Keys.A)
            {
                this.listView1.SelectedItems.Clear();
                foreach(ListViewItem item in this.listView1.Items)
                {
                    item.Selected = true;
                }
                e.Handled = true;
            }
            //Del, BackSpace
            else if (e.KeyCode== Keys.Delete || e.KeyCode == Keys.Back)
            {
                DelItem();
                e.Handled = true;
            }
        }

        //单点捕捉
        private void BtnCapturePosition_Click(object sender, EventArgs e)
        {
            this.StatusBarText.Text = "捕捉中";
            this.BtnCapturePosition.Enabled = false;
            this.BtnCaptureTrajectory.Enabled = false;
            this.Disable_listView1_ItemSelectionChanged = true;
            this.WindowState = FormWindowState.Minimized;
            GlobalStatus.capturePosition = new CapturePosition(true);
            GlobalStatus.capturePosition.Show();
        }

        //轨迹捕捉
        private void BtnCaptureTrajectory_Click(object sender, EventArgs e)
        {
            this.StatusBarText.Text = "捕捉中";
            this.BtnCapturePosition.Enabled = false;
            this.BtnCaptureTrajectory.Enabled = false;
            this.Disable_listView1_ItemSelectionChanged = true;
            this.WindowState = FormWindowState.Minimized;
            GlobalStatus.capturePosition = new CapturePosition(false);
            GlobalStatus.capturePosition.Show();
        }

        //导出
        private void BtnExport_Click(object sender, EventArgs e)
        {
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            foreach(ListViewItem items in this.listView1.Items)
            {
                sb2.Clear();
                for(int i = 0; i < items.SubItems.Count; i++)
                {
                    string item = items.SubItems[i].Text;
                    sb2.Append(item + ";");
                }
                sb2.Remove(sb2.Length - 1, 1);
                sb1.AppendLine(sb2.ToString());
            }
            if (Control.ModifierKeys == Keys.Shift) //按下了 Shift
            {
                Clipboard.SetText(sb1.ToString());
                StatusBarTipsShow("成功保存预设文件在剪切板上. ", true);
            }
            else
            {
                SaveFileDialog dig = new SaveFileDialog();
                dig.Filter = "文本文档(*.txt)|*.txt";
                if (this.ComboBoxAssumption.Text.Trim().Length == 0)
                {
                    dig.FileName = "new.txt";
                }
                else
                {
                    dig.FileName = this.ComboBoxAssumption.Text + ".txt";
                }
                dig.AddExtension = true;
                dig.OverwritePrompt = true;
                dig.SupportMultiDottedExtensions = false;
                dig.DefaultExt = "*.txt";
                dig.Title = "保存";
                dig.InitialDirectory = Application.ExecutablePath;
                if (dig.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(dig.FileName, sb1.ToString());
                        StatusBarTipsShow("成功保存预设文件: " + dig.FileName, true);
                    }
                    catch (Exception ex)
                    {
                        SystemSounds.Hand.Play();
                        StatusBarTipsShow("保存失败哩, 原因是: " + ex.Message, true);
                    }
                }
            }
        }

        //导入
        private void BtnImport_Click(object sender, EventArgs e)
        {
            string improt = "";
            string type = null; //载入类型 (其实是暂存载入路径)
            if (Control.ModifierKeys == Keys.Shift && Clipboard.ContainsText() == true) //按下了 Shift
            {
                improt = Clipboard.GetText();
            }
            else
            {
                OpenFileDialog dig = new OpenFileDialog();
                dig.Filter = "文本文档(*.txt)|*.txt";
                dig.FileName = "";
                dig.AddExtension = true;
                dig.CheckFileExists = true;
                dig.CheckPathExists = true;
                dig.Multiselect = false;
                dig.DefaultExt = "*.txt";
                dig.Title = "加载";
                dig.InitialDirectory = Application.ExecutablePath;
                if (dig.ShowDialog() == DialogResult.OK)
                {
                    if(File.Exists(dig.FileName) == false)
                    {
                        SystemSounds.Hand.Play();
                        StatusBarTipsShow("找不到该文件: " + dig.FileName, true);
                        return;
                    }
                    else
                    {
                        improt = File.ReadAllText(dig.FileName);
                        this.ComboBoxAssumption.Text = dig.FileName.Substring(dig.FileName.LastIndexOf("\\") + 1, dig.FileName.LastIndexOf(".") - dig.FileName.LastIndexOf("\\") - 1 );
                        type = dig.FileName;
                    }
                }
            }

            if (improt.Length == 0)
            {
                return;
            }
            //开始加载
            else
            {
                try
                {
                    LoadAssumption(improt);
                    if (type != null)
                    {
                        StatusBarTipsShow("成功加载预设文件: " + type, true);
                    }
                    else
                    {
                        StatusBarTipsShow("成功从剪切板加载预设文件. ", true);
                    }
                }
                catch (Exception ex)
                {
                    SystemSounds.Hand.Play();
                    StatusBarTipsShow("加载失败哩, 原因是: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 强制等待状态栏文本结束
        /// </summary>
        public bool StatusBarTipsWait = false;
        public Timer StatusBarTipsTimer = new Timer();
        /// <summary>
        /// 在状态栏显示文本
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="wait">是否强制等待状态栏文本结束</param>
        public void StatusBarTipsShow(string text, bool wait = false)
        {
            if (this.StatusBarTipsWait == false)
            {
                this.StatusBarTipsWait = wait;
                this.StatusBarTipsTimer.Stop();
                this.StatusBarTips.Text = text;
                this.StatusBarTipsTimer.Start();
            }
            else if(wait == true) //即使强制等待状态栏文本结束, 在遇到同种级别的文本时, 照样更新. 
            {
                this.StatusBarTipsTimer.Stop();
                this.StatusBarTips.Text = text;
                this.StatusBarTipsTimer.Start();
            }
        }

        /// <summary>
        /// 加载预设
        /// </summary>
        /// <param name="input">配置文本</param>
        public void LoadAssumption(string input)
        {
            int num = 0;
            this.listView1.Items.Clear();
            string[] array = input.Split(new char[] { '\r', '\t' }); //切成每一行
            string[] substrings; //单行切成每一项
            ListViewItem newitem;
            foreach (string items in array)
            {
                newitem = new ListViewItem();
                substrings = items.Split(';');
                if(substrings.Length < 5)
                {
                    continue;
                }
                newitem.Text = num.ToString();
                //把每一项写进列表里
                for (int i = 1; i < substrings.Length; i++) //跳过序号
                {
                    newitem.SubItems.Add(substrings[i]);
                }
                this.listView1.Items.Add(newitem);
                num++;
            }

            if (this.listView1.Items.Count == 0)
            {
                NewItem();
            }
        }

        /// <summary>
        /// 展开 "动作" 复选下拉框
        /// </summary>
        public void UnfoldCheckBoxListMouseAction()
        {
            ComboBoxAction.DroppedDown = false;
            if (this.CheckBoxListMouseAction.Enabled != true)
            {
                this.CheckBoxListMouseAction.Height = 0;
                this.CheckBoxListMouseAction.Enabled = true;
                this.CheckBoxListMouseAction.Visible = true;
                Task.Run( () =>
                {
                    UnfoldCheckBoxListMouseActionAni();
                });
            }
            else
            {
                FoldCheckBoxListMouseAction();
            }
        }
        public void UnfoldCheckBoxListMouseActionAni()
        {
            try
            {
                while (this.CheckBoxListMouseAction.Enabled == true && this.CheckBoxListMouseAction.Height < (this.CheckBoxMouseLeft.Height * 5) + 2)
                {
                    DLL.DwmFlush();
                    this.CheckBoxListMouseAction.Invoke( new MethodInvoker( () => {
                        this.CheckBoxListMouseAction.Height = this.CheckBoxListMouseAction.Height + 2;
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"Oops! ");
            }
        }

        /// <summary>
        /// 关闭 "动作" 复选下拉框
        /// </summary>
        public void FoldCheckBoxListMouseAction(string type = "none")
        {
            if(this.CheckBoxListMouseAction == null) //如果是从命令行打开的化, 不会载入控件, 然后这个下拉框肯定不存在
            {
                return;
            }

            void Fold()
            {
                this.CheckBoxListMouseAction.Enabled = false;
                this.CheckBoxListMouseAction.Visible = false;
            }

            if (this.CheckBoxListMouseAction.Enabled == true)
            {
                switch (type)
                {
                    case "mouse":
                        Point cursor = Cursor.Position;
                        Point control = GlobalStatus.main.ComboBoxAction.PointToScreen(new Point(0, 0));
                        int w = GlobalStatus.main.CheckBoxListMouseAction.Width;
                        int h = GlobalStatus.main.CheckBoxListMouseAction.Height + GlobalStatus.main.ComboBoxAction.Height;
                        //Console.WriteLine("X: " + X + "\tY: " + Y);

                        if ((cursor.X < control.X || cursor.X > control.X + w) || (cursor.Y < control.Y || cursor.Y > control.Y + h))
                        {
                            Fold();
                        }

                        break;
                    default:
                        Fold();
                        break;
                }
            }
        }

        private void ComboBoxAction_MouseDown(object sender, MouseEventArgs e)
        {
            this.ComboBoxAction.SelectAll();
            UnfoldCheckBoxListMouseAction();
        }
        private void ComboBoxAction_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                UnfoldCheckBoxListMouseAction();
            }
            else {
                string[] list = { "None", "MouseLeft", "MouseMiddle", "MouseRight", "MouseXButton1", "MouseXButton2" };
                int index = -1;
                //上翻
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.PageUp)
                {
                    index = list.Length - 1;
                    for (int i = list.Length - 1; i > -1; i--)
                    {
                        if (list[i] == this.ComboBoxAction.Text)
                        {
                            index = i - 1;
                        }
                    }
                    if(index < 0)
                    {
                        index = 0;
                    }
                }
                //下翻
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.PageDown)
                {
                    index = 1;
                    for (int i = 0; i < list.Length; i++)
                    {
                        if (list[i] == this.ComboBoxAction.Text)
                        {
                            index = i + 1;
                        }
                    }
                    if(index >= list.Length)
                    {
                        index = list.Length - 1;
                    }
                }
                //顶部
                else if(e.KeyCode == Keys.Home)
                {
                    index = 0;
                }
                //底部
                else if(e.KeyCode == Keys.End)
                {
                    index = list.Length - 1;
                }

                //更新复选框组
                if(index != -1)
                {
                    switch (index)
                    {
                        case 0: //None
                            this.CheckBoxMouseLeft.Checked = false;
                            this.CheckBoxMouseMiddle.Checked = false;
                            this.CheckBoxMouseRight.Checked = false;
                            this.CheckBoxMouseXButton1.Checked = false;
                            this.CheckBoxMouseXButton2.Checked = false;
                            break;
                        case 1: //MouseLeft
                            this.CheckBoxMouseLeft.Checked = true;
                            this.CheckBoxMouseMiddle.Checked = false;
                            this.CheckBoxMouseRight.Checked = false;
                            this.CheckBoxMouseXButton1.Checked = false;
                            this.CheckBoxMouseXButton2.Checked = false;
                            break;
                        case 2: //MouseMiddle
                            this.CheckBoxMouseLeft.Checked = false;
                            this.CheckBoxMouseMiddle.Checked = true;
                            this.CheckBoxMouseRight.Checked = false;
                            this.CheckBoxMouseXButton1.Checked = false;
                            this.CheckBoxMouseXButton2.Checked = false;
                            break;
                        case 3: //MouseRight
                            this.CheckBoxMouseLeft.Checked = false;
                            this.CheckBoxMouseMiddle.Checked = false;
                            this.CheckBoxMouseRight.Checked = true;
                            this.CheckBoxMouseXButton1.Checked = false;
                            this.CheckBoxMouseXButton2.Checked = false;
                            break;
                        case 4: //MouseXButton1
                            this.CheckBoxMouseLeft.Checked = false;
                            this.CheckBoxMouseMiddle.Checked = false;
                            this.CheckBoxMouseRight.Checked = false;
                            this.CheckBoxMouseXButton1.Checked = true;
                            this.CheckBoxMouseXButton2.Checked = false;
                            break;
                        case 5: //MouseXButton2
                            this.CheckBoxMouseLeft.Checked = false;
                            this.CheckBoxMouseMiddle.Checked = false;
                            this.CheckBoxMouseRight.Checked = false;
                            this.CheckBoxMouseXButton1.Checked = false;
                            this.CheckBoxMouseXButton2.Checked = true;
                            break;
                    }
                }
            }
        }
        public void CheckBoxListMouseAction_MouseWhell(object sender, MouseEventArgs e)
        {
            string[] list = { "None", "MouseLeft", "MouseMiddle", "MouseRight", "MouseXButton1", "MouseXButton2" };
            int index = -1;
            if (e.Delta > 0)
            {
                index = list.Length - 1;
                for (int i = list.Length - 1; i > -1; i--)
                {
                    if (list[i] == this.ComboBoxAction.Text)
                    {
                        index = i - 1;
                    }
                }
                if (index < 0)
                {
                    index = 0;
                }
            }
            else if(e.Delta < 0)
            {
                index = 1;
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i] == this.ComboBoxAction.Text)
                    {
                        index = i + 1;
                    }
                }
                if (index >= list.Length)
                {
                    index = list.Length - 1;
                }
            }

            //更新复选框组
            if (index != -1)
            {
                switch (index)
                {
                    case 0: //None
                        this.CheckBoxMouseLeft.Checked = false;
                        this.CheckBoxMouseMiddle.Checked = false;
                        this.CheckBoxMouseRight.Checked = false;
                        this.CheckBoxMouseXButton1.Checked = false;
                        this.CheckBoxMouseXButton2.Checked = false;
                        break;
                    case 1: //MouseLeft
                        this.CheckBoxMouseLeft.Checked = true;
                        this.CheckBoxMouseMiddle.Checked = false;
                        this.CheckBoxMouseRight.Checked = false;
                        this.CheckBoxMouseXButton1.Checked = false;
                        this.CheckBoxMouseXButton2.Checked = false;
                        break;
                    case 2: //MouseMiddle
                        this.CheckBoxMouseLeft.Checked = false;
                        this.CheckBoxMouseMiddle.Checked = true;
                        this.CheckBoxMouseRight.Checked = false;
                        this.CheckBoxMouseXButton1.Checked = false;
                        this.CheckBoxMouseXButton2.Checked = false;
                        break;
                    case 3: //MouseRight
                        this.CheckBoxMouseLeft.Checked = false;
                        this.CheckBoxMouseMiddle.Checked = false;
                        this.CheckBoxMouseRight.Checked = true;
                        this.CheckBoxMouseXButton1.Checked = false;
                        this.CheckBoxMouseXButton2.Checked = false;
                        break;
                    case 4: //MouseXButton1
                        this.CheckBoxMouseLeft.Checked = false;
                        this.CheckBoxMouseMiddle.Checked = false;
                        this.CheckBoxMouseRight.Checked = false;
                        this.CheckBoxMouseXButton1.Checked = true;
                        this.CheckBoxMouseXButton2.Checked = false;
                        break;
                    case 5: //MouseXButton2
                        this.CheckBoxMouseLeft.Checked = false;
                        this.CheckBoxMouseMiddle.Checked = false;
                        this.CheckBoxMouseRight.Checked = false;
                        this.CheckBoxMouseXButton1.Checked = false;
                        this.CheckBoxMouseXButton2.Checked = true;
                        break;
                }
            }

        }


        /// <summary>
        /// 处理消息监听
        /// </summary>
        public class MsgFilter: IMessageFilter
        {
            public bool PreFilterMessage(ref System.Windows.Forms.Message msg)
            {
                //Console.WriteLine(msg.Msg);
                switch (msg.Msg)
                {
                    case 0x0201: //鼠标左键按下
                        GlobalStatus.main.FoldCheckBoxListMouseAction("mouse");
                        break;
                    case 0x0202: //鼠标左键松开
                        GlobalStatus.main.FoldCheckBoxListMouseAction("mouse");
                        break;
                    case 0x0204: //鼠标右键按下
                        GlobalStatus.main.FoldCheckBoxListMouseAction("mouse");
                        break;
                    case 0x0205: //鼠标右键松开
                        GlobalStatus.main.FoldCheckBoxListMouseAction("mouse");
                        break;
                    case 0x0207: //鼠标中键按下
                        GlobalStatus.main.FoldCheckBoxListMouseAction("mouse");
                        break;
                    case 0x0208: //鼠标中键松开
                        GlobalStatus.main.FoldCheckBoxListMouseAction("mouse");
                        break;
                    case 0x020B: //鼠标侧键按下
                        GlobalStatus.main.FoldCheckBoxListMouseAction("mouse");
                        break;
                    case 0x020C: //鼠标侧键松开
                        GlobalStatus.main.FoldCheckBoxListMouseAction("mouse");
                        break;
                }
                return false;
            }
        }
        protected override void WndProc(ref Message msg)
        {
            //Console.WriteLine(msg.Msg);
            switch (msg.Msg)
            {
                case 0x0003: //窗体移动
                    FoldCheckBoxListMouseAction();
                    break;
                case 0x0005: //窗体大小变化
                    FoldCheckBoxListMouseAction();
                    break;
                case 0x0216: //窗体移动中
                    FoldCheckBoxListMouseAction();
                    break;
                case 0x0214: //窗体大小变化中
                    FoldCheckBoxListMouseAction();
                    break;
                case 0x0006: //获得焦点 (单独窗体)
                    FoldCheckBoxListMouseAction();
                    break;
                case 0x001C: //获得焦点 (整个应用程式)
                    FoldCheckBoxListMouseAction();
                    break;
                case 0x00A1: //点击标题栏
                    FoldCheckBoxListMouseAction();
                    break;
            }
            base.WndProc(ref msg);
        }

        /// <summary>
        /// 复选框变更时
        /// </summary>
        public bool Disable_CheckBoxMouse_CheckedChanged_TextChange = false;
        private void CheckBoxMouse_CheckedChanged(object sender, EventArgs e)
        {
            /*CheckBox checkBox;
            if(sender != null && sender is CheckBox)
            {
                checkBox = sender as CheckBox;
            }
            else
            {
                return;
            }*/

            List<string> texts = new List<string>();
            if(this.CheckBoxMouseLeft.Checked == true)
            {
                texts.Add("MouseLeft");
            }
            if(this.CheckBoxMouseMiddle.Checked == true)
            {
                texts.Add("MouseMiddle");
            }
            if(this.CheckBoxMouseRight.Checked == true)
            {
                texts.Add("MouseRight");
            }
            if(this.CheckBoxMouseXButton1.Checked == true)
            {
                texts.Add("MouseXButton1");
            }
            if(this.CheckBoxMouseXButton2.Checked == true)
            {
                texts.Add("MouseXButton2");
            }

            //更新文本
            if (this.Disable_CheckBoxMouse_CheckedChanged_TextChange == false)
            {
                string text = "None";
                if (texts.Count > 0)
                {
                    text = string.Join("|", texts.ToArray());
                }
                this.ComboBoxAction.Text = text;
                if (this.listView1.SelectedItems.Count == 1)
                {
                    this.IsEdit = true;
                    this.listView1.SelectedItems[0].SubItems[4].Text = text;
                }
            }

        }

        //开始
        async private void BtnStart_Click(object sender, EventArgs e)
        {
            //this.Enabled = false;
            this.Invoke(new MethodInvoker( () => {
                this.WindowState = FormWindowState.Minimized;
                this.PanelAssumption.Enabled = false;
                this.PanelEditor.Enabled = false;
                this.PanelListControl.Enabled = false;
                this.listView1.Enabled = false;
                this.StatusBarText.Text = "执行中";
            }));

            //枚举鼠标动作
            List<MouseActionItem> items = new List<MouseActionItem>();
            foreach (ListViewItem i in this.listView1.Items)
            {
                string[] xy = i.SubItems[1].Text.Split(',');
                MouseActionItem item = new MouseActionItem(int.Parse(xy[0]), int.Parse(xy[1]) );
                item.Delay = int.Parse( i.SubItems[2].Text.Substring(0, i.SubItems[2].Text.Length - 2) );
                item.Wheel = int.Parse(i.SubItems[3].Text);
                item.Action = i.SubItems[4].Text;
                items.Add(item);
            }

            await Task.Run( () =>
            {
                RunStart(items);
            });

            this.Invoke(new MethodInvoker(() => {
                if (this.listView1 != null)
                {
                    this.PanelAssumption.Enabled = true;
                    this.PanelEditor.Enabled = true;
                    this.PanelListControl.Enabled = true;
                    this.listView1.Enabled = true;
                    this.StatusBarText.Text = "就绪";
                }
            }));
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="items">鼠标动作项列表</param>
        public void RunStart(List<MouseActionItem> items)
        {
            //创建媒体计时器
            /*DLL.timecaps_tag timecaps = new DLL.timecaps_tag();
            Console.WriteLine(DLL.timeGetDevCaps(ref timecaps, (uint)Marshal.SizeOf(typeof(tagINPUT))));
            Console.WriteLine("Timecasp: MIX: " + timecaps.wPeriodMin + "\tMAX:" + timecaps.wPeriodMax);
            Console.WriteLine(DLL.timeBeginPeriod(timecaps.wPeriodMin));*/
            IntPtr timer = DLL.CreateWaitableTimerExW(IntPtr.Zero, null, DLL.WaitableTimerFlags.CREATE_WAITABLE_TIMER_HIGH_RESOLUTION, DLL.DesiredAccesss.TIMER_ALL_ACCESS);
            DLL.FILETIME lpDueTime = new DLL.FILETIME();
            lpDueTime.AsLong = -10*1000*10L;
            DLL.SetWaitableTimer(timer, ref lpDueTime, 1, IntPtr.Zero, IntPtr.Zero, false);

            Point lastpos = new Point();
            DLL.GetCursorPos(out lastpos);
            long lasttime = Command.GetTimeStampMs(); //最后获取的时间
            long now = lasttime; //当前时间
            long spend = 0; // now - lasttime
            tagINPUT[] inputs = new tagINPUT[1];
            int taginputsize = Marshal.SizeOf(typeof(tagINPUT));
            int dx = Screen.PrimaryScreen.Bounds.Width;
            int dy = Screen.PrimaryScreen.Bounds.Height;
            //暂存按键状态
            int[] status = {0, 0, 0, 0, 0 }; //改用数组来表示状态, 0 = 无, 1 = 松开, 2 = 按下, 3 = 已按下
            

            foreach (MouseActionItem item in items)
            {
                //延时
                //await Task.Delay(item.Delay);
                spend = now - lasttime;
                while (spend < item.Delay && item.Delay > 1)
                {
                    //DLL.DwmFlush();
                    DLL.WaitForSingleObject(timer, 15);
                    now = Command.GetTimeStampMs();
                    spend = now - lasttime;
                }
                lasttime = now;

                //DLL.SetCursorPos(item.XY); 
                int x = ((ushort.MaxValue * item.X) / dx) + 1;
                int y = ((ushort.MaxValue * item.Y) / dy) + 1;

                //获取要按下哪个按键
                bool[] dumpbuttonstatus = { false, false, false, false, false }; //暂存按键是否读取过状态
                string[] actions = item.Action.ToLower().Split('|');
                foreach (string action in actions)
                {
                    //便利
                    for(int i = 0; i < GlobalStatus.MouseButtons.Length; i++)
                    {
                        //按下
                        if (action == GlobalStatus.MouseButtons[i])
                        if (string.Equals(action, GlobalStatus.MouseButtons[i], StringComparison.CurrentCultureIgnoreCase))
                        {
                            dumpbuttonstatus[i] = true;
                        }
                    }
                }
                for(int i = 0; i < dumpbuttonstatus.Length; i++)
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
                if (status[3] + status[4] == 0 || status[3] + status[4] == 6 || (status[3] * status[4] == 0 && status[3] + status[4] == 3) )
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
                if (Debugger.IsAttached == true)
                {
                    Console.WriteLine("X: " + inputs[0].mi.dx + "\tY: " + inputs[0].mi.dy + "\tTime: " + item.Delay +
                        "ms\tStatus: " + status[0].ToString() + status[1].ToString() + status[2].ToString() + status[3].ToString() +
                        status[4].ToString() + "\tMouseData: " + inputs[0].mi.mouseData + "\tSpend: " + spend.ToString() + "ms");
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
        }

        //预览
        private void BtnStart_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                PreViewNow();
            }
        }

        /// <summary>
        /// 预览轨迹
        /// </summary>
        public void PreViewNow()
        {
            Preview preview = new Preview();
            preview.Show();

            List<MouseActionItem> items = new List<MouseActionItem>();

            //枚举坐标和时间
            foreach (ListViewItem listViewItem in this.listView1.Items)
            {
                string[] xy = listViewItem.SubItems[1].Text.Split(',');
                MouseActionItem item = new MouseActionItem(int.Parse(xy[0]),int.Parse(xy[1]));
                item.Delay = int.Parse(listViewItem.SubItems[2].Text.Substring(0, listViewItem.SubItems[2].Text.Length - 2));
                items.Add(item);
            }

            //创建计时器
            IntPtr timer = DLL.CreateWaitableTimerExW(IntPtr.Zero, null, DLL.WaitableTimerFlags.CREATE_WAITABLE_TIMER_HIGH_RESOLUTION, DLL.DesiredAccesss.TIMER_ALL_ACCESS);
            DLL.FILETIME lpDueTime = new DLL.FILETIME();
            lpDueTime.AsLong = -10*1000*10L;
            DLL.SetWaitableTimer(timer, ref lpDueTime, 1, IntPtr.Zero, IntPtr.Zero, false);

            //绘制轨迹
            void Draw()
            {
                for (int i = 0; i < items.Count; i++)
                {
                    long lasttime = Command.GetTimeStampMs();
                    long now = lasttime;
                    //await Task.Delay(items[i].Delay);
                    while (now - lasttime < items[i].Delay)
                    {
                        //DLL.DwmFlush();
                        DLL.WaitForSingleObject(timer, 15);
                        now = Command.GetTimeStampMs();
                    }
                    lasttime = now;

                    if (preview.pen != null)
                    {
                        preview.Invoke(new MethodInvoker(() =>
                        {
                            preview.Draw(items[i].XY);
                        }));
                    }
                    else
                    {
                        return;
                    }

                }

                preview.Invoke(new MethodInvoker(() =>
                {
                    preview.label1.Visible = true;
                }));
            }

            Task.Run( () => { 
                Draw();
                DLL.CloseHandle(timer);
            });
        }

        /// <summary>
        /// 鼠标 Hover 时的提示文本
        /// </summary>
        private void Control_MouseEnter(object sender, EventArgs e)
        {
            Control control;
            if(sender != null && sender is Control)
            {
                control = sender as Control;
            }
            else
            {
                return;
            }

            string text = "";
            switch (control.Name)
            {
                case "BtnAssumptionDel":
                    text = "移除该预设. ";
                    break;
                case "BtnAssumptionRename":
                    text = "重命名该预设. ";
                    break;
                case "BtnAssumptionSave":
                    text = "保存该预设. ";
                    break;
                case "BtnAssumptionNew":
                    text = "创建新预设. ";
                    break;
                case "BtnCapturePosition":
                    text = "捕捉鼠标最后一次的坐标, 按 \"ESC\" 结束. ";
                    break;
                case "BtnCaptureTrajectory":
                    text = "捕捉鼠标轨迹. ";
                    break;
                case "BtnExit":
                    text = "结束该程式. ";
                    break;
                case "BtnStart":
                    text = "开始执行鼠标操作, 右键可以预览鼠标轨迹. ";
                    break;
                case "BtnExport":
                    text = "导出预设, 按住 \"SHIFT\" 复制到剪切板. ";
                    break;
                case "BtnImport":
                    text = "导入预设, 按住 \"SHIFT\" 从剪切板导入. ";
                    break;
                case "BtnHelp":
                    text = "获取帮助. ";
                    break;
                case "BtnListDel":
                    text = "移除选中的列表项. ";
                    break;
                case "BtnListDown":
                    text = "将选中的列表项下移. ";
                    break;
                case "BtnListUp":
                    text = "将选中的列表项上移. ";
                    break;
                case "BtnListNew":
                    text = "添加新的列表项. ";
                    break;
                case "ComboBoxAssumption":
                    text = "选择预设. ";
                    break;
                case "ComboBoxAction":
                    text = "选择鼠标按键动作. ";
                    break;
                case "NumDelay":
                    text = "持续时间间隔. ";
                    break;
                case "NumWheel":
                    text = "鼠标滚轮偏移量. ";
                    break;
                case "TextBoxPosition":
                    text = "鼠标坐标. ";
                    break;
            }
            StatusBarTipsShow(text);
        }

        /// <summary>
        /// 预设变更时
        /// </summary>
        private void ComboBoxAssumption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ComboBoxAssumption.Text.Length == 0)
            {
                this.IsNew = true;
                return;
            }
            this.IsNew = false;
            this.IsEdit = false;
            string filename = GlobalStatus.AssumptionPath + this.ComboBoxAssumption.Text + ".txt";
            if (File.Exists(filename) == false)
            {
                return;
            }

            try
            {
                LoadAssumption(File.ReadAllText(filename));
                StatusBarTipsShow("成功载入预设: " + this.ComboBoxAssumption.Text, true);
            }
            catch (Exception ex)
            {
                StatusBarTipsShow("载入预设时出错了, 原因是: " + ex.Message, true);
                NewItem();
            }

            this.listView1.Items[0].Selected = true;
            this.IsEdit = false;
            this.IsNew = false;
        }

        /// <summary>
        /// 新建
        /// </summary>
        private void BtnAssumptionNew_Click(object sender, EventArgs e)
        {
            NewAssumption();
        }

        /// <summary>
        /// 保存
        /// </summary>
        private void BtnAssumptionSave_Click(object sender, EventArgs e)
        {
            if (this.ComboBoxAssumption.Text.Trim().Length == 0)
            {
                SystemSounds.Beep.Play();
                StatusBarTipsShow("预设名称不能为空. ", true);
                Blinking(this.ComboBoxAssumption);
                return;
            }
            string name = this.ComboBoxAssumption.Text;
            //检查有没有非法字符
            string chars = "\\/:*?\"<>|";
            foreach(char c in chars)
            {
                if (name.IndexOf(c) != -1)
                {
                    SystemSounds.Beep.Play();
                    StatusBarTipsShow("预设名称不能包含下列任何字符: " + chars, true);
                    Blinking(this.ComboBoxAssumption);
                    return;
                }
            }
            //检查无误之后保存
            string path = GlobalStatus.AssumptionPath + name + ".txt";
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            foreach (ListViewItem items in this.listView1.Items)
            {
                sb2.Clear();
                for (int i = 0; i < items.SubItems.Count; i++)
                {
                    string item = items.SubItems[i].Text;
                    sb2.Append(item + ";");
                }
                sb2.Remove(sb2.Length - 1, 1);
                sb1.AppendLine(sb2.ToString());
            }

            try
            {
                //如果文件夹不存在就创建
                if (Directory.Exists(GlobalStatus.AssumptionPath) == false)
                {
                    Directory.CreateDirectory(GlobalStatus.AssumptionPath);
                }
                File.WriteAllText(path, sb1.ToString());
                StatusBarTipsShow("成功保存预设: " + name, true);
                RefleshAssumption();
                this.IsNew = false;
                this.IsEdit = false;
            }
            catch (Exception ex)
            {
                SystemSounds.Hand.Play();
                StatusBarTipsShow("保存失败哩, 原因是: " + ex.Message, true);
            }

            sb1 = null;
            sb2 = null;
        }

        /// <summary>
        /// 移除
        /// </summary>
        private void BtnAssumptionDel_Click(object sender, EventArgs e)
        {
            string name = this.ComboBoxAssumption.Text;
            if(name.Trim().Length == 0)
            {
                return;
            }

            if (MessageBox.Show("要移除 " + name  + " 吗? ", "移除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                string path = GlobalStatus.AssumptionPath + name + ".txt";
                try
                {
                    if (File.Exists(path) == false)
                    {
                        throw new Exception("找不到预设文件: " + path);
                    }
                    File.Delete(path);
                    StatusBarTipsShow("移除预设成功: " + name, true);
                    RefleshAssumption();
                    NewAssumption();
                }
                catch (Exception ex)
                {
                    SystemSounds.Hand.Play();
                    StatusBarTipsShow("移除失败哩, 原因是: " + ex.Message, true);
                }
            }
        }

        //右键菜单
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.listView1.Items.Count > 0 && GlobalStatus.capturePosition == null)
            {
                this.清除ToolStripMenuItem.Enabled = true;
                this.全选ToolStripMenuItem.Enabled = true;
                if (this.listView1.SelectedItems.Count > 0)
                {
                    this.取消选择ToolStripMenuItem.Enabled = true;
                    this.移除ToolStripMenuItem.Enabled = true;
                }
            }
        }
        private void ConMenu_ListView_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.清除ToolStripMenuItem.Enabled = false;
            this.全选ToolStripMenuItem.Enabled = false;
            this.取消选择ToolStripMenuItem.Enabled = false;
            this.移除ToolStripMenuItem.Enabled = false;
        }
        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.Items.Clear();
            NewItem();
        }
        private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.SelectedItems.Clear();
            foreach (ListViewItem item in this.listView1.Items)
            {
                item.Selected = true;
            }
        }
        private void 取消选择ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.SelectedItems.Clear();
        }
        private void 移除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DelItem();
        }

        //帮助
        private void BtnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Join("\r\n", GlobalStatus.helptext), "帮助");
        }
    }
}
