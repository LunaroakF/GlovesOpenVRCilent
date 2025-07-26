namespace GlovesOpenVRCilent
{
    partial class Monitor
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Monitor));
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            groupBox2 = new GroupBox();
            label5 = new Label();
            label4 = new Label();
            RightHandFingerHorizontal = new Label();
            RightHandFingerVertical = new Label();
            groupBox1 = new GroupBox();
            label8 = new Label();
            label6 = new Label();
            LeftHandFingerHorizontal = new Label();
            LeftHandFingerVertical = new Label();
            tabPage2 = new TabPage();
            DontShowLog = new RadioButton();
            RightradioButton = new RadioButton();
            LeftradioButton = new RadioButton();
            button1 = new Button();
            LogBox = new TextBox();
            timer1 = new System.Windows.Forms.Timer(components);
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            resources.ApplyResources(tabControl1, "tabControl1");
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tabPage1
            // 
            resources.ApplyResources(tabPage1, "tabPage1");
            tabPage1.Controls.Add(groupBox2);
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Name = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(RightHandFingerHorizontal);
            groupBox2.Controls.Add(RightHandFingerVertical);
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // RightHandFingerHorizontal
            // 
            resources.ApplyResources(RightHandFingerHorizontal, "RightHandFingerHorizontal");
            RightHandFingerHorizontal.Name = "RightHandFingerHorizontal";
            // 
            // RightHandFingerVertical
            // 
            resources.ApplyResources(RightHandFingerVertical, "RightHandFingerVertical");
            RightHandFingerVertical.Name = "RightHandFingerVertical";
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(LeftHandFingerHorizontal);
            groupBox1.Controls.Add(LeftHandFingerVertical);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // label8
            // 
            resources.ApplyResources(label8, "label8");
            label8.Name = "label8";
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // LeftHandFingerHorizontal
            // 
            resources.ApplyResources(LeftHandFingerHorizontal, "LeftHandFingerHorizontal");
            LeftHandFingerHorizontal.Name = "LeftHandFingerHorizontal";
            // 
            // LeftHandFingerVertical
            // 
            resources.ApplyResources(LeftHandFingerVertical, "LeftHandFingerVertical");
            LeftHandFingerVertical.Name = "LeftHandFingerVertical";
            // 
            // tabPage2
            // 
            resources.ApplyResources(tabPage2, "tabPage2");
            tabPage2.Controls.Add(DontShowLog);
            tabPage2.Controls.Add(RightradioButton);
            tabPage2.Controls.Add(LeftradioButton);
            tabPage2.Controls.Add(button1);
            tabPage2.Controls.Add(LogBox);
            tabPage2.Name = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // DontShowLog
            // 
            resources.ApplyResources(DontShowLog, "DontShowLog");
            DontShowLog.Checked = true;
            DontShowLog.Name = "DontShowLog";
            DontShowLog.TabStop = true;
            DontShowLog.UseVisualStyleBackColor = true;
            DontShowLog.CheckedChanged += DontShowLog_CheckedChanged;
            // 
            // RightradioButton
            // 
            resources.ApplyResources(RightradioButton, "RightradioButton");
            RightradioButton.Name = "RightradioButton";
            RightradioButton.UseVisualStyleBackColor = true;
            RightradioButton.CheckedChanged += RightradioButton_CheckedChanged;
            // 
            // LeftradioButton
            // 
            resources.ApplyResources(LeftradioButton, "LeftradioButton");
            LeftradioButton.Name = "LeftradioButton";
            LeftradioButton.UseVisualStyleBackColor = true;
            LeftradioButton.CheckedChanged += LeftradioButton_CheckedChanged;
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // LogBox
            // 
            resources.ApplyResources(LogBox, "LogBox");
            LogBox.Name = "LogBox";
            LogBox.ReadOnly = true;
            LogBox.TextChanged += textBox1_TextChanged;
            // 
            // Monitor
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Monitor";
            FormClosing += Monitor_FormClosing;
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TextBox LogBox;
        private System.Windows.Forms.Timer timer1;
        private Button button1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label RightHandFingerVertical;
        private Label LeftHandFingerVertical;
        private RadioButton RightradioButton;
        private RadioButton LeftradioButton;
        private RadioButton DontShowLog;
        private Label label5;
        private Label label4;
        private Label RightHandFingerHorizontal;
        private Label label8;
        private Label label6;
        private Label LeftHandFingerHorizontal;
    }
}
