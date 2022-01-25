namespace EnigmaGUI
{
    partial class Enigma_GUI
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
            this.components = new System.ComponentModel.Container();
            this.txtIn = new System.Windows.Forms.TextBox();
            this.log = new System.Windows.Forms.TextBox();
            this.txtOut = new System.Windows.Forms.TextBox();
            this.lblWin1 = new System.Windows.Forms.Label();
            this.lblWin2 = new System.Windows.Forms.Label();
            this.lblWin3 = new System.Windows.Forms.Label();
            this.mainMnu = new System.Windows.Forms.MenuStrip();
            this.testsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iIIIIIABCDEFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lLLLLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ohMyLoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.machineSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRotors = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPlugs = new System.Windows.Forms.TextBox();
            this.lblPlugBoard = new System.Windows.Forms.Label();
            this.btnAdvance = new System.Windows.Forms.Button();
            this.lblFastTurnover = new System.Windows.Forms.Label();
            this.lblMedmTurnover = new System.Windows.Forms.Label();
            this.lr3 = new System.Windows.Forms.Label();
            this.contextMenuRotor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeRingSettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.swapRotorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lr1 = new System.Windows.Forms.Label();
            this.lr2 = new System.Windows.Forms.Label();
            this.btnSetWindows = new System.Windows.Forms.Button();
            this.txtRotorInit = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtPlain = new System.Windows.Forms.TextBox();
            this.txtCipher = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnClearReset = new System.Windows.Forms.Button();
            this.mainMnu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuRotor.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtIn
            // 
            this.txtIn.AcceptsReturn = true;
            this.txtIn.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtIn.Location = new System.Drawing.Point(22, 35);
            this.txtIn.Multiline = true;
            this.txtIn.Name = "txtIn";
            this.txtIn.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtIn.Size = new System.Drawing.Size(303, 247);
            this.txtIn.TabIndex = 0;
            this.txtIn.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textIn_KeyPress);
            // 
            // log
            // 
            this.log.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.log.Location = new System.Drawing.Point(274, 368);
            this.log.Multiline = true;
            this.log.Name = "log";
            this.log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.log.Size = new System.Drawing.Size(649, 200);
            this.log.TabIndex = 1;
            // 
            // txtOut
            // 
            this.txtOut.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtOut.Location = new System.Drawing.Point(664, 35);
            this.txtOut.Multiline = true;
            this.txtOut.Name = "txtOut";
            this.txtOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOut.Size = new System.Drawing.Size(303, 247);
            this.txtOut.TabIndex = 2;
            // 
            // lblWin1
            // 
            this.lblWin1.AutoSize = true;
            this.lblWin1.BackColor = System.Drawing.Color.LightGray;
            this.lblWin1.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblWin1.Location = new System.Drawing.Point(14, 69);
            this.lblWin1.Name = "lblWin1";
            this.lblWin1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.lblWin1.Size = new System.Drawing.Size(51, 38);
            this.lblWin1.TabIndex = 3;
            this.lblWin1.Text = "01A";
            // 
            // lblWin2
            // 
            this.lblWin2.AutoSize = true;
            this.lblWin2.BackColor = System.Drawing.Color.LightGray;
            this.lblWin2.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblWin2.Location = new System.Drawing.Point(116, 69);
            this.lblWin2.Name = "lblWin2";
            this.lblWin2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.lblWin2.Size = new System.Drawing.Size(51, 38);
            this.lblWin2.TabIndex = 4;
            this.lblWin2.Text = "01A";
            // 
            // lblWin3
            // 
            this.lblWin3.AutoSize = true;
            this.lblWin3.BackColor = System.Drawing.Color.LightGray;
            this.lblWin3.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblWin3.Location = new System.Drawing.Point(220, 69);
            this.lblWin3.Name = "lblWin3";
            this.lblWin3.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.lblWin3.Size = new System.Drawing.Size(51, 38);
            this.lblWin3.TabIndex = 5;
            this.lblWin3.Text = "01A";
            // 
            // mainMnu
            // 
            this.mainMnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testsToolStripMenuItem,
            this.machineSetupToolStripMenuItem});
            this.mainMnu.Location = new System.Drawing.Point(0, 0);
            this.mainMnu.Name = "mainMnu";
            this.mainMnu.Size = new System.Drawing.Size(1002, 24);
            this.mainMnu.TabIndex = 6;
            this.mainMnu.Text = "menuStrip1";
            // 
            // testsToolStripMenuItem
            // 
            this.testsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iIIIIIABCDEFToolStripMenuItem,
            this.lLLLLToolStripMenuItem,
            this.ohMyLoveToolStripMenuItem});
            this.testsToolStripMenuItem.Name = "testsToolStripMenuItem";
            this.testsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.testsToolStripMenuItem.Text = "Tests";
            // 
            // iIIIIIABCDEFToolStripMenuItem
            // 
            this.iIIIIIABCDEFToolStripMenuItem.Name = "iIIIIIABCDEFToolStripMenuItem";
            this.iIIIIIABCDEFToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.iIIIIIABCDEFToolStripMenuItem.Text = "I II III ABC DEF";
            this.iIIIIIABCDEFToolStripMenuItem.Click += new System.EventHandler(this.iIIIIIABCDEFToolStripMenuItem_Click);
            // 
            // lLLLLToolStripMenuItem
            // 
            this.lLLLLToolStripMenuItem.Name = "lLLLLToolStripMenuItem";
            this.lLLLLToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.lLLLLToolStripMenuItem.Text = "LLLLL";
            // 
            // ohMyLoveToolStripMenuItem
            // 
            this.ohMyLoveToolStripMenuItem.Name = "ohMyLoveToolStripMenuItem";
            this.ohMyLoveToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.ohMyLoveToolStripMenuItem.Text = "Oh my love";
            // 
            // machineSetupToolStripMenuItem
            // 
            this.machineSetupToolStripMenuItem.Name = "machineSetupToolStripMenuItem";
            this.machineSetupToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.machineSetupToolStripMenuItem.Text = "Machine Setup";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkGray;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtRotors);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtPlugs);
            this.panel1.Controls.Add(this.lblPlugBoard);
            this.panel1.Controls.Add(this.btnAdvance);
            this.panel1.Controls.Add(this.lblFastTurnover);
            this.panel1.Controls.Add(this.lblMedmTurnover);
            this.panel1.Controls.Add(this.lr3);
            this.panel1.Controls.Add(this.lr1);
            this.panel1.Controls.Add(this.lr2);
            this.panel1.Controls.Add(this.btnSetWindows);
            this.panel1.Controls.Add(this.txtRotorInit);
            this.panel1.Controls.Add(this.lblWin3);
            this.panel1.Controls.Add(this.lblWin1);
            this.panel1.Controls.Add(this.lblWin2);
            this.panel1.Location = new System.Drawing.Point(342, 43);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(310, 227);
            this.panel1.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(17, 168);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 19);
            this.label2.TabIndex = 18;
            this.label2.Text = "Rotors/Rings";
            // 
            // txtRotors
            // 
            this.txtRotors.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtRotors.Location = new System.Drawing.Point(144, 165);
            this.txtRotors.Name = "txtRotors";
            this.txtRotors.Size = new System.Drawing.Size(154, 26);
            this.txtRotors.TabIndex = 17;
            this.txtRotors.Text = "III/A II/A I/A";
            this.toolTip1.SetToolTip(this.txtRotors, "Enter either 3 letters like XYZ or 3 numbers like 17 11 5");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(64, 199);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 19);
            this.label1.TabIndex = 16;
            this.label1.Text = "Initial";
            // 
            // txtPlugs
            // 
            this.txtPlugs.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtPlugs.Location = new System.Drawing.Point(6, 137);
            this.txtPlugs.Name = "txtPlugs";
            this.txtPlugs.Size = new System.Drawing.Size(293, 26);
            this.txtPlugs.TabIndex = 15;
            this.txtPlugs.Text = "AC DE GS IM NO PV QY TZ BH KW";
            // 
            // lblPlugBoard
            // 
            this.lblPlugBoard.AutoSize = true;
            this.lblPlugBoard.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPlugBoard.Location = new System.Drawing.Point(3, 115);
            this.lblPlugBoard.Name = "lblPlugBoard";
            this.lblPlugBoard.Size = new System.Drawing.Size(90, 19);
            this.lblPlugBoard.TabIndex = 14;
            this.lblPlugBoard.Text = "Plugboard";
            // 
            // btnAdvance
            // 
            this.btnAdvance.Location = new System.Drawing.Point(218, 110);
            this.btnAdvance.Name = "btnAdvance";
            this.btnAdvance.Size = new System.Drawing.Size(75, 24);
            this.btnAdvance.TabIndex = 13;
            this.btnAdvance.Text = "Advance";
            this.toolTip1.SetToolTip(this.btnAdvance, "Asvance rotors by one keypress");
            this.btnAdvance.UseVisualStyleBackColor = true;
            this.btnAdvance.Click += new System.EventHandler(this.btnAdvance_Click);
            // 
            // lblFastTurnover
            // 
            this.lblFastTurnover.AutoSize = true;
            this.lblFastTurnover.BackColor = System.Drawing.Color.Transparent;
            this.lblFastTurnover.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFastTurnover.ForeColor = System.Drawing.Color.Maroon;
            this.lblFastTurnover.Location = new System.Drawing.Point(196, 73);
            this.lblFastTurnover.Name = "lblFastTurnover";
            this.lblFastTurnover.Size = new System.Drawing.Size(24, 21);
            this.lblFastTurnover.TabIndex = 12;
            this.lblFastTurnover.Text = "**";
            // 
            // lblMedmTurnover
            // 
            this.lblMedmTurnover.AutoSize = true;
            this.lblMedmTurnover.BackColor = System.Drawing.Color.Transparent;
            this.lblMedmTurnover.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMedmTurnover.ForeColor = System.Drawing.Color.Maroon;
            this.lblMedmTurnover.Location = new System.Drawing.Point(92, 72);
            this.lblMedmTurnover.Name = "lblMedmTurnover";
            this.lblMedmTurnover.Size = new System.Drawing.Size(24, 21);
            this.lblMedmTurnover.TabIndex = 11;
            this.lblMedmTurnover.Text = "**";
            // 
            // lr3
            // 
            this.lr3.AutoSize = true;
            this.lr3.BackColor = System.Drawing.Color.LightGray;
            this.lr3.ContextMenuStrip = this.contextMenuRotor;
            this.lr3.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lr3.Location = new System.Drawing.Point(213, 6);
            this.lr3.Name = "lr3";
            this.lr3.Size = new System.Drawing.Size(40, 18);
            this.lr3.TabIndex = 10;
            this.lr3.Text = "hint";
            this.toolTip1.SetToolTip(this.lr3, "Rotor name and ring setting");
            // 
            // contextMenuRotor
            // 
            this.contextMenuRotor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeRingSettingToolStripMenuItem,
            this.swapRotorToolStripMenuItem});
            this.contextMenuRotor.Name = "contextMenuRotor";
            this.contextMenuRotor.Size = new System.Drawing.Size(180, 48);
            // 
            // changeRingSettingToolStripMenuItem
            // 
            this.changeRingSettingToolStripMenuItem.Name = "changeRingSettingToolStripMenuItem";
            this.changeRingSettingToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.changeRingSettingToolStripMenuItem.Text = "Change RingSetting";
            // 
            // swapRotorToolStripMenuItem
            // 
            this.swapRotorToolStripMenuItem.Name = "swapRotorToolStripMenuItem";
            this.swapRotorToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.swapRotorToolStripMenuItem.Text = "Swap Rotor";
            // 
            // lr1
            // 
            this.lr1.AutoSize = true;
            this.lr1.BackColor = System.Drawing.Color.LightGray;
            this.lr1.ContextMenuStrip = this.contextMenuRotor;
            this.lr1.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lr1.Location = new System.Drawing.Point(9, 6);
            this.lr1.Name = "lr1";
            this.lr1.Size = new System.Drawing.Size(40, 18);
            this.lr1.TabIndex = 8;
            this.lr1.Text = "hint";
            this.toolTip1.SetToolTip(this.lr1, "Rotor name and ring setting");
            // 
            // lr2
            // 
            this.lr2.AutoSize = true;
            this.lr2.BackColor = System.Drawing.Color.LightGray;
            this.lr2.ContextMenuStrip = this.contextMenuRotor;
            this.lr2.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lr2.Location = new System.Drawing.Point(112, 6);
            this.lr2.Name = "lr2";
            this.lr2.Size = new System.Drawing.Size(40, 18);
            this.lr2.TabIndex = 9;
            this.lr2.Text = "hint";
            this.toolTip1.SetToolTip(this.lr2, "Rotor name and ring setting");
            // 
            // btnSetWindows
            // 
            this.btnSetWindows.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSetWindows.Location = new System.Drawing.Point(220, 192);
            this.btnSetWindows.Name = "btnSetWindows";
            this.btnSetWindows.Size = new System.Drawing.Size(85, 35);
            this.btnSetWindows.TabIndex = 7;
            this.btnSetWindows.Text = "Apply";
            this.btnSetWindows.UseVisualStyleBackColor = true;
            this.btnSetWindows.Click += new System.EventHandler(this.btnSetWindows_Click);
            // 
            // txtRotorInit
            // 
            this.txtRotorInit.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtRotorInit.Location = new System.Drawing.Point(143, 196);
            this.txtRotorInit.Name = "txtRotorInit";
            this.txtRotorInit.Size = new System.Drawing.Size(72, 26);
            this.txtRotorInit.TabIndex = 6;
            this.txtRotorInit.Text = "FJY";
            this.toolTip1.SetToolTip(this.txtRotorInit, "Enter either 3 letters like XYZ or 3 numbers like 17 11 5");
            this.txtRotorInit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRotorInit_KeyPress);
            // 
            // txtPlain
            // 
            this.txtPlain.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtPlain.Location = new System.Drawing.Point(136, 296);
            this.txtPlain.Name = "txtPlain";
            this.txtPlain.Size = new System.Drawing.Size(607, 26);
            this.txtPlain.TabIndex = 8;
            this.txtPlain.Text = "SASUNARUTOANIMEDATTEBAYOANIME";
            // 
            // txtCipher
            // 
            this.txtCipher.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtCipher.Location = new System.Drawing.Point(136, 322);
            this.txtCipher.Name = "txtCipher";
            this.txtCipher.Size = new System.Drawing.Size(607, 26);
            this.txtCipher.TabIndex = 9;
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(763, 287);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(78, 44);
            this.btnGo.TabIndex = 10;
            this.btnGo.Text = "Encrypt";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // btnClearReset
            // 
            this.btnClearReset.Location = new System.Drawing.Point(17, 297);
            this.btnClearReset.Name = "btnClearReset";
            this.btnClearReset.Size = new System.Drawing.Size(89, 52);
            this.btnClearReset.TabIndex = 11;
            this.btnClearReset.Text = "Clear +  Reset";
            this.btnClearReset.UseVisualStyleBackColor = true;
            this.btnClearReset.Click += new System.EventHandler(this.btnClearReset_Click);
            // 
            // Enigma_GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 636);
            this.Controls.Add(this.btnClearReset);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtCipher);
            this.Controls.Add(this.txtPlain);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtOut);
            this.Controls.Add(this.log);
            this.Controls.Add(this.txtIn);
            this.Controls.Add(this.mainMnu);
            this.Name = "Enigma_GUI";
            this.Text = "Pete\'s Enigma Playground";
            this.Shown += new System.EventHandler(this.Enigma_GUI_Shown);
            this.mainMnu.ResumeLayout(false);
            this.mainMnu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuRotor.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox txtIn;
        private TextBox log;
        private TextBox txtOut;
        private Label lblWin1;
        private Label lblWin2;
        private Label lblWin3;
        private MenuStrip mainMnu;
        private ToolStripMenuItem testsToolStripMenuItem;
        private ToolStripMenuItem iIIIIIABCDEFToolStripMenuItem;
        private ToolStripMenuItem lLLLLToolStripMenuItem;
        private ToolStripMenuItem ohMyLoveToolStripMenuItem;
        private ToolStripMenuItem machineSetupToolStripMenuItem;
        private Panel panel1;
        private Button btnSetWindows;
        private TextBox txtRotorInit;
        private ToolTip toolTip1;
        private Label lr3;
        private Label lr1;
        private Label lr2;
        private ContextMenuStrip contextMenuRotor;
        private ToolStripMenuItem changeRingSettingToolStripMenuItem;
        private ToolStripMenuItem swapRotorToolStripMenuItem;
        private Label lblFastTurnover;
        private Label lblMedmTurnover;
        private Button btnAdvance;
        private Label label1;
        private TextBox txtPlugs;
        private Label lblPlugBoard;
        private TextBox txtPlain;
        private TextBox txtCipher;
        private Button btnGo;
        private Label label2;
        private TextBox txtRotors;
        private Button btnClearReset;
    }
}