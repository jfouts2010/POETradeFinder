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
            this.chkDoBreach = new System.Windows.Forms.CheckBox();
            this.chkDoWatchlist = new System.Windows.Forms.CheckBox();
            this.chkDoUniques = new System.Windows.Forms.CheckBox();
            this.chkUniqueRanges = new System.Windows.Forms.CheckBox();
            this.txtEsh = new System.Windows.Forms.TextBox();
            this.txtXoph = new System.Windows.Forms.TextBox();
            this.txtTul = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnAddNew
            // 
            this.btnAddNew.Location = new System.Drawing.Point(185, 299);
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
            this.listBox1.Size = new System.Drawing.Size(300, 212);
            this.listBox1.TabIndex = 1;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(315, 55);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Remove Item at Index";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDeleteClick);
            // 
            // txtItemName
            // 
            this.txtItemName.Location = new System.Drawing.Point(36, 273);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(100, 20);
            this.txtItemName.TabIndex = 3;
            // 
            // txtItemValue
            // 
            this.txtItemValue.Location = new System.Drawing.Point(160, 273);
            this.txtItemValue.Name = "txtItemValue";
            this.txtItemValue.Size = new System.Drawing.Size(100, 20);
            this.txtItemValue.TabIndex = 4;
            // 
            // txtIDK
            // 
            this.txtIDK.Location = new System.Drawing.Point(290, 273);
            this.txtIDK.Name = "txtIDK";
            this.txtIDK.Size = new System.Drawing.Size(100, 20);
            this.txtIDK.TabIndex = 5;
            // 
            // txtIndex
            // 
            this.txtIndex.Location = new System.Drawing.Point(353, 29);
            this.txtIndex.Name = "txtIndex";
            this.txtIndex.Size = new System.Drawing.Size(37, 20);
            this.txtIndex.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(312, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Index";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 257);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Item Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(189, 257);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Item Value";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(324, 257);
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
            this.btnSave.Location = new System.Drawing.Point(315, 142);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkDoBreach
            // 
            this.chkDoBreach.AutoSize = true;
            this.chkDoBreach.Location = new System.Drawing.Point(25, 337);
            this.chkDoBreach.Name = "chkDoBreach";
            this.chkDoBreach.Size = new System.Drawing.Size(77, 17);
            this.chkDoBreach.TabIndex = 13;
            this.chkDoBreach.Text = "Do Breach";
            this.chkDoBreach.UseVisualStyleBackColor = true;
            // 
            // chkDoWatchlist
            // 
            this.chkDoWatchlist.AutoSize = true;
            this.chkDoWatchlist.Location = new System.Drawing.Point(122, 337);
            this.chkDoWatchlist.Name = "chkDoWatchlist";
            this.chkDoWatchlist.Size = new System.Drawing.Size(87, 17);
            this.chkDoWatchlist.TabIndex = 14;
            this.chkDoWatchlist.Text = "Do Watchlist";
            this.chkDoWatchlist.UseVisualStyleBackColor = true;
            // 
            // chkDoUniques
            // 
            this.chkDoUniques.AutoSize = true;
            this.chkDoUniques.Location = new System.Drawing.Point(231, 337);
            this.chkDoUniques.Name = "chkDoUniques";
            this.chkDoUniques.Size = new System.Drawing.Size(82, 17);
            this.chkDoUniques.TabIndex = 15;
            this.chkDoUniques.Text = "Do Uniques";
            this.chkDoUniques.UseVisualStyleBackColor = true;
            // 
            // chkUniqueRanges
            // 
            this.chkUniqueRanges.AutoSize = true;
            this.chkUniqueRanges.Location = new System.Drawing.Point(327, 337);
            this.chkUniqueRanges.Name = "chkUniqueRanges";
            this.chkUniqueRanges.Size = new System.Drawing.Size(117, 17);
            this.chkUniqueRanges.TabIndex = 16;
            this.chkUniqueRanges.Text = "Do Unique Ranges";
            this.chkUniqueRanges.UseVisualStyleBackColor = true;
            // 
            // txtEsh
            // 
            this.txtEsh.Location = new System.Drawing.Point(19, 414);
            this.txtEsh.Name = "txtEsh";
            this.txtEsh.Size = new System.Drawing.Size(40, 20);
            this.txtEsh.TabIndex = 17;
            this.txtEsh.TextChanged += new System.EventHandler(this.txtEsh_TextChanged);
            // 
            // txtXoph
            // 
            this.txtXoph.Location = new System.Drawing.Point(89, 414);
            this.txtXoph.Name = "txtXoph";
            this.txtXoph.Size = new System.Drawing.Size(40, 20);
            this.txtXoph.TabIndex = 18;
            // 
            // txtTul
            // 
            this.txtTul.Location = new System.Drawing.Point(166, 414);
            this.txtTul.Name = "txtTul";
            this.txtTul.Size = new System.Drawing.Size(40, 20);
            this.txtTul.TabIndex = 19;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 398);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Esh";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(88, 398);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Xoph";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(163, 398);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(22, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Tul";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(231, 385);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(181, 49);
            this.button1.TabIndex = 23;
            this.button1.Text = "Continue";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(79, 372);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "Splinters Per Chaos";
            // 
            // StartupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 463);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtTul);
            this.Controls.Add(this.txtXoph);
            this.Controls.Add(this.txtEsh);
            this.Controls.Add(this.chkUniqueRanges);
            this.Controls.Add(this.chkDoUniques);
            this.Controls.Add(this.chkDoWatchlist);
            this.Controls.Add(this.chkDoBreach);
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
            this.Load += new System.EventHandler(this.StartupForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.CheckBox chkDoBreach;
        private System.Windows.Forms.CheckBox chkDoWatchlist;
        private System.Windows.Forms.CheckBox chkDoUniques;
        private System.Windows.Forms.CheckBox chkUniqueRanges;
        private System.Windows.Forms.TextBox txtEsh;
        private System.Windows.Forms.TextBox txtXoph;
        private System.Windows.Forms.TextBox txtTul;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label9;
    }
}