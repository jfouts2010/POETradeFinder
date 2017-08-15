
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ItemWatcher2;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ItemWatcher2
{
    public partial class EditCraftables : Form
    {
        public List<POETradeCraftable> craftables;
        public Dictionary<string, string> requiredExplicits = new Dictionary<string, string>();
        public Dictionary<string, string> craftableExplicits = new Dictionary<string, string>();
        public EditCraftables()
        {
            InitializeComponent();
            LoadBasicInfo();
            reload();
        }

        private void EditWatchedRares_Load(object sender, EventArgs e)
        {
            dropDownBaseType.DataSource = Enum.GetValues(typeof(POETradeConfig.BaseType));

            ddlRarity.DataSource = Enum.GetValues(typeof(POETradeConfig.Rarity));
            

            List<string> locExplicits = new List<string>();
            locExplicits.Add("none");
            foreach (FieldInfo field in typeof(POETradeConfig).GetFields())
            {
                if (field.Name.Contains("final"))
                    locExplicits.Add(field.GetValue(typeof(POETradeConfig)).ToString());
            }
            ddlExplicit.DataSource = locExplicits;
        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void btnAddExplicit_Click(object sender, EventArgs e)
        {
            if (ddlExplicit.SelectedValue.ToString() != "none")
                requiredExplicits.Add(ddlExplicit.SelectedValue.ToString(), txtExplicitValue.Text);
            else
                requiredExplicits.Add(ddlExplicit.SelectedValue.ToString(), txtExplicitValue.Text);
            reload();

        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            POETradeCraftable item = new POETradeCraftable()
            {
                aps = txtAps.Text,
                armour = txtArmour.Text,
                baseType = txtBase.Text,
                crit_chance = txtCrit.Text,
                damage = txtDamage.Text,
                dps = txtDPS.Text,
                edps = txtEdps.Text,
                evasion = txtEvasion.Text,
                ilvl = txtILevel.Text,
                level = txtLevel.Text,
                links = txtLinks.Text,
                name = txtName.Text,
                pdps = txtPdps.Text,
                quality = txtQuality.Text,
                shield = txtShield.Text,
                sockets = txtSockets.Text,
            };

            item.corrupted = false;
            item.crafted = false;
            POETradeConfig.Rarity rarity;
            Enum.TryParse<POETradeConfig.Rarity>(ddlRarity.SelectedValue.ToString(), out rarity);
            POETradeConfig.BaseType type;
            Enum.TryParse<POETradeConfig.BaseType>(dropDownBaseType.SelectedValue.ToString(), out type);
            item.type = type;
            foreach (string key in requiredExplicits.Keys)
            {
                item.requiredMods.Add(key, requiredExplicits[key]);
            }
            foreach (string key in craftableExplicits.Keys)
            {
                item.craftableMods.Add(key, craftableExplicits[key]);
            }
            if (string.IsNullOrEmpty(item.armour) && string.IsNullOrEmpty(item.evasion) && string.IsNullOrEmpty(item.shield) && string.IsNullOrEmpty(item.pdps) && string.IsNullOrEmpty(item.damage))
                item.normalize_q = null;
            requiredExplicits.Clear();
            craftableExplicits.Clear();
            craftables.Add(item);
            reload();
        }
        private void reload()
        {
            this.listBoxWatched.Items.Clear();
            this.listBoxCraftable.Items.Clear();
            foreach (POETradeConfig item in craftables)
            {
                this.listBoxWatched.Items.Add(item);
            }
            this.listBoxExplicits.Items.Clear();
            foreach (string key in requiredExplicits.Keys)
            {
                this.listBoxExplicits.Items.Add(key + ":" + requiredExplicits[key]);
            }
            foreach (string key in craftableExplicits.Keys)
            {
                this.listBoxCraftable.Items.Add(key + ":" + craftableExplicits[key]);
            }
        }

        public void LoadBasicInfo()
        {
            try
            {
                craftables = Newtonsoft.Json.JsonConvert.DeserializeObject<List<POETradeCraftable>>(System.IO.File.ReadAllText(FinalVariables.craftablesFileNames));
            }
            catch (Exception e)
            {
                craftables = new List<POETradeCraftable>();
            }
        }

        private void btnDeleteClick(object sender, EventArgs e)
        {
            try
            {
                craftables.RemoveAt(listBoxWatched.SelectedIndex);
                reload();
            }
            catch (Exception eeeee)
            {

            }
        }

        private void btnSaveClick(object sender, EventArgs e)
        {
            string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(craftables);
            JArray ja = JArray.Parse(serialized);
            serialized = ja.ToString();
            System.IO.File.Delete(FinalVariables.craftablesFileNames);
            System.IO.File.WriteAllText(FinalVariables.craftablesFileNames, serialized);
        }

        private void btnAddCraftableAffix_Click(object sender, EventArgs e)
        {
            
            if (ddlExplicit.SelectedValue.ToString() != "none")
                craftableExplicits.Add(ddlExplicit.SelectedValue.ToString(), txtExplicitValue.Text);
            else
                craftableExplicits.Add(ddlExplicit.SelectedValue.ToString(), txtExplicitValue.Text);
            reload();

        }
    }
    
}
