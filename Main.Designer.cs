namespace Auto_Touch
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusBarVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusBarText = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusBarAction = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusBarTips = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.PanelEditor = new System.Windows.Forms.Panel();
            this.CheckBoxListMouseAction = new System.Windows.Forms.Panel();
            this.CheckBoxMouseXButton2 = new System.Windows.Forms.CheckBox();
            this.CheckBoxMouseXButton1 = new System.Windows.Forms.CheckBox();
            this.CheckBoxMouseRight = new System.Windows.Forms.CheckBox();
            this.CheckBoxMouseMiddle = new System.Windows.Forms.CheckBox();
            this.CheckBoxMouseLeft = new System.Windows.Forms.CheckBox();
            this.NumWheel = new System.Windows.Forms.NumericUpDown();
            this.NumDelay = new System.Windows.Forms.NumericUpDown();
            this.BtnExit = new System.Windows.Forms.Button();
            this.BtnCaptureTrajectory = new System.Windows.Forms.Button();
            this.BtnCapturePosition = new System.Windows.Forms.Button();
            this.BtnHelp = new System.Windows.Forms.Button();
            this.BtnStart = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.TextBoxPosition = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ComboBoxAction = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PanelAssumption = new System.Windows.Forms.Panel();
            this.BtnAssumptionDel = new System.Windows.Forms.Button();
            this.BtnAssumptionRename = new System.Windows.Forms.Button();
            this.BtnAssumptionSave = new System.Windows.Forms.Button();
            this.ComboBoxAssumption = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PanelListControl = new System.Windows.Forms.Panel();
            this.BtnExport = new System.Windows.Forms.Button();
            this.BtnImport = new System.Windows.Forms.Button();
            this.BtnListDown = new System.Windows.Forms.Button();
            this.BtnListUp = new System.Windows.Forms.Button();
            this.BtnListDel = new System.Windows.Forms.Button();
            this.BtnListNew = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.PanelEditor.SuspendLayout();
            this.CheckBoxListMouseAction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumWheel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDelay)).BeginInit();
            this.PanelAssumption.SuspendLayout();
            this.PanelListControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusBarVersion,
            this.StatusBarText,
            this.StatusBarAction,
            this.StatusBarTips});
            this.statusStrip1.Location = new System.Drawing.Point(0, 420);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 30);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusBarVersion
            // 
            this.StatusBarVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.StatusBarVersion.Name = "StatusBarVersion";
            this.StatusBarVersion.Size = new System.Drawing.Size(69, 24);
            this.StatusBarVersion.Text = "v1.1.0.0";
            this.StatusBarVersion.Click += new System.EventHandler(this.StatusBarVersion_Click);
            // 
            // StatusBarText
            // 
            this.StatusBarText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.StatusBarText.Name = "StatusBarText";
            this.StatusBarText.Size = new System.Drawing.Size(51, 24);
            this.StatusBarText.Text = "就绪. ";
            // 
            // StatusBarAction
            // 
            this.StatusBarAction.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.StatusBarAction.Name = "StatusBarAction";
            this.StatusBarAction.Size = new System.Drawing.Size(43, 24);
            this.StatusBarAction.Text = "0ms";
            // 
            // StatusBarTips
            // 
            this.StatusBarTips.Name = "StatusBarTips";
            this.StatusBarTips.Size = new System.Drawing.Size(0, 24);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.PanelEditor);
            this.panel1.Controls.Add(this.PanelAssumption);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(575, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(225, 420);
            this.panel1.TabIndex = 1;
            // 
            // PanelEditor
            // 
            this.PanelEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelEditor.Controls.Add(this.CheckBoxListMouseAction);
            this.PanelEditor.Controls.Add(this.NumWheel);
            this.PanelEditor.Controls.Add(this.NumDelay);
            this.PanelEditor.Controls.Add(this.BtnExit);
            this.PanelEditor.Controls.Add(this.BtnCaptureTrajectory);
            this.PanelEditor.Controls.Add(this.BtnCapturePosition);
            this.PanelEditor.Controls.Add(this.BtnHelp);
            this.PanelEditor.Controls.Add(this.BtnStart);
            this.PanelEditor.Controls.Add(this.label5);
            this.PanelEditor.Controls.Add(this.TextBoxPosition);
            this.PanelEditor.Controls.Add(this.label3);
            this.PanelEditor.Controls.Add(this.label2);
            this.PanelEditor.Controls.Add(this.ComboBoxAction);
            this.PanelEditor.Controls.Add(this.label4);
            this.PanelEditor.Location = new System.Drawing.Point(0, 92);
            this.PanelEditor.Margin = new System.Windows.Forms.Padding(0);
            this.PanelEditor.Name = "PanelEditor";
            this.PanelEditor.Size = new System.Drawing.Size(225, 221);
            this.PanelEditor.TabIndex = 0;
            // 
            // CheckBoxListMouseAction
            // 
            this.CheckBoxListMouseAction.BackColor = System.Drawing.Color.White;
            this.CheckBoxListMouseAction.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CheckBoxListMouseAction.Controls.Add(this.CheckBoxMouseXButton2);
            this.CheckBoxListMouseAction.Controls.Add(this.CheckBoxMouseXButton1);
            this.CheckBoxListMouseAction.Controls.Add(this.CheckBoxMouseRight);
            this.CheckBoxListMouseAction.Controls.Add(this.CheckBoxMouseMiddle);
            this.CheckBoxListMouseAction.Controls.Add(this.CheckBoxMouseLeft);
            this.CheckBoxListMouseAction.Enabled = false;
            this.CheckBoxListMouseAction.Location = new System.Drawing.Point(50, 120);
            this.CheckBoxListMouseAction.Margin = new System.Windows.Forms.Padding(0);
            this.CheckBoxListMouseAction.Name = "CheckBoxListMouseAction";
            this.CheckBoxListMouseAction.Padding = new System.Windows.Forms.Padding(3);
            this.CheckBoxListMouseAction.Size = new System.Drawing.Size(164, 0);
            this.CheckBoxListMouseAction.TabIndex = 11;
            this.CheckBoxListMouseAction.Visible = false;
            this.CheckBoxListMouseAction.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.CheckBoxListMouseAction_MouseWhell);
            // 
            // CheckBoxMouseXButton2
            // 
            this.CheckBoxMouseXButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CheckBoxMouseXButton2.AutoSize = true;
            this.CheckBoxMouseXButton2.Location = new System.Drawing.Point(3, -21);
            this.CheckBoxMouseXButton2.Margin = new System.Windows.Forms.Padding(0);
            this.CheckBoxMouseXButton2.Name = "CheckBoxMouseXButton2";
            this.CheckBoxMouseXButton2.Size = new System.Drawing.Size(133, 19);
            this.CheckBoxMouseXButton2.TabIndex = 0;
            this.CheckBoxMouseXButton2.Text = "MouseXButton2";
            this.CheckBoxMouseXButton2.UseVisualStyleBackColor = true;
            this.CheckBoxMouseXButton2.CheckedChanged += new System.EventHandler(this.CheckBoxMouse_CheckedChanged);
            // 
            // CheckBoxMouseXButton1
            // 
            this.CheckBoxMouseXButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CheckBoxMouseXButton1.AutoSize = true;
            this.CheckBoxMouseXButton1.Location = new System.Drawing.Point(3, -40);
            this.CheckBoxMouseXButton1.Margin = new System.Windows.Forms.Padding(0);
            this.CheckBoxMouseXButton1.Name = "CheckBoxMouseXButton1";
            this.CheckBoxMouseXButton1.Size = new System.Drawing.Size(133, 19);
            this.CheckBoxMouseXButton1.TabIndex = 0;
            this.CheckBoxMouseXButton1.Text = "MouseXButton1";
            this.CheckBoxMouseXButton1.UseVisualStyleBackColor = true;
            this.CheckBoxMouseXButton1.CheckedChanged += new System.EventHandler(this.CheckBoxMouse_CheckedChanged);
            // 
            // CheckBoxMouseRight
            // 
            this.CheckBoxMouseRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CheckBoxMouseRight.AutoSize = true;
            this.CheckBoxMouseRight.Location = new System.Drawing.Point(3, -59);
            this.CheckBoxMouseRight.Margin = new System.Windows.Forms.Padding(0);
            this.CheckBoxMouseRight.Name = "CheckBoxMouseRight";
            this.CheckBoxMouseRight.Size = new System.Drawing.Size(109, 19);
            this.CheckBoxMouseRight.TabIndex = 0;
            this.CheckBoxMouseRight.Text = "MouseRight";
            this.CheckBoxMouseRight.UseVisualStyleBackColor = true;
            this.CheckBoxMouseRight.CheckedChanged += new System.EventHandler(this.CheckBoxMouse_CheckedChanged);
            // 
            // CheckBoxMouseMiddle
            // 
            this.CheckBoxMouseMiddle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CheckBoxMouseMiddle.AutoSize = true;
            this.CheckBoxMouseMiddle.Location = new System.Drawing.Point(3, -78);
            this.CheckBoxMouseMiddle.Margin = new System.Windows.Forms.Padding(0);
            this.CheckBoxMouseMiddle.Name = "CheckBoxMouseMiddle";
            this.CheckBoxMouseMiddle.Size = new System.Drawing.Size(117, 19);
            this.CheckBoxMouseMiddle.TabIndex = 0;
            this.CheckBoxMouseMiddle.Text = "MouseMiddle";
            this.CheckBoxMouseMiddle.UseVisualStyleBackColor = true;
            this.CheckBoxMouseMiddle.CheckedChanged += new System.EventHandler(this.CheckBoxMouse_CheckedChanged);
            // 
            // CheckBoxMouseLeft
            // 
            this.CheckBoxMouseLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CheckBoxMouseLeft.AutoSize = true;
            this.CheckBoxMouseLeft.Location = new System.Drawing.Point(3, -96);
            this.CheckBoxMouseLeft.Margin = new System.Windows.Forms.Padding(0);
            this.CheckBoxMouseLeft.Name = "CheckBoxMouseLeft";
            this.CheckBoxMouseLeft.Size = new System.Drawing.Size(101, 19);
            this.CheckBoxMouseLeft.TabIndex = 0;
            this.CheckBoxMouseLeft.Text = "MouseLeft";
            this.CheckBoxMouseLeft.UseVisualStyleBackColor = true;
            this.CheckBoxMouseLeft.CheckedChanged += new System.EventHandler(this.CheckBoxMouse_CheckedChanged);
            // 
            // NumWheel
            // 
            this.NumWheel.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NumWheel.Location = new System.Drawing.Point(50, 66);
            this.NumWheel.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NumWheel.Minimum = new decimal(new int[] {
            2147483647,
            0,
            0,
            -2147483648});
            this.NumWheel.Name = "NumWheel";
            this.NumWheel.Size = new System.Drawing.Size(163, 25);
            this.NumWheel.TabIndex = 1;
            this.NumWheel.ValueChanged += new System.EventHandler(this.NumWheel_ValueChanged);
            // 
            // NumDelay
            // 
            this.NumDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NumDelay.Location = new System.Drawing.Point(50, 35);
            this.NumDelay.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NumDelay.Name = "NumDelay";
            this.NumDelay.Size = new System.Drawing.Size(163, 25);
            this.NumDelay.TabIndex = 1;
            this.NumDelay.ValueChanged += new System.EventHandler(this.NumDelay_ValueChanged);
            // 
            // BtnExit
            // 
            this.BtnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnExit.Location = new System.Drawing.Point(146, 144);
            this.BtnExit.Name = "BtnExit";
            this.BtnExit.Size = new System.Drawing.Size(68, 36);
            this.BtnExit.TabIndex = 5;
            this.BtnExit.Text = "退出";
            this.BtnExit.UseVisualStyleBackColor = true;
            this.BtnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // BtnCaptureTrajectory
            // 
            this.BtnCaptureTrajectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCaptureTrajectory.Location = new System.Drawing.Point(112, 180);
            this.BtnCaptureTrajectory.Name = "BtnCaptureTrajectory";
            this.BtnCaptureTrajectory.Size = new System.Drawing.Size(102, 36);
            this.BtnCaptureTrajectory.TabIndex = 7;
            this.BtnCaptureTrajectory.Text = "轨迹捕捉";
            this.BtnCaptureTrajectory.UseVisualStyleBackColor = true;
            this.BtnCaptureTrajectory.Click += new System.EventHandler(this.BtnCaptureTrajectory_Click);
            // 
            // BtnCapturePosition
            // 
            this.BtnCapturePosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCapturePosition.Location = new System.Drawing.Point(10, 180);
            this.BtnCapturePosition.Name = "BtnCapturePosition";
            this.BtnCapturePosition.Size = new System.Drawing.Size(102, 36);
            this.BtnCapturePosition.TabIndex = 6;
            this.BtnCapturePosition.Text = "单点捕捉";
            this.BtnCapturePosition.UseVisualStyleBackColor = true;
            this.BtnCapturePosition.Click += new System.EventHandler(this.BtnCapturePosition_Click);
            // 
            // BtnHelp
            // 
            this.BtnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnHelp.Location = new System.Drawing.Point(10, 144);
            this.BtnHelp.Name = "BtnHelp";
            this.BtnHelp.Size = new System.Drawing.Size(68, 36);
            this.BtnHelp.TabIndex = 3;
            this.BtnHelp.Text = "帮助";
            this.BtnHelp.UseVisualStyleBackColor = true;
            // 
            // BtnStart
            // 
            this.BtnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnStart.Location = new System.Drawing.Point(78, 144);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(68, 36);
            this.BtnStart.TabIndex = 4;
            this.BtnStart.Text = "开始";
            this.BtnStart.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "滚轮";
            // 
            // TextBoxPosition
            // 
            this.TextBoxPosition.Location = new System.Drawing.Point(50, 3);
            this.TextBoxPosition.Name = "TextBoxPosition";
            this.TextBoxPosition.Size = new System.Drawing.Size(164, 25);
            this.TextBoxPosition.TabIndex = 0;
            this.TextBoxPosition.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBoxPosition_KeyUp);
            this.TextBoxPosition.Leave += new System.EventHandler(this.TextBoxPosition_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "延时";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "坐标";
            // 
            // ComboBoxAction
            // 
            this.ComboBoxAction.DropDownHeight = 1;
            this.ComboBoxAction.FormattingEnabled = true;
            this.ComboBoxAction.IntegralHeight = false;
            this.ComboBoxAction.ItemHeight = 15;
            this.ComboBoxAction.Location = new System.Drawing.Point(50, 97);
            this.ComboBoxAction.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.ComboBoxAction.MaxDropDownItems = 1;
            this.ComboBoxAction.Name = "ComboBoxAction";
            this.ComboBoxAction.Size = new System.Drawing.Size(164, 23);
            this.ComboBoxAction.TabIndex = 2;
            this.ComboBoxAction.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ComboBoxAction_KeyDown);
            this.ComboBoxAction.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ComboBoxAction_MouseDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "动作";
            // 
            // PanelAssumption
            // 
            this.PanelAssumption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelAssumption.Controls.Add(this.BtnAssumptionDel);
            this.PanelAssumption.Controls.Add(this.BtnAssumptionRename);
            this.PanelAssumption.Controls.Add(this.BtnAssumptionSave);
            this.PanelAssumption.Controls.Add(this.ComboBoxAssumption);
            this.PanelAssumption.Controls.Add(this.label1);
            this.PanelAssumption.Location = new System.Drawing.Point(0, 0);
            this.PanelAssumption.Margin = new System.Windows.Forms.Padding(0);
            this.PanelAssumption.Name = "PanelAssumption";
            this.PanelAssumption.Size = new System.Drawing.Size(225, 93);
            this.PanelAssumption.TabIndex = 1;
            // 
            // BtnAssumptionDel
            // 
            this.BtnAssumptionDel.Location = new System.Drawing.Point(146, 47);
            this.BtnAssumptionDel.Name = "BtnAssumptionDel";
            this.BtnAssumptionDel.Size = new System.Drawing.Size(68, 36);
            this.BtnAssumptionDel.TabIndex = 3;
            this.BtnAssumptionDel.Text = "移除";
            this.BtnAssumptionDel.UseVisualStyleBackColor = true;
            // 
            // BtnAssumptionRename
            // 
            this.BtnAssumptionRename.Location = new System.Drawing.Point(78, 47);
            this.BtnAssumptionRename.Name = "BtnAssumptionRename";
            this.BtnAssumptionRename.Size = new System.Drawing.Size(68, 36);
            this.BtnAssumptionRename.TabIndex = 2;
            this.BtnAssumptionRename.Text = "重命名";
            this.BtnAssumptionRename.UseVisualStyleBackColor = true;
            // 
            // BtnAssumptionSave
            // 
            this.BtnAssumptionSave.Location = new System.Drawing.Point(10, 47);
            this.BtnAssumptionSave.Name = "BtnAssumptionSave";
            this.BtnAssumptionSave.Size = new System.Drawing.Size(68, 36);
            this.BtnAssumptionSave.TabIndex = 1;
            this.BtnAssumptionSave.Text = "保存";
            this.BtnAssumptionSave.UseVisualStyleBackColor = true;
            // 
            // ComboBoxAssumption
            // 
            this.ComboBoxAssumption.FormattingEnabled = true;
            this.ComboBoxAssumption.Location = new System.Drawing.Point(50, 12);
            this.ComboBoxAssumption.Name = "ComboBoxAssumption";
            this.ComboBoxAssumption.Size = new System.Drawing.Size(164, 23);
            this.ComboBoxAssumption.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "预设";
            // 
            // PanelListControl
            // 
            this.PanelListControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelListControl.Controls.Add(this.BtnExport);
            this.PanelListControl.Controls.Add(this.BtnImport);
            this.PanelListControl.Controls.Add(this.BtnListDown);
            this.PanelListControl.Controls.Add(this.BtnListUp);
            this.PanelListControl.Controls.Add(this.BtnListDel);
            this.PanelListControl.Controls.Add(this.BtnListNew);
            this.PanelListControl.Location = new System.Drawing.Point(575, 336);
            this.PanelListControl.Margin = new System.Windows.Forms.Padding(0);
            this.PanelListControl.Name = "PanelListControl";
            this.PanelListControl.Size = new System.Drawing.Size(225, 84);
            this.PanelListControl.TabIndex = 2;
            // 
            // BtnExport
            // 
            this.BtnExport.Location = new System.Drawing.Point(111, 10);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(102, 36);
            this.BtnExport.TabIndex = 4;
            this.BtnExport.Text = "导出";
            this.BtnExport.UseVisualStyleBackColor = true;
            this.BtnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // BtnImport
            // 
            this.BtnImport.Location = new System.Drawing.Point(10, 10);
            this.BtnImport.Name = "BtnImport";
            this.BtnImport.Size = new System.Drawing.Size(102, 36);
            this.BtnImport.TabIndex = 4;
            this.BtnImport.Text = "导入";
            this.BtnImport.UseVisualStyleBackColor = true;
            this.BtnImport.Click += new System.EventHandler(this.BtnImport_Click);
            // 
            // BtnListDown
            // 
            this.BtnListDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnListDown.Location = new System.Drawing.Point(114, 52);
            this.BtnListDown.Name = "BtnListDown";
            this.BtnListDown.Size = new System.Drawing.Size(48, 23);
            this.BtnListDown.TabIndex = 2;
            this.BtnListDown.Text = "∨";
            this.BtnListDown.UseVisualStyleBackColor = true;
            this.BtnListDown.Click += new System.EventHandler(this.BtnListDown_Click);
            // 
            // BtnListUp
            // 
            this.BtnListUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnListUp.Location = new System.Drawing.Point(62, 52);
            this.BtnListUp.Name = "BtnListUp";
            this.BtnListUp.Size = new System.Drawing.Size(48, 23);
            this.BtnListUp.TabIndex = 1;
            this.BtnListUp.Text = "∧";
            this.BtnListUp.UseVisualStyleBackColor = true;
            this.BtnListUp.Click += new System.EventHandler(this.BtnListUp_Click);
            // 
            // BtnListDel
            // 
            this.BtnListDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnListDel.Location = new System.Drawing.Point(166, 52);
            this.BtnListDel.Name = "BtnListDel";
            this.BtnListDel.Size = new System.Drawing.Size(48, 23);
            this.BtnListDel.TabIndex = 3;
            this.BtnListDel.Text = "-";
            this.BtnListDel.UseVisualStyleBackColor = true;
            this.BtnListDel.Click += new System.EventHandler(this.BtnListDel_Click);
            // 
            // BtnListNew
            // 
            this.BtnListNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnListNew.Location = new System.Drawing.Point(10, 52);
            this.BtnListNew.Name = "BtnListNew";
            this.BtnListNew.Size = new System.Drawing.Size(48, 23);
            this.BtnListNew.TabIndex = 0;
            this.BtnListNew.Text = "+";
            this.BtnListNew.UseVisualStyleBackColor = true;
            this.BtnListNew.Click += new System.EventHandler(this.BtnListNew_Click);
            // 
            // listView1
            // 
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(575, 420);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
            this.listView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyUp);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "#";
            this.columnHeader1.Width = 48;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "坐标";
            this.columnHeader2.Width = 126;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "延时";
            this.columnHeader3.Width = 126;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "滚轮";
            this.columnHeader4.Width = 126;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "操作";
            this.columnHeader5.Width = 126;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.PanelListControl);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "Main";
            this.Text = "Auto Touch";
            this.Load += new System.EventHandler(this.Main_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.PanelEditor.ResumeLayout(false);
            this.PanelEditor.PerformLayout();
            this.CheckBoxListMouseAction.ResumeLayout(false);
            this.CheckBoxListMouseAction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumWheel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDelay)).EndInit();
            this.PanelAssumption.ResumeLayout(false);
            this.PanelAssumption.PerformLayout();
            this.PanelListControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusBarVersion;
        private System.Windows.Forms.ToolStripStatusLabel StatusBarText;
        private System.Windows.Forms.ToolStripStatusLabel StatusBarAction;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        public System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Panel PanelAssumption;
        private System.Windows.Forms.Button BtnAssumptionDel;
        private System.Windows.Forms.Button BtnAssumptionRename;
        private System.Windows.Forms.Button BtnAssumptionSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel PanelListControl;
        private System.Windows.Forms.Button BtnListDown;
        private System.Windows.Forms.Button BtnListUp;
        private System.Windows.Forms.Button BtnListDel;
        private System.Windows.Forms.Button BtnListNew;
        private System.Windows.Forms.Panel PanelEditor;
        private System.Windows.Forms.Button BtnExit;
        private System.Windows.Forms.Button BtnHelp;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox TextBoxPosition;
        public System.Windows.Forms.ComboBox ComboBoxAssumption;
        public System.Windows.Forms.ComboBox ComboBoxAction;
        public System.Windows.Forms.NumericUpDown NumDelay;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        public System.Windows.Forms.NumericUpDown NumWheel;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.Button BtnCaptureTrajectory;
        public System.Windows.Forms.Button BtnCapturePosition;
        private System.Windows.Forms.Button BtnExport;
        private System.Windows.Forms.Button BtnImport;
        public System.Windows.Forms.ToolStripStatusLabel StatusBarTips;
        private System.Windows.Forms.CheckBox CheckBoxMouseXButton2;
        private System.Windows.Forms.CheckBox CheckBoxMouseXButton1;
        private System.Windows.Forms.CheckBox CheckBoxMouseRight;
        private System.Windows.Forms.CheckBox CheckBoxMouseMiddle;
        private System.Windows.Forms.CheckBox CheckBoxMouseLeft;
        public System.Windows.Forms.Panel CheckBoxListMouseAction;
    }
}