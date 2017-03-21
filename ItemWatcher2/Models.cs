using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemWatcher2
{
    public class POETradeConfig
    {
        public POETradeConfig()
        {
            league = "Legacy";
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
        public string league { get; set; }
        public string name { get; set; }
        public BaseType? type { get; set; }
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
        public Dictionary<string,string> mods { get; set; }
        public string quality { get; set; }
        public string level { get; set; }
        public string ilvl { get; set; }
        public Rarity rarity { get; set; }
        public bool? corrupted { get; set; }
        public bool normalize_q { get; set; }
        public bool? crafted { get; set; }
        public bool? enchanted { get; set; }


        public enum Rarity
        {
            normal=0,
             magic=1,
             rare=2,
             unique=3,
             relic=9
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
            Amulet = 17,
            Belt = 18,
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
            Map_Fragments = 32,
        }

        public static readonly string TotalResString = "(pseudo) +#% total Elemental Resistance";
        public static readonly string Life = "(pseudo) (total) +# to maximum Life";
        public static readonly string EnergyShieldFlat = "(pseudo) (total) +# to maximum Emergy Shield";
        public static readonly string MovementSpeed = "#% increased Movement Speed";

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
        public int pdps { get; set; }

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
