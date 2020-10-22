using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Editor
{
    public partial class EditorControl : UserControl
    {
        // To be handled by host
        public OperationDelegate AutoSaveProcess { get; set; }
        public FormatOperationDelegate FormatProcess { get; set; }

    public EditorControl()
        {
            InitializeComponent();

            //            System.Diagnostics.Process.Start("explorer.exe", "https://plantuml.com/salt");

            ContextMenuStrip contextMenu = new System.Windows.Forms.ContextMenuStrip();
            ToolStripMenuItem menuItem = new ToolStripMenuItem("Cut");
            menuItem.Click += new EventHandler(CutAction);
            contextMenu.Items.Add(menuItem);
            menuItem = new ToolStripMenuItem("Copy");
            menuItem.Click += new EventHandler(CopyAction);
            contextMenu.Items.Add(menuItem);
            menuItem = new ToolStripMenuItem("Paste");
            menuItem.Click += new EventHandler(PasteAction);
            contextMenu.Items.Add(menuItem);

            richTextBox1.ContextMenuStrip = contextMenu;


            autoSaveTimer = new Timer();
            autoSaveTimer.Tick += AutoSaveTimer_Tick;
            autoSaveTimer.Interval = 3000;
            autoSaveTimer.Enabled = false;
        }

        private string GenerateName(string text)
        {
            return "eb:" + text.Replace("|", "_").Replace(" ", "_");
        }

        public void AddButton(string text, string toolTipText = "",
            OperationDelegate operation = null,
            Image image = null,
            bool showText = true
            )
        {
            ToolStripButton tsb = new ToolStripButton();
            tsb.Click += MenuItem_Click;
            tsb.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsb.Text = text;
            tsb.ToolTipText = string.IsNullOrEmpty(toolTipText) ? text : toolTipText;
            tsb.Name = GenerateName(text);
            if (image != null)
            {
                tsb.Image = image;
                if (showText)
                {
                    tsb.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    tsb.TextImageRelation = TextImageRelation.ImageBeforeText;
                }
                else
                {
                    tsb.DisplayStyle = ToolStripItemDisplayStyle.Image;
                }
            }

            AddUserItem(tsb);

            if (operation != null)
            {
                operations.Add(tsb.Name, operation);
            }
        }

        public void AddMenuItem(string text, string toolTipText = "", OperationDelegate operation = null)
        {
            ToolStripItemCollection coll = FindParent(toolStrip1.Items, text);

            ToolStripMenuItem ddb = new ToolStripMenuItem();
            ddb.Text = text;
            if (text.Contains('|'))
            {
                ddb.Text = text.Substring(text.LastIndexOf('|') + 1);
            }

            ddb.ToolTipText = string.IsNullOrEmpty(toolTipText) ? ddb.Text : toolTipText;
            ddb.Click += MenuItem_Click;
            ddb.Name = GenerateName(text);
            coll.Add(ddb);

            if (operation != null)
            {
                operations.Add(ddb.Name, operation);
            }
        }

        ToolStripItemCollection FindParent(ToolStripItemCollection coll, string name)
        {
            ToolStripItemCollection tsi = coll;

            string searchText;
            string remainder;

            if (name.Contains('|'))
            {
                searchText = name.Substring(0, name.IndexOf('|'));
                remainder = name.Substring(name.IndexOf('|') + 1);

                bool found = false;
                foreach (ToolStripItem item in coll)
                {
                    if (item.Text == searchText)
                    {
                        if ((item is ToolStripDropDownItem) ||
                            item.GetType().IsSubclassOf(typeof(ToolStripDropDownItem)))
                        {
                            tsi = ((ToolStripDropDownItem)item).DropDownItems;
                        }
                        else
                        {
                            tsi = null;
                        }
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    //                    ToolStripMenuItem mi = new ToolStripMenuItem(searchText);
                    ToolStripMenuItem mi = new ToolStripMenuItem();
                    mi.Text = searchText;
                    mi.DisplayStyle = ToolStripItemDisplayStyle.Text;
                    mi.AutoSize = true;
                    coll.Add(mi);
                    tsi = mi.DropDownItems;
                }

                if (remainder.Contains('|'))
                {
                    tsi = FindParent(tsi, remainder);
                }
            }

            return tsi;
        }

        public void AddDropdownButton(string text, string toolTipText = "")
        {
            ToolStripDropDownButton ddb = new ToolStripDropDownButton();
            ddb.Text = text;
            ddb.ToolTipText = string.IsNullOrEmpty(toolTipText) ? text : toolTipText;
            ddb.Name = GenerateName(text);

            AddUserItem(ddb);
        }

        public void AddSeparator()
        {
            ToolStripSeparator tss = new ToolStripSeparator();

            AddUserItem(tss);
        }

        // Puts the new item at the end of the user items. Leaves the separator and refresh button in place, at the end
        private void AddUserItem(ToolStripItem item)
        {
            toolStrip1.Items.Insert(toolStrip1.Items.Count - 2, item);
        }


        public void SetOperation(string text, OperationDelegate operation = null)
        {
            string key = GenerateName(text);

            if (operations.ContainsKey(key))
            {
                if (operation == null)
                {
                    operations.Remove(key);
                }
                else
                {
                    operations[key] = operation;
                }
            }
            else
            {
                if (operation != null)
                {
                    operations.Add(key, operation);
                }
            }
        }

        Timer autoSaveTimer;

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            autoSaveTimer.Stop();
            autoSaveTimer.Enabled = false;

            if (AutoSaveProcess != null)
            {
                AutoSaveProcess(richTextBox1.Text);
            }

            ChangesPending = false;

            Show();
        }

        Dictionary<string, OperationDelegate> operations = new Dictionary<string, OperationDelegate>();

        private new void Show()
        {
            base.Show();
            ShowText(richTextBox1.Text, webBrowser1);
        }

        private void ShowText(string rawText, WebBrowser control)
        {
            if (FormatProcess != null)
            {
                FormatDetails details = new FormatDetails();
                details.Input = richTextBox1.Text;

                details = FormatProcess(details);

                if (details.IsUrl)
                {
                    control.Navigate(details.Result);
                }
            }
        }

        private void Modify(RichTextBox control, OperationDelegate op)
        {
            string currentText = control.SelectedText.TrimEnd();
            string content = currentText;

            if (op != null)
            {
                content = op(currentText);
            }

            Modify(richTextBox1, content);
        }

        private void Modify(RichTextBox control, string content)
        {
            string currentText = control.SelectedText.TrimEnd();
            bool replaceEOL = !control.SelectedText.Equals(control.SelectedText.TrimEnd());
            string newText = string.Format("{0}{1}", string.IsNullOrEmpty(content) ? currentText : content, replaceEOL ? "\r\n" : string.Empty);
            control.SelectedText = newText;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            string buttonText = GetButtonText(sender);

            if (operations.ContainsKey(buttonText))
            {
                OperationDelegate op = operations[buttonText];

                Modify(richTextBox1, op);
                Show();
            }
        }

        private string GetButtonText(object sender)
        {
            string rtnval = string.Empty;

            dynamic ctrl = sender;
            //rtnval = GenerateName((string)ctrl.Name);
            rtnval = (string)ctrl.Name;

            return rtnval;
        }

        public bool ChangesPending { get; set; } = false;
        public string Content
        {
            get { return richTextBox1.Text; }
            set
            {
                richTextBox1.Text = value;
                ShowText(richTextBox1.Text, webBrowser1);
                ChangesPending = false;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            ChangesPending = true;
            autoSaveTimer.Enabled = true;
            autoSaveTimer.Stop();   // in case the user is still typing
            autoSaveTimer.Start();
        }

        void CutAction(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        void CopyAction(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.SelectedText);
        }

        void PasteAction(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                int start = richTextBox1.SelectionStart;
                int end = richTextBox1.SelectionStart + richTextBox1.SelectionLength;

                string newText = Clipboard.GetText(TextDataFormat.Text).ToString();

                string left = richTextBox1.Text.Substring(0, start);

                string right = richTextBox1.Text.Substring(end);
                richTextBox1.Text = $"{left}{newText}{right}";

                richTextBox1.SelectionStart = start + newText.Length;
            }
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            Show();
        }
    }

    public delegate string OperationDelegate(string s);

    public delegate FormatDetails FormatOperationDelegate(FormatDetails d);
}
