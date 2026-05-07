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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auto_Touch
{
    public partial class Main : Form
    {
        public Main(string[] args = null)
        {
            InitializeComponent();
            this.Text = Application.ProductName;
            this.StatusBarVersion.Text = "v" + GlobalStatus.Version;

            //接受到启动参数
            if (args != null && args.Length > 0)
            {

            }
            //正常打开
            else
            {
                NewAssumption();
                this.listView1.Items[0].Selected = true;
            }
            this.MinimumSize = this.Size;
        }

        //启动时运行
        private void Main_Load(object sender, EventArgs e)
        {

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
            //先暂存被覆盖列表项列表项
            string[] olditem = new string[5];
            for (int i = 0; i < 5; i++)
            {
                string text = this.listView1.Items[this.listView1.SelectedItems[0].Index - 1].SubItems[i].Text;
                olditem[i] = text;
            }
            //移动选中的列表项
            foreach (ListViewItem item in this.listView1.SelectedItems)
            {
                for (int i = 0; i < 5; i++)
                {
                    string text = item.SubItems[i].Text;
                    this.listView1.Items[item.Index - 1].SubItems[i].Text = text;
                }
            }
            //再将之前的被覆盖的数据一回来
            for (int i = 0; i < 5; i++)
            {
                this.listView1.Items[this.listView1.SelectedItems[this.listView1.SelectedItems.Count - 1].Index].SubItems[i].Text = olditem[i];
            }
            //刷新选中状态
            this.listView1.Items[this.listView1.SelectedItems[0].Index - 1].Selected = true;
            this.listView1.SelectedItems[this.listView1.SelectedItems.Count - 1].Selected = false;

            this.listView1.EndUpdate();
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
            //先暂存被覆盖列表项列表项
            string[] olditem = new string[5];
            for (int i = 0; i < 5; i++)
            {
                string text = this.listView1.Items[this.listView1.SelectedItems[this.listView1.SelectedItems.Count - 1].Index + 1].SubItems[i].Text;
                olditem[i] = text;
            }
            //移动选中的列表项
            for (int i = this.listView1.SelectedItems.Count - 1; i > -1; i--)
            {
                ListViewItem item = this.listView1.SelectedItems[i];
                for (int ii = 0; ii < 5; ii++)
                {
                    this.listView1.Items[item.Index + 1].SubItems[ii].Text = item.SubItems[ii].Text;
                }
            }
            //再将之前的被覆盖的数据一回来
            for (int i = 0; i < 5; i++)
            {
                this.listView1.SelectedItems[0].SubItems[i].Text = olditem[i];
            }
            //刷新选中状态
            this.listView1.Items[this.listView1.SelectedItems[this.listView1.SelectedItems.Count - 1].Index + 1].Selected = true;
            this.listView1.SelectedItems[0].Selected = false;

            this.listView1.EndUpdate();
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

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //列表选择项变动时
        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                ListViewItem list = this.listView1.SelectedItems[0];
                EnableEditor(true);
                this.TextBoxPosition.Text = list.SubItems[1].Text;
                this.NumDelay.Value = decimal.Parse(list.SubItems[2].Text.Substring(0,list.SubItems[2].Text.Length - 2));
                this.NumWheel.Value = decimal.Parse(list.SubItems[3].Text);
                switch (list.SubItems[4].Text)
                {
                    case "None":
                        this.ComboBoxAction.SelectedIndex = 0;
                        break;
                    case "MouseLeft":
                        this.ComboBoxAction.SelectedIndex = 1;
                        break;
                    case "MouseMiddle":
                        this.ComboBoxAction.SelectedIndex = 2;
                        break;
                    case "MouseRight":
                        this.ComboBoxAction.SelectedIndex = 3;
                        break;
                    default:
                        this.ComboBoxAction.SelectedIndex = 0;
                        break;
                }
            }
            else
            {
                EnableEditor(false);
            }
        }
        //"动作" 下拉框
        private void ComboBoxAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                ListViewItem list = this.listView1.SelectedItems[0];
                list.SubItems[4].Text = this.ComboBoxAction.Text;
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
            this.WindowState = FormWindowState.Minimized;
            GlobalStatus.capturePosition = new CapturePosition(true);
            GlobalStatus.capturePosition.Show();
        }

        //轨迹捕捉
        private void BtnCaptureTrajectory_Click(object sender, EventArgs e)
        {
            this.BtnCapturePosition.Enabled = false;
            this.BtnCaptureTrajectory.Enabled = false;
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
                this.StatusBarTips.Text = "成功保存配置文件在剪切板上. ";
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
                        this.StatusBarTips.Text = "成功保存配置文件: " + dig.FileName;
                    }
                    catch (Exception ex)
                    {
                        this.StatusBarTips.Text = "保存失败哩, 原因是: " + ex.Message;
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
                        this.StatusBarTips.Text = "找不到该文件: " + dig.FileName;
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
                        this.StatusBarTips.Text = "成功加载配置文件: " + type;
                    }
                    else
                    {
                        this.StatusBarTips.Text = "成功从剪切板加载配置文件. ";
                    }
                }
                catch (Exception ex)
                {
                    this.StatusBarTips.Text = "加载失败哩, 原因是: " + ex.Message;
                    SystemSounds.Hand.Play();
                }
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
        }
    }
}
