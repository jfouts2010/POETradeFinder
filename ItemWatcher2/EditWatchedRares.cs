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
    public partial class EditWatchedRares : Form
    {
        public List<POETradeConfig> watchedRares;
        public Dictionary<string, string> explicits = new Dictionary<string, string>();
        public EditWatchedRares()
        {
            InitializeComponent();
            LoadBasicInfo();
            reload();
        }

        private void EditWatchedRares_Load(object sender, EventArgs e)
        {
            dropDownBaseType.DataSource = Enum.GetValues(typeof(POETradeConfig.BaseType));

            ddlRarity.DataSource = Enum.GetValues(typeof(POETradeConfig.Rarity));
            ddlCorrupted.Items.Add("Either");
            ddlCorrupted.Items.Add("Yes");
            ddlCorrupted.Items.Add("No");
            
            ddlCrafted.Items.Add("Either");
            ddlCrafted.Items.Add("Yes");
            ddlCrafted.Items.Add("No");
            ddlEnchanted.Items.Add("Either");
            ddlEnchanted.Items.Add("Yes");
            ddlEnchanted.Items.Add("No");

            ddlCrafted.SelectedIndex = ddlCorrupted.SelectedIndex = ddlEnchanted.SelectedIndex = 0;

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
                explicits.Add(ddlExplicit.SelectedValue.ToString(), txtExplicitValue.Text);
            else
                explicits.Add(ddlExplicit.SelectedValue.ToString(), txtExplicitValue.Text);
            reload();

        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            POETradeConfig item = new POETradeConfig()
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
            if (ddlCorrupted.SelectedIndex != 0)
                item.corrupted = (ddlCorrupted.SelectedIndex == 1);
            if (ddlEnchanted.SelectedIndex != 0)
                item.enchanted = (ddlEnchanted.SelectedIndex == 1);
            if (ddlCrafted.SelectedIndex != 0)
                item.crafted = (ddlCrafted.SelectedIndex == 1);
            POETradeConfig.Rarity rarity;
            Enum.TryParse<POETradeConfig.Rarity>(ddlRarity.SelectedValue.ToString(), out rarity);
            POETradeConfig.BaseType type;
            Enum.TryParse<POETradeConfig.BaseType>(dropDownBaseType.SelectedValue.ToString(), out type);
            item.type = type;
            foreach (string key in explicits.Keys)
            {
                item.mods.Add(key, explicits[key]);
            }
            if (string.IsNullOrEmpty(item.armour) && string.IsNullOrEmpty(item.evasion) && string.IsNullOrEmpty(item.shield) && string.IsNullOrEmpty(item.pdps) && string.IsNullOrEmpty(item.damage))
                item.normalize_q = null;
            explicits.Clear();
            watchedRares.Add(item);
            reload();
        }
        private void reload()
        {
            this.listBoxWatched.Items.Clear();
            foreach (POETradeConfig item in watchedRares)
            {
                this.listBoxWatched.Items.Add(item);
            }
            this.listBoxExplicits.Items.Clear();
            foreach (string key in explicits.Keys)
            {
                this.listBoxExplicits.Items.Add(key + ":" + explicits[key]);
            }
        }

        public void LoadBasicInfo()
        {
            try
            {
                watchedRares = Newtonsoft.Json.JsonConvert.DeserializeObject<List<POETradeConfig>>(System.IO.File.ReadAllText(FinalVariables.rareFileName));
            }
            catch (Exception e)
            {
                watchedRares = new List<POETradeConfig>();
            }
        }

        private void btnDeleteClick(object sender, EventArgs e)
        {
            try
            {
                watchedRares.RemoveAt(listBoxWatched.SelectedIndex);
                reload();
            }
            catch (Exception eeeee)
            {

            }
        }

        private void btnSaveClick(object sender, EventArgs e)
        {
            string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(watchedRares);
            JArray ja = JArray.Parse(serialized);
            serialized = ja.ToString();
            System.IO.File.Delete(FinalVariables.rareFileName);
            System.IO.File.WriteAllText(FinalVariables.rareFileName, serialized);
        }
    }
}
