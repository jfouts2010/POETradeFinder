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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ItemWatcher2
{
    public partial class Form1 : Form
    {
        public static List<NinjaItem> allItems;
        public static ItemWatchConfig config;
        public static string itemfilename = "SavedItems.json";
        public static string currencyfilename = "SavedCurrencies.json";
        public static string configfile = "Config.json";

        private static BackgroundWorker bgw;
        public static List<Slot> Slots = new List<Form1.Slot>();
        public static List<Slot> LeaguestoneSlots = new List<Form1.Slot>();
        public static List<NotChaosCurrencyConversion> othercurrencies;
        public Form1()
        {
            InitializeComponent();
            bgw = new BackgroundWorker();
            bgw.DoWork += DoBackgroundWork;
            BackgroundWorker bgw2 = new BackgroundWorker();
            bgw2.DoWork += SyncNinja;
            bgw.RunWorkerAsync();
            bgw2.RunWorkerAsync();

        }
        private void SyncNinja(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(10000);
            LoadBasicInfo();
            while (true)
            {
                if (config.LastSaved.AddHours(1) < DateTime.Now)
                {
                    SetNinjaValues(new List<NinjaItem>());
                    LoadBasicInfo();
                }
                else
                    System.Threading.Thread.Sleep(60000);
            }
        }
        [STAThread]
        private void DoBackgroundWork(object sender, DoWorkEventArgs e)
        {
            LoadBasicInfo();
            DateTime lastTimeAPIcalled = DateTime.Now;
            Dictionary<string, Item> seenItems = new Dictionary<string, Item>();
            DateTime lastClearedSeen = DateTime.Now;
            DateTime refreshConfig = DateTime.Now;
            Slots.Add(new Slot());
            Slots.Add(new Slot());
            Slots.Add(new Slot());
            LeaguestoneSlots.Add(new Slot());
            LeaguestoneSlots.Add(new Slot());
            LeaguestoneSlots.Add(new Slot());
            List<NinjaItem> NinjaItems = new List<NinjaItem>();
            textBox1.Invoke((MethodInvoker)delegate
            {
                textBox1.Text = "Converting Poe.Ninja Items";
            });
            if (config.do_all_uniques) ;// && config.LastSaved.AddDays(1) < DateTime.Now)
                NinjaItems = SetNinjaValues(NinjaItems);
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
                try
                {
                    if (DateTime.Now.Subtract(lastClearedSeen).TotalSeconds > 3600)
                    {
                        seenItems.Clear();
                    }
                    SetTimeseconds(Slots);
                    if (config.do_all_uniques_with_ranges &&  DateTime.Now.Subtract(refreshConfig).TotalSeconds > 600)
                    {
                        LoadBasicInfo();
                        NinjaItems = config.SavedItems;
                        refreshConfig = DateTime.Now;
                    }
                    HttpWebRequest request = WebRequest.Create("http://www.pathofexile.com/api/public-stash-tabs?id=" + changeID) as HttpWebRequest;

                    // Get response  
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        // Get the response stream  
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // Console application output  
                            lastTimeAPIcalled = DateTime.Now;
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
                                    if (string.IsNullOrEmpty(itemProp.note))
                                        continue;
                                    if (seenItems.ContainsKey(itemProp.id))
                                        continue;
                                    else
                                        seenItems.Add(itemProp.id, itemProp);
                                    /*if (config.number_of_people > 1)
                                    {
                                        int whogot = findWhoGets(itemProp.id, config.number_of_people);
                                        if (config.my_number != whogot)
                                            continue;
                                    }*/
                                    if (itemProp.implicitMods == null)
                                        itemProp.implicitMods = new string[] { "" };
                                    if (itemProp.explicitMods == null)
                                        itemProp.explicitMods = new string[] { "" };

                                    decimal itemValue = GetPriceInChaos(itemProp.note);
                                    if (itemValue > config.max_price)
                                        continue;
                                    itemProp.name = itemProp.name.Replace("<<set:MS>><<set:M>><<set:S>>", "");
                                    itemProp.typeLine = itemProp.typeLine.Replace("<<set:MS>><<set:M>><<set:S>>", "");
                                    if (config.do_all_uniques)
                                        if (NinjaItems.Where(p => p.name == itemProp.name && p.type == itemProp.frameType.ToString() && p.base_type == itemProp.typeLine).Count() > 0)
                                        {
                                            NinjaItem NinjaItem = NinjaItems.First(p => p.name == itemProp.name && p.type == itemProp.frameType.ToString() && p.base_type == itemProp.typeLine);
                                            if (NinjaItem.chaos_value * config.profit_percent > itemValue && NinjaItem.chaos_value - config.min_profit_range > itemValue)
                                            {
                                                if (NinjaItem.chaos_value - itemValue > 50)
                                                {
                                                    SoundPlayer player2 = new SoundPlayer();
                                                    player2.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                    player2.Play();
                                                    System.Threading.Thread.Sleep(50);
                                                    player2.Play();
                                                    System.Threading.Thread.Sleep(50);
                                                    player2.Play();
                                                    System.Threading.Thread.Sleep(50);
                                                }
                                                Slot s = new Slot();
                                                s.BaseItem = NinjaItem;
                                                s.SellItem = itemProp;
                                                s.name = name;
                                                s.is_mine = findWhoGets(itemProp.id, config.number_of_people) == config.my_number;
                                                s.Message = "@" + name + " Hi, I would like to buy your " + itemProp.name + " " + itemProp.typeLine + " listed for " + GetOriginalPrice(itemProp.note) + " in Legacy (stash tab \"" + itemProp.inventoryId + "\"; position: left " + itemProp.x + ", top " + itemProp.y + ")";
                                                if (Slots.Count == 3)
                                                    Slots.RemoveAt(2);
                                                Slots.Insert(0, s);
                                                SetSlots(Slots);
                                                SoundPlayer player = new SoundPlayer();
                                                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                player.Play();
                                            }
                                        }
                                    if (config.do_watch_list)
                                        if (allItems.Where(p => itemProp.name.ToLower().Contains(p.name.ToLower()) || itemProp.typeLine.ToLower().Contains(p.name.ToLower())).Count() > 0)
                                        {
                                            NinjaItem localitem = allItems.Where(p => itemProp.
                                            name.ToLower().Contains(p.name.ToLower()) || itemProp.typeLine.ToLower().Contains(p.name.ToLower())).OrderByDescending(p => p.name.Length).FirstOrDefault();
                                            if (localitem.chaos_value * config.profit_percent > itemValue && localitem.chaos_value - config.min_profit_range > itemValue)
                                            {
                                                Slot s = new Slot();

                                                s.BaseItem = localitem;
                                                s.SellItem = itemProp;
                                                if (Slots.Count == 3)
                                                    Slots.RemoveAt(2);
                                                Slots.Insert(0, s);
                                                SetSlots(Slots);
                                            }
                                        }
                                    if (config.do_breachstones)
                                    {

                                        //old
                                        if (item.Where(p => p.Path.EndsWith(".properties")).Count() > 0 && (itemProp.typeLine.Contains("Breach Leaguestone") || itemProp.typeLine.Contains("Talisman Leaguestone")))
                                        {
                                            string what = item.First(p => p.Path.EndsWith(".properties")).First.First.Children().ToList()[1].First.Children().ToList()[0].First.ToString();
                                            if (what == "5")
                                            {
                                                if (itemProp.typeLine.Contains("Talisman Leaguestone of Terror") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                                {

                                                    if (itemProp.note != null && itemProp.note.Contains("chaos") && itemValue < 20)
                                                    {
                                                        SetLeaguestoneSlots(LeaguestoneSlots, itemProp, name, " worth 60c");
                                                        SoundPlayer player = new SoundPlayer();
                                                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                        player.Play();

                                                    }
                                                }
                                                else if (itemProp.typeLine.Contains("Talisman Leaguestone of Fear") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                                {

                                                    if (itemProp.note != null && itemProp.note.Contains("chaos") && itemValue < 7)
                                                    {
                                                        SetLeaguestoneSlots(LeaguestoneSlots, itemProp, name, " worth 7c");
                                                        SoundPlayer player = new SoundPlayer();
                                                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                        player.Play();
                                                    }
                                                }
                                                //2.1 per breach * 5 * 3 = 31c
                                                if (itemProp.typeLine.Contains("Plentiful Breach Leaguestone of Splinters") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                                {

                                                    if (itemProp.note != null && itemProp.note.Contains("chaos") && itemValue < 20)
                                                    {
                                                        SetLeaguestoneSlots(LeaguestoneSlots, itemProp, name, " worth 31c");
                                                        SoundPlayer player = new SoundPlayer();
                                                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                        player.Play();
                                                    }
                                                }
                                                //2.1 per breach * 5 * 2 = 21c
                                                else if (itemProp.typeLine.Contains("Ample Breach Leaguestone of Splinters") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                                {
                                                    if (itemProp.note != null && itemProp.note.Contains("chaos") && itemValue < 12)
                                                    {
                                                        SetLeaguestoneSlots(LeaguestoneSlots, itemProp, name, " worth 21c");
                                                        SoundPlayer player = new SoundPlayer();
                                                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                        player.Play();
                                                    }
                                                }
                                                //1.26 per breach * 5 * 3 = 19c
                                                else if (itemProp.typeLine.Contains("Plentiful Breach Leaguestone") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                                {
                                                    if (itemProp.note != null && itemProp.note.Contains("chaos") && itemValue < 12)
                                                    {
                                                        SetLeaguestoneSlots(LeaguestoneSlots, itemProp, name, " worth 19c");
                                                        SoundPlayer player = new SoundPlayer();
                                                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                        player.Play();
                                                    }
                                                }
                                                //2.1 per breach * 5 = 10c
                                                else if (itemProp.typeLine.Contains("Breach Leaguestone of Splinters") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                                {
                                                    if (itemProp.note != null && itemProp.note.Contains("chaos") && itemValue < 5)
                                                    {
                                                        SetLeaguestoneSlots(LeaguestoneSlots, itemProp, name, " worth 10c");
                                                        SoundPlayer player = new SoundPlayer();
                                                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                        player.Play();
                                                    }
                                                }
                                                //1.26 per breach * 5 * 2 = 12c
                                                else if (itemProp.typeLine.Contains("Ample Breach") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                                {
                                                    if (itemProp.note != null && itemProp.note.Contains("chaos") && itemValue < 6)
                                                    {
                                                        SetLeaguestoneSlots(LeaguestoneSlots, itemProp, name, " worth 12c");
                                                        SoundPlayer player = new SoundPlayer();
                                                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                        player.Play();
                                                    }
                                                }
                                                //1c per splinter * 10 * 5 = 50c
                                                if (itemProp.typeLine.Contains("Dreaming Breach Leaguestone of Splinters") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                                {
                                                    if (itemProp.note != null && itemProp.note.Contains("chaos") && itemValue < 35)
                                                    {
                                                        SetLeaguestoneSlots(LeaguestoneSlots, itemProp, name, " worth 50c");
                                                        SoundPlayer player = new SoundPlayer();
                                                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                                                        player.Play();
                                                    }
                                                }
                                                //1c per splinter * 6 * 5 = 30c
                                                else if (itemProp.typeLine.Contains("Dreaming Breach") && itemProp.league == "Legacy" && Convert.ToInt32(itemProp.ilvl) > 65)
                                                {
                                                    if (itemProp.note != null && itemProp.note.Contains("chaos") && itemValue < 20)
                                                    {
                                                        SetLeaguestoneSlots(LeaguestoneSlots, itemProp, name, " worth 30c");
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
                        if (DateTime.Now.Subtract(lastTimeAPIcalled).TotalSeconds < 1)
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception eee)
                {
                }
            }
        }

        public void SetLeaguestoneSlots(List<Slot> LeaguestoneSlots, Item itemProp, string name, string value)
        {
            Slot s = new Slot();
            s.SellItem = itemProp;
            s.name = name;
            s.worth = value;

            s.Message = "@" + name + " Hi, I would like to buy your " + s.SellItem.typeLine + " listed for " + GetOriginalPrice(s.SellItem.note) + " in Legacy (stash tab \"" + itemProp.inventoryId + "\"; position: left " + itemProp.x + ", top " + itemProp.y + ")";
            if (LeaguestoneSlots.Count == 3)
                LeaguestoneSlots.RemoveAt(2);
            LeaguestoneSlots.Insert(0, s);
            if (LeaguestoneSlots[0].SellItem != null)
            {

                richTxtBox8Rep.Invoke((MethodInvoker)delegate
                {
                    richTxtBox8Rep.Text = LeaguestoneSlots[0].SellItem.typeLine + " for " + GetPriceInChaos(LeaguestoneSlots[0].SellItem.note) + " chaos:" + LeaguestoneSlots[0].worth + " : " + DateTime.Now.ToShortTimeString() + " " + LeaguestoneSlots[0].name;
                    richTxtBox8Rep.ForeColor = Color.DarkGreen;
                });
            }
            richtxtBox2Rep.Invoke((MethodInvoker)delegate
            {
                richtxtBox2Rep.ForeColor = Color.Black;
            });
            if (LeaguestoneSlots[1].SellItem != null)
            {
                textBox9.Invoke((MethodInvoker)delegate
                {
                    textBox9.Text = LeaguestoneSlots[1].SellItem.typeLine + " for " + GetPriceInChaos(LeaguestoneSlots[1].SellItem.note) + " chaos:" + LeaguestoneSlots[1].worth + " : " + DateTime.Now.ToShortTimeString() + " " + LeaguestoneSlots[1].name;
                });
            }
            if (LeaguestoneSlots[2].SellItem != null)
            {
                textBox10.Invoke((MethodInvoker)delegate
                {
                    textBox10.Text = LeaguestoneSlots[2].SellItem.typeLine + " for " + GetPriceInChaos(LeaguestoneSlots[2].SellItem.note) + " chaos:" + LeaguestoneSlots[2].worth + " : " + DateTime.Now.ToShortTimeString() + " " + LeaguestoneSlots[2].name;
                });
            }
        }
        public void SetTimeseconds(List<Slot> slots)
        {
            DateTime now = DateTime.Now;
            lblSeconds1.Invoke((MethodInvoker)delegate
            {
                int seconds = (int)now.Subtract(Slots[0].timeset).TotalSeconds;
                lblSeconds1.Text = seconds.ToString() + " s";
                lblSeconds1.ForeColor = GetColor(seconds);
            });
            lblSeconds2.Invoke((MethodInvoker)delegate
            {
                int seconds = (int)now.Subtract(Slots[1].timeset).TotalSeconds;
                lblSeconds2.Text = seconds.ToString() + " s";
                lblSeconds2.ForeColor = GetColor(seconds);
            });
            lblSeconds3.Invoke((MethodInvoker)delegate
            {
                double seconds = (int)now.Subtract(Slots[2].timeset).TotalSeconds;
                lblSeconds3.Text = seconds.ToString() + " s";
                lblSeconds3.ForeColor = GetColor(seconds);
            });
        }

        public Color GetColor(double seconds)
        {
            if (seconds < 10)
            {
                return Color.Green;
            }
            else if (seconds < 30)
            {
                return Color.Orange;
            }
            else return Color.Red;
        }
        public void SetSlots(List<Slot> Slots)
        {

            if (Slots[0].BaseItem != null)
            {
                Slot localslot = Slots[0];
                richtxtBox2Rep.Invoke((MethodInvoker)delegate
                {
                    richtxtBox2Rep.Text = localslot.BaseItem.name + " " + localslot.SellItem.typeLine;
                    richtxtBox2Rep.ForeColor = Color.DarkGreen;
                });
                richTxtBox8Rep.Invoke((MethodInvoker)delegate
                {
                    richTxtBox8Rep.ForeColor = Color.Black;
                });

                lblSeller1.Invoke((MethodInvoker)delegate
                {
                    lblSeller1.Text = localslot.name;
                });
                lblTime1.Invoke((MethodInvoker)delegate
                {
                    lblTime1.Text = DateTime.Now.ToShortTimeString();
                });

                textBox3.Invoke((MethodInvoker)delegate
                {
                    textBox3.Text = GetPriceInChaos(localslot.SellItem.note) + " : " + localslot.BaseItem.chaos_value + " : " + ((Decimal)localslot.BaseItem.chaos_value - GetPriceInChaos(localslot.SellItem.note)) / ((Decimal)GetPriceInChaos(localslot.SellItem.note)) * 100 + "%";
                });
                richTextBox1.Invoke((MethodInvoker)delegate
                {
                    richTextBox1.Lines = localslot.SellItem.implicitMods.Concat(new string[] { "__________________" }).ToArray().Concat(localslot.SellItem.explicitMods).ToArray();
                });
                richTextBox2.Invoke((MethodInvoker)delegate
                {
                    richTextBox2.Lines = localslot.BaseItem.Implicits.Concat(new string[] { "__________________" }).ToArray().Concat(localslot.BaseItem.Explicits).ToArray();
                });
                checkBox1.Invoke((MethodInvoker)delegate
                {
                    checkBox1.Checked = localslot.BaseItem.type == "9";
                    if (checkBox1.Checked)
                        checkBox1.ForeColor = Color.DarkGoldenrod;
                    else
                        checkBox1.ForeColor = Color.Black;
                });
                checkBox4.Invoke((MethodInvoker)delegate
                {
                    checkBox4.Checked = localslot.SellItem.corrupted.ToLower().Contains("true");
                    if (checkBox4.Checked)
                        checkBox4.ForeColor = Color.Red;
                    else
                        checkBox4.ForeColor = Color.Black;
                });
                button7.Invoke((MethodInvoker)delegate
                {

                    if (localslot.is_mine)
                    {
                        button7.Text = "Message";
                        button7.ForeColor = Color.Green;
                    }
                    else
                    {
                        button7.Text = "Not Mine Msg";
                        button7.ForeColor = Color.Red;
                    }
                    //this.button13.Font = new Font("Arial", 12, FontStyle.Bold);
                });
                slot0minandavrg.Invoke((MethodInvoker)delegate
                               {
                                   slot0minandavrg.Lines = Slots[0].BaseItem.Top5Sells?.ToArray();
                               });
            }
            if (Slots[1].BaseItem != null)
            {
                Slot localslot = Slots[1];
                textBox4.Invoke((MethodInvoker)delegate
                {
                    textBox4.Text = localslot.BaseItem.name + " " + localslot.SellItem.typeLine;
                });
                lblSeller2.Invoke((MethodInvoker)delegate
                {
                    lblSeller2.Text = localslot.name;
                });
                lblTime2.Invoke((MethodInvoker)delegate
                {
                    lblTime2.Text = DateTime.Now.ToShortTimeString();
                });
                textBox5.Invoke((MethodInvoker)delegate
                {
                    textBox5.Text = GetPriceInChaos(localslot.SellItem.note) + " : " + localslot.BaseItem.chaos_value + " : " + ((Decimal)localslot.BaseItem.chaos_value - GetPriceInChaos(localslot.SellItem.note)) / ((Decimal)GetPriceInChaos(localslot.SellItem.note)) * 100 + "%";
                });
                richTextBox3.Invoke((MethodInvoker)delegate
                {
                    richTextBox3.Lines = localslot.SellItem.implicitMods.Concat(new string[] { "__________________" }).ToArray().Concat(localslot.SellItem.explicitMods).ToArray();
                });
                richTextBox4.Invoke((MethodInvoker)delegate
                {
                    richTextBox4.Lines = localslot.BaseItem.Implicits.Concat(new string[] { "__________________" }).ToArray().Concat(localslot.BaseItem.Explicits).ToArray();
                });
                checkBox2.Invoke((MethodInvoker)delegate
                {
                    checkBox2.Checked = localslot.BaseItem.type == "9";
                    if (checkBox2.Checked)
                        checkBox2.ForeColor = Color.DarkGoldenrod;
                    else
                        checkBox2.ForeColor = Color.Black;
                });
                checkBox5.Invoke((MethodInvoker)delegate
                {
                    checkBox5.Checked = localslot.SellItem.corrupted.ToLower().Contains("true");
                    if (checkBox5.Checked)
                        checkBox5.ForeColor = Color.Red;
                    else
                        checkBox5.ForeColor = Color.Black;
                });
                button8.Invoke((MethodInvoker)delegate
                {

                    if (localslot.is_mine)
                    {
                        button8.Text = "Message";
                        button8.ForeColor = Color.Green;
                    }
                    else
                    {
                        button8.Text = "Not Mine Msg";
                        button8.ForeColor = Color.Red;
                    }

                });
                slot1minandavrg.Invoke((MethodInvoker)delegate
                              {
                                  slot1minandavrg.Lines = Slots[1].BaseItem.Top5Sells?.ToArray();
                              });
            }
            if (Slots[2].BaseItem != null)
            {
                Slot localslot = Slots[2];
                textBox6.Invoke((MethodInvoker)delegate
                {
                    textBox6.Text = localslot.BaseItem.name + " " + localslot.SellItem.typeLine;
                });
                lblSeller3.Invoke((MethodInvoker)delegate
                {
                    lblSeller3.Text = localslot.name;
                });
                lblTime3.Invoke((MethodInvoker)delegate
                {
                    lblTime3.Text = DateTime.Now.ToShortTimeString();
                });
                textBox7.Invoke((MethodInvoker)delegate
                {
                    textBox7.Text = GetPriceInChaos(localslot.SellItem.note) + " : " + localslot.BaseItem.chaos_value + " : " + ((Decimal)localslot.BaseItem.chaos_value - GetPriceInChaos(localslot.SellItem.note)) / ((Decimal)GetPriceInChaos(localslot.SellItem.note)) * 100 + "%";
                });
                richTextBox5.Invoke((MethodInvoker)delegate
                {
                    richTextBox5.Lines = localslot.SellItem.implicitMods.Concat(new string[] { "__________________" }).ToArray().Concat(localslot.SellItem.explicitMods).ToArray();
                });
                richTextBox6.Invoke((MethodInvoker)delegate
                {
                    richTextBox6.Lines = localslot.BaseItem.Implicits.Concat(new string[] { "__________________" }).ToArray().Concat(localslot.BaseItem.Explicits).ToArray();
                });
                checkBox3.Invoke((MethodInvoker)delegate
                {
                    checkBox3.Checked = localslot.BaseItem.type == "9";
                    if (checkBox3.Checked)
                        checkBox3.ForeColor = Color.DarkGoldenrod;
                    else
                        checkBox3.ForeColor = Color.Black;
                });
                checkBox6.Invoke((MethodInvoker)delegate
                {
                    checkBox6.Checked = localslot.SellItem.corrupted.ToLower().Contains("true");
                    if (checkBox6.Checked)
                        checkBox6.ForeColor = Color.Red;
                    else
                        checkBox6.ForeColor = Color.Black;
                });
                button13.Invoke((MethodInvoker)delegate
                {
                    if (localslot.is_mine)
                    {
                        button13.Text = "Message";
                        button13.ForeColor = Color.Green;
                    }
                    else
                    {
                        button13.Text = "Not Mine Msg";
                        button13.ForeColor = Color.Red;
                    }
                });
                slot2minandavrg.Invoke((MethodInvoker)delegate
                             {
                                 slot2minandavrg.Lines = Slots[2].BaseItem.Top5Sells?.ToArray();
                             });
            }
        }
        public static string GetOriginalPrice(string input)
        {
            try
            {
                char[] x = input.Where(c => char.IsDigit(c)).ToArray();
                string y = new string(input.Where(c => char.IsDigit(c)).ToArray());
                decimal value = (Convert.ToDecimal(y));

                if (input.Contains("exa"))
                {
                    return value + " exalted";
                }
                else if (input.Contains("chaos"))
                {
                    return value + " chaos";
                }
                else if (input.Contains("fuse"))
                {
                    return value + " fusing";
                }
                else if (input.Contains("alch"))
                {
                    return value + " alchemy";
                }
                return "";
            }
            catch (Exception eeee)
            {
                return "";
            }
        }
        public int findWhoGets(string input, int number_of_people)
        {
            string final = string.Join("", input.Where(p => Char.IsDigit(p)));
            double realnumber = Convert.ToDouble(final);
            return (int)((realnumber % number_of_people) + 1);

        }
        public static decimal GetPriceInChaos(string input)
        {
            try
            {
                decimal multiplier = 0;
                if (input.Contains("exa"))
                {
                    multiplier = config.exalt_ratio;
                }
                else if (input.Contains("chaos"))
                {
                    multiplier = 1;
                }
                else if (input.Contains("fuse"))
                {
                    multiplier = config.fusing_ratio;
                }
                else if (input.Contains("alch"))
                {
                    multiplier = config.alch_ratio;
                }
                else
                    return 1000000000;


                char[] x = input.Where(c => char.IsDigit(c)).ToArray();
                string y = new string(input.Where(c => char.IsDigit(c)).ToArray());
                decimal value = Convert.ToInt32(y) * multiplier;
                if (value == 0)
                    return 1000000;
                return value;
            }
            catch (Exception eeee)
            {
                return 999999;
            }
        }
        public static decimal GetMultipleNumbers(string input)
        {
            if (input == "")
                return -1;
            char[] x = input.ToCharArray().Where(c => Char.IsDigit(c) || c == '.').ToArray();
            string y = new string(input.Where(c => char.IsDigit(c) || c == '.').ToArray());
            if (!y.Contains("."))
                y += ".0";
            double dub = Convert.ToDouble(y);
            return (decimal)(dub);
        }
        public static List<NinjaItem> SetNinjaValues(List<NinjaItem> NinjaItems)
        {
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
                        if (implicits.Count == 0)
                            implicits.Add("");
                        List<string> explicits = new List<string>();
                        foreach (JArray j in jo2.Children().ToList().First(p => p.Path.EndsWith(".explicitModifiers")))
                        {
                            foreach (JObject explicitRoll in j)
                            {
                                explicits.Add(explicitRoll.First.First.ToString());
                            }
                        }
                        if (explicits.Count == 0)
                            explicits.Add("");
                        newNinjaItem.Explicits = explicits;
                        newNinjaItem.Implicits = implicits;
                        newNinjaItem.chaos_value = Convert.ToDecimal(jo2.Children().ToList().First(p => p.Path.EndsWith(".chaosValue")).First.ToString());
                        if (/*newNinjaItem.chaos_value > 20 &&*/ !newNinjaItem.name.Contains("Atziri's Splendour") && !newNinjaItem.name.Contains("Doryani's Invitation") && !newNinjaItem.name.Contains("Vessel of Vinktar") && NinjaItems.Where(p => p.name == newNinjaItem.name && p.base_type == newNinjaItem.base_type && p.type == newNinjaItem.type).Count() == 0)
                            NinjaItems.Add(newNinjaItem);
                    }
                }
                if (config.do_all_uniques_with_ranges)
                    foreach (NinjaItem nj in NinjaItems)
                    {
                        //lets look for rolls
                        if (nj.Explicits.Count > 0 && !nj.base_type.Contains("Map") && nj.chaos_value > 15 && nj.base_type != "")
                        {
                            List<ExplicitField> explicitsToCheck = new List<ExplicitField>();
                            foreach (string explicitRoll in nj.Explicits)
                            {
                                if (explicitRoll.Contains("(") && explicitRoll.Contains("-") && explicitRoll.Contains(")"))
                                {
                                    string s = SearchString(explicitRoll);
                                    // lets see if there are multi rolls in a explicit
                                    int countasdf = explicitRoll.Count(c => c == '(');
                                    if (explicitRoll.Count(c => c == '(') > 1 && explicitRoll.Count(c => c == ')') > 1)
                                    {
                                        if (explicitRoll.Contains(" to "))
                                        {
                                            List<string> Rolls = explicitRoll.Split(new string[] { " to " }, StringSplitOptions.None).ToList();
                                            List<decimal> MinRolls = new List<decimal>();
                                            List<decimal> MaxRolls = new List<decimal>();
                                            foreach (string roll in Rolls)
                                            {
                                                decimal MinRollsTemp = GetMultipleNumbers(roll.Substring(roll.IndexOf("(") + 1, roll.IndexOf("-") - roll.IndexOf("(")));
                                                decimal MaxRollsTemp = (decimal)(GetMultipleNumbers(roll.Substring(roll.IndexOf("-") + 1, roll.IndexOf(")") - roll.IndexOf("-"))) * 0.9M);
                                                if (MaxRollsTemp < MinRollsTemp)
                                                    MaxRolls = MinRolls;
                                                MinRolls.Add(MinRollsTemp);
                                                MaxRolls.Add(MaxRollsTemp);
                                            }
                                            explicitsToCheck.Add(new ExplicitField() { SearchField = s, MinRoll = (MinRolls[0] + MinRolls[1]) / 2, MaxRoll = (MaxRolls[0] + MaxRolls[1]) / 2 });
                                        }
                                    }
                                    else
                                    {
                                        if (explicitRoll.Contains(" to ("))
                                        {
                                            List<string> Rolls = explicitRoll.Split(new string[] { " to " }, StringSplitOptions.None).ToList();
                                            decimal MinRolls = 0;
                                            decimal MaxRolls = 0;
                                            decimal SingularRoll = 0;
                                            foreach (string roll in Rolls)
                                            {
                                                if (roll.Contains("(") && roll.Contains(")"))
                                                {
                                                    MinRolls = GetMultipleNumbers(roll.Substring(roll.IndexOf("(") + 1, roll.IndexOf("-") - roll.IndexOf("(")));
                                                    MaxRolls = (decimal)(GetMultipleNumbers(roll.Substring(roll.IndexOf("-") + 1, roll.IndexOf(")") - roll.IndexOf("-"))) * 0.9M);
                                                    if (MaxRolls < MinRolls)
                                                        MaxRolls = MinRolls;
                                                }
                                                else
                                                {
                                                    SingularRoll = GetMultipleNumbers(roll);
                                                }
                                            }
                                            explicitsToCheck.Add(new ExplicitField() { SearchField = s, MinRoll = (MinRolls + SingularRoll) / 2, MaxRoll = (MaxRolls + SingularRoll) / 2 });
                                        }
                                        else
                                        {
                                            decimal MinRoll = GetMultipleNumbers(explicitRoll.Substring(explicitRoll.IndexOf("(") + 1, explicitRoll.IndexOf("-") - explicitRoll.IndexOf("(")));
                                            decimal MaxRoll = (int)(GetMultipleNumbers(explicitRoll.Substring(explicitRoll.IndexOf("-") + 1, explicitRoll.IndexOf(")") - explicitRoll.IndexOf("-"))) * 0.9M);
                                            if (MaxRoll < MinRoll)
                                                MaxRoll = MinRoll;
                                            explicitsToCheck.Add(new ExplicitField() { SearchField = s, MinRoll = MinRoll, MaxRoll = MaxRoll });
                                        }
                                    }
                                }

                            }

                            string modsMinSearch = "mod_name=&mod_min=&mod_max=&";
                            string modsMaxSearch = "mod_name=&mod_min=&mod_max=&";
                            foreach (ExplicitField ef in explicitsToCheck)
                            {
                                modsMinSearch += "mod_name=" + WebUtility.UrlEncode(ef.SearchField) + "&mod_min=" + WebUtility.UrlEncode(ef.MinRoll.ToString()) + "&mod_max=&";
                                modsMaxSearch += "mod_name=" + WebUtility.UrlEncode(ef.SearchField) + "&mod_min=" + WebUtility.UrlEncode(ef.MaxRoll.ToString()) + "&mod_max=&";
                            }
                            if (explicitsToCheck.Count == 0)
                            {
                                MinSearch(nj, modsMinSearch, explicitsToCheck);
                                nj.HasRolls = false;
                            }
                            else
                            {
                                MinSearch(nj, modsMinSearch, explicitsToCheck);
                                MaxSearch(nj, modsMaxSearch, explicitsToCheck);
                                nj.HasRolls = true;
                            }

                        }
                    }
                config.SavedItems = NinjaItems;
                config.LastSaved = DateTime.Now;
                string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(config);
                System.IO.File.Delete(configfile);
                System.IO.File.WriteAllText(configfile, serialized);
            }
            return NinjaItems;
        }
        public static void MinSearch(NinjaItem nj, string modsMinSearch, List<ExplicitField> explicitsToCheck)
        {
            string rarity = "unique";
            if (nj.type == "9")
                rarity = "relic";
            //min search
            decimal MinSell = 0;
            decimal AvrgSellTop5 = 0;
            List<string> Top5Prices = new List<string>();
            int count = 0;
            bool first = true;
            string redirectUrl = "";
            HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/search");
            request23.Method = "POST";
            request23.KeepAlive = true;
            request23.ContentType = "application/x-www-form-urlencoded";
            StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());
            postwriter.Write("league=Legacy&type=&base=&name=" + WebUtility.UrlEncode(nj.name + " " + nj.base_type) + "&dmg_min=&dmg_max=&aps_min=&aps_max=&crit_min=&crit_max=&dps_min=&dps_max=&edps_min=&edps_max=&pdps_min=&pdps_max=&armour_min=&armour_max=&evasion_min=&evasion_max=&shield_min=&shield_max=&block_min=&block_max=&sockets_min=&sockets_max=&link_min=&link_max=&sockets_r=&sockets_g=&sockets_b=&sockets_w=&linked_r=&linked_g=&linked_b=&linked_w=&rlevel_min=&rlevel_max=&rstr_min=&rstr_max=&rdex_min=&rdex_max=&rint_min=&rint_max=&" + modsMinSearch + "group_type=And&group_min=&group_max=&group_count=" + explicitsToCheck.Count().ToString() + "&q_min=&q_max=&level_min=&level_max=&ilvl_min=&ilvl_max=&rarity=" + rarity + "&seller=&thread=&identified=&corrupted=&online=x&has_buyout=&altart=&capquality=x&buyout_min=&buyout_max=&buyout_currency=&crafted=&enchanted=");
            postwriter.Close();
            using (HttpWebResponse response2 = request23.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                using (StreamReader reader = new StreamReader(response2.GetResponseStream()))
                {
                    string s = reader.ReadToEnd();
                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(s);
                    var inputs = htmlDoc.DocumentNode.Descendants("tbody");

                    foreach (var input in inputs)
                    {
                        if (input.Attributes.Contains("id") && input.Attributes["id"].Value.Contains("item-container-") && count < 5)
                        {
                            if (input.Attributes["data-buyout"].Value != "")
                            {
                                if (first)
                                {
                                    MinSell = input.Attributes["data-buyout"].Value.Contains("exalted") ? GetMultipleNumbers(input.Attributes["data-buyout"].Value) * config.exalt_ratio : GetMultipleNumbers(input.Attributes["data-buyout"].Value);
                                    AvrgSellTop5 += MinSell;
                                    Top5Prices.Add(MinSell.ToString());
                                    first = false;
                                }
                                else
                                {
                                    Top5Prices.Add(input.Attributes["data-buyout"].Value.Contains("exalted") ? (GetMultipleNumbers(input.Attributes["data-buyout"].Value) * config.exalt_ratio).ToString() : GetMultipleNumbers(input.Attributes["data-buyout"].Value).ToString());
                                    AvrgSellTop5 += input.Attributes["data-buyout"].Value.Contains("exalted") ? GetMultipleNumbers(input.Attributes["data-buyout"].Value) * config.exalt_ratio : GetMultipleNumbers(input.Attributes["data-buyout"].Value);
                                }
                                count++;
                            }
                        }

                    }
                }
                if (count > 0)
                    AvrgSellTop5 = AvrgSellTop5 / count;
            }
            nj.MinSell = MinSell;
            nj.MinAverage = AvrgSellTop5;
            nj.Top5Sells = Top5Prices;
        }



        public static void MaxSearch(NinjaItem nj, string modsMaxSearch, List<ExplicitField> explicitsToCheck)
        {
            string rarity = "unique";
            if (nj.type == "9")
                rarity = "relic";
            //max search
            decimal HighRollMinSell = 0;
            decimal HighRollAvrgSell = 0;
            int count = 0;
            bool first = true;
            string redirectUrl = "";
            HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/search");
            request23.Method = "POST";
            request23.KeepAlive = true;
            request23.ContentType = "application/x-www-form-urlencoded";
            StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());
            postwriter.Write("league=Legacy&type=&base=&name=" + WebUtility.UrlEncode(nj.name + nj.base_type) + "&dmg_min=&dmg_max=&aps_min=&aps_max=&crit_min=&crit_max=&dps_min=&dps_max=&edps_min=&edps_max=&pdps_min=&pdps_max=&armour_min=&armour_max=&evasion_min=&evasion_max=&shield_min=&shield_max=&block_min=&block_max=&sockets_min=&sockets_max=&link_min=&link_max=&sockets_r=&sockets_g=&sockets_b=&sockets_w=&linked_r=&linked_g=&linked_b=&linked_w=&rlevel_min=&rlevel_max=&rstr_min=&rstr_max=&rdex_min=&rdex_max=&rint_min=&rint_max=&" + modsMaxSearch + "group_type=And&group_min=&group_max=&group_count=" + explicitsToCheck.Count().ToString() + "&q_min=&q_max=&level_min=&level_max=&ilvl_min=&ilvl_max=&rarity=" + rarity + "&seller=&thread=&identified=&corrupted=&online=x&has_buyout=&altart=&capquality=x&buyout_min=&buyout_max=&buyout_currency=&crafted=&enchanted=");
            postwriter.Close();
            using (HttpWebResponse response2 = request23.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                using (StreamReader reader = new StreamReader(response2.GetResponseStream()))
                {
                    string s = reader.ReadToEnd();
                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(s);
                    var inputs = htmlDoc.DocumentNode.Descendants("tbody");

                    foreach (var input in inputs)
                    {
                        if (input.Attributes.Contains("id") && input.Attributes["id"].Value.Contains("item-container-") && count < 5)
                        {
                            if (first)
                            {
                                HighRollMinSell = input.Attributes["data-buyout"].Value.Contains("exalted") ? GetMultipleNumbers(input.Attributes["data-buyout"].Value) * config.exalt_ratio : GetMultipleNumbers(input.Attributes["data-buyout"].Value);
                                HighRollMinSell += HighRollMinSell;
                                first = false;
                            }
                        }
                        else
                        {
                            HighRollAvrgSell += input.Attributes["data-buyout"].Value.Contains("exalted") ? GetMultipleNumbers(input.Attributes["data-buyout"].Value) * config.exalt_ratio : GetMultipleNumbers(input.Attributes["data-buyout"].Value);
                        }
                        count++;
                    }
                }
                if (count > 0)
                    HighRollMinSell = HighRollMinSell / count;
            }
            nj.HighRollMinSell = HighRollMinSell;
            nj.HighRollAvrgSell = HighRollMinSell;
        }
        public static string SearchString(string explicitString)
        {
            string s = Regex.Replace(explicitString, @"\d", "#").Replace("(", "").Replace(")", "").Replace("-", "");
            return s.Replace("####", "#").Replace("###", "#").Replace("##", "#").Replace("#.#", "#");
        }
        public class ExplicitField
        {
            public string SearchField { get; set; }
            public decimal MinRoll { get; set; }
            public decimal MaxRoll { get; set; }
        }
        [Serializable]
        public class NinjaItem
        {
            public NinjaItem()
            {
                Implicits = new List<string>();
                Explicits = new List<string>();
            }

            public string name { get; set; }
            public decimal chaos_value { get; set; }
            public string type { get; set; }
            public string base_type { get; set; }
            public List<string> Implicits { get; set; }
            public List<string> Explicits { get; set; }
            public decimal MinSell { get; set; }
            public decimal MinAverage { get; set; }
            public decimal HighRollMinSell { get; set; }
            public decimal HighRollAvrgSell { get; set; }
            public List<string> Top5Sells { get; set; }
            public bool HasRolls { get; set; }
            public override string ToString()
            {
                return name + " : " + chaos_value;
            }
        }
        public static void AddNewName(string name, string value)
        {
            allItems.Add(new NinjaItem()
            {
                name = name,
                chaos_value = Convert.ToDecimal(value),
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
                allItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NinjaItem>>(System.IO.File.ReadAllText(itemfilename));
            }
            catch (Exception e)
            {
                allItems = new List<NinjaItem>();
            }
            try
            {
                config = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemWatchConfig>(System.IO.File.ReadAllText(configfile));
            }
            catch (Exception e)
            {
                config = new ItemWatchConfig();
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
            public Slot()
            {
                timeset = DateTime.Now;
            }
            public DateTime timeset;
            public Item SellItem;
            public NinjaItem BaseItem;
            public string Message;
            public string name;
            public string worth;
            public bool is_mine { get; set; }
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
            public string inventoryId { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            //public string requirements { get; set; }
            public string[] implicitMods { get; set; }
            public string[] explicitMods { get; set; }


            public override bool Equals(object obj)
            {
                Item input = (Item)obj;
                return input.note == this.note && input.w == this.w && input.h == this.h && input.id == this.id && input.name == this.name && input.typeLine == this.typeLine && input.ilvl == this.ilvl;
            }
        }
        [Serializable]
        public class WatchedItem
        {
            public string name { get; set; }
            public double value { get; set; }
        }
        [Serializable]
        public class ItemWatchConfig
        {
            public decimal esh_value { get; set; }
            public decimal xoph_value { get; set; }
            public decimal tul_value { get; set; }

            public bool do_breachstones { get; set; }
            public bool do_watch_list { get; set; }
            public bool do_all_uniques { get; set; }
            public bool do_all_uniques_with_ranges { get; set; }

            public decimal exalt_ratio { get; set; }
            public decimal fusing_ratio { get; set; }
            public decimal alch_ratio { get; set; }

            public int max_price { get; set; }
            public decimal profit_percent { get; set; }
            public int min_profit_range { get; set; }
            public int my_number { get; set; }
            public int number_of_people { get; set; }

            public List<NinjaItem> SavedItems { get; set; }
            public DateTime LastSaved { get; set; }
            public bool refresh_items { get; set; }
        }

        public class NotChaosCurrencyConversion
        {
            public string name { get; set; }
            public decimal valueInChaos { get; set; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {

        }
        [STAThread]
        private void button7_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Slots[0].Message))
            {
                string s = Slots[0].Message;
                Clipboard.SetText(s);
            }
        }
        [STAThread]
        private void button8_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Slots[1].Message))
            {
                string s = Slots[1].Message;
                Clipboard.SetText(s);
            }
        }
        [STAThread]
        private void button13_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Slots[2].Message))
            {
                string s = Slots[2].Message;
                Clipboard.SetText(s);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(LeaguestoneSlots[0].Message))
            {
                string s = LeaguestoneSlots[0].Message;
                Clipboard.SetText(s);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(LeaguestoneSlots[1].Message))
            {
                string s = LeaguestoneSlots[1].Message;
                Clipboard.SetText(s);
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(LeaguestoneSlots[2].Message))
            {
                string s = LeaguestoneSlots[2].Message;
                Clipboard.SetText(s);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (Slots[0].BaseItem != null)
            {
                string rarity = "unique";
                if (Slots[0].BaseItem.type == "9")
                    rarity = "relic";
                string redirectUrl = "";
                HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/search");
                request23.Method = "POST";
                request23.KeepAlive = true;
                request23.ContentType = "application/x-www-form-urlencoded";
                StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());
                postwriter.Write("league=Legacy&type=&base=&name=" + WebUtility.UrlEncode(Slots[0].BaseItem.name) + "&dmg_min=&dmg_max=&aps_min=&aps_max=&crit_min=&crit_max=&dps_min=&dps_max=&edps_min=&edps_max=&pdps_min=&pdps_max=&armour_min=&armour_max=&evasion_min=&evasion_max=&shield_min=&shield_max=&block_min=&block_max=&sockets_min=&sockets_max=&link_min=&link_max=&sockets_r=&sockets_g=&sockets_b=&sockets_w=&linked_r=&linked_g=&linked_b=&linked_w=&rlevel_min=&rlevel_max=&rstr_min=&rstr_max=&rdex_min=&rdex_max=&rint_min=&rint_max=&mod_name=&mod_min=&mod_max=&group_type=And&group_min=&group_max=&group_count=1&q_min=&q_max=&level_min=&level_max=&ilvl_min=&ilvl_max=&rarity=" + rarity + "&seller=&thread=&identified=&corrupted=&online=x&has_buyout=&altart=&capquality=x&buyout_min=&buyout_max=&buyout_currency=&crafted=&enchanted=");
                postwriter.Close();
                using (HttpWebResponse response2 = request23.GetResponse() as HttpWebResponse)
                {
                    System.Diagnostics.Process.Start(response2.ResponseUri.OriginalString);
                }
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (Slots[1].BaseItem != null)
            {
                string rarity = "unique";
                if (Slots[1].BaseItem.type == "9")
                    rarity = "relic";
                string redirectUrl = "";
                HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/search");
                request23.Method = "POST";
                request23.KeepAlive = true;
                request23.ContentType = "application/x-www-form-urlencoded";
                StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());
                postwriter.Write("league=Legacy&type=&base=&name=" + WebUtility.UrlEncode(Slots[1].BaseItem.name) + "&dmg_min=&dmg_max=&aps_min=&aps_max=&crit_min=&crit_max=&dps_min=&dps_max=&edps_min=&edps_max=&pdps_min=&pdps_max=&armour_min=&armour_max=&evasion_min=&evasion_max=&shield_min=&shield_max=&block_min=&block_max=&sockets_min=&sockets_max=&link_min=&link_max=&sockets_r=&sockets_g=&sockets_b=&sockets_w=&linked_r=&linked_g=&linked_b=&linked_w=&rlevel_min=&rlevel_max=&rstr_min=&rstr_max=&rdex_min=&rdex_max=&rint_min=&rint_max=&mod_name=&mod_min=&mod_max=&group_type=And&group_min=&group_max=&group_count=1&q_min=&q_max=&level_min=&level_max=&ilvl_min=&ilvl_max=&rarity=" + rarity + "&seller=&thread=&identified=&corrupted=&online=x&has_buyout=&altart=&capquality=x&buyout_min=&buyout_max=&buyout_currency=&crafted=&enchanted=");
                postwriter.Close();
                using (HttpWebResponse response2 = request23.GetResponse() as HttpWebResponse)
                {
                    System.Diagnostics.Process.Start(response2.ResponseUri.OriginalString);
                }
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (Slots[2].BaseItem != null)
            {
                string rarity = "unique";
                if (Slots[2].BaseItem.type == "9")
                    rarity = "relic";
                string redirectUrl = "";
                HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/search");
                request23.Method = "POST";
                request23.KeepAlive = true;
                request23.ContentType = "application/x-www-form-urlencoded";
                StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());
                postwriter.Write("league=Legacy&type=&base=&name=" + WebUtility.UrlEncode(Slots[2].BaseItem.name) + "&dmg_min=&dmg_max=&aps_min=&aps_max=&crit_min=&crit_max=&dps_min=&dps_max=&edps_min=&edps_max=&pdps_min=&pdps_max=&armour_min=&armour_max=&evasion_min=&evasion_max=&shield_min=&shield_max=&block_min=&block_max=&sockets_min=&sockets_max=&link_min=&link_max=&sockets_r=&sockets_g=&sockets_b=&sockets_w=&linked_r=&linked_g=&linked_b=&linked_w=&rlevel_min=&rlevel_max=&rstr_min=&rstr_max=&rdex_min=&rdex_max=&rint_min=&rint_max=&mod_name=&mod_min=&mod_max=&group_type=And&group_min=&group_max=&group_count=1&q_min=&q_max=&level_min=&level_max=&ilvl_min=&ilvl_max=&rarity=" + rarity + "&seller=&thread=&identified=&corrupted=&online=x&has_buyout=&altart=&capquality=x&buyout_min=&buyout_max=&buyout_currency=&crafted=&enchanted=");
                postwriter.Close();
                using (HttpWebResponse response2 = request23.GetResponse() as HttpWebResponse)
                {
                    System.Diagnostics.Process.Start(response2.ResponseUri.OriginalString);
                }
            }
        }
    }
}
