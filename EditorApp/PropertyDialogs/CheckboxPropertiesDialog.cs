using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditorApp.PropertyDialogs
{
    public partial class CheckboxPropertiesDialog : Form
    {
        public CheckboxPropertiesDialog()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            comboBox1.SelectedIndex = 0;
        }

        public static string CreateText()
        {
            string rtnVal = string.Empty;

            CheckboxPropertiesDialog bpd = new CheckboxPropertiesDialog();
            if (bpd.ShowDialog().Equals(DialogResult.OK))
            {
                string checkedText = bpd.Checked ? "X" : "";
                rtnVal = $"[{checkedText}] {bpd.ButtonText}\r\n";
            }

            return rtnVal;
        }

        public string ButtonText { get; set; }
        public bool Checked { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonText = textBox1.Text;
            Checked = comboBox1.SelectedIndex == 0;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
