using Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
                switch (list.SubItems[3].Text)
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
                list.SubItems[3].Text = this.ComboBoxAction.Text;
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
            this.WindowState = FormWindowState.Minimized;
            GlobalStatus.capturePosition = new CapturePosition();
            GlobalStatus.capturePosition.Show();
        }

    }
}
