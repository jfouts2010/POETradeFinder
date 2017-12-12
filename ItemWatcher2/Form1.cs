using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
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
        public static List<NinjaItem> watched_items;
        public static List<WeaponBaseItem> allBaseTypes;
        public static List<POETradeConfig> watchedRares;
        public static Dictionary<string, string> all_base_types;
        public static List<POETradeCraftable> craftables;
        public static ItemWatchConfig config;
        public static ConcurrentQueue<Item> raresQueue = new ConcurrentQueue<Item>();
        public static ConcurrentQueue<Item> secondrareQueue = new ConcurrentQueue<Item>();

        public static List<NinjaItem> ninjaItems = new List<NinjaItem>();
        private static BackgroundWorker bgw;
        private static string RealChangeId = "";
        private static string UsedChangeId = "";
        public static string LeagueName = "Abyss";
        public static List<Slot> Slots = new List<Slot>();
        public static List<Slot> justTextSlots = new List<Slot>();
        public static List<NotChaosCurrencyConversion> othercurrencies;
        public static DateTime last_time_clicked { get; set; }
        public Form1()
        {
            LoadBasicInfo();

            //GenerateAllBaseWepsFromString();
            //NinjaPoETradeMethods.CalcDPSOfAllWeps();
            InitializeComponent();
            BackgroundWorker bgw = new BackgroundWorker();
            BackgroundWorker bgw2 = new BackgroundWorker();
            BackgroundWorker bgw3 = new BackgroundWorker();
            BackgroundWorker bgw4 = new BackgroundWorker();
            BackgroundWorker bgw5 = new BackgroundWorker();
            BackgroundWorker bgw6 = new BackgroundWorker();

            bgw.DoWork += DoBackgroundWork;
            bgw2.DoWork += SyncNinja;
            bgw3.DoWork += SyncHead;
            bgw4.DoWork += ProcessItems;
            bgw5.DoWork += ProcessItems;
            bgw6.DoWork += ProcessItems;

            bgw.RunWorkerAsync();
            bgw2.RunWorkerAsync();
            bgw3.RunWorkerAsync();
            bgw4.RunWorkerAsync();
            if (config.do_watch_rares)
            {
                BackgroundWorker bgw7 = new BackgroundWorker();
                bgw7.DoWork += SyncRares;
                bgw7.RunWorkerAsync();
            }
            if (config.do_craft_watch)
            {
                BackgroundWorker bgw8 = new BackgroundWorker();
                bgw8.DoWork += SyncCraftables;
                bgw8.RunWorkerAsync();
            }
        }

        #region Syncs
        private void SyncNinja(object sender, DoWorkEventArgs e)
        {

            LoadBasicInfo();
            while (true)
            {
                txtBoxUpdateThread.Invoke((MethodInvoker)delegate
                {
                    txtBoxUpdateThread.Text = "Doing Nothing";
                });

                if (config.LastSaved.AddMinutes(config.refresh_minutes) < DateTime.Now && textBox1.Text != "Converting Poe.Ninja Items")
                {
                    txtBoxUpdateThread.Invoke((MethodInvoker)delegate
                    {
                        txtBoxUpdateThread.Text = "Starting Ninja Update";
                    });
                    ninjaItems = NinjaPoETradeMethods.SetNinjaValues(new List<NinjaItem>(), txtBoxUpdateThread, all_base_types, true);

                }
                else
                    System.Threading.Thread.Sleep(1000);
                LoadBasicInfo();
            }
        }
        private void SyncRares(object sender, DoWorkEventArgs e)
        {

            LoadBasicInfo();
            while (true)
            {
                txtRareUpdateStatus.Invoke((MethodInvoker)delegate
                {
                    txtRareUpdateStatus.Text = "Doing Nothing";
                });

                if (config.lastRareSave.AddMinutes(config.refresh_minutes * 2) < DateTime.Now && textBox1.Text != "Converting Poe.Ninja Items")
                {
                    txtRareUpdateStatus.Invoke((MethodInvoker)delegate
                    {
                        txtRareUpdateStatus.Text = "Starting Rare Update";
                    });
                    LoadBasicInfo();
                    GetValuesOfWatchedRares();
                    config.lastRareSave = DateTime.Now;
                }
                else
                    System.Threading.Thread.Sleep(60000);

            }
        }
        private void SyncHead(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                System.Threading.Thread.Sleep(5000);
                string lower = textBox1.Text.ToLower();
                if (lower.Contains("running") || lower.Contains("failed") || lower.Contains("found head"))
                    break;
            }
            if (config.do_catchup_thread)
                while (true)
                {
                    for (int i = 0; i < 60; i++)
                    {
                        txtBoxFasterSearch.Invoke((MethodInvoker)delegate
                        {
                            txtBoxFasterSearch.Text = "Run in: " + (900 - i * 15) + "s";
                            txtBoxFasterSearch.ForeColor = Color.Black;
                        });
                        System.Threading.Thread.Sleep(5 * 3000);
                    }

                    txtBoxFasterSearch.Invoke((MethodInvoker)delegate
                    {
                        txtBoxFasterSearch.Text = "Running";
                        txtBoxFasterSearch.ForeColor = Color.Red;
                    });
                    string tempid = FindHead(UsedChangeId, true);
                    int newchange = int.Parse(tempid.Split('-').Last());
                    int oldchange = int.Parse(RealChangeId.Split('-').Last());
                    int difference = newchange - oldchange;
                    txtBoxFasterSearch.Invoke((MethodInvoker)delegate
                    {
                        txtBoxFasterSearch.Text = tempid.Split('-').Last() + " : " + difference;
                        txtBoxFasterSearch.ForeColor = Color.Black;
                    });
                    RealChangeId = tempid;
                }
        }
        private void SyncCraftables(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    if (DateTime.Now.Subtract(config.lastCraftableSave).TotalMinutes > config.refresh_minutes * 4)
                    {
                        GetValuesOfCraftables();
                    }
                }
                catch { }
                System.Threading.Thread.Sleep(60000);
            }
        }
        #endregion

        #region RunableHelpers
        [STAThread]
        private string FindHead(string tempChangeID = "", bool secondaryRun = false)
        {
            HttpWebRequest request2 = WebRequest.Create("http://api.poe.ninja/api/Data/GetStats") as HttpWebRequest;


            // Get response  
            if (string.IsNullOrEmpty(tempChangeID))
                using (HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    using (StreamReader reader = new StreamReader(response2.GetResponseStream()))
                    {
                        JObject jo = JObject.Parse(reader.ReadToEnd());
                        tempChangeID = jo.Children().ToList()[1].First.ToString();
                    }
                }
            int changeby = 10000;
            while (true)
            {
                HttpWebRequest request = WebRequest.Create("http://www.pathofexile.com/api/public-stash-tabs?id=" + tempChangeID) as HttpWebRequest;
                //textBox1.Invoke((MethodInvoker)delegate
                //{
                //    textBox1.Text = "Waiting for POE Change Response";
                //});
                // Get response  
                if (secondaryRun)
                    System.Threading.Thread.Sleep(config.number_of_people * 3000);
                else
                    System.Threading.Thread.Sleep(config.number_of_people * 1500);

                try
                {
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        // Get the response stream  
                        using (Stream stream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {

                                char[] buffer = new char[100];
                                reader.ReadBlock(buffer, 0, 99);
                                string newstring = new string(buffer);
                                int index = newstring.IndexOf("\",");
                                newstring = newstring.Substring(0, index + 1);
                                newstring = JObject.Parse(newstring + "}")["next_change_id"].ToString();
                                if (newstring.Equals(tempChangeID))
                                {
                                    if (changeby == 400)
                                    {
                                        List<int> intsold = tempChangeID.Split('-').Select(p => int.Parse(p)).ToList();
                                        string x = "";
                                        foreach (int i in intsold)
                                            x += "-" + (i - changeby);
                                        tempChangeID = x.Substring(1);
                                        RealChangeId = tempChangeID;
                                        System.Threading.Thread.Sleep(3000);
                                        return tempChangeID;
                                    }
                                    else
                                    {
                                        List<int> intsold = tempChangeID.Split('-').Select(p => int.Parse(p)).ToList();
                                        string x = "";
                                        foreach (int i in intsold)
                                            x += "-" + (i - changeby);
                                        tempChangeID = x.Substring(1);
                                        changeby = changeby / 5;

                                    }
                                }
                                else
                                {
                                    List<int> ints = newstring.Split('-').Select(p => int.Parse(p)).ToList();
                                    List<int> intsold = tempChangeID.Split('-').Select(p => int.Parse(p)).ToList();
                                    for (int i = 0; i < ints.Count; i++)
                                    {
                                        if (ints[i] > intsold[i])
                                            ints[i] += changeby;

                                    }

                                    txtBoxFasterSearch.Invoke((MethodInvoker)delegate
                                    {
                                        txtBoxFasterSearch.Text = ints.Last().ToString();
                                        txtBoxFasterSearch.ForeColor = Color.Red;
                                    });
                                    string x = "";
                                    foreach (int i in ints)
                                        x += "-" + i;
                                    tempChangeID = x.Substring(1);

                                }
                            }
                        }
                    }
                }
                catch
                {
                    PlayErrorSound();
                }
            }
        }
        private void GenerateAllBaseWepsFromString()
        {
            allBaseTypes = new List<WeaponBaseItem>();
            Dictionary<string, string> allbasetypesstring = NinjaPoETradeMethods.LoadAllBaseWeaponTypes();
            foreach (string s in allbasetypesstring.Keys)
            {
                if (allbasetypesstring[s] == "Weapon")
                {
                    WeaponBaseItem wep = NinjaPoETradeMethods.FindBaseOnHitAndAttackSpeed(s);
                    if (wep != null)
                        allBaseTypes.Add(wep);
                }
            }
            string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(allBaseTypes);
            JArray ja = JArray.Parse(serialized);
            serialized = ja.ToString();
            System.IO.File.Delete(FinalVariables.itemfilename);
            System.IO.File.WriteAllText("allBaseTypes.json", serialized);
        }
        private void GetValuesOfCraftables()
        {
            List<POETradeCraftable> copy = craftables.ToList();
            int count = 0;
            foreach (POETradeCraftable craft in copy)
            {
                lblDebug.Invoke((MethodInvoker)delegate
                {
                    lblDebug.Text = "craft : " + count++ + "/" + copy.Count;
                });
                if (DateTime.Now.Subtract(craft.last_time_saved).TotalHours < 2)
                    continue;
                craft.dpsBenchmarks = new Dictionary<int, ValueUrlCombo>();
                int x = int.Parse(craft.pdps);
                for (int i = x; i <= x * 2; i += 10)
                {
                    POETradeConfig temp = new POETradeConfig();
                    temp.type = craft.type;
                    temp.crafted = null;
                    temp.corrupted = false;
                    temp.pdps = i.ToString();
                    temp.mods = craft.requiredMods;
                    temp.rarity = POETradeConfig.Rarity.none;
                    string url = "";
                    List<int> ret = NinjaPoETradeMethods.GetPoeLowest5Prices(temp, out url);
                    if (ret.Count > 0)
                        craft.dpsBenchmarks.Add(i, new ValueUrlCombo((int)ret.Average(), url));
                    else
                    {
                        craft.dpsBenchmarks.Add(i, new ValueUrlCombo(10000, url));
                        craft.last_time_saved = DateTime.Now;
                        break;
                    }


                }
                craft.last_time_saved = DateTime.Now;
            }
            config.lastCraftableSave = DateTime.Now;
            craftables = copy;
            saveCraftables(copy);
            SaveConfigAndWatchlist();

            LoadBasicInfo();
            lblDebug.Invoke((MethodInvoker)delegate
            {
                lblDebug.Text = "Craft Complete";
            });
        }
        private void GetValuesOfWatchedRares()
        {
            List<POETradeConfig> tempRareList = watchedRares.ToArray().ToList();
            for (int i = 0; i < tempRareList.Count; i++)
            {
                POETradeConfig rare = tempRareList[i];
                txtRareUpdateStatus.Invoke((MethodInvoker)delegate
                {
                    txtRareUpdateStatus.Text = (i + 1) + " / " + tempRareList.Count;
                });
                if (DateTime.Now.Subtract(rare.last_time_saved).TotalHours < 2)
                    continue;
                if (rare.manual_price_override.HasValue)
                    rare.estimated_value = rare.manual_price_override.Value;
                else
                {
                    string killme = "";
                    List<int> prices = NinjaPoETradeMethods.GetPoeLowest5Prices(rare, out killme);
                    if (prices.Count > 0)
                        rare.estimated_value = prices.Sum(p => p) / prices.Count;
                    else
                        rare.estimated_value = 10000;
                }
                rare.last_time_saved = DateTime.Now;
                tempRareList[i] = rare;
            }
            saveRares(tempRareList);
        }
        #endregion

        private void ProcessItems(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(15000);
            double timeCraft = 0;
            double timeRare = 0;
            while (true)
            {
                try
                {
                    Item itemProp = null;
                    while (raresQueue.TryDequeue(out itemProp))
                    {
                        if (itemProp.implicitMods == null)
                            itemProp.implicitMods = new string[] { "" };
                        if (itemProp.explicitMods == null)
                            itemProp.explicitMods = new string[] { "" };
                        if (raresQueue.Count % 5 == 0)
                            txtUniqueStatus.Invoke((MethodInvoker)delegate
                            {
                                txtUniqueStatus.Text = "Queue: " + raresQueue.Count;
                            });
                        if (itemProp.value > config.max_price)
                            continue;
                        itemProp.name = itemProp.name.Replace("<<set:MS>><<set:M>><<set:S>>", "");
                        itemProp.typeLine = itemProp.typeLine.Replace("<<set:MS>><<set:M>><<set:S>>", "");
                        if (config.do_all_uniques)
                        {
                            if ((ninjaItems.Where(p => p.name == itemProp.name && p.type == itemProp.frameType.ToString() && p.base_type == itemProp.typeLine).Count() > 0) || (itemProp.frameType == 6 && ninjaItems.Where(p => p.name == itemProp.typeLine).Count() > 0))
                            {
                                NinjaItem ninja = new NinjaItem();
                                if (itemProp.frameType != 6)
                                    ninja = ninjaItems.First(p => p.name == itemProp.name && p.type == itemProp.frameType.ToString() && p.base_type == itemProp.typeLine);
                                else
                                    ninja = ninjaItems.First(p => p.name == itemProp.typeLine && p.type == itemProp.frameType.ToString());

                                if (ninja.chaos_value > 15)
                                    GetExplicitFields(ninja, itemProp);
                                if (ninja.chaos_value * config.profit_percent > itemProp.value && ninja.chaos_value - config.min_profit_range > itemProp.value)
                                {
                                    if (ninja.is_weapon)
                                        itemProp.pdps = NinjaPoETradeMethods.GetDdpsOfLocalWeapon(itemProp);


                                    SetSlots(itemProp, ninja);
                                    continue;
                                }
                            }
                        }
                        if (config.do_watch_list)
                        {
                            if (watched_items.Where(p => itemProp.name.ToLower().Contains(p.name.ToLower()) || itemProp.typeLine.ToLower().Contains(p.name.ToLower())).Count() > 0)
                            {
                                NinjaItem localitem = watched_items.Where(p => itemProp.
                                name.ToLower().Contains(p.name.ToLower()) || itemProp.typeLine.ToLower().Contains(p.name.ToLower())).OrderByDescending(p => p.name.Length).FirstOrDefault();
                                if (localitem.chaos_value % 1 == .01m && localitem.chaos_value >= itemProp.value || (localitem.chaos_value * config.profit_percent > itemProp.value && localitem.chaos_value - config.min_profit_range > itemProp.value))
                                {

                                    SetSlots(itemProp, localitem);
                                    continue;
                                }
                            }
                        }
                        
                        secondrareQueue.Enqueue(itemProp);
                    }
                    
                    while (raresQueue.Count < 10 && secondrareQueue.Count > 0)
                    {

                        if (secondrareQueue.Count % 5 == 0)
                            txtRareStatus.Invoke((MethodInvoker)delegate
                            {
                                txtRareStatus.Text = "Queue: " + secondrareQueue.Count;
                            });
                        if (secondrareQueue.Count > 10000)
                        {
                            
                            secondrareQueue = new ConcurrentQueue<Item>();
                        }
                        Item rareItemProp = null;
                        secondrareQueue.TryDequeue(out rareItemProp);
                        if (rareItemProp == null)
                            continue;
                        if ((rareItemProp.frameType != 2 && rareItemProp.frameType != 1 && rareItemProp.frameType != 4) || (rareItemProp.craftedMods != null && rareItemProp.craftedMods.Count() > 0))
                            continue;
                        DateTime now = DateTime.Now;
                        if (config.do_watch_rares && watchedRares.Count > 0)//is rare
                        {
                            foreach (POETradeConfig rare in watchedRares.OrderByDescending(p => p.estimated_value))
                            {
                                if (rare.estimated_value * config.profit_percent > rareItemProp.value && POETradeConfig.SeeIfItemMatchesRare(rare, rareItemProp, all_base_types))
                                {
                                    NinjaItem fakeNinja = new NinjaItem();
                                    fakeNinja.name = "Rare:" + rare.type.ToString();
                                    fakeNinja.chaos_value = rare.estimated_value;
                                    foreach (KeyValuePair<string, string> kvp in rare.mods)
                                        fakeNinja.Explicits.Add(string.Format("{0} : {1}", kvp.Key, kvp.Value));
                                    SetSlots(rareItemProp, fakeNinja, rare.url);
                                    break;
                                }
                            }
                            timeRare += DateTime.Now.Subtract(now).TotalSeconds;
                        }
                        if (config.do_craft_watch)
                        {
                            now = DateTime.Now;
                            //each craftable

                            foreach (POETradeCraftable craft in craftables.OrderByDescending(p => p.requiredMods.Count))//order by more precice requirements. crit over non crit for same base. 
                            {
                                //basic check
                                if (craft.dpsBenchmarks.Count == 0)
                                    continue;
                                if (POETradeCraftable.SeeIfItemMatchesRare(craft, rareItemProp, all_base_types))
                                {
                                    //get items mods and values

                                    //not full of affixes
                                    if (rareItemProp.explicitMods.Count() < 7)
                                    {
                                        Dictionary<string, decimal> itemMods = POETradeConfig.GetItemMods(rareItemProp);
                                        //if has required mods (CRIT)
                                        if (craft.requiredMods.Count > 0)
                                        {
                                            string req = POETradeConfig.ConvertToCommonForm(craft.requiredMods.FirstOrDefault().Key);
                                            if (itemMods.ContainsKey(req))
                                            {
                                                if (itemMods[req] < decimal.Parse(craft.requiredMods.FirstOrDefault().Value))
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    //see if any craftables are absent.
                                                    foreach (string key in craft.craftableMods.OrderBy(p => p.Value).Select(p => p.Key))
                                                    {
                                                        string common = POETradeConfig.ConvertToCommonForm(key);
                                                        if (!itemMods.ContainsKey(common))
                                                        {
                                                            //get new item value.
                                                            Item craftedVersion = new Item();
                                                            List<string> mods = rareItemProp.explicitMods.ToList();
                                                            mods.Add(key.Replace("#", craft.craftableMods[key]));
                                                            craftedVersion.explicitMods = mods.ToArray();
                                                            craftedVersion.typeLine = rareItemProp.typeLine;
                                                            int dps = NinjaPoETradeMethods.GetDdpsOfLocalWeapon(craftedVersion);
                                                            ValueUrlCombo value = craft.dpsBenchmarks.Where(p => dps > p.Key).OrderByDescending(p => p.Value.value).FirstOrDefault().Value;
                                                            if (value.value * (1 - (1 - config.profit_percent) * 2) > rareItemProp.value)//double profit
                                                            {
                                                                NinjaItem fakeNinja = new NinjaItem();
                                                                fakeNinja.name = "Craft:" + craft.type.ToString();
                                                                fakeNinja.chaos_value = value.value;
                                                                foreach (KeyValuePair<string, string> kvp in craft.requiredMods)
                                                                    fakeNinja.Explicits.Add(string.Format("Req : {0} : {1}", kvp.Key, kvp.Value));
                                                                foreach (KeyValuePair<string, string> kvp in craft.craftableMods)
                                                                    fakeNinja.Explicits.Add(string.Format("Craft : {0} : {1}", kvp.Key, kvp.Value));
                                                                craftedVersion.explicitMods[craftedVersion.explicitMods.Count() - 1] = "Craft: " + craftedVersion.explicitMods.Last();
                                                                rareItemProp.explicitMods = craftedVersion.explicitMods;
                                                                rareItemProp.pdps = dps;
                                                                SetSlots(rareItemProp, fakeNinja, value.url);

                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //see if required craft is profitable. 
                                                Item craftedVersion = new Item();
                                                List<string> mods = rareItemProp.explicitMods.ToList();
                                                mods.Add(craft.requiredMods.FirstOrDefault().Key.Replace("#", craft.requiredMods.FirstOrDefault().Key));
                                                craftedVersion.explicitMods = mods.ToArray();
                                                craftedVersion.typeLine = rareItemProp.typeLine;
                                                int dps = NinjaPoETradeMethods.GetDdpsOfLocalWeapon(craftedVersion);
                                                ValueUrlCombo value = craft.dpsBenchmarks.Where(p => dps > p.Key).OrderByDescending(p => p.Value.value).FirstOrDefault().Value;
                                                if (value.value * (1 - (1 - config.profit_percent) * 2) > rareItemProp.value)//double profit
                                                {
                                                    NinjaItem fakeNinja = new NinjaItem();
                                                    fakeNinja.name = "Craft:" + craft.type.ToString();
                                                    fakeNinja.chaos_value = value.value;
                                                    foreach (KeyValuePair<string, string> kvp in craft.requiredMods)
                                                        fakeNinja.Explicits.Add(string.Format("Req : {0} : {1}", kvp.Key, kvp.Value));
                                                    foreach (KeyValuePair<string, string> kvp in craft.craftableMods)
                                                        fakeNinja.Explicits.Add(string.Format("Craft : {0} : {1}", kvp.Key, kvp.Value));
                                                    craftedVersion.explicitMods[craftedVersion.explicitMods.Count() - 1] = "Craft: " + craftedVersion.explicitMods.Last();
                                                    rareItemProp.explicitMods = craftedVersion.explicitMods;
                                                    SetSlots(rareItemProp, fakeNinja, value.url);
                                                }
                                            }


                                        }
                                        else
                                        {
                                            //see if any craftables are absent.
                                            foreach (string key in craft.craftableMods.Keys)
                                            {
                                                if (!itemMods.ContainsKey(key))
                                                {
                                                    //get new item value.
                                                    Item craftedVersion = new Item();
                                                    List<string> mods = rareItemProp.explicitMods.ToList();
                                                    mods.Add(key.Replace("#", craft.craftableMods[key]));
                                                    craftedVersion.explicitMods = mods.ToArray();
                                                    craftedVersion.typeLine = rareItemProp.typeLine;

                                                    int dps = NinjaPoETradeMethods.GetDdpsOfLocalWeapon(craftedVersion);
                                                    ValueUrlCombo value = craft.dpsBenchmarks.Where(p => dps > p.Key).OrderByDescending(p => p.Value.value).FirstOrDefault().Value;
                                                    if (value.value * (1 - (1 - config.profit_percent) * 2) > rareItemProp.value)//double profit
                                                    {
                                                        NinjaItem fakeNinja = new NinjaItem();
                                                        fakeNinja.name = "Craft:" + craft.type.ToString();
                                                        fakeNinja.chaos_value = value.value;
                                                        foreach (KeyValuePair<string, string> kvp in craft.requiredMods)
                                                            fakeNinja.Explicits.Add(string.Format("Req : {0} : {1}", kvp.Key, kvp.Value));
                                                        foreach (KeyValuePair<string, string> kvp in craft.craftableMods)
                                                            fakeNinja.Explicits.Add(string.Format("Craft : {0} : {1}", kvp.Key, kvp.Value));
                                                        craftedVersion.explicitMods[craftedVersion.explicitMods.Count() - 1] = "Craft: " + craftedVersion.explicitMods.Last();
                                                        rareItemProp.explicitMods = craftedVersion.explicitMods;
                                                        rareItemProp.pdps = dps;
                                                        SetSlots(rareItemProp, fakeNinja, value.url);
                                                    }

                                                }
                                            }
                                        }

                                    }

                                }
                            }
                            timeCraft += DateTime.Now.Subtract(now).TotalSeconds;
                        }


                    }

                }
                catch (Exception eee)
                {
                    PlayErrorSound();
                    //System.Threading.Thread.Sleep(5000);
                }
            }
        }

        [STAThread]
        private void DoBackgroundWork(object sender, DoWorkEventArgs e)
        {

            DateTime lastTimeAPIcalled = DateTime.Now;
            Dictionary<string, decimal> seenItems = new Dictionary<string, decimal>();
            DateTime lastClearedSeen = DateTime.Now;
            DateTime refreshConfig = DateTime.Now;
            Slots.Add(new Slot());
            Slots.Add(new Slot());
            Slots.Add(new Slot());
            justTextSlots.Add(new Slot());
            justTextSlots.Add(new Slot());
            justTextSlots.Add(new Slot());
            System.Threading.Thread.Sleep(1000);
            textBox1.Invoke((MethodInvoker)delegate
            {
                textBox1.Text = "Converting Poe.Ninja Items";
            });

            //why even do this when we overwrite
            /*
            if (config.do_all_uniques && config.LastSaved.AddHours(1) < DateTime.Now)
                ninjaItems = NinjaPoETradeMethods.SetNinjaValues(ninjaItems, txtBoxUpdateThread);
                */

            ninjaItems = config.SavedItems;


            textBox1.Invoke((MethodInvoker)delegate
            {
                textBox1.Text = "Poe.Ninja Items Converted";
            });

            HttpWebRequest request2 = WebRequest.Create("http://api.poe.ninja/api/Data/GetStats") as HttpWebRequest;
            textBox1.Invoke((MethodInvoker)delegate
            {
                textBox1.Text = "Locating Head";
            });

            UsedChangeId = FindHead();
            // Get response  
            /*using (HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                using (StreamReader reader = new StreamReader(response2.GetResponseStream()))
                {
                    JObject jo = JObject.Parse(reader.ReadToEnd());
                    changeID = jo.Children().ToList()[1].First.ToString();
                }
            }*/

            // Create the web request  
            textBox1.Invoke((MethodInvoker)delegate
            {
                textBox1.Text = "Found Head";
            });
            while (true)
            {
                try
                {
                    if (DateTime.Now.Subtract(lastClearedSeen).TotalSeconds > 3600)
                    {
                        seenItems.Clear();
                        lastClearedSeen = DateTime.Now;
                    }
                    SetTimeseconds(Slots);
                    /* shouldnt need this anymore
                    if (config.do_all_uniques_with_ranges && DateTime.Now.Subtract(refreshConfig).TotalSeconds > 50)
                    {
                        LoadBasicInfo();
                        ninjaItems = config.SavedItems;
                        refreshConfig = DateTime.Now;
                    }
                    */
                    //detect if change is too old

                    int currchange = int.Parse(UsedChangeId.Split('-').Last());
                    int otherchange = int.Parse(RealChangeId.Split('-').Last());
                    if (otherchange - currchange >= 400)
                        UsedChangeId = UsedChangeId.Substring(0, 36) + otherchange;
                    if (!txtBoxFasterSearch.Text.ToLower().Contains("run in"))
                    {
                        System.Threading.Thread.Sleep(5000);
                    }
                    HttpWebRequest request = WebRequest.Create("http://www.pathofexile.com/api/public-stash-tabs?id=" + UsedChangeId) as HttpWebRequest;
                    //textBox1.Invoke((MethodInvoker)delegate
                    //{
                    //    textBox1.Text = "Waiting for POE Change Response";
                    //});
                    // Get response  
                    DateTime now = DateTime.Now;
                    lastTimeAPIcalled = DateTime.Now;
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        // Get the response stream  
                        using (Stream stream = response.GetResponseStream())
                        {

                            double seconds0 = (DateTime.Now - now).TotalSeconds;
                            List<JToken> jo;
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                // Console application output  

                                double secondsToResponse = (DateTime.Now - now).TotalSeconds;
                                char[] buffer = new char[100];
                                reader.ReadBlock(buffer, 0, 99);
                                string newstring = new string(buffer);
                                int index = newstring.IndexOf("\",");
                                string newchange = newstring.Substring(0, index + 1);
                                newchange = JObject.Parse(newchange + "}")["next_change_id"].ToString();

                                UsedChangeId = newchange;
                                textBox1.Invoke((MethodInvoker)delegate
                                {
                                    textBox1.Text = UsedChangeId.Split('-').Last();
                                });
                                double secondsToChangeID = (DateTime.Now - now).TotalSeconds;
                                string line = String.Empty;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    newstring += line;
                                }

                                jo = JObject.Parse(newstring).Children().ToList();
                            }

                            List<JToken> stashes = jo[1].First().Children().ToList();

                            //textBox1.Invoke((MethodInvoker)delegate
                            //{
                            //    textBox1.Text = "Parsing " + stashes.Count + " stashes";
                            //});

                            foreach (JToken jt in stashes)
                            {
                                //textBox1.Invoke((MethodInvoker)delegate
                                //{
                                //    textBox1.Text = "Parsing " + counter++ + " / " + stashes.Count;
                                //});
                                JEnumerable<JToken> stash = jt.Children();
                                string name = stash.First(p => p.Path.EndsWith(".lastCharacterName")).First.ToString();
                                string accName = stash.First(p => p.Path.EndsWith(".accountName")).First.ToString();
                                if (!string.IsNullOrEmpty(config.account_name) && accName.ToLower() == config.account_name.ToLower())
                                    PlayItsMeSound();

                                if (config.blocked_accounts.Contains(accName))
                                    continue;
                                JEnumerable<JToken> items = stash.First(p => p.Path.EndsWith(".items")).First.Children();

                                foreach (JToken item in items)
                                {
                                    try
                                    {
                                        Item itemProp = item.ToObject<Item>();

                                        if (itemProp.league != LeagueName)
                                            break;
                                        if (string.IsNullOrEmpty(itemProp.note))
                                            continue;
                                        decimal itemValue = GetPriceInChaos(itemProp.note);
                                        if (seenItems.ContainsKey(itemProp.id))
                                        {
                                            if (itemValue == seenItems[itemProp.id])
                                                continue;
                                            else
                                                seenItems[itemProp.id] = itemValue;
                                        }
                                        else
                                        {
                                            seenItems.Add(itemProp.id, itemValue);
                                        }
                                        itemProp.value = itemValue;
                                        itemProp.char_name = name;
                                        itemProp.acc_name = accName;
                                        raresQueue.Enqueue(itemProp);
                                    }
                                    catch (Exception othere)
                                    {
                                        int x = 5;
                                        PlayErrorSound();
                                    }
                                }
                            }
                            txtMainStatus.Invoke((MethodInvoker)delegate
                            {
                                txtMainStatus.Text = "Running";
                            });

                        }
                        if (DateTime.Now.Subtract(lastTimeAPIcalled).TotalSeconds < config.number_of_people * 2)
                        {

                            System.Threading.Thread.Sleep((int)(config.number_of_people * 2 - DateTime.Now.Subtract(lastTimeAPIcalled).TotalSeconds) * 2000);
                        }
                    }
                }
                catch (Exception eee)
                {
                    txtMainStatus.Invoke((MethodInvoker)delegate
                    {
                        txtMainStatus.Text = "Failed";
                    });
                    PlayErrorSound();
                    System.Threading.Thread.Sleep(5000);
                }

            }

        }
        public static List<ExplicitField> GetExplicitFields(NinjaItem nj, Item sellItem)
        {
            return GetExplicitFields(nj.ExplicitFields, sellItem);
        }
        public static List<ExplicitField> GetExplicitFields(List<ExplicitField> fields, Item sellItem)
        {

            List<ExplicitField> Explicits = new List<ExplicitField>();
            foreach (ExplicitField ef in fields)
            {
                try
                {
                    //alright its a rollable field, lets compare
                    Regex rgx = new Regex("[^a-zA-Z -]");
                    string LettersOnly = rgx.Replace(ef.SearchField, "");
                    string ItemRoll = sellItem.explicitMods.First(p => rgx.Replace(p, "") == LettersOnly);
                    ExplicitField sellItemEF = new ExplicitField();
                    sellItemEF.SearchField = ef.SearchField;
                    sellItemEF.MinRoll = GetMultipleNumbers(ItemRoll);
                    Explicits.Add(sellItemEF);
                }
                catch (Exception e)
                {
                    int x = 5;
                    //return new List<ExplicitField>();
                }
            }
            return Explicits;
        }
        [STAThread]
        private void PlayItsMeSound()
        {
            SoundPlayer player = new SoundPlayer();

            if (!config.johnsounds)
                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\Mario.wav";
            else
                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
            player.Play();
        }
        [STAThread]
        private void PlayErrorSound()
        {
            if (!config.johnsounds)
            {
                SoundPlayer player = new SoundPlayer();
                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\Error.wav";
                player.Play();
            }
        }

        [STAThread]
        private void SetjustTextSlots(Slot s)
        {
            if (s != null)
            {
                if (justTextSlots.Count == 3)
                    justTextSlots.RemoveAt(2);
                justTextSlots.Insert(0, s);
            }
            if (justTextSlots[0].SellItem != null)
            {

                richTxtBox8Rep.Invoke((MethodInvoker)delegate
                {
                    richTxtBox8Rep.Text = justTextSlots[0].SellItem.name + " " + justTextSlots[0].SellItem.typeLine + " for " + GetPriceInChaos(justTextSlots[0].SellItem.note) + " chaos: " + justTextSlots[0].worth + " : " + DateTime.Now.ToShortTimeString() + " " + justTextSlots[0].name;
                    richTxtBox8Rep.ForeColor = Color.DarkGreen;
                });
            }
            richtxtBox2Rep.Invoke((MethodInvoker)delegate
            {
                richtxtBox2Rep.ForeColor = Color.Black;
            });
            if (justTextSlots[1].SellItem != null)
            {
                textBox9.Invoke((MethodInvoker)delegate
                {
                    textBox9.Text = justTextSlots[1].SellItem.name + " " + justTextSlots[1].SellItem.typeLine + " for " + GetPriceInChaos(justTextSlots[1].SellItem.note) + " chaos: " + justTextSlots[1].worth + " : " + DateTime.Now.ToShortTimeString() + " " + justTextSlots[1].name;
                });
            }
            if (justTextSlots[2].SellItem != null)
            {
                textBox10.Invoke((MethodInvoker)delegate
                {
                    textBox10.Text = justTextSlots[2].SellItem.name + " " + justTextSlots[2].SellItem.typeLine + " for " + GetPriceInChaos(justTextSlots[2].SellItem.note) + " chaos: " + justTextSlots[2].worth + " : " + DateTime.Now.ToShortTimeString() + " " + justTextSlots[2].name;
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
            lblCopyLock.Invoke((MethodInvoker)delegate
            {
                if (now.Subtract(last_time_clicked).TotalDays > 1)
                    last_time_clicked = DateTime.Now.AddMinutes(-5);
                int seconds = (int)now.Subtract(last_time_clicked).TotalSeconds;
                if (seconds < 20)
                {
                    lblCopyLock.Text = "Copying Blocked for :" + (20 - seconds) + "s";
                    lblCopyLock.ForeColor = Color.Red;
                }
                else
                {
                    lblCopyLock.Text = "Copying Enabled";
                    lblCopyLock.ForeColor = Color.Green;
                }
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
        [STAThread]
        public void SetSlots(Item itemProp, NinjaItem ninja, string url = "")
        {
            if (itemProp != null)
            {
                Slot s = new Slot();

                s.account_name = itemProp.acc_name;
                s.BaseItem = ninja;
                s.SellItem = itemProp;
                s.name = itemProp.char_name;
                s.worth = ninja.chaos_value.ToString();
                int x = findWhoGets(itemProp.id, config.number_of_people);
                s.is_mine = x == config.my_number;
                s.url = url;
                s.Message = "@" + itemProp.char_name + " Hi, I would like to buy your " + itemProp.name + " " + itemProp.typeLine + " listed for " + GetOriginalPrice(itemProp.note) + " in Abyss (stash tab \"" + itemProp.inventoryId + "\"; position: left " + itemProp.x + ", top " + itemProp.y + ")";


                if (Slots.Count == 3)
                {
                    SetjustTextSlots(Slots[2]);
                    Slots.RemoveAt(2);
                }
                Slots.Insert(0, s);
            }


            if (Slots[0].BaseItem != null)
            {

                Slot localslot = Slots[0];
                SoundPlayer player = new SoundPlayer();
                if (localslot.is_mine)
                {
                    if (!config.johnsounds)
                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\cartoon022.wav";
                    else
                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                }
                else
                    player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                if (localslot.BaseItem.chaos_value - Convert.ToDecimal(GetPriceInChaos(localslot.SellItem.note)) > 30)
                {
                    if (!config.johnsounds)
                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\mine.wav";
                    else
                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ding.wav";
                }
                player.Play();


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
                    textBox3.Text = GetPriceInChaos(localslot.SellItem.note) + " : " + localslot.BaseItem.chaos_value + " : " + (int)(((Decimal)localslot.BaseItem.chaos_value - GetPriceInChaos(localslot.SellItem.note)) / ((Decimal)GetPriceInChaos(localslot.SellItem.note)) * 100) + "%";
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
                    checkBox4.Checked = localslot.SellItem.corrupted;
                    if (checkBox4.Checked)
                        checkBox4.ForeColor = Color.Red;
                    else
                        checkBox4.ForeColor = Color.Black;
                });
                chkEnchanted1.Invoke((MethodInvoker)delegate
                {
                    chkEnchanted1.Checked = localslot.SellItem.enchantMods != null;
                    if (chkEnchanted1.Checked)
                        chkEnchanted1.ForeColor = Color.Blue;
                    else
                        chkEnchanted1.ForeColor = Color.Black;
                });
                if (DateTime.Now.Subtract(last_time_clicked).TotalSeconds > 20)
                {
                    btnFakeFirstMsg.Invoke((MethodInvoker)delegate
                    {
                        btnFakeFirstMsg.PerformClick();
                    });
                }
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
                    slot0minandavrg.Lines = localslot.BaseItem.Top5Sells?.ConvertAll(p => p.ToString()).ToArray();
                });

                lblUpdatedTime1.Invoke((MethodInvoker)delegate
                {
                    if (localslot.BaseItem.last_updated_poetrade.HasValue)
                    {
                        int minutes = (int)DateTime.Now.Subtract(localslot.BaseItem.last_updated_poetrade.Value).TotalMinutes;
                        if (minutes < 1000)
                        {
                            if (minutes > 60)
                                lblUpdatedTime1.Text = (int)(minutes / 60) + " hours";
                            else
                                lblUpdatedTime1.Text = minutes + " min";
                        }
                        else
                            lblUpdatedTime1.Text = "";
                    }
                    else
                        lblUpdatedTime1.Text = "";
                });
                if (localslot.SellItem.pdps != 0)
                {
                    lblLocalDps1.Invoke((MethodInvoker)delegate
                    {
                        lblLocalDps1.Text = localslot.SellItem.pdps.ToString();
                    });

                    lblDpsRange1.Invoke((MethodInvoker)delegate
                    {
                        if (localslot.BaseItem.minPdps != 0)
                        {
                            lblDpsRange1.Text = ((int)localslot.BaseItem.minPdps) + "-" + ((int)localslot.BaseItem.maxPdps);
                        }
                        else
                            lblDpsRange1.Text = "???";
                    });
                }
                else
                {
                    lblLocalDps1.Invoke((MethodInvoker)delegate
                    {
                        lblLocalDps1.Text = "";
                    });
                    lblDpsRange1.Invoke((MethodInvoker)delegate
                    {
                        lblDpsRange1.Text = "";
                    });
                }
                lblLinks1.Invoke((MethodInvoker)delegate
                {
                    lblLinks1.Text = "Links: " + localslot.SellItem.Links;
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
                    textBox5.Text = GetPriceInChaos(localslot.SellItem.note) + " : " + localslot.BaseItem.chaos_value + " : " + (int)(((Decimal)localslot.BaseItem.chaos_value - GetPriceInChaos(localslot.SellItem.note)) / ((Decimal)GetPriceInChaos(localslot.SellItem.note)) * 100) + "%";
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
                    checkBox5.Checked = localslot.SellItem.corrupted;
                    if (checkBox5.Checked)
                        checkBox5.ForeColor = Color.Red;
                    else
                        checkBox5.ForeColor = Color.Black;
                });
                chkEnchanted2.Invoke((MethodInvoker)delegate
                {
                    chkEnchanted2.Checked = localslot.SellItem.enchantMods != null;
                    if (chkEnchanted2.Checked)
                        chkEnchanted2.ForeColor = Color.Blue;
                    else
                        chkEnchanted2.ForeColor = Color.Black;
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
                    slot1minandavrg.Lines = Slots[1].BaseItem.Top5Sells?.ConvertAll(p => p.ToString()).ToArray();
                });
                lblUpdatedTime2.Invoke((MethodInvoker)delegate
                {
                    if (localslot.BaseItem.last_updated_poetrade.HasValue)
                    {
                        int minutes = (int)DateTime.Now.Subtract(localslot.BaseItem.last_updated_poetrade.Value).TotalMinutes;
                        if (minutes < 1000)
                        {
                            if (minutes > 60)
                                lblUpdatedTime2.Text = (int)(minutes / 60) + " hours";
                            else
                                lblUpdatedTime2.Text = minutes + " min";
                        }
                        else
                            lblUpdatedTime2.Text = "";
                    }
                    else
                        lblUpdatedTime2.Text = "";
                });
                if (localslot.SellItem.pdps != 0)
                {
                    lblLocalDps2.Invoke((MethodInvoker)delegate
                    {
                        lblLocalDps2.Text = localslot.SellItem.pdps.ToString();
                    });

                    lblDpsRange2.Invoke((MethodInvoker)delegate
                    {
                        if (localslot.BaseItem.minPdps != 0)
                        {
                            lblDpsRange2.Text = ((int)localslot.BaseItem.minPdps) + "-" + ((int)localslot.BaseItem.maxPdps);
                        }
                        else
                            lblDpsRange2.Text = "???";
                    });
                }
                else
                {
                    lblLocalDps2.Invoke((MethodInvoker)delegate
                    {
                        lblLocalDps2.Text = "";
                    });
                    lblDpsRange2.Invoke((MethodInvoker)delegate
                    {
                        lblDpsRange2.Text = "";
                    });
                }
                lblLinks2.Invoke((MethodInvoker)delegate
                {
                    lblLinks2.Text = "Links: " + localslot.SellItem.Links;
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
                    textBox7.Text = GetPriceInChaos(localslot.SellItem.note) + " : " + localslot.BaseItem.chaos_value + " : " + (int)(((Decimal)localslot.BaseItem.chaos_value - GetPriceInChaos(localslot.SellItem.note)) / ((Decimal)GetPriceInChaos(localslot.SellItem.note)) * 100) + "%";
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
                    checkBox6.Checked = localslot.SellItem.corrupted;
                    if (checkBox6.Checked)
                        checkBox6.ForeColor = Color.Red;
                    else
                        checkBox6.ForeColor = Color.Black;
                });
                chkEnchanted3.Invoke((MethodInvoker)delegate
                {
                    chkEnchanted3.Checked = localslot.SellItem.enchantMods != null;
                    if (chkEnchanted3.Checked)
                        chkEnchanted3.ForeColor = Color.Blue;
                    else
                        chkEnchanted3.ForeColor = Color.Black;
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
                    slot2minandavrg.Lines = Slots[2].BaseItem.Top5Sells?.ConvertAll(p => p.ToString()).ToArray();
                });
                lblUpdatedTime3.Invoke((MethodInvoker)delegate
                {
                    if (localslot.BaseItem.last_updated_poetrade.HasValue)
                    {
                        int minutes = (int)DateTime.Now.Subtract(localslot.BaseItem.last_updated_poetrade.Value).TotalMinutes;
                        if (minutes < 1000)
                        {
                            if (minutes > 60)
                                lblUpdatedTime3.Text = (int)(minutes / 60) + " hours";
                            else
                                lblUpdatedTime3.Text = minutes + " min";
                        }
                        else
                            lblUpdatedTime3.Text = "";
                    }
                    else
                        lblUpdatedTime3.Text = "";
                });
                if (localslot.SellItem.pdps != 0)
                {
                    lblLocalDps3.Invoke((MethodInvoker)delegate
                    {
                        lblLocalDps3.Text = localslot.SellItem.pdps.ToString();
                    });

                    lblDpsRange3.Invoke((MethodInvoker)delegate
                    {
                        if (localslot.BaseItem.minPdps != 0)
                        {
                            lblDpsRange3.Text = ((int)localslot.BaseItem.minPdps) + "-" + ((int)localslot.BaseItem.maxPdps);
                        }
                        else
                            lblDpsRange3.Text = "???";
                    });
                }
                else
                {
                    lblLocalDps3.Invoke((MethodInvoker)delegate
                    {
                        lblLocalDps3.Text = "";
                    });
                    lblDpsRange3.Invoke((MethodInvoker)delegate
                    {
                        lblDpsRange3.Text = "";
                    });
                }
                lblLinks3.Invoke((MethodInvoker)delegate
                {
                    lblLinks3.Text = "Links: " + localslot.SellItem.Links;
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
            int realnumber = 0;
            foreach (char c in final)
            {
                realnumber += Convert.ToInt32(c);
            }

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
        public static decimal? GetMultipleNumbersNullable(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            string[] parts = input.Split((" to ".ToCharArray()));
            decimal total = 0;
            foreach (string s in parts)
            {
                try
                {
                    total += Convert.ToInt32(s);
                }
                catch (Exception easdf)
                {

                }
            }

            return total / 2;
        }
        public static decimal? GetSingleNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            string y = new string(input.Where(c => char.IsDigit(c)).ToArray());
            return Convert.ToDecimal(y);
        }





        public static void AddNewName(string name, string value)
        {
            watched_items.Add(new NinjaItem()
            {
                name = name,
                chaos_value = Convert.ToDecimal(value),
            });
        }

        public static void RemoveName(int index)
        {
            watched_items.RemoveAt(index);
        }

        public static void SaveConfigAndWatchlist()
        {
            string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(watched_items);
            JArray ja = JArray.Parse(serialized);
            serialized = ja.ToString();
            System.IO.File.Delete(FinalVariables.itemfilename);
            System.IO.File.WriteAllText(FinalVariables.itemfilename, serialized);
            serialized = Newtonsoft.Json.JsonConvert.SerializeObject(config);
            JObject jo = JObject.Parse(serialized);
            serialized = jo.ToString();
            System.IO.File.Delete(FinalVariables.configfile);
            System.IO.File.WriteAllText(FinalVariables.configfile, serialized);
        }
        public static void LoadBasicInfo()
        {

            try
            {
                watched_items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NinjaItem>>(System.IO.File.ReadAllText(FinalVariables.itemfilename));
            }
            catch (Exception e)
            {
                watched_items = new List<NinjaItem>();
            }
            try
            {
                config = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemWatchConfig>(System.IO.File.ReadAllText(FinalVariables.configfile));
            }
            catch (Exception e)
            {
                config = new ItemWatchConfig();
            }
            try
            {
                othercurrencies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NotChaosCurrencyConversion>>(System.IO.File.ReadAllText(FinalVariables.currencyfilename));
            }
            catch (Exception e)
            {
                othercurrencies = new List<NotChaosCurrencyConversion>();
            }
            try
            {
                allBaseTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WeaponBaseItem>>(System.IO.File.ReadAllText(FinalVariables.wepBaseTypesFile));
            }
            catch (Exception e)
            {
                allBaseTypes = new List<WeaponBaseItem>();
            }
            try
            {
                watchedRares = Newtonsoft.Json.JsonConvert.DeserializeObject<List<POETradeConfig>>(System.IO.File.ReadAllText(FinalVariables.rareFileName));
            }
            catch (Exception e)
            {
                watchedRares = new List<POETradeConfig>();
            }
            try
            {
                all_base_types = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(FinalVariables.baseTypesStringFilename));
            }
            catch (Exception e)
            {
                all_base_types = new Dictionary<string, string>();
            }
            try
            {
                craftables = Newtonsoft.Json.JsonConvert.DeserializeObject<List<POETradeCraftable>>(System.IO.File.ReadAllText(FinalVariables.craftablesFileNames));
            }
            catch (Exception e)
            {
                craftables = new List<POETradeCraftable>();
            }
        }
        public static void saveRares(List<POETradeConfig> rares = null)
        {
            if (rares == null)
                rares = watchedRares;
            string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(rares);
            JArray ja = JArray.Parse(serialized);
            serialized = ja.ToString();
            System.IO.File.Delete(FinalVariables.rareFileName);
            System.IO.File.WriteAllText(FinalVariables.rareFileName, serialized);
        }
        public static void saveCraftables(List<POETradeCraftable> setcraft = null)
        {
            string serialized;
            if (setcraft != null)
                serialized = Newtonsoft.Json.JsonConvert.SerializeObject(setcraft);
            else
                serialized = Newtonsoft.Json.JsonConvert.SerializeObject(craftables);
            JArray ja = JArray.Parse(serialized);
            serialized = ja.ToString();
            System.IO.File.Delete(FinalVariables.craftablesFileNames);
            System.IO.File.WriteAllText(FinalVariables.craftablesFileNames, serialized);
        }

        public static JEnumerable<JToken> DeserializeFromStream(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                JToken jo = JObject.ReadFrom(jsonTextReader);
                return jo.Children();
            }
        }
        private static MemoryStream CopyAndClose(Stream inputStream)
        {
            const int readSize = 256;
            byte[] buffer = new byte[readSize];
            MemoryStream ms = new MemoryStream();

            int count = inputStream.Read(buffer, 0, readSize);
            while (count > 0)
            {
                ms.Write(buffer, 0, count);
                count = inputStream.Read(buffer, 0, readSize);
            }
            ms.Position = 0;
            inputStream.Close();
            return ms;
        }

        public static bool isFileOpen(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return true;
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
                Button btn = (Button)sender;
                if (btn.Name != "btnFakeFirstMsg")
                    last_time_clicked = DateTime.Now;
                Clipboard.SetText(s);
            }
        }
        [STAThread]
        private void button8_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Slots[1].Message))
            {
                string s = Slots[1].Message;
                last_time_clicked = DateTime.Now;
                Clipboard.SetText(s);
            }
        }
        [STAThread]
        private void button13_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Slots[2].Message))
            {
                string s = Slots[2].Message;
                last_time_clicked = DateTime.Now;
                Clipboard.SetText(s);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(justTextSlots[0].Message))
            {
                string s = justTextSlots[0].Message;
                last_time_clicked = DateTime.Now;
                Clipboard.SetText(s);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(justTextSlots[1].Message))
            {
                string s = justTextSlots[1].Message;
                last_time_clicked = DateTime.Now;
                Clipboard.SetText(s);
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(justTextSlots[2].Message))
            {
                string s = justTextSlots[2].Message;
                last_time_clicked = DateTime.Now;
                Clipboard.SetText(s);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Slots[0].url))
            {
                System.Diagnostics.Process.Start(Slots[0].url);
            }
            else if (Slots[0].BaseItem != null)
            {
                string rarity = "unique";
                if (Slots[0].BaseItem.type == "9")
                    rarity = "relic";
                else if (Slots[0].BaseItem.item_class == 6 || Slots[0].BaseItem.type == "6")
                    rarity = "";

                HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/search");
                request23.Method = "POST";
                request23.KeepAlive = true;
                request23.ContentType = "application/x-www-form-urlencoded";
                StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());
                postwriter.Write("league=Abyss&type=&base=&name=" + WebUtility.UrlEncode(Slots[0].BaseItem.name) + "&dmg_min=&dmg_max=&aps_min=&aps_max=&crit_min=&crit_max=&dps_min=&dps_max=&edps_min=&edps_max=&pdps_min=&pdps_max=&armour_min=&armour_max=&evasion_min=&evasion_max=&shield_min=&shield_max=&block_min=&block_max=&sockets_min=&sockets_max=&link_min=&link_max=&sockets_r=&sockets_g=&sockets_b=&sockets_w=&linked_r=&linked_g=&linked_b=&linked_w=&rlevel_min=&rlevel_max=&rstr_min=&rstr_max=&rdex_min=&rdex_max=&rint_min=&rint_max=&mod_name=&mod_min=&mod_max=&group_type=And&group_min=&group_max=&group_count=1&q_min=&q_max=&level_min=&level_max=&ilvl_min=&ilvl_max=&rarity=" + rarity + "&seller=&thread=&identified=&corrupted=&online=x&has_buyout=&altart=&capquality=x&buyout_min=&buyout_max=&buyout_currency=&crafted=&enchanted=");
                postwriter.Close();
                using (HttpWebResponse response2 = request23.GetResponse() as HttpWebResponse)
                {
                    System.Diagnostics.Process.Start(response2.ResponseUri.OriginalString);
                }
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Slots[1].url))
            {
                System.Diagnostics.Process.Start(Slots[1].url);
            }
            else if (Slots[1].BaseItem != null)
            {
                string rarity = "unique";
                if (Slots[1].BaseItem.type == "9")
                    rarity = "relic";
                else if (Slots[1].BaseItem.item_class == 6 || Slots[1].BaseItem.type == "6")
                    rarity = "";
                string redirectUrl = "";
                HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/search");
                request23.Method = "POST";
                request23.KeepAlive = true;
                request23.ContentType = "application/x-www-form-urlencoded";
                StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());
                postwriter.Write("league=Abyss&type=&base=&name=" + WebUtility.UrlEncode(Slots[1].BaseItem.name) + "&dmg_min=&dmg_max=&aps_min=&aps_max=&crit_min=&crit_max=&dps_min=&dps_max=&edps_min=&edps_max=&pdps_min=&pdps_max=&armour_min=&armour_max=&evasion_min=&evasion_max=&shield_min=&shield_max=&block_min=&block_max=&sockets_min=&sockets_max=&link_min=&link_max=&sockets_r=&sockets_g=&sockets_b=&sockets_w=&linked_r=&linked_g=&linked_b=&linked_w=&rlevel_min=&rlevel_max=&rstr_min=&rstr_max=&rdex_min=&rdex_max=&rint_min=&rint_max=&mod_name=&mod_min=&mod_max=&group_type=And&group_min=&group_max=&group_count=1&q_min=&q_max=&level_min=&level_max=&ilvl_min=&ilvl_max=&rarity=" + rarity + "&seller=&thread=&identified=&corrupted=&online=x&has_buyout=&altart=&capquality=x&buyout_min=&buyout_max=&buyout_currency=&crafted=&enchanted=");
                postwriter.Close();
                using (HttpWebResponse response2 = request23.GetResponse() as HttpWebResponse)
                {
                    System.Diagnostics.Process.Start(response2.ResponseUri.OriginalString);
                }
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Slots[2].url))
            {
                System.Diagnostics.Process.Start(Slots[2].url);
            }
            else if (Slots[2].BaseItem != null)
            {
                string rarity = "unique";
                if (Slots[2].BaseItem.type == "9")
                    rarity = "relic";
                else if (Slots[2].BaseItem.item_class == 6 || Slots[2].BaseItem.type == "6")
                    rarity = "";
                string redirectUrl = "";
                HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/search");
                request23.Method = "POST";
                request23.KeepAlive = true;
                request23.ContentType = "application/x-www-form-urlencoded";
                StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());
                postwriter.Write("league=Abyss&type=&base=&name=" + WebUtility.UrlEncode(Slots[2].BaseItem.name) + "&dmg_min=&dmg_max=&aps_min=&aps_max=&crit_min=&crit_max=&dps_min=&dps_max=&edps_min=&edps_max=&pdps_min=&pdps_max=&armour_min=&armour_max=&evasion_min=&evasion_max=&shield_min=&shield_max=&block_min=&block_max=&sockets_min=&sockets_max=&link_min=&link_max=&sockets_r=&sockets_g=&sockets_b=&sockets_w=&linked_r=&linked_g=&linked_b=&linked_w=&rlevel_min=&rlevel_max=&rstr_min=&rstr_max=&rdex_min=&rdex_max=&rint_min=&rint_max=&mod_name=&mod_min=&mod_max=&group_type=And&group_min=&group_max=&group_count=1&q_min=&q_max=&level_min=&level_max=&ilvl_min=&ilvl_max=&rarity=" + rarity + "&seller=&thread=&identified=&corrupted=&online=x&has_buyout=&altart=&capquality=x&buyout_min=&buyout_max=&buyout_currency=&crafted=&enchanted=");
                postwriter.Close();
                using (HttpWebResponse response2 = request23.GetResponse() as HttpWebResponse)
                {
                    System.Diagnostics.Process.Start(response2.ResponseUri.OriginalString);
                }
            }
        }

        private void btnRefreshPoe1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Slots[0].url))
                NinjaPoETradeMethods.ItemExplicitFieldSearch(Slots[0].BaseItem, true);
            else
                NinjaPoETradeMethods.GetLowestPricesForRares(Slots[0]);
            SetSlots(null, null);
        }

        private void btnRefreshPoe2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Slots[1].url))
                NinjaPoETradeMethods.ItemExplicitFieldSearch(Slots[1].BaseItem, true);
            else
                NinjaPoETradeMethods.GetLowestPricesForRares(Slots[1]);
            SetSlots(null, null);
        }

        private void btnRefreshPoe3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Slots[2].url))
                NinjaPoETradeMethods.ItemExplicitFieldSearch(Slots[2].BaseItem, true);
            else
                NinjaPoETradeMethods.GetLowestPricesForRares(Slots[2]);
            SetSlots(null, null);
        }

        private void btnRefreshPoe_Click(object sender, EventArgs e)
        {
            config.LastSaved = DateTime.Now.AddDays(-2);
            SaveConfigAndWatchlist();
        }

        private void btnOverride1_Click(object sender, EventArgs e)
        {
            Slot localslot = Slots[0];
            NinjaItem real = ninjaItems.FirstOrDefault(p => p.type == localslot.BaseItem.type && p.name == localslot.BaseItem.name && p.base_type == localslot.BaseItem.base_type);

            decimal total = localslot.BaseItem.Top5Sells.Sum(p => p);
            int avg = (int)(total / localslot.BaseItem.Top5Sells.Count);
            localslot.BaseItem.chaos_value = avg;
            real.chaos_value = avg;
            SetSlots(null, null);
            SaveConfigAndWatchlist();
        }

        private void btnOverride2_Click(object sender, EventArgs e)
        {
            Slot localslot = Slots[1];
            NinjaItem real = ninjaItems.FirstOrDefault(p => p.type == localslot.BaseItem.type && p.name == localslot.BaseItem.name && p.base_type == localslot.BaseItem.base_type);

            decimal total = localslot.BaseItem.Top5Sells.Sum(p => p);
            int avg = (int)(total / localslot.BaseItem.Top5Sells.Count);
            localslot.BaseItem.chaos_value = avg;
            real.chaos_value = avg;
            SetSlots(null, null);
            SaveConfigAndWatchlist();
        }

        private void btnOverride3_Click(object sender, EventArgs e)
        {
            Slot localslot = Slots[2];
            NinjaItem real = ninjaItems.FirstOrDefault(p => p.type == localslot.BaseItem.type && p.name == localslot.BaseItem.name && p.base_type == localslot.BaseItem.base_type);

            decimal total = localslot.BaseItem.Top5Sells.Sum(p => p);
            int avg = (int)(total / localslot.BaseItem.Top5Sells.Count);
            localslot.BaseItem.chaos_value = avg;
            real.chaos_value = avg;
            SetSlots(null, null);
            SaveConfigAndWatchlist();
        }
        [STAThread]
        private void btnFakeFirstMsg_Click(object sender, EventArgs e)
        {
            if (config.autoCopy)
                if (!string.IsNullOrEmpty(Slots[0].Message))
                {
                    string s = Slots[0].Message;
                    Button btn = (Button)sender;
                    if (btn.Name != "btnFakeFirstMsg")
                        last_time_clicked = DateTime.Now;
                    Clipboard.SetText(s);
                }
        }

        private void btnBlock1_Click(object sender, EventArgs e)
        {
            config.blocked_accounts.Add(Slots[0].account_name);
            SaveConfigAndWatchlist();
        }

        private void btnBlock2_Click(object sender, EventArgs e)
        {
            config.blocked_accounts.Add(Slots[1].account_name);
            SaveConfigAndWatchlist();
        }

        private void btnBlock3_Click(object sender, EventArgs e)
        {
            config.blocked_accounts.Add(Slots[2].account_name);
            SaveConfigAndWatchlist();
        }



    }
}
