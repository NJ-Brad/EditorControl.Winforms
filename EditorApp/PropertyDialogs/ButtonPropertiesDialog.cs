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
    public partial class ButtonPropertiesDialog : Form
    {
        public ButtonPropertiesDialog()
        {
            InitializeComponent();
        }

        public static string CreateText()
        {
            string rtnVal = string.Empty;

            ButtonPropertiesDialog bpd = new ButtonPropertiesDialog();
            if (bpd.ShowDialog().Equals(DialogResult.OK))
            {
                rtnVal = $"[{bpd.ButtonText}]";
            }

            return rtnVal;
        }

        public string ButtonText { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonText = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
