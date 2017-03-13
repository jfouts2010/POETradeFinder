using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Media;

namespace ConsoleApplication2
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            List<NinjaItem> NinjaItems = new List<NinjaItem>();
            SetNinjaValues(NinjaItems);
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
                                itemProp.name = itemProp.name.Replace("<<set:MS>><<set:M>><<set:S>>", "");
                                if (NinjaItems.Where(p => p.name == itemProp.name).Count() > 0 && itemProp.note != null && itemProp.note.Contains("chaos"))
                                {
                                    NinjaItem NinjaItem = NinjaItems.First(p => p.name == itemProp.name);
                                    if (NinjaItem.chaos_value * 0.75 < getTheNumbers(itemProp.note))
                                    {

                                    }
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
                    newNinjaItem.chaos_value = Convert.ToDouble(jo2.Children().ToList().First(p => p.Path.EndsWith(".chaosValue")).First.ToString());
                    if(newNinjaItem.chaos_value > 5)
                        NinjaItems.Add(newNinjaItem);
                }
            }
        }
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
        //public string requirements { get; set; }
        public string[] implicitMods { get; set; }
        public string[] explicitMods { get; set; }
    }
    public class NinjaItem
    {
        public string name { get; set; }
        public double chaos_value { get; set; }
        public string type { get; set; }
    }
}
