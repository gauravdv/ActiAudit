namespace ImageScroller
{
    partial class frm_Popoup
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_Save = new System.Windows.Forms.Button();
            this.txt_EnterInput = new System.Windows.Forms.TextBox();
            this.cmb_TagReason = new System.Windows.Forms.ComboBox();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lab_Close = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel9.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Controls.Add(this.lab_Close);
            this.groupBox1.Controls.Add(this.btn_Save);
            this.groupBox1.Controls.Add(this.txt_EnterInput);
            this.groupBox1.Controls.Add(this.cmb_TagReason);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(2, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(266, 209);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(98)))), ((int)(((byte)(34)))));
            this.btn_Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Save.ForeColor = System.Drawing.Color.White;
            this.btn_Save.Location = new System.Drawing.Point(76, 141);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(104, 35);
            this.btn_Save.TabIndex = 4;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // txt_EnterInput
            // 
            this.txt_EnterInput.Location = new System.Drawing.Point(51, 93);
            this.txt_EnterInput.Name = "txt_EnterInput";
            this.txt_EnterInput.Size = new System.Drawing.Size(162, 23);
            this.txt_EnterInput.TabIndex = 3;
            // 
            // cmb_TagReason
            // 
            this.cmb_TagReason.FormattingEnabled = true;
            this.cmb_TagReason.Location = new System.Drawing.Point(51, 48);
            this.cmb_TagReason.Name = "cmb_TagReason";
            this.cmb_TagReason.Size = new System.Drawing.Size(162, 24);
            this.cmb_TagReason.TabIndex = 2;
            this.cmb_TagReason.Text = "Select Reason";
            this.cmb_TagReason.SelectedIndexChanged += new System.EventHandler(this.cmb_TagReason_SelectedIndexChanged);
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(98)))), ((int)(((byte)(34)))));
            this.panel9.Controls.Add(this.label2);
            this.panel9.Location = new System.Drawing.Point(12, 4);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(125, 25);
            this.panel9.TabIndex = 925;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(7, 2);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 20);
            this.label2.TabIndex = 19;
            this.label2.Text = "Reason Tag";
            // 
            // lab_Close
            // 
            this.lab_Close.AutoSize = true;
            this.lab_Close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(98)))), ((int)(((byte)(34)))));
            this.lab_Close.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lab_Close.ForeColor = System.Drawing.Color.White;
            this.lab_Close.Location = new System.Drawing.Point(249, -2);
            this.lab_Close.Name = "lab_Close";
            this.lab_Close.Size = new System.Drawing.Size(17, 20);
            this.lab_Close.TabIndex = 908;
            this.lab_Close.Text = "x";
            this.lab_Close.Click += new System.EventHandler(this.lab_Close_Click);
            this.lab_Close.MouseEnter += new System.EventHandler(this.lab_Close_MouseEnter);
            this.lab_Close.MouseLeave += new System.EventHandler(this.lab_Close_MouseLeave);
            // 
            // frm_Popoup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(98)))), ((int)(((byte)(34)))));
            this.ClientSize = new System.Drawing.Size(271, 217);
            this.Controls.Add(this.panel9);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frm_Popoup";
            this.Text = "Acti Audit : SnapShot Title";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_Popoup_FormClosed);
            this.Load += new System.EventHandler(this.frm_Popoup_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmb_TagReason;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.TextBox txt_EnterInput;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lab_Close;
    }
}