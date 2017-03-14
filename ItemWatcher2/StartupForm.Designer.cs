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
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtFuse = new System.Windows.Forms.TextBox();
            this.txtExalt = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtAlch = new System.Windows.Forms.TextBox();
            this.txtMaxPrice = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtProfitPercent = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtMinProfit = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
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
            this.button1.Location = new System.Drawing.Point(245, 447);
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
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 447);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(58, 13);
            this.label10.TabIndex = 25;
            this.label10.Text = "Exalt Ratio";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(86, 447);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "Fuse Ratio";
            // 
            // txtFuse
            // 
            this.txtFuse.Location = new System.Drawing.Point(89, 474);
            this.txtFuse.Name = "txtFuse";
            this.txtFuse.Size = new System.Drawing.Size(40, 20);
            this.txtFuse.TabIndex = 29;
            // 
            // txtExalt
            // 
            this.txtExalt.Location = new System.Drawing.Point(19, 474);
            this.txtExalt.Name = "txtExalt";
            this.txtExalt.Size = new System.Drawing.Size(40, 20);
            this.txtExalt.TabIndex = 28;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(163, 447);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 13);
            this.label12.TabIndex = 27;
            this.label12.Text = "Alch Ratio";
            // 
            // txtAlch
            // 
            this.txtAlch.Location = new System.Drawing.Point(166, 474);
            this.txtAlch.Name = "txtAlch";
            this.txtAlch.Size = new System.Drawing.Size(40, 20);
            this.txtAlch.TabIndex = 30;
            // 
            // txtMaxPrice
            // 
            this.txtMaxPrice.Location = new System.Drawing.Point(224, 414);
            this.txtMaxPrice.Name = "txtMaxPrice";
            this.txtMaxPrice.Size = new System.Drawing.Size(88, 20);
            this.txtMaxPrice.TabIndex = 31;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(221, 398);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(88, 13);
            this.label13.TabIndex = 32;
            this.label13.Text = "Max Flip Price (c)";
            // 
            // txtProfitPercent
            // 
            this.txtProfitPercent.Location = new System.Drawing.Point(315, 414);
            this.txtProfitPercent.Name = "txtProfitPercent";
            this.txtProfitPercent.Size = new System.Drawing.Size(67, 20);
            this.txtProfitPercent.TabIndex = 33;
            this.txtProfitPercent.TextChanged += new System.EventHandler(this.txtProfitPercent_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(315, 398);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(76, 13);
            this.label14.TabIndex = 34;
            this.label14.Text = "Profit Ratio (%)";
            // 
            // txtMinProfit
            // 
            this.txtMinProfit.Location = new System.Drawing.Point(390, 414);
            this.txtMinProfit.Name = "txtMinProfit";
            this.txtMinProfit.Size = new System.Drawing.Size(50, 20);
            this.txtMinProfit.TabIndex = 35;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(390, 398);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(69, 13);
            this.label15.TabIndex = 36;
            this.label15.Text = "Flat profit min";
            this.label15.Click += new System.EventHandler(this.label15_Click);
            // 
            // StartupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 522);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txtMinProfit);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtProfitPercent);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtMaxPrice);
            this.Controls.Add(this.txtAlch);
            this.Controls.Add(this.txtFuse);
            this.Controls.Add(this.txtExalt);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
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
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtFuse;
        private System.Windows.Forms.TextBox txtExalt;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtAlch;
        private System.Windows.Forms.TextBox txtMaxPrice;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtProfitPercent;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtMinProfit;
        private System.Windows.Forms.Label label15;
    }
}