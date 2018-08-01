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
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_Save);
            this.groupBox1.Controls.Add(this.txt_EnterInput);
            this.groupBox1.Controls.Add(this.cmb_TagReason);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(247, 193);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SnapShot Reasons Tag";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(60, 135);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(104, 35);
            this.btn_Save.TabIndex = 4;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // txt_EnterInput
            // 
            this.txt_EnterInput.Location = new System.Drawing.Point(34, 87);
            this.txt_EnterInput.Name = "txt_EnterInput";
            this.txt_EnterInput.Size = new System.Drawing.Size(162, 23);
            this.txt_EnterInput.TabIndex = 3;
            // 
            // cmb_TagReason
            // 
            this.cmb_TagReason.FormattingEnabled = true;
            this.cmb_TagReason.Location = new System.Drawing.Point(34, 42);
            this.cmb_TagReason.Name = "cmb_TagReason";
            this.cmb_TagReason.Size = new System.Drawing.Size(162, 24);
            this.cmb_TagReason.TabIndex = 2;
            this.cmb_TagReason.Text = "Select Reason";
            this.cmb_TagReason.SelectedIndexChanged += new System.EventHandler(this.cmb_TagReason_SelectedIndexChanged);
            // 
            // frm_Popoup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(271, 217);
            this.Controls.Add(this.groupBox1);
            this.Name = "frm_Popoup";
            this.Text = "Acti Audit : SnapShot Title";
            this.Load += new System.EventHandler(this.frm_Popoup_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmb_TagReason;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.TextBox txt_EnterInput;
    }
}