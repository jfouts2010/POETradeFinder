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
            this.txtAccountName = new System.Windows.Forms.TextBox();
            this.txtIndex = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkDoWatchNames = new System.Windows.Forms.CheckBox();
            this.chkDoWatchRares = new System.Windows.Forms.CheckBox();
            this.chkDoUniques = new System.Windows.Forms.CheckBox();
            this.chkUniqueRanges = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
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
            this.txtNumberOfPeople = new System.Windows.Forms.TextBox();
            this.txtYourNumber = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txtRefreshHours = new System.Windows.Forms.TextBox();
            this.chkAutoCopy = new System.Windows.Forms.CheckBox();
            this.JohnSounds = new System.Windows.Forms.CheckBox();
            this.chkBoxCatchup = new System.Windows.Forms.CheckBox();
            this.chkCraftables = new System.Windows.Forms.CheckBox();
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
            // txtAccountName
            // 
            this.txtAccountName.Location = new System.Drawing.Point(290, 273);
            this.txtAccountName.Name = "txtAccountName";
            this.txtAccountName.Size = new System.Drawing.Size(100, 20);
            this.txtAccountName.TabIndex = 5;
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
            this.label4.Location = new System.Drawing.Point(293, 257);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Your Account Name";
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
            this.btnSave.Location = new System.Drawing.Point(315, 84);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkDoWatchNames
            // 
            this.chkDoWatchNames.AutoSize = true;
            this.chkDoWatchNames.Location = new System.Drawing.Point(12, 337);
            this.chkDoWatchNames.Name = "chkDoWatchNames";
            this.chkDoWatchNames.Size = new System.Drawing.Size(117, 17);
            this.chkDoWatchNames.TabIndex = 13;
            this.chkDoWatchNames.Text = "Do Rare WatchList";
            this.chkDoWatchNames.UseVisualStyleBackColor = true;
            // 
            // chkDoWatchRares
            // 
            this.chkDoWatchRares.AutoSize = true;
            this.chkDoWatchRares.Location = new System.Drawing.Point(132, 337);
            this.chkDoWatchRares.Name = "chkDoWatchRares";
            this.chkDoWatchRares.Size = new System.Drawing.Size(87, 17);
            this.chkDoWatchRares.TabIndex = 14;
            this.chkDoWatchRares.Text = "Do Watchlist";
            this.chkDoWatchRares.UseVisualStyleBackColor = true;
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
            // txtNumberOfPeople
            // 
            this.txtNumberOfPeople.Location = new System.Drawing.Point(315, 135);
            this.txtNumberOfPeople.Name = "txtNumberOfPeople";
            this.txtNumberOfPeople.Size = new System.Drawing.Size(100, 20);
            this.txtNumberOfPeople.TabIndex = 37;
            // 
            // txtYourNumber
            // 
            this.txtYourNumber.Location = new System.Drawing.Point(315, 176);
            this.txtYourNumber.Name = "txtYourNumber";
            this.txtYourNumber.Size = new System.Drawing.Size(100, 20);
            this.txtYourNumber.TabIndex = 38;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(315, 115);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(92, 13);
            this.label16.TabIndex = 39;
            this.label16.Text = "Number of People";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(312, 160);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(69, 13);
            this.label17.TabIndex = 40;
            this.label17.Text = "Your Number";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(312, 199);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(84, 13);
            this.label18.TabIndex = 42;
            this.label18.Text = "Refresh Minutes";
            // 
            // txtRefreshHours
            // 
            this.txtRefreshHours.Location = new System.Drawing.Point(315, 215);
            this.txtRefreshHours.Name = "txtRefreshHours";
            this.txtRefreshHours.Size = new System.Drawing.Size(100, 20);
            this.txtRefreshHours.TabIndex = 41;
            // 
            // chkAutoCopy
            // 
            this.chkAutoCopy.AutoSize = true;
            this.chkAutoCopy.Location = new System.Drawing.Point(231, 369);
            this.chkAutoCopy.Name = "chkAutoCopy";
            this.chkAutoCopy.Size = new System.Drawing.Size(72, 17);
            this.chkAutoCopy.TabIndex = 43;
            this.chkAutoCopy.Text = "AutoCopy";
            this.chkAutoCopy.UseVisualStyleBackColor = true;
            this.chkAutoCopy.CheckedChanged += new System.EventHandler(this.chkAutoCopy_CheckedChanged);
            // 
            // JohnSounds
            // 
            this.JohnSounds.AutoSize = true;
            this.JohnSounds.Location = new System.Drawing.Point(327, 369);
            this.JohnSounds.Name = "JohnSounds";
            this.JohnSounds.Size = new System.Drawing.Size(88, 17);
            this.JohnSounds.TabIndex = 44;
            this.JohnSounds.Text = "John Sounds";
            this.JohnSounds.UseVisualStyleBackColor = true;
            this.JohnSounds.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // chkBoxCatchup
            // 
            this.chkBoxCatchup.AutoSize = true;
            this.chkBoxCatchup.Location = new System.Drawing.Point(12, 369);
            this.chkBoxCatchup.Name = "chkBoxCatchup";
            this.chkBoxCatchup.Size = new System.Drawing.Size(126, 17);
            this.chkBoxCatchup.TabIndex = 45;
            this.chkBoxCatchup.Text = "Run Catchup Thread";
            this.chkBoxCatchup.UseVisualStyleBackColor = true;
            // 
            // chkCraftables
            // 
            this.chkCraftables.AutoSize = true;
            this.chkCraftables.Location = new System.Drawing.Point(132, 369);
            this.chkCraftables.Name = "chkCraftables";
            this.chkCraftables.Size = new System.Drawing.Size(90, 17);
            this.chkCraftables.TabIndex = 46;
            this.chkCraftables.Text = "Do Craftables";
            this.chkCraftables.UseVisualStyleBackColor = true;
            // 
            // StartupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 522);
            this.Controls.Add(this.chkCraftables);
            this.Controls.Add(this.chkBoxCatchup);
            this.Controls.Add(this.JohnSounds);
            this.Controls.Add(this.chkAutoCopy);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.txtRefreshHours);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.txtYourNumber);
            this.Controls.Add(this.txtNumberOfPeople);
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
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkUniqueRanges);
            this.Controls.Add(this.chkDoUniques);
            this.Controls.Add(this.chkDoWatchRares);
            this.Controls.Add(this.chkDoWatchNames);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIndex);
            this.Controls.Add(this.txtAccountName);
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
        private System.Windows.Forms.TextBox txtAccountName;
        private System.Windows.Forms.TextBox txtIndex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkDoWatchNames;
        private System.Windows.Forms.CheckBox chkDoWatchRares;
        private System.Windows.Forms.CheckBox chkDoUniques;
        private System.Windows.Forms.CheckBox chkUniqueRanges;
        private System.Windows.Forms.Button button1;
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
        private System.Windows.Forms.TextBox txtNumberOfPeople;
        private System.Windows.Forms.TextBox txtYourNumber;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtRefreshHours;
        private System.Windows.Forms.CheckBox chkAutoCopy;
        private System.Windows.Forms.CheckBox JohnSounds;
        private System.Windows.Forms.CheckBox chkBoxCatchup;
        private System.Windows.Forms.CheckBox chkCraftables;
    }
}