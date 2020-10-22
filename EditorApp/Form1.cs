using EditorApp.PropertyDialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Translators;

namespace EditorApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string fileName = string.Empty;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            editorControl1.AddButton("Frame", image: GetImage("Dialog.png"), showText: false, operation: t => { return @"@startsalt
{
{Content goes here}
}
@endsalt
"; });
            editorControl1.AddSeparator();
            editorControl1.AddButton("Button", image: GetImage("Button.png"), showText: false, operation: t => { return ButtonPropertiesDialog.CreateText(); });
            editorControl1.AddButton("Radio Button", image: GetImage("Radio.png"), showText: false, operation: t => { return RadioButtonPropertiesDialog.CreateText(); });
            editorControl1.AddButton("Checkbox", image: GetImage("Checkbox.png"), showText: false, operation: t => { return CheckboxPropertiesDialog.CreateText(); });
            editorControl1.AddButton("Text Box", image: GetImage("Textbox.png"), showText: false, operation: t => { return TextboxPropertiesDialog.CreateText(); });
            editorControl1.AddButton("Droplist", image: GetImage("Droplist.png"), showText: false, operation: t => { return DroplistPropertiesDialog.CreateText(); });
            editorControl1.AddSeparator();


            // Create a new button
            editorControl1.AddButton("Lower", image: GetImage("Button.png"), showText: false, operation: t => { return t.ToLower(); });

            //editorControl1.AddDropdownButton("DDB1", "Dropdown Button tests");

            //            editorControl1.AddMenuItem("DDB1|Menu One", operation: t => { MessageBox.Show(t); return t; });

            editorControl1.AddDropdownButton("Level One", "Menu Tests");

            editorControl1.AddMenuItem("Level One|Level Two|Menu Two", operation: t => { MessageBox.Show(t); return t; });
            editorControl1.AddMenuItem("Level One|Menu Three", operation: t => { MessageBox.Show(t, "Menu Three"); return t; });


            //            editorControl1.AutoSaveProcess = t => { MessageBox.Show(t, "AutoSave"); return ""; };
            //            editorControl1.FormatProcess = t => { return $"<html><body><h1>{t}</h2></body></html>"; };  // H1 document
            //editorControl1.FormatProcess = t => { return $"<html><body>{t.Replace("\n", "<br/>")}</body></html>"; };  // H1 document
            //editorControl1.FormatProcess = t => { 
            //    return new Editor.FormatDetails { 
            //        Result = "https://cnn.com", 
            //        IsUrl = true 
            //    }; 
            //};
            editorControl1.FormatProcess = t => {
                PlantUmlTranslator translator = new PlantUmlTranslator();
                return new Editor.FormatDetails
                {
                    Result = translator.Translate(t.Input),
                    IsUrl = true
                };
            };

            
        }

        private Image GetImage(string resourceName)
        {
            Image rtnVal = null;

            string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            foreach (string str in resourceNames)
            {
                if (str.EndsWith($".{resourceName}"))
                {
                    Stream imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(str);
                    rtnVal = new Bitmap(imgStream);
                }
            }

            return rtnVal;
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            editorControl1.ResetText();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Wireframe Files (*.puml)|*.puml|All Files (*.*)|*.*";
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFileToEdit(ofd.FileName);
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            else
            {
                File.WriteAllText(fileName, editorControl1.Content);
            }
            editorControl1.ChangesPending = false;
        }


        private void LoadFileToEdit(string fileName)
        {
            this.fileName = fileName;
            editorControl1.Content = File.ReadAllText(fileName);
        }


        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Wireframe Files (*.puml)|*.puml|All Files (*.*)|*.*";
            sfd.OverwritePrompt = true;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = sfd.FileName;
                File.WriteAllText(fileName, editorControl1.Content);
                editorControl1.ChangesPending = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (editorControl1.ChangesPending)
            {
                switch (MessageBox.Show("Changes have not been saved.  Save now?", "Warning", MessageBoxButtons.YesNoCancel))
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        saveToolStripButton_Click(this, EventArgs.Empty);
                        //File.WriteAllText(fileName, richTextBox1.Text);
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }
    }
}
