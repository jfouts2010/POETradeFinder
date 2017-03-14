namespace ItemWatcher2
{
    partial class StartupForm
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
            this.btnAddNew = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.txtItemValue = new System.Windows.Forms.TextBox();
            this.txtIDK = new System.Windows.Forms.TextBox();
            this.txtIndex = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAddNew
            // 
            this.btnAddNew.Location = new System.Drawing.Point(149, 299);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(75, 23);
            this.btnAddNew.TabIndex = 0;
            this.btnAddNew.Text = "Add New Item";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNewClick);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(9, 29);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(232, 212);
            this.listBox1.TabIndex = 1;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(250, 55);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Remove Item at Index";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDeleteClick);
            // 
            // txtIDK
            // 
            this.txtItemName.Location = new System.Drawing.Point(26, 274);
            this.txtItemName.Name = "txtIDK";
            this.txtItemName.Size = new System.Drawing.Size(100, 20);
            this.txtItemName.TabIndex = 3;
            // 
            // txtItemValue
            // 
            this.txtItemValue.Location = new System.Drawing.Point(133, 273);
            this.txtItemValue.Name = "txtItemValue";
            this.txtItemValue.Size = new System.Drawing.Size(100, 20);
            this.txtItemValue.TabIndex = 4;
            // 
            // txtItemName
            // 
            this.txtIDK.Location = new System.Drawing.Point(239, 273);
            this.txtIDK.Name = "txtItemName";
            this.txtIDK.Size = new System.Drawing.Size(100, 20);
            this.txtIDK.TabIndex = 5;
            // 
            // txtIndex
            // 
            this.txtIndex.Location = new System.Drawing.Point(288, 29);
            this.txtIndex.Name = "txtIndex";
            this.txtIndex.Size = new System.Drawing.Size(37, 20);
            this.txtIndex.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(247, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Index";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 258);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Item Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(162, 257);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Item Value";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(273, 257);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "idk";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Currently Watched Items";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(250, 142);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // StartupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 338);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIndex);
            this.Controls.Add(this.txtIDK);
            this.Controls.Add(this.txtItemValue);
            this.Controls.Add(this.txtItemName);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnAddNew);
            this.Name = "StartupForm";
            this.Text = "StartupForm";
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Load += new System.EventHandler(this.StartupForm_Load);

        }

        #endregion

        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.TextBox txtItemValue;
        private System.Windows.Forms.TextBox txtIDK;
        private System.Windows.Forms.TextBox txtIndex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSave;
    }
}