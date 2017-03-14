using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ItemWatcher2
{
    public partial class Form1 : Form
    {
        public static List<WatchedItem> allItems;
        public static string itemfilename = "SavedItems.json";
        public static string currencyfilename = "SavedCurrencies.json";
        private static BackgroundWorker bgw;
        public static List<Slot> Slots = new List<Form1.Slot>();
        public static List<NotChaosCurrencyConversion> othercurrencies;
        public Form1()
        {
            InitializeComponent();
            bgw = new BackgroundWorker();
            bgw.DoWork += DoBackgroundWork;
            bgw.RunWorkerAsync();

        }
        private void DoBackgroundWork(object sender, DoWorkEventArgs e)
        {

            List<NinjaItem> NinjaItems = new List<NinjaItem>();
            textBox1.Invoke((MethodInvoker)delegate
            {
                textBox1.Text = "Converting Poe.Ninja Items";
            });
            SetNinjaValues(NinjaItems);
            textBox1.Invoke((MethodInvoker)delegate
            {
                textBox1.Text = "Poe.Ninja Items Converted";
            });
            HttpWebRequest request2 = WebRequest.Create("http://api.poe.ninja/api/Data/GetStats") as HttpWebRequest;

            string changeID = "48923177-51911962-48505106-56446125-56515275";
            // Get response  
            using (HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                using (StreamReader reader = new StreamReader(response2.GetResponseStream()))
                {
                    JObject jo = JObject.Parse(reader.ReadToEnd());
                    changeID = jo.Children().ToList()[1].First.ToString();
                }
            }
            // Create the web request  
            textBox1.Invoke((MethodInvoker)delegate
            {
                textBox1.Text = "Running";
            });
            while (true)
            {
                HttpWebRequest request = WebRequest.Create("http://www.pathofexile.com/api/public-stash-tabs?id=" + changeID) as HttpWebRequest;

                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        // Console application output  
                        System.Threading.Thread.Sleep(1000);
                        List<JToken> jo = JObject.Parse(reader.ReadToEnd()).Children().ToList();
                        changeID = jo[0].First.ToString();
                        List<JToken> stashes = jo[1].First.Children().ToList();
                        foreach (JToken jt in stashes)
                        {
                            List<JToken> stash = jt.Children().ToList();
                            string name = stash.First(p => p.Path.EndsWith(".lastCharacterName")).First.ToString();
                            List<JToken> items = stash.First(p => p.Path.EndsWith(".items")).First.Children().ToList();
                            foreach (JToken item in items)
                            {
                                Item itemProp = item.ToObject<Item>();
                                if (itemProp.league != "Legacy")
                                    continue;
                                itemProp.name = itemProp.name.Replace("<<set:MS>><<set:M>><<set:S>>", "");
                                if (NinjaItems.Where(p => p.name == itemProp.name && p.type == itemProp.frameType.ToString() && p.base_type == itemProp.typeLine).Count() > 0 && itemProp.note != null && itemProp.note.Contains("chaos"))
                                {
                                    NinjaItem NinjaItem = NinjaItems.First(p => p.name == itemProp.name && p.type == itemProp.frameType.ToString() && p.base_type == itemProp.typeLine);
                                    if (NinjaItem.chaos_value * 0.75 > getTheNumbers(itemProp.note))
                                    {

                                    }
                                }

                                if(allItems.Where(p => itemProp.name.Contains( p.name)).Count()>0)
                                {

                                }
                                if (item.Where(p => p.Path.EndsWith(".properties")).Count() > 0 && itemProp.typeLine.Contains("Breach Leaguestone"))
                                {
                                    string what = item.First(p => p.Path.EndsWith(".properties")).First.First.Children().ToList()[1].First.Children().ToList()[0].First.ToString();
                                    if (what == "5")
                                    {
                                        if (itemProp.typeLine.Contains("Talisman Leaguestone of Terror") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                        {

                                            if (itemProp.note != null && itemProp.note.Contains("chaos") && getTheNumbers(itemProp.note) < 20)
                                            {
                                                string s = name + " has a " + itemProp.typeLine + " with note:" + itemProp.note + " (worth60c)";
                                                Console.WriteLine(s);
                                                s = "@" + name + " Hi, I'd like to buy your Talisman Leaguestone of Terror for " + getTheNumbers(itemProp.note) + " chaos";
                                                Clipboard.SetText(s);
                                                SoundPlayer player = new SoundPlayer();
                                                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                player.Play();
                                            }
                                        }
                                        else if (itemProp.typeLine.Contains("Talisman Leaguestone of Fear") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                        {

                                            if (itemProp.note != null && itemProp.note.Contains("chaos") && getTheNumbers(itemProp.note) < 7)
                                            {
                                                string s = name + " has a " + itemProp.typeLine + " with note:" + itemProp.note + " (worth7c)";
                                                Console.WriteLine(s);
                                                s = "@" + name + " Hi, I'd like to buy your Talisman Leaguestone of Fear for " + getTheNumbers(itemProp.note) + " chaos";
                                                Clipboard.SetText(s);
                                                SoundPlayer player = new SoundPlayer();
                                                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                player.Play();
                                            }
                                        }
                                        //2.1 per breach * 5 * 3 = 31c
                                        if (itemProp.typeLine.Contains("Plentiful Breach Leaguestone of Splinters") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                        {

                                            if (itemProp.note != null && itemProp.note.Contains("chaos") && getTheNumbers(itemProp.note) < 20)
                                            {
                                                string s = name + " has a PLENTIFUL SPLINTERS Breachstone with note:" + itemProp.note + " (worth31c)";
                                                Console.WriteLine(s);
                                                s = "@" + name + " Hi, I'd like to buy your Plentiful Breach Leaguestone of Splinters for " + getTheNumbers(itemProp.note) + " chaos";
                                                Clipboard.SetText(s);
                                                SoundPlayer player = new SoundPlayer();
                                                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                player.Play();
                                            }
                                        }
                                        //2.1 per breach * 5 * 2 = 21c
                                        else if (itemProp.typeLine.Contains("Ample Breach Leaguestone of Splinters") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                        {
                                            if (itemProp.note != null && itemProp.note.Contains("chaos") && getTheNumbers(itemProp.note) < 12)
                                            {
                                                string s = name + " has a AMPLE SPLINTERS Breachstone with note:" + itemProp.note + " (worth21c)";
                                                Console.WriteLine(s);
                                                s = "@" + name + " Hi, I'd like to buy your Ample Breach Leaguestone of Splinters for " + getTheNumbers(itemProp.note) + " chaos";
                                                Clipboard.SetText(s);
                                                SoundPlayer player = new SoundPlayer();
                                                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                player.Play();
                                            }
                                        }
                                        //1.26 per breach * 5 * 3 = 19c
                                        else if (itemProp.typeLine.Contains("Plentiful Breach Leaguestone") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                        {
                                            if (itemProp.note != null && itemProp.note.Contains("chaos") && getTheNumbers(itemProp.note) < 12)
                                            {
                                                string s = name + " has a PLENTIFUL Breachstone with note:" + itemProp.note + " (worth19c)";
                                                Console.WriteLine(s);
                                                s = "@" + name + " Hi, I'd like to buy your Plentiful Breach Leaguestone for " + getTheNumbers(itemProp.note) + " chaos";
                                                Clipboard.SetText(s);
                                                SoundPlayer player = new SoundPlayer();
                                                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                player.Play();
                                            }
                                        }
                                        //2.1 per breach * 5 = 10c
                                        else if (itemProp.typeLine.Contains("Breach Leaguestone of Splinters") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                        {
                                            if (itemProp.note != null && itemProp.note.Contains("chaos") && getTheNumbers(itemProp.note) < 5)
                                            {
                                                string s = name + " has a SPLINTERS Breachstone with note:" + itemProp.note + " (worth10c)";
                                                Console.WriteLine(s);
                                                s = "@" + name + " Hi, I'd like to buy your Breach Leaguestone of Splinters for " + getTheNumbers(itemProp.note) + " chaos";
                                                Clipboard.SetText(s);
                                                SoundPlayer player = new SoundPlayer();
                                                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                player.Play();
                                            }
                                        }
                                        //1.26 per breach * 5 * 2 = 12c
                                        else if (itemProp.typeLine.Contains("Ample Breach") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                        {
                                            if (itemProp.note != null && itemProp.note.Contains("chaos") && getTheNumbers(itemProp.note) < 6)
                                            {
                                                string s = name + " has a AMPLE Breachstone with note:" + itemProp.note + " (worth12c)";
                                                Console.WriteLine(s);
                                                s = "@" + name + " Hi, I'd like to buy your Ample Breach Leaguestone for " + getTheNumbers(itemProp.note) + " chaos";
                                                Clipboard.SetText(s);
                                                SoundPlayer player = new SoundPlayer();
                                                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                player.Play();
                                            }
                                        }
                                        //1c per splinter * 10 * 5 = 50c
                                        if (itemProp.typeLine.Contains("Dreaming Breach Leaguestone of Splinters") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                        {
                                            if (itemProp.note != null && itemProp.note.Contains("chaos") && getTheNumbers(itemProp.note) < 35)
                                            {
                                                string s = name + " has a CHALUPA SPLINTERS Breachstone with note:" + itemProp.note + " (worth50c)";
                                                Console.WriteLine(s);
                                                s = "@" + name + " Hi, I'd like to buy your Dreaming Breach Leaguestone of Splinters for " + getTheNumbers(itemProp.note) + " chaos";
                                                Clipboard.SetText(s);
                                                SoundPlayer player = new SoundPlayer();
                                                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                player.Play();
                                            }
                                        }
                                        //1c per splinter * 6 * 5 = 30c
                                        else if (itemProp.typeLine.Contains("Dreaming Breach") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                        {
                                            if (itemProp.note != null && itemProp.note.Contains("chaos") && getTheNumbers(itemProp.note) < 20)
                                            {
                                                string s = name + " has a CHALUPA Breachstone with note:" + itemProp.note + " (worth30c)";
                                                Console.WriteLine(s);
                                                s = "@" + name + " Hi, I'd like to buy your Dreaming Breach Leaguestone for " + getTheNumbers(itemProp.note) + " chaos";
                                                Clipboard.SetText(s);
                                                SoundPlayer player = new SoundPlayer();
                                                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                player.Play();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static int getTheNumbers(string input)
        {
            char[] x = input.Where(c => char.IsDigit(c)).ToArray();
            string y = new string(input.Where(c => char.IsDigit(c)).ToArray());
            return (int)(Convert.ToInt32(y));
        }
        public static void SetNinjaValues(List<NinjaItem> NinjaItems)
        {
            List<JObject> Jsons = new List<JObject>();
            List<string> APIURLS = new List<string>();
            APIURLS.Add("http://api.poe.ninja/api/Data/GetDivinationCardsOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"));
            APIURLS.Add("http://api.poe.ninja/api/Data/GetProphecyOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"));
            APIURLS.Add("http://api.poe.ninja/api/Data/GetUniqueMapOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"));
            APIURLS.Add("http://api.poe.ninja/api/Data/GetUniqueJewelOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"));
            APIURLS.Add("http://api.poe.ninja/api/Data/GetUniqueFlaskOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"));
            APIURLS.Add("http://api.poe.ninja/api/Data/GetUniqueWeaponOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"));
            APIURLS.Add("http://api.poe.ninja/api/Data/GetUniqueArmourOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"));
            APIURLS.Add("http://api.poe.ninja/api/Data/GetUniqueAccessoryOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"));
            foreach (string s in APIURLS)
            {
                HttpWebRequest request2 = WebRequest.Create(s) as HttpWebRequest;
                // Get response  
                using (HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    using (StreamReader reader = new StreamReader(response2.GetResponseStream()))
                    {
                        Jsons.Add(JObject.Parse(reader.ReadToEnd()));
                    }
                }
            }
            foreach (JObject jo in Jsons)
            {
                foreach (JObject jo2 in jo.First.First.Children().ToList())
                {
                    NinjaItem newNinjaItem = new NinjaItem();
                    newNinjaItem.name = jo2.Children().ToList().First(p => p.Path.EndsWith(".name")).First.ToString();
                    newNinjaItem.type = jo2.Children().ToList().First(p => p.Path.EndsWith(".itemClass")).First.ToString();
                    newNinjaItem.base_type = jo2.Children().ToList().First(p => p.Path.EndsWith(".baseType")).First.ToString();
                    List<string> implicits = new List<string>();
                    foreach (JArray j in jo2.Children().ToList().First(p => p.Path.EndsWith(".implicitModifiers")))
                    {
                        foreach (JObject implicitRoll in j)
                        {
                            implicits.Add(implicitRoll.First.First.ToString());
                        }
                    }
                    List<string> explicits = new List<string>();
                    foreach (JArray j in jo2.Children().ToList().First(p => p.Path.EndsWith(".explicitModifiers")))
                    {
                        foreach (JObject explicitRoll in j)
                        {
                            explicits.Add(explicitRoll.First.First.ToString());
                        }
                    }
                    newNinjaItem.Explicits = explicits;
                    newNinjaItem.Implicits = implicits;
                    newNinjaItem.chaos_value = Convert.ToDouble(jo2.Children().ToList().First(p => p.Path.EndsWith(".chaosValue")).First.ToString());
                    if (newNinjaItem.chaos_value > 5 && !newNinjaItem.name.Contains("Atziri's Splendour") && !newNinjaItem.name.Contains("Doryani's Invitation") && !newNinjaItem.name.Contains("Vessel of Vinktar"))
                        NinjaItems.Add(newNinjaItem);
                }
            }
        }
        public class NinjaItem
        {
            public string name { get; set; }
            public double chaos_value { get; set; }
            public string type { get; set; }
            public string base_type { get; set; }
            public List<string> Implicits { get; set; }
            public List<string> Explicits { get; set; }

            public override string ToString()
            {
                return "name: " + name + " cost: " + chaos_value; 

            }
        }
        public static void AddNewName(string name, string value)
        {
            allItems.Add(new WatchedItem()
            {
                name = name,
                value = Convert.ToDouble(value),
            });
        }

        public static void RemoveName(int index)
        {
            allItems.RemoveAt(index);
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









        public class Slot
        {
            public Item SellItem;
            public NinjaItem BaseItem;
        }
        public class Item
        {
            public string verified { get; set; }
            public string w { get; set; }
            public string h { get; set; }
            public string ilvl { get; set; }
            public string league { get; set; }
            public string id { get; set; }
            //public string[] sockets { get; set; }
            public string name { get; set; }
            public string typeLine { get; set; }
            public string identified { get; set; }
            public string corrupted { get; set; }
            public string note { get; set; }
            public int frameType { get; set; }
            //public string requirements { get; set; }
            public string[] implicitMods { get; set; }
            public string[] explicitMods { get; set; }
        }
        [Serializable]
        public class WatchedItem
        {
            public string name { get; set; }
            public double value { get; set; }

            public override string ToString()
            {
                return name+" : " + value;
            }
        }

        public class NotChaosCurrencyConversion
        {
            public string name { get; set; }
            public decimal valueInChaos { get; set; }
        }
    }
}
