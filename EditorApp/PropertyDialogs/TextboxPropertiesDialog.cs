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
    public partial class TextboxPropertiesDialog : Form
    {
        public TextboxPropertiesDialog()
        {
            InitializeComponent();
        }

        public static string CreateText()
        {
            string rtnVal = string.Empty;

            TextboxPropertiesDialog bpd = new TextboxPropertiesDialog();
            if (bpd.ShowDialog().Equals(DialogResult.OK))
            {
                rtnVal = $"\"{bpd.TextboxText}\"\r\n";
            }

            return rtnVal;
        }

        public string TextboxText { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            TextboxText = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
