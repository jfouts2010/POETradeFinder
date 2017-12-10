using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ItemWatcher2.Form1;

namespace ItemWatcher2
{
    public partial class StartupForm : Form
    {
        public static List<NinjaItem> allItems;
        public static ItemWatchConfig config;
        public static string itemfilename = "SavedItems.json";
        public static string currencyfilename = "SavedCurrencies.json";


        public StartupForm()
        {
            InitializeComponent();

            string SearchIDforGreaterThan5c = GetSearchID();

            List<string> ItemIDs = GetIdsToGetMoreInfoFor(SearchIDforGreaterThan5c);
            string s = string.Join(",", ItemIDs.ToArray());
            HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("https://www.pathofexile.com/api/trade/fetch/"+string.Join(",", ItemIDs.ToArray())+ "?query=yrgjgnha");
            request23.Method = "GET";
            request23.KeepAlive = true;
            request23.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            //request23.ContentType = "application/json";
            request23.Host = "www.pathofexile.com";
            request23.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";
            //code to get the ID of the search
            using (HttpWebResponse response2 = request23.GetResponse() as HttpWebResponse)
            {
                using (var reader = new StreamReader(response2.GetResponseStream()))
                {
                    JObject jo = JObject.Parse(reader.ReadToEnd());
                }
            }
        }
        public string GetSearchID()
        {
            HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("https://www.pathofexile.com/api/trade/search/Standard");
            request23.Method = "POST";
            request23.KeepAlive = true;
            request23.ContentType = "application/json";
            StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());
            string SearchIDforGreaterThan5c = "";
            postwriter.Write("{\"query\":{\"status\":{\"option\":\"any\"},\"name\":\"Yoke of Suffering\",\"type\":\"Onyx Amulet\",\"stats\":[{\"type\":\"and\",\"filters\":[]}]},\"sort\":{\"price\":\"asc\"}}");
            //postwriter.Write("{\"query\":{\"status\":{\"option\":\"any\"},\"stats\":[{\"type\":\"and\",\"filters\":[]}],\"filters\":{\"trade_filters\":{\"filters\":{\"price\":{\"min\":5}}}}},\"sort\":{\"price\":\"asc\"}}");
            postwriter.Close();
            //code to get the ID of the search
            using (HttpWebResponse response2 = request23.GetResponse() as HttpWebResponse)
            {
                using (var reader = new StreamReader(response2.GetResponseStream()))
                {
                    JObject jo = JObject.Parse(reader.ReadToEnd());
                    SearchIDforGreaterThan5c = jo["id"].ToString();
                }
            }
            return SearchIDforGreaterThan5c;
        }
        public List<string> GetIdsToGetMoreInfoFor(string id)
        {
            List<string> ItemIds = new List<string>();
            HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("https://www.pathofexile.com/api/trade/search/Standard/" + id);
            request23.Method = "POST";
            request23.KeepAlive = true;
            request23.ContentType = "application/json";
            //code to get the item IDs to ask more information for
            using (HttpWebResponse response2 = request23.GetResponse() as HttpWebResponse)
            {
                using (var reader = new StreamReader(response2.GetResponseStream()))
                {
                    JObject jo = JObject.Parse(reader.ReadToEnd());
                    foreach(JToken jt in jo["result"])
                    {
                        ItemIds.Add(jt.ToString());
                    }
                }
            }
            return ItemIds;
        }
        private void StartupForm_Load(object sender, EventArgs e)
        {
            LoadBasicInfo();
            reload();
        }

        private void reload()
        {
            this.listBox1.Items.Clear();
            foreach (NinjaItem item in allItems)
            {
                this.listBox1.Items.Add(item);
            }

        }



        public void SaveNames()
        {
            string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(allItems);
            System.IO.File.Delete(itemfilename);
            System.IO.File.WriteAllText(itemfilename, serialized);


            if (config.blocked_accounts == null)
                config.blocked_accounts = new List<string>();
            config.autoCopy = chkAutoCopy.Checked;
            config.johnsounds = JohnSounds.Checked;
            config.do_catchup_thread = chkBoxCatchup.Checked;


            config.do_watch_rares = this.chkDoWatchNames.Checked;
            config.do_all_uniques = this.chkDoUniques.Checked;
            config.do_craft_watch = this.chkCraftables.Checked;
            config.do_all_uniques_with_ranges = this.chkUniqueRanges.Checked;
            config.do_watch_list = this.chkDoWatchRares.Checked;

            config.account_name = this.txtAccountName.Text;

            config.alch_ratio = Convert.ToDecimal(this.txtAlch.Text);
            config.exalt_ratio = Convert.ToDecimal(this.txtExalt.Text);
            config.fusing_ratio = Convert.ToDecimal(this.txtFuse.Text);

            config.profit_percent = Convert.ToDecimal(this.txtProfitPercent.Text);
            config.max_price = Convert.ToInt32(this.txtMaxPrice.Text);
            config.min_profit_range = Convert.ToInt32(this.txtMinProfit.Text);

            config.my_number = Convert.ToInt32(this.txtYourNumber.Text);
            config.number_of_people = Convert.ToInt32(this.txtNumberOfPeople.Text);
            config.refresh_minutes = Convert.ToDouble(this.txtRefreshHours.Text);
            serialized = Newtonsoft.Json.JsonConvert.SerializeObject(config);
            JObject jo = JObject.Parse(serialized);
            serialized = jo.ToString();
            System.IO.File.Delete(FinalVariables.configfile);
            System.IO.File.WriteAllText(FinalVariables.configfile, serialized);
        }
        public void LoadBasicInfo()
        {
            try
            {
                allItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NinjaItem>>(System.IO.File.ReadAllText(itemfilename));
            }
            catch (Exception e)
            {
                allItems = new List<NinjaItem>();
            }
            try
            {
                config = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemWatchConfig>(System.IO.File.ReadAllText(FinalVariables.configfile));
            }
            catch (Exception e)
            {
                config = new ItemWatchConfig()
                {
                    do_all_uniques = true,
                    do_all_uniques_with_ranges = false,
                    do_watch_rares = true,
                    do_watch_list = true,
                    do_craft_watch = true,
                    exalt_ratio = 68,
                    alch_ratio = .333M,
                    fusing_ratio = .444M,
                    refresh_minutes = 1,
                    LastSaved = DateTime.Now,
                    number_of_people = 1,
                    my_number = 1,
                    max_price = 100,
                    profit_percent = .9M,

                };
            }

            this.chkDoWatchNames.Checked = config.do_watch_rares;
            this.chkDoWatchRares.Checked = config.do_watch_list;
            this.chkCraftables.Checked = config.do_craft_watch;
            this.chkDoUniques.Checked = config.do_all_uniques;
            this.chkUniqueRanges.Checked = config.do_all_uniques_with_ranges;
            this.txtExalt.Text = config.exalt_ratio.ToString();
            this.txtFuse.Text = config.fusing_ratio.ToString();
            this.txtAlch.Text = config.alch_ratio.ToString();
            this.txtMaxPrice.Text = config.max_price.ToString();
            this.txtProfitPercent.Text = config.profit_percent.ToString();
            this.txtMinProfit.Text = config.min_profit_range.ToString();
            this.txtNumberOfPeople.Text = config.number_of_people.ToString();
            this.txtYourNumber.Text = config.my_number.ToString();
            this.txtRefreshHours.Text = config.refresh_minutes.ToString();
            this.chkAutoCopy.Checked = config.autoCopy;
            this.JohnSounds.Checked = config.johnsounds;
            this.txtAccountName.Text = config.account_name;
            this.chkBoxCatchup.Checked = config.do_catchup_thread;
        }

        private void btnAddNewClick(object sender, EventArgs e)
        {
            allItems.Add(new NinjaItem()
            {
                name = txtItemName.Text,
                chaos_value = Convert.ToDecimal(txtItemValue.Text)
            });
            reload();
        }

        private void btnDeleteClick(object sender, EventArgs e)
        {
            try
            {
                allItems.RemoveAt(listBox1.SelectedIndex);
                reload();
            }
            catch (Exception eeeee)
            {

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveNames();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveNames();
            this.Close();
        }

        private void txtEsh_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void txtProfitPercent_TextChanged(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkAutoCopy_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
