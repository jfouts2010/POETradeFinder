using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ItemWatcher2
{
    public class FinalVariables
    {
        public static string rareFileName = "rareWatcheItems.json";
        public static string itemfilename = "SavedItems.json";
        public static string currencyfilename = "SavedCurrencies.json";
        public static string baseTypesStringFilename = "AllBaseTypesStrings.json";
        public static string wepBaseTypesFile = "AllBaseTypes.json";
        public static string configfile = "Config.json";
    }


    public class POETradeConfig
    {
        public POETradeConfig()
        {
            league = "Harbinger";
            normalize_q = true;
            mods = new Dictionary<string, string>();
        }

        public POETradeConfig(NinjaItem uniqueItem) : this()
        {
            if (!string.IsNullOrEmpty(uniqueItem.base_type))
                name += " " + uniqueItem.base_type;
            else
                name = uniqueItem.name;
        }
        public NinjaItem CreateNinja()
        {
            NinjaItem nj = new NinjaItem()
            {
                base_type = type.ToString(),
                chaos_value = estimated_value,
                Explicits = mods.Keys.ToList(),
                name = "",
                tradeConfig = this

            };
            return nj;
        }

        public string league { get; set; }
        public string name { get; set; }
        public BaseType type { get; set; }
        public string baseType { get; set; }
        public string damage { get; set; }
        public string aps { get; set; }
        public string crit_chance { get; set; }
        public string dps { get; set; }
        public string edps { get; set; }
        public string pdps { get; set; }
        public string armour { get; set; }
        public string evasion { get; set; }
        public string shield { get; set; }
        public string sockets { get; set; }
        public string links { get; set; }
        public Dictionary<string, string> mods { get; set; }
        public string quality { get; set; }
        public string level { get; set; }
        public string ilvl { get; set; }
        public Rarity rarity { get; set; }
        public bool? corrupted { get; set; }
        public bool? normalize_q { get; set; }
        public bool? crafted { get; set; }
        public bool? enchanted { get; set; }

        //Calculated later
        public decimal estimated_value { get; set; }
        public string url { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Base:" + this.type.ToString());
            foreach (PropertyInfo p in typeof(POETradeConfig).GetProperties().OrderBy(asd => asd.Name))
            {
                try
                {

                    if (p.Name != "mods" && p.Name != "rarity" && p.Name != "type" && !string.IsNullOrEmpty(p.GetValue(this).ToString()))
                        sb.Append("  " + p.Name + ":" + p.GetValue(this).ToString());
                    if (p.Name == "mods")
                    {
                        Dictionary<string, string> mods = (Dictionary<string, string>)p.GetValue(this);
                        foreach (string key in mods.Keys)
                        {
                            sb.Append(" " + key.Replace("#", mods[key]));
                        }
                    }
                }
                catch (Exception e)
                {
                    int x = 5;
                }
            }
            return sb.ToString();
        }


        public enum Rarity
        {
            none = 0,
            normal = 4,
            magic = 1,
            rare = 2,
            unique = 3,
            relic = 9
        }
        public enum BaseType
        {

            Bow = 0,
            Claw = 1,
            Dagger = 2,
            One_Hand_Axe = 3,
            One_Hand_Sword = 4,
            One_Hand_Mace = 5,
            Sceptre = 6,
            Staff = 7,
            Two_Hand_Axe = 8,
            Two_Hand_Mace = 9,
            Two_Hand_Sword = 11,
            Wand = 10,
            Body_Armour = 12,
            Boots = 13,
            Gloves = 14,
            Helmet = 15,
            Shield = 16,

            Breach = 19,
            Currency = 20,
            Divination_Card = 21,
            Essence = 22,
            Fishing_Rods = 23,
            Flask = 24,
            Gem = 25,
            Jewel = 26,
            Leaguestone = 27,
            Map = 28,
            Prophecy = 29,
            Quiver = 30,
            Ring = 31,
            Amulet = 33,
            Belt = 34,
            Map_Fragments = 32,
        }

        public static readonly string final_TotalResString = "(pseudo) +#% total Elemental Resistance";
        public static readonly string final_Life = "(pseudo) (total) +# to maximum Life";
        public static readonly string final_EnergyShieldFlat = "(pseudo) (total) +# to maximum Emergy Shield";
        public static readonly string final_MovementSpeed = "#% increased Movement Speed";
        public static readonly string final_Strength = "(pseudo) (total) +# to Strength";
        public static readonly string final_Intelligence = "(pseudo) (total) +# to Intelligence";
        public static readonly string final_Dexterity = "(pseudo) (total) +# to Dexterity";
        public static readonly string final_WED = "(pseudo) (total) #% increased Elemental Damage with Weapons";
        public static readonly string final_FireRes = "(pseudo) (total) +#% to Fire Resistance";
        public static readonly string final_ColdRes = "(pseudo) (total) +#% to Cold Resistance";
        public static readonly string final_LightningRes = "(pseudo) (total) +#% to Lightning Resistance";

        public static bool SeeIfItemMatchesRare(POETradeConfig conf, Item itemProp, Dictionary<string, string> baseStrings)
        {
            try
            {
                if (itemProp.frameType >= 4 && itemProp.frameType != 9)
                    return false;
                if (conf.rarity != POETradeConfig.Rarity.none && (int)conf.rarity != itemProp.frameType)
                    return false;
                string baseType = "";
                if (itemProp.properties != null && itemProp.properties.First()["type"] == null)
                    baseType = itemProp.properties.First()["name"].ToString();
                string typeLine = itemProp.typeLine;

                typeLine = typeLine.Replace("Superior ", "");
                if (itemProp.frameType == 1)
                {
                    
                    typeLine = typeLine.Split(new string[] { " of" }, StringSplitOptions.None)[0];
                    if(typeLine.Contains("Map"))
                    {
                        int x = 5;
                    }
                    if (typeLine.Split(' ').Count() >= 3)
                    {
                        typeLine = typeLine.Substring(typeLine.IndexOf(' ')).Trim();
                    }
                }
                if (!baseStrings.ContainsKey(typeLine) || baseStrings.Where(p => typeLine.Contains(typeLine)).Count() == 0)
                {
                    if (!string.IsNullOrEmpty(baseType))
                    {
                        if (typeLine.ToLower().Contains("leaguestone"))
                            return false;
                        baseStrings.Add(typeLine, "Weapon");
                        NinjaPoETradeMethods.SaveBaseStrings(baseStrings);
                    }
                    else
                    {
                        string[] halves = typeLine.Split(' ');
                        if (halves.Count() > 1)
                        {
                            List<string> allKeys = baseStrings.Keys.ToList();
                            bool found = false;
                            foreach (string key in allKeys)
                            {
                                if (halves.Last() == key.Split(' ').Last())
                                {
                                    string value = baseStrings[key];

                                    baseStrings.Add(typeLine, value);
                                    NinjaPoETradeMethods.SaveBaseStrings(baseStrings);
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                string last_try = NinjaPoETradeMethods.FindBaseType(typeLine);
                                if (last_try == "idk")
                                {
                                    baseStrings.Add(typeLine, "IDK");
                                    NinjaPoETradeMethods.SaveBaseStrings(baseStrings);
                                    return false;
                                }
                                else
                                {
                                    baseStrings.Add(typeLine, last_try);
                                    NinjaPoETradeMethods.SaveBaseStrings(baseStrings);
                                }
                            }
                        }
                        else
                            return false;
                    }
                }
                string generalBase = "";
                try
                {
                    generalBase = baseStrings[typeLine];
                }
                catch
                {
                    generalBase = baseStrings.FirstOrDefault(p => typeLine.Contains(p.Key)).Value;
                }

                if (generalBase == "Weapon")
                    if (itemProp.properties.First()["type"] == null)
                        baseType = itemProp.properties.First()["name"].ToString();
                    else
                        return false;
                if (!string.IsNullOrEmpty(baseType))
                {
                    if (baseType.ToLower() != conf.type.ToString().ToLower())
                        return false;
                }
                else
                {
                    if (conf.type.ToString() != generalBase)
                        return false;
                }
                if (!string.IsNullOrEmpty(conf.aps))
                {
                    JToken aps = itemProp.properties.FirstOrDefault(p => p["name"].ToString() == "Attacks per Second");
                    JToken aps2 = aps.Children().FirstOrDefault();
                    JToken values = aps2.Next;
                    JToken SAPS = values.Children().FirstOrDefault().FirstOrDefault()[0];
                    if (Convert.ToDecimal(SAPS.ToString()) < Convert.ToDecimal(conf.aps))
                        return false;
                }
                if (!string.IsNullOrEmpty(conf.armour))
                {

                }
                if (!string.IsNullOrEmpty(conf.baseType))
                {

                }
                if (!string.IsNullOrEmpty(conf.crit_chance))
                {
                    JToken aps = itemProp.properties.FirstOrDefault(p => p["name"].ToString() == "Critical Strike Chance");
                    JToken aps2 = aps.Children().FirstOrDefault();
                    JToken values = aps2.Next;
                    JToken SAPS = values.Children().FirstOrDefault().FirstOrDefault()[0];
                    if (Convert.ToDecimal(SAPS.ToString().Replace("%", "")) < Convert.ToDecimal(conf.crit_chance))
                        return false;
                }
                if (!string.IsNullOrEmpty(conf.evasion))
                {

                }
                if (!string.IsNullOrEmpty(conf.ilvl))
                {

                }
                if (!string.IsNullOrEmpty(conf.level))
                {

                }
                if (!string.IsNullOrEmpty(conf.links))
                {

                }
                if (!string.IsNullOrEmpty(conf.name))
                {

                }
                if (!string.IsNullOrEmpty(conf.pdps))
                {
                    decimal pdps = NinjaPoETradeMethods.GetDdpsOfLocalWeapon(itemProp);

                    if (pdps < Convert.ToDecimal(conf.pdps))
                        return false;
                }
                if (!string.IsNullOrEmpty(conf.quality))
                {

                }
                if (!string.IsNullOrEmpty(conf.shield))
                {

                }
                if (!string.IsNullOrEmpty(conf.sockets))
                {

                }
                if (conf.corrupted.HasValue)
                {
                    if (itemProp.corrupted != conf.corrupted.Value)
                        return false;
                }
                if (conf.mods.Count > 0)
                {
                    Dictionary<string, decimal> initialMods = new Dictionary<string, decimal>();
                    Dictionary<string, decimal> FinalMods = new Dictionary<string, decimal>();
                    string coldres = "+ to cold resistance";
                    string fireres = "+ to fire resistance";
                    string lightningres = "+ to lightning resistance";
                    foreach (string ex in itemProp.explicitMods)
                    {
                        if (ex.ToLower().Contains("(calc"))
                            continue;
                        string key = ConvertToCommonForm(ex);
                        decimal value = GetValueOrAvgValue(ex);
                        initialMods.Add(key, value);
                    }
                    foreach (string ex in itemProp.implicitMods)
                    {
                        if (string.IsNullOrEmpty(ex))
                            continue;
                        string key = ConvertToCommonForm(ex);
                        decimal value = GetValueOrAvgValue(ex);

                        if (initialMods.ContainsKey(key))
                            initialMods[key] += value;
                        else
                            initialMods.Add(key, value);
                    }
                    foreach (string key in initialMods.Keys)
                        FinalMods.Add(key, initialMods[key]);
                    foreach (string expl in initialMods.Keys)
                    {

                        if (expl == "+ to all elemental resistances")
                        {

                            if (!FinalMods.ContainsKey(coldres))
                                FinalMods.Add(coldres, initialMods[expl]);
                            else
                                FinalMods[coldres] += initialMods[expl];
                            if (!FinalMods.ContainsKey(fireres))
                                FinalMods.Add(fireres, initialMods[expl]);
                            else
                                FinalMods[fireres] += initialMods[expl];
                            if (!FinalMods.ContainsKey(lightningres))
                                FinalMods.Add(lightningres, initialMods[expl]);
                            else
                                FinalMods[lightningres] += initialMods[expl];
                        }
                        else if (expl == "+ to fire and lightning resistances")
                        {

                            if (!FinalMods.ContainsKey(fireres))
                                FinalMods.Add(fireres, 15);
                            else
                                FinalMods[fireres] += 15;
                            if (!FinalMods.ContainsKey(lightningres))
                                FinalMods.Add(lightningres, 15);
                            else
                                FinalMods[lightningres] += 15;
                        }
                        else if (expl == "+ to cold and lightning resistances")
                        {
                            if (!FinalMods.ContainsKey(coldres))
                                FinalMods.Add(coldres, 15);
                            else
                                FinalMods[coldres] += 15;
                            if (!FinalMods.ContainsKey(lightningres))
                                FinalMods.Add(lightningres, 15);
                            else
                                FinalMods[lightningres] += 15;
                        }
                        else if (expl == "+ to fire and cold resistances")
                        {
                            if (!FinalMods.ContainsKey(coldres))
                                FinalMods.Add(coldres, 15);
                            else
                                FinalMods[coldres] += 15;
                            if (!FinalMods.ContainsKey(fireres))
                                FinalMods.Add(fireres, 15);
                            else
                                FinalMods[fireres] += 15;
                        }
                    }
                    foreach (string mod in conf.mods.Keys)
                    {
                        if (mod == POETradeConfig.final_TotalResString)
                        {
                            decimal totalRes = 0;
                            if (FinalMods.ContainsKey(coldres))
                                totalRes += FinalMods[coldres];
                            if (FinalMods.ContainsKey(fireres))
                                totalRes += FinalMods[fireres];
                            if (FinalMods.ContainsKey(lightningres))
                                totalRes += FinalMods[lightningres];
                            if (totalRes < Convert.ToDecimal(conf.mods[mod]))
                                return false;
                            else
                            {
                                string[] temparray = new string[itemProp.explicitMods.Count() + 1];
                                for (int i = 0; i < itemProp.explicitMods.Count(); i++)
                                    temparray[i] = itemProp.explicitMods[i];
                                temparray[itemProp.explicitMods.Count()] = "(Calc) Total Ele Res: " + totalRes;
                                itemProp.explicitMods = temparray;
                            }
                        }
                        else
                        {
                            string field = ConvertToCommonForm(mod);
                            if (FinalMods.ContainsKey(field))
                            {
                                decimal value = Convert.ToDecimal(FinalMods[field]);
                                if (value < Convert.ToDecimal(conf.mods[mod]))
                                    return false;
                            }
                            else
                                return false;

                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {

                int x = 5;
                return false;
            }
        }
        public static string ConvertToCommonForm(string input)
        {
            input = input.Replace("(pseudo)", "").Replace("(total)", "").Replace("%", "").Replace("#", "").ToLower().Trim();
            return (new string(input.Where(c => !char.IsDigit(c) && c != '.' && c != '%').ToArray()).ToLower().Trim());
        }
        public static decimal GetValueOrAvgValue(string input)
        {
            string value = new string(input.Where(c => char.IsDigit(c) || c == ' ' || c == '.').ToArray()).Trim();
            if (string.IsNullOrEmpty(value))
                return 0;
            if (value.Contains(" "))
            {
                decimal total = 0;
                foreach (string s in value.Split(' '))
                    try { total += Convert.ToDecimal(s); } catch (Exception e) { }
                return (total / 2);
            }
            else
                return (Convert.ToDecimal(value));
        }
    }

    [Serializable]
    public class NinjaItem
    {
        public string name { get; set; }
        public NinjaItem()
        {
            Implicits = new List<string>();
            Explicits = new List<string>();
        }
        public int item_class { get; set; }

        public decimal chaos_value { get; set; }
        public string type { get; set; }
        public string base_type { get; set; }
        public List<string> Implicits { get; set; }
        public List<string> Explicits { get; set; }
        public decimal MinSell { get; set; }
        public decimal MinAverage { get; set; }
        public decimal HighRollMinSell { get; set; }
        public decimal HighRollAvrgSell { get; set; }
        public decimal MidLowSell { get; set; }
        public decimal MidAvrgSell { get; set; }
        public List<decimal> Top5Sells { get; set; }
        public bool HasRolls { get; set; }
        public bool UseNinjaPrice = true;
        public DateTime? last_updated_poetrade { get; set; }
        public decimal poetrade_chaos_value { get; set; }
        public List<ExplicitField> ExplicitFields = new List<ExplicitField>();
        public bool is_weapon { get; set; }
        public decimal minPdps { get; set; }
        public decimal maxPdps { get; set; }
        public POETradeConfig tradeConfig { get; set; }

        public override string ToString()
        {
            return name + " : " + chaos_value;
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
        public string account_name { get; set; }
        public string url { get; set; }
    }
    public class Item
    {
        public string verified { get; set; }
        public string w { get; set; }
        public string h { get; set; }
        public string ilvl { get; set; }
        public string league { get; set; }
        public string id { get; set; }
        public string[] enchantMods { get; set; }
        public JArray sockets { get; set; }
        public int Links
        {
            get
            {
                int count = 0;
                foreach (JObject jo in sockets.Children())
                {
                    count++;
                    if (jo["group"].ToString() != "0")
                        return count;
                }
                return count;
            }
        }
        public string name { get; set; }
        public string typeLine { get; set; }
        public string identified { get; set; }
        public Boolean corrupted { get; set; }
        public string note { get; set; }
        public int frameType { get; set; }
        public string inventoryId { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        //public string requirements { get; set; }
        //public List<Dictionary<string,string>> properties { get; set; }
        public string[] implicitMods { get; set; }
        public string[] explicitMods { get; set; }
        public int pdps { get; set; }
        public JArray properties { get; set; }
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

        public bool do_watch_rares { get; set; }
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

        public string account_name { get; set; }

        public List<NinjaItem> SavedItems { get; set; }
        public List<string> avaliableExplicits { get; set; }
        public DateTime LastSaved { get; set; }
        public double refresh_minutes { get; set; }
        public bool refresh_items { get; set; }
        public bool update_ninja_when_manul_refresh { get; set; }
        public List<string> blocked_accounts { get; set; }
        public bool autoCopy { get; set; }
    }

    public class NotChaosCurrencyConversion
    {
        public string name { get; set; }
        public decimal valueInChaos { get; set; }
    }
    public class ExplicitField
    {
        public string SearchField { get; set; }
        public decimal MinRoll { get; set; }
        public decimal MaxRoll { get; set; }
    }
    public class WeaponBaseItem
    {
        public string base_name { get; set; }
        public decimal pd { get; set; }
        public decimal aps { get; set; }
        public override string ToString()
        {
            return this.base_name;
        }
    }
}
