using Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
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
        public Main(string[] args = null)
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
            this.StatusBarTipsTimer.Tick += new EventHandler( (obj, e) => {
                this.StatusBarTips.Text = "";
            });
            //尝试为每个控件设置提示文本
            this.BtnAssumptionDel.MouseEnter += new EventHandler(Control_MouseEnter);
            this.BtnAssumptionRename.MouseEnter += new EventHandler(Control_MouseEnter);
            this.BtnAssumptionSave.MouseEnter += new EventHandler(Control_MouseEnter);
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

            //接受到启动参数
            if (args != null && args.Length > 0)
            {

            }
            //正常打开
            else
            {
                RefleshAssumption();
                if (this.ComboBoxAssumption.Items.Count > 0)
                {
                    this.ComboBoxAssumption.SelectedIndex = 0;
                }
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
                this.listView1.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// 创建新预设
        /// </summary>
        public void NewAssumption()
        {
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
        }

        /// <summary>
        /// 移除动作
        /// </summary>
        public void DelItem()
        {
            this.listView1.BeginUpdate();
            for (int i = this.listView1.SelectedItems.Count - 1; i > -1; i--)
            {
                this.listView1.Items.RemoveAt(this.listView1.SelectedItems[i].Index);
            }
            this.listView1.EndUpdate();
            UpdateItemIndex();
            //当全部清空时, 新建一个
            if (this.listView1.Items.Count == 0)
            {
                NewItem();
                this.listView1.Items[0].Selected = true;
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
                ListViewItem list = this.listView1.SelectedItems[0];
                list.SubItems[2].Text = this.NumDelay.Value.ToString() + "ms";
            }
        }
        //"滚轮" 数值选择器
        private void NumWheel_ValueChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
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
                this.StatusBarText.Text = "这不是一个有效的坐标值. ";
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
                    this.StatusBarText.Text = "这不是一个有效的坐标值. ";
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
            //Ctrl + A
            if(e.Control == true && e.KeyCode == Keys.A)
            {
                this.listView1.SelectedItems.Clear();
                foreach(ListViewItem item in this.listView1.Items)
                {
                    item.Selected = true;
                }
                e.Handled = true;
            }
            //Del, BackSpace
            else if(e.KeyCode== Keys.Delete || e.KeyCode == Keys.Back)
            {
                DelItem();
                e.Handled = true;
            }
        }

        //单点捕捉
        private void BtnCapturePosition_Click(object sender, EventArgs e)
        {
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
                ToolBarTipsShow("成功保存预设文件在剪切板上. ");
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
                        ToolBarTipsShow("成功保存预设文件: " + dig.FileName);
                    }
                    catch (Exception ex)
                    {
                        ToolBarTipsShow("保存失败哩, 原因是: " + ex.Message);
                        SystemSounds.Hand.Play();
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
                        ToolBarTipsShow("找不到该文件: " + dig.FileName);
                        SystemSounds.Hand.Play();
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
                        ToolBarTipsShow("成功加载预设文件: " + type);
                    }
                    else
                    {
                        ToolBarTipsShow("成功从剪切板加载预设文件. ");
                    }
                }
                catch (Exception ex)
                {
                    ToolBarTipsShow("加载失败哩, 原因是: " + ex.Message);
                    SystemSounds.Hand.Play();
                }
            }
        }

        public Timer StatusBarTipsTimer = new Timer();
        public void ToolBarTipsShow(string text)
        {
            this.StatusBarTipsTimer.Stop();
            this.StatusBarTips.Text = text;
            this.StatusBarTipsTimer.Start();
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
                this.listView1.Items[0].Selected = true;
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
                    this.listView1.SelectedItems[0].SubItems[4].Text = text;
                }
            }

        }


        //开始
        private void BtnStart_Click(object sender, EventArgs e)
        {
            
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

            List<Point> points = new List<Point>();
            List<int> delays = new List<int>();

            //枚举坐标和时间
            foreach (ListViewItem listViewItem in this.listView1.Items)
            {
                string[] xy = listViewItem.SubItems[1].Text.Split(',');
                Point point = new Point(int.Parse(xy[0]),int.Parse(xy[1]));
                points.Add(point);
                delays.Add(int.Parse(listViewItem.SubItems[2].Text.Substring(0, listViewItem.SubItems[2].Text.Length - 2)));
            }
            
            //绘制轨迹
            async void Draw()
            {
                for (int i = 0; i < points.Count; i++)
                {
                    await Task.Delay(delays[i]);
                    //DLL.DwmFlush();
                    preview.Draw(points[i]);
                }
                preview.label1.Visible = true;
            }

            Draw();
        }

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
            ToolBarTipsShow(text);
        }

        /// <summary>
        /// 预设变更时
        /// </summary>
        private void ComboBoxAssumption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ComboBoxAssumption.Text.Length == 0)
            {
                return;
            }
            string filename = GlobalStatus.AssumptionPath + this.ComboBoxAssumption.Text + ".txt";
            if (File.Exists(filename) == false)
            {
                return;
            }

            try
            {
                LoadAssumption(File.ReadAllText(filename));
                ToolBarTipsShow("成功载入预设: " + this.ComboBoxAssumption.Text);
            }
            catch (Exception ex)
            {
                ToolBarTipsShow("载入预设时出错了, 原因是: " + ex.Message);
                NewItem();
            }

            this.listView1.Items[0].Selected = true;

        }
    }
}
