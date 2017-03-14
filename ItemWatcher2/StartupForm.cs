using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ItemWatcher2.Form1;

namespace ItemWatcher2
{
    public partial class StartupForm : Form
    {
        public static List<WatchedItem> allItems;
        public static string itemfilename = "SavedItems.json";
        public static string currencyfilename = "SavedCurrencies.json";
       
        
        public StartupForm()
        {
            InitializeComponent();
        }

        private void StartupForm_Load(object sender, EventArgs e)
        {
            LoadBasicInfo();

            reload();
            

        }

        private void reload()
        {
            this.listBox1.Items.Clear();
            foreach(WatchedItem item in allItems)
            {
                this.listBox1.Items.Add(item);

            }
            
        }

       
        
        public static void SaveNames()
        {
            string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(allItems);
            System.IO.File.Delete(itemfilename);
            System.IO.File.WriteAllText(itemfilename, serialized);
        }
        public static void LoadBasicInfo()
        {
            try
            {
                allItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WatchedItem>>(System.IO.File.ReadAllText(itemfilename));
            }
            catch (Exception e)
            {
                allItems = new List<WatchedItem>();
            }
            try
            {
                othercurrencies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NotChaosCurrencyConversion>>(System.IO.File.ReadAllText(currencyfilename));
            }
            catch (Exception e)
            {
                othercurrencies = new List<NotChaosCurrencyConversion>();
            }
        }

        private void btnAddNewClick(object sender, EventArgs e)
        {
            allItems.Add(new WatchedItem()
            {
                name = txtItemName.Text,
                value = Convert.ToDouble(txtItemValue.Text)
            });
            reload();
        }

        private void btnDeleteClick(object sender, EventArgs e)
        {
            try
            {
                allItems.RemoveAt(Convert.ToInt32(txtIndex.Text));
                reload();
            }
            catch(Exception eeeee)
            {

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveNames();
        }
    }
}
