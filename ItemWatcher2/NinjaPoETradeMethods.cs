﻿using Newtonsoft.Json.Linq;
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
using static ItemWatcher2.Form1;

namespace ItemWatcher2
{
    public class NinjaPoETradeMethods
    {

        public static List<string> avaliableExplicits = new List<string>();
        public static List<string> all_base_types = new List<string>();

        public static List<NinjaItem> SetNinjaValues(List<NinjaItem> NinjaItems, System.Windows.Forms.TextBox txtBoxUpdateThread, Boolean do_poetrade_lookup = false)
        {
            List<JObject> Jsons = new List<JObject>();
            List<JObject> Jweps = new List<JObject>();
            Dictionary<string, bool> APIURLS = new Dictionary<string, bool>();
            APIURLS.Add("http://api.poe.ninja/api/Data/GetDivinationCardsOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"), false);
            APIURLS.Add("http://api.poe.ninja/api/Data/GetProphecyOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"), false);
            APIURLS.Add("http://api.poe.ninja/api/Data/GetUniqueMapOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"), false);
            APIURLS.Add("http://api.poe.ninja/api/Data/GetUniqueJewelOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"), false);
            APIURLS.Add("http://api.poe.ninja/api/Data/GetUniqueFlaskOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"), false);
            APIURLS.Add("http://api.poe.ninja/api/Data/GetUniqueWeaponOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"), true);
            APIURLS.Add("http://api.poe.ninja/api/Data/GetUniqueArmourOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"), false);
            APIURLS.Add("http://api.poe.ninja/api/Data/GetUniqueAccessoryOverview?league=Legacy&date=" + DateTime.Now.ToString("YYYY-mm-dd"), false);
            txtBoxUpdateThread.Invoke((MethodInvoker)delegate
            {
                txtBoxUpdateThread.Text = "Doing Ninja Update";
            });
            foreach (string s in APIURLS.Keys)
            {
                HttpWebRequest request2 = WebRequest.Create(s) as HttpWebRequest;
                // Get response  
                using (HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    using (StreamReader reader = new StreamReader(response2.GetResponseStream()))
                    {
                        if (APIURLS[s])
                            Jweps.Add(JObject.Parse(reader.ReadToEnd()));
                        else
                            Jsons.Add(JObject.Parse(reader.ReadToEnd()));
                    }
                }
            }
            foreach (JObject jo in Jsons)
                NinjaItems.AddRange(DoNinjaParsing(jo));
            foreach (JObject jo in Jweps)
                NinjaItems.AddRange(DoNinjaParsing(jo, true));
            string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(all_base_types);
            JArray ja = JArray.Parse(serialized);
            serialized = ja.ToString();
            System.IO.File.Delete("AllBaseTypesStrings.json");
            System.IO.File.WriteAllText("AllBaseTypesStrings.json", serialized);
            if (config.do_all_uniques_with_ranges && do_poetrade_lookup)
            {
                avaliableExplicits = FindPoETradeExplicits();
                config.avaliableExplicits = avaliableExplicits;

                int counter = 0;
                foreach (NinjaItem nj in NinjaItems)
                {
                    txtBoxUpdateThread.Invoke((MethodInvoker)delegate
                    {
                        txtBoxUpdateThread.Text = "Updating Poe.Trade " + counter++ + "/" + NinjaItems.Count;
                    });
                    ItemExplicitFieldSearch(nj);

                    if (nj.is_weapon && allBaseTypes.Count(p => p.base_name == nj.base_type) > 0)
                    {
                        decimal[] minmax = GetMinMaxPdps(nj);
                        nj.minPdps = minmax[0];
                        nj.maxPdps = minmax[1];
                    }
                }
            }
            config.SavedItems = NinjaItems;
            config.LastSaved = DateTime.Now;

            SaveNames();

            return NinjaItems;
        }

        public static int GetDdpsOfLocalWeapon(Item item)//godamn "items"
        {
            WeaponBaseItem baseItem = allBaseTypes.FirstOrDefault(p => p.base_name == item.typeLine);
            if (baseItem == null)
                return 0;
            string phys = item.explicitMods.FirstOrDefault(p => p.Contains("increased Physical Damage"));
            string flatPhys = item.explicitMods.FirstOrDefault(p => p.Contains("Adds") && p.Contains("Physical Damage"));
            string ias = item.explicitMods.FirstOrDefault(p => p.Contains("increased Attack Speed"));

            decimal physD = (GetSingleNumber(phys) ?? 0);
            decimal flatPhysD = (GetMultipleNumbersNullable(flatPhys) ?? 0);
            decimal iasD = (GetSingleNumber(ias) ?? 0);
            decimal TotalDps = (baseItem.pd + flatPhysD) * (1.2M + physD / 100) * (baseItem.aps * (1 + iasD / 100));
            return (int)TotalDps;
        }

        public static void CalcDPSOfAllWeps()
        {
            LoadBasicInfo();
            ninjaItems = config.SavedItems;
            foreach (NinjaItem item in ninjaItems)
            {
                if (item.is_weapon && item.ExplicitFields.Count > 0)
                {
                    decimal[] minmax = GetMinMaxPdps(item);
                    item.minPdps = minmax[0];
                    item.maxPdps = minmax[1];
                }
                else
                {
                    item.minPdps = 0;
                    item.maxPdps = 0;
                }
            }

            SaveNames();
        }
        public static List<NinjaItem> DoNinjaParsing(JObject jo, bool is_wep = false)
        {
            List<NinjaItem> localNinjaItems = new List<NinjaItem>();
            foreach (JObject jo2 in jo.First.First.Children().ToList())
            {
                NinjaItem newNinjaItem = new NinjaItem();
                newNinjaItem.name = jo2.Children().ToList().First(p => p.Path.EndsWith(".name")).First.ToString();
                newNinjaItem.is_weapon = is_wep;
                newNinjaItem.type = jo2.Children().ToList().First(p => p.Path.EndsWith(".itemClass")).First.ToString();
                newNinjaItem.base_type = jo2.Children().ToList().First(p => p.Path.EndsWith(".baseType")).First.ToString();
                if (!all_base_types.Contains(newNinjaItem.base_type) && newNinjaItem.is_weapon)
                    all_base_types.Add(newNinjaItem.base_type);

                newNinjaItem.item_class = Convert.ToInt32(jo2.Children().ToList().First(p => p.Path.EndsWith(".itemClass")).First.ToString());
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
                if (/*newNinjaItem.chaos_value > 20 &&*/ !newNinjaItem.name.Contains("Atziri's Splendour") && !newNinjaItem.name.Contains("Doryani's Invitation") && !newNinjaItem.name.Contains("Vessel of Vinktar") && localNinjaItems.Where(p => p.name == newNinjaItem.name && p.base_type == newNinjaItem.base_type && p.type == newNinjaItem.type).Count() == 0)
                    localNinjaItems.Add(newNinjaItem);
            }
            return localNinjaItems;
        }

        public static WeaponBaseItem FindBaseOnHitAndAttackSpeed(string itemName)
        {
            HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/search");
            request23.Method = "POST";
            request23.KeepAlive = true;
            request23.ContentType = "application/x-www-form-urlencoded";
            string rarity = "normal";
            StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());


            postwriter.Write("league=Legacy&type=&base=" + WebUtility.UrlEncode(itemName) + "&rarity=" + rarity + "&q_max=0");
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
                        if (input.Attributes.Contains("id") && input.Attributes["id"].Value.Contains("item-container-"))
                        {
                            var tds = input.Descendants("td");
                            WeaponBaseItem wep = new WeaponBaseItem();
                            wep.base_name = itemName;
                            foreach (var td in tds)
                            {
                                if (td.Attributes.Contains("data-name"))
                                {
                                    if (td.Attributes["data-name"].Value.Equals("aps"))
                                    {
                                        wep.aps = Convert.ToDecimal(td.Attributes["data-value"].Value);
                                    }
                                    if (td.Attributes["data-name"].Value.Equals("pd"))
                                    {
                                        wep.pd = Convert.ToDecimal(td.Attributes["data-value"].Value);
                                    }
                                }
                            }
                            return wep;
                        }
                    }
                }
            }
            return null;
        }
        public static void ItemExplicitFieldSearch(NinjaItem nj, bool manual = false)
        {

            //lets look for rolls
            if (/*nj.Explicits.Count > 0 && !nj.base_type.Contains("Map")*/ nj.chaos_value > 15 || manual)
            {
                List<ExplicitField> explicitsToCheck = new List<ExplicitField>();
                foreach (string explicitRoll in nj.Explicits)
                {
                    if (explicitRoll.Contains("(") && explicitRoll.Contains("-") && explicitRoll.Contains(")"))
                    {
                        string s = SearchString(explicitRoll);
                        if (!avaliableExplicits.Contains(s))
                            continue;
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
                                    decimal MaxRollsTemp = (decimal)(GetMultipleNumbers(roll.Substring(roll.IndexOf("-") + 1, roll.IndexOf(")") - roll.IndexOf("-"))));
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
                                        MaxRolls = (decimal)(GetMultipleNumbers(roll.Substring(roll.IndexOf("-") + 1, roll.IndexOf(")") - roll.IndexOf("-"))));
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
                                decimal MaxRoll = (int)(GetMultipleNumbers(explicitRoll.Substring(explicitRoll.IndexOf("-") + 1, explicitRoll.IndexOf(")") - explicitRoll.IndexOf("-"))));
                                if (MaxRoll < MinRoll)
                                    MaxRoll = MinRoll;
                                explicitsToCheck.Add(new ExplicitField() { SearchField = s, MinRoll = MinRoll, MaxRoll = MaxRoll });
                            }
                        }
                    }

                }

                string modsMinSearch = "";
                string modsMaxSearch = "";
                string modsMidSearch = "";
                nj.ExplicitFields = explicitsToCheck;
                foreach (ExplicitField ef in explicitsToCheck)
                {
                    if (avaliableExplicits.Where(p => !p.Contains("(total)")).Contains(ef.SearchField))
                    {
                        modsMinSearch += "mod_name=" + WebUtility.UrlEncode(ef.SearchField) + "&mod_min=" + WebUtility.UrlEncode(ef.MinRoll.ToString()) + "&mod_max=&";
                        decimal HighRoll = ((ef.MaxRoll - ef.MinRoll) * 0.9M) + ef.MinRoll;
                        decimal MidRoll = ((ef.MaxRoll - ef.MinRoll) * 0.5M) + ef.MinRoll;
                        modsMaxSearch += "mod_name=" + WebUtility.UrlEncode(ef.SearchField) + "&mod_min=" + WebUtility.UrlEncode((HighRoll).ToString()) + "&mod_max=&";
                        modsMidSearch += "mod_name=" + WebUtility.UrlEncode(ef.SearchField) + "&mod_min=" + WebUtility.UrlEncode((MidRoll).ToString()) + "&mod_max=&";
                    }
                }
                if (explicitsToCheck.Count == 0)
                {
                    modsMinSearch = "mod_name=&mod_min=&mod_max=&";
                    MinSearch(nj, modsMinSearch, explicitsToCheck);
                    nj.HasRolls = false;
                }
                else
                {
                    if (nj.chaos_value > 30)
                    {
                        MinSearch(nj, modsMinSearch, explicitsToCheck);
                        MidSearch(nj, modsMidSearch, explicitsToCheck);
                        MaxSearch(nj, modsMaxSearch, explicitsToCheck);
                        nj.HasRolls = true;
                    }
                    else
                    {
                        MinSearch(nj, modsMinSearch, explicitsToCheck);
                        MaxSearch(nj, modsMaxSearch, explicitsToCheck);
                        nj.HasRolls = true;
                    }
                }

            }
            else
            {
                if (nj.type == "6" && nj.base_type.Contains("Map"))
                {
                    MinSearch(nj, "", new List<ExplicitField>());
                }
            }
            nj.last_updated_poetrade = DateTime.Now;
        }
        public static void MinSearch(NinjaItem nj, string modsMinSearch, List<ExplicitField> explicitsToCheck)
        {
            string rarity = "unique";
            if (nj.type == "9")
                rarity = "relic";
            else if (nj.item_class == 6 || nj.type == "6")
                rarity = "";
            //min search
            decimal MinSell = 0;
            decimal AvrgSellTop5 = 0;

            List<decimal> Top5Prices = new List<decimal>();
            int count = 0;
            bool first = true;
            string redirectUrl = "";
            HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/search");
            request23.Method = "POST";
            request23.KeepAlive = true;
            request23.ContentType = "application/x-www-form-urlencoded";
            StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());
            string name = nj.name;
            if (!string.IsNullOrEmpty(nj.base_type))
                name += " " + nj.base_type;
            postwriter.Write("league=Legacy&type=&base=&name=" + WebUtility.UrlEncode(name) + "&dmg_min=&dmg_max=&aps_min=&aps_max=&crit_min=&crit_max=&dps_min=&dps_max=&edps_min=&edps_max=&pdps_min=&pdps_max=&armour_min=&armour_max=&evasion_min=&evasion_max=&shield_min=&shield_max=&block_min=&block_max=&sockets_min=&sockets_max=&link_min=&link_max=&sockets_r=&sockets_g=&sockets_b=&sockets_w=&linked_r=&linked_g=&linked_b=&linked_w=&rlevel_min=&rlevel_max=&rstr_min=&rstr_max=&rdex_min=&rdex_max=&rint_min=&rint_max=&mod_name=&mod_min=&mod_max=&group_type=And&group_min=&group_max=&group_count=0&q_min=&q_max=&level_min=&level_max=&ilvl_min=&ilvl_max=&rarity=" + rarity + "&seller=&thread=&identified=&corrupted=&online=x&has_buyout=&altart=&capquality=x&buyout_min=&buyout_max=&buyout_currency=&crafted=&enchanted=");
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

                                    Top5Prices.Add(MinSell);
                                    first = false;
                                }
                                else
                                {
                                    Top5Prices.Add(input.Attributes["data-buyout"].Value.Contains("exalted") ? (GetMultipleNumbers(input.Attributes["data-buyout"].Value) * config.exalt_ratio) : GetMultipleNumbers(input.Attributes["data-buyout"].Value));
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

            nj.Top5Sells = Top5Prices.OrderBy(p => p).ToList();
        }

        public static decimal[] GetMinMaxPdps(NinjaItem item)
        {
            if (item.name.Contains("Opus"))
            {
                int x = 5;
            }
            ExplicitField phys = item.ExplicitFields.FirstOrDefault(p => p.SearchField == "#% increased Physical Damage");
            ExplicitField flatPhys = item.ExplicitFields.FirstOrDefault(p => p.SearchField == "Adds # to # Physical Damage");
            ExplicitField ias = item.ExplicitFields.FirstOrDefault(p => p.SearchField == "#% increased Attack Speed");
            decimal minPhysPRoll = (phys?.MinRoll ?? 0);
            decimal maxPhysPRoll = (phys?.MaxRoll ?? 0);
            decimal minPhysARoll = (flatPhys?.MinRoll ?? 0);
            decimal maxPhysARoll = (flatPhys?.MaxRoll ?? 0);
            decimal minIASRoll = (ias?.MinRoll ?? 0);
            decimal maxIASRoll = (ias?.MaxRoll ?? 0);
            if (phys == null)
            {
                string physString = item.Explicits.FirstOrDefault(p => p.Contains("increased Physical Damage"));
                minPhysPRoll = maxPhysPRoll = (GetSingleNumber(physString) ?? 0);
            }
            if (flatPhys == null)
            {

                string flatPhysString = item.Explicits.FirstOrDefault(p => p.Contains("Adds") && p.Contains("Physical Damage"));
                if (!string.IsNullOrEmpty(flatPhysString))
                    minPhysARoll = maxPhysARoll = (GetMultipleNumbersNullable(flatPhysString) ?? 0);

            }
            if (ias == null)
            {
                string iasString = item.Explicits.FirstOrDefault(p => p.Contains("increased Attack Speed"));
                if (!string.IsNullOrEmpty(iasString))
                    minIASRoll = maxIASRoll = (GetSingleNumber(iasString) ?? 0);
            }

            WeaponBaseItem baseItem = allBaseTypes.FirstOrDefault(p => p.base_name == item.base_type);
            if (baseItem == null)
                return new decimal[] { 0, 0 };
            decimal minTotalDps = (baseItem.pd + minPhysARoll) * (1.2M + minPhysPRoll / 100) * (baseItem.aps) * (1 + minIASRoll / 100);
            decimal maxTotalDps = (baseItem.pd + maxPhysARoll) * (1.2M + maxPhysPRoll / 100) * (baseItem.aps) * (1 + maxIASRoll / 100);
            return new decimal[] { minTotalDps, maxTotalDps };
        }


        public static void MaxSearch(NinjaItem nj, string modsMaxSearch, List<ExplicitField> explicitsToCheck)
        {
            string rarity = "unique";
            if (nj.type == "9")
                rarity = "relic";
            else if (nj.item_class == 6 || nj.type == "6")
                rarity = "";
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
            string name = nj.name;
            if (!string.IsNullOrEmpty(nj.base_type))
                name += " " + nj.base_type;
            postwriter.Write("league=Legacy&type=&base=&name=" + WebUtility.UrlEncode(name) + "&dmg_min=&dmg_max=&aps_min=&aps_max=&crit_min=&crit_max=&dps_min=&dps_max=&edps_min=&edps_max=&pdps_min=&pdps_max=&armour_min=&armour_max=&evasion_min=&evasion_max=&shield_min=&shield_max=&block_min=&block_max=&sockets_min=&sockets_max=&link_min=&link_max=&sockets_r=&sockets_g=&sockets_b=&sockets_w=&linked_r=&linked_g=&linked_b=&linked_w=&rlevel_min=&rlevel_max=&rstr_min=&rstr_max=&rdex_min=&rdex_max=&rint_min=&rint_max=&" + modsMaxSearch + "group_type=And&group_min=&group_max=&group_count=" + explicitsToCheck.Count().ToString() + "&q_min=&q_max=&level_min=&level_max=&ilvl_min=&ilvl_max=&rarity=" + rarity + "&seller=&thread=&identified=&corrupted=&online=x&has_buyout=&altart=&capquality=x&buyout_min=&buyout_max=&buyout_currency=&crafted=&enchanted=");
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
                        if (input.Attributes.Contains("id") && input.Attributes["id"].Value.Contains("item-container-") && count < 5 && (input.Attributes["data-buyout"].Value.Contains("chaos") || input.Attributes["data-buyout"].Value.Contains("exalted")))
                        {
                            if (first)
                            {
                                HighRollMinSell = input.Attributes["data-buyout"].Value.Contains("exalted") ? GetMultipleNumbers(input.Attributes["data-buyout"].Value) * config.exalt_ratio : GetMultipleNumbers(input.Attributes["data-buyout"].Value);
                                HighRollAvrgSell += HighRollMinSell;
                                first = false;
                            }
                            else
                            {
                                HighRollAvrgSell += input.Attributes["data-buyout"].Value.Contains("exalted") ? GetMultipleNumbers(input.Attributes["data-buyout"].Value) * config.exalt_ratio : GetMultipleNumbers(input.Attributes["data-buyout"].Value);
                            }
                            count++;
                        }

                    }
                }
                if (count > 0)
                    HighRollAvrgSell = HighRollAvrgSell / count;
            }
            nj.HighRollMinSell = HighRollMinSell;
            nj.HighRollAvrgSell = HighRollAvrgSell;
        }
        public static void MidSearch(NinjaItem nj, string modsMidSearch, List<ExplicitField> explicitsToCheck)
        {
            string rarity = "unique";
            if (nj.type == "9")
                rarity = "relic";
            else if (nj.item_class == 6 || nj.type == "6")
                rarity = "";
            //max search
            decimal MidLowSell = 0;
            decimal MidAvrgSell = 0;
            int count = 0;
            bool first = true;
            string redirectUrl = "";
            HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/search");
            request23.Method = "POST";
            request23.KeepAlive = true;
            request23.ContentType = "application/x-www-form-urlencoded";
            StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());
            string name = nj.name;
            if (!string.IsNullOrEmpty(nj.base_type))
                name += " " + nj.base_type;
            postwriter.Write("league=Legacy&type=&base=&name=" + WebUtility.UrlEncode(name) + "&dmg_min=&dmg_max=&aps_min=&aps_max=&crit_min=&crit_max=&dps_min=&dps_max=&edps_min=&edps_max=&pdps_min=&pdps_max=&armour_min=&armour_max=&evasion_min=&evasion_max=&shield_min=&shield_max=&block_min=&block_max=&sockets_min=&sockets_max=&link_min=&link_max=&sockets_r=&sockets_g=&sockets_b=&sockets_w=&linked_r=&linked_g=&linked_b=&linked_w=&rlevel_min=&rlevel_max=&rstr_min=&rstr_max=&rdex_min=&rdex_max=&rint_min=&rint_max=&" + modsMidSearch + "group_type=And&group_min=&group_max=&group_count=" + explicitsToCheck.Count().ToString() + "&q_min=&q_max=&level_min=&level_max=&ilvl_min=&ilvl_max=&rarity=" + rarity + "&seller=&thread=&identified=&corrupted=&online=x&has_buyout=&altart=&capquality=x&buyout_min=&buyout_max=&buyout_currency=&crafted=&enchanted=");
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
                        if (input.Attributes.Contains("id") && input.Attributes["id"].Value.Contains("item-container-") && count < 5 && (input.Attributes["data-buyout"].Value.Contains("chaos") || input.Attributes["data-buyout"].Value.Contains("exalted")))
                        {
                            if (first)
                            {
                                MidLowSell = input.Attributes["data-buyout"].Value.Contains("exalted") ? GetMultipleNumbers(input.Attributes["data-buyout"].Value) * config.exalt_ratio : GetMultipleNumbers(input.Attributes["data-buyout"].Value);
                                MidAvrgSell += MidLowSell;
                                first = false;
                            }
                            else
                            {
                                MidAvrgSell += input.Attributes["data-buyout"].Value.Contains("exalted") ? GetMultipleNumbers(input.Attributes["data-buyout"].Value) * config.exalt_ratio : GetMultipleNumbers(input.Attributes["data-buyout"].Value);
                            }
                            count++;
                        }

                    }
                }
                if (count > 0)
                    MidAvrgSell = MidAvrgSell / count;
            }
            nj.MidLowSell = MidLowSell;
            nj.MidAvrgSell = MidAvrgSell;
        }
        public static List<string> LoadAllBaseWeaponTypes()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(System.IO.File.ReadAllText("AllBaseTypesStrings.json"));
        }
        public static void SaveAllBaseWeaponTypes(List<string> allWepBases)
        {
            string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(allWepBases);
            JArray ja = JArray.Parse(serialized);
            serialized = ja.ToString();
            System.IO.File.Delete(itemfilename);
            System.IO.File.WriteAllText("AllBaseTypesStrings.json", serialized);
        }

        public static List<int> GetPoeLowest5Prices(POETradeConfig tradeConfig)
        {
            HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/search");
            request23.Method = "POST";
            request23.KeepAlive = true;
            request23.ContentType = "application/x-www-form-urlencoded";
            StreamWriter postwriter = new StreamWriter(request23.GetRequestStream());
            StringBuilder sb = new StringBuilder();
            sb.Append("league=" + tradeConfig.league);
            sb.Append("&online=x");
            if (!string.IsNullOrEmpty(tradeConfig.aps))
                sb.Append("&aps_min=" + tradeConfig.aps);
            if (tradeConfig.type.HasValue)
                sb.Append("&type=" + tradeConfig.type.ToString().Replace('_',' '));
            if (!string.IsNullOrEmpty(tradeConfig.armour))
                sb.Append("&armour_min=" + tradeConfig.armour);

            if (!string.IsNullOrEmpty(tradeConfig.crit_chance))
                sb.Append("&crit_min=" + tradeConfig.crit_chance);
            if (!string.IsNullOrEmpty(tradeConfig.damage))
                sb.Append("&dmg_min=" + tradeConfig.damage);
            if (!string.IsNullOrEmpty(tradeConfig.dps))
                sb.Append("&dps_min=" + tradeConfig.dps);
            if (!string.IsNullOrEmpty(tradeConfig.edps))
                sb.Append("&edps_min=" + tradeConfig.edps);

            if (!string.IsNullOrEmpty(tradeConfig.evasion))
                sb.Append("&evasion_min=" + tradeConfig.evasion);
            if (!string.IsNullOrEmpty(tradeConfig.ilvl))
                sb.Append("&ilvl_min=" + tradeConfig.ilvl);
            if (!string.IsNullOrEmpty(tradeConfig.level))
                sb.Append("&level_min=" + tradeConfig.level);
            if (!string.IsNullOrEmpty(tradeConfig.links))
                sb.Append("&link_min=" + tradeConfig.links);

            if (!string.IsNullOrEmpty(tradeConfig.pdps))
                sb.Append("&pdps_min=" + tradeConfig.pdps);
            if (!string.IsNullOrEmpty(tradeConfig.quality))
                sb.Append("&q_min=" + tradeConfig.quality);

            if (!string.IsNullOrEmpty(tradeConfig.shield))
                sb.Append("&shield_min=" + tradeConfig.shield);
            if (!string.IsNullOrEmpty(tradeConfig.sockets))
                sb.Append("&sockets_min=" + tradeConfig.sockets);

            //Bools
            if (tradeConfig.corrupted.HasValue)
                sb.Append("&corrupted=" + Convert.ToInt32(tradeConfig.corrupted));
            if (tradeConfig.crafted.HasValue)
                sb.Append("&crafted=" + Convert.ToInt32(tradeConfig.crafted));
            if (tradeConfig.normalize_q)
                sb.Append("&capquality=x");
            if (tradeConfig.enchanted.HasValue)
                sb.Append("&enchanted=" + Convert.ToInt32(tradeConfig.enchanted));
           

            //Mods

            foreach (string mod in tradeConfig.mods.Keys)
            {
                sb.Append("&mod_name=" + WebUtility.UrlEncode(mod));
                
                sb.Append("&mod_min=" + tradeConfig.mods[mod]);
                sb.Append("&mod_max=");
            }
            sb.Append("&group_type=And");
            sb.Append("&group_min=");
            sb.Append("&group_max=");
            sb.Append("&group_count=" + tradeConfig.mods.Count);

            int count = 0;
            List<decimal> Top5Prices = new List<decimal>();
            postwriter.Write(sb.ToString());
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
                        if (input.Attributes.Contains("id") && input.Attributes["id"].Value.Contains("item-container-") && count < 5 && (input.Attributes["data-buyout"].Value.Contains("chaos") || input.Attributes["data-buyout"].Value.Contains("exalted")))
                        {
                            Top5Prices.Add(input.Attributes["data-buyout"].Value.Contains("exalted") ? (GetMultipleNumbers(input.Attributes["data-buyout"].Value) * config.exalt_ratio) : GetMultipleNumbers(input.Attributes["data-buyout"].Value));
                            count++;
                        }

                    }
                }
            }
            return Top5Prices.ConvertAll(p => Convert.ToInt32(p)).ToList();
        }

        public static List<string> FindPoETradeExplicits()
        {
            List<string> avaliableExplicits = new List<string>();
            HttpWebRequest request23 = (HttpWebRequest)HttpWebRequest.Create("http://poe.trade/");
            using (HttpWebResponse response2 = request23.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                using (StreamReader reader = new StreamReader(response2.GetResponseStream()))
                {
                    string s = reader.ReadToEnd();
                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(s);
                    var inputs = htmlDoc.DocumentNode.Descendants("div");

                    foreach (var input in inputs)
                    {
                        if (input.Attributes.Contains("class") && input.Attributes["class"].Value == "row explicit")
                        {
                            List<string> Explicits = input.InnerText.Split(new string[] { "  " }, StringSplitOptions.None).ToList();

                            foreach (string explicitString in Explicits)
                            {
                                if (!explicitString.Contains("(enchant)") && !explicitString.Contains("(implicit)"))
                                {
                                    avaliableExplicits.Add(explicitString.Replace("(crafted)", ""));
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return avaliableExplicits;
        }
        public static string SearchString(string explicitString)
        {
            string s = Regex.Replace(explicitString, @"\d", "#").Replace("(", "").Replace(")", "").Replace("-", "");
            return s.Replace("####", "#").Replace("###", "#").Replace("##", "#").Replace("#.#", "#");
        }
    }
}
