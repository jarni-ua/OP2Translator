using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StalTran
{
    public partial class MainForm : Form
    {
        private class TranslationLanguage
        {
            public string tag { get; set; }
            public string id { get; set; }
        }

        private Settings m_settings;
        private StringTable m_stringTable;
        private bool m_ctrlPressed = false;
        private bool m_shiftPressed = false;
        private List<TranslationLanguage> m_translationLanguages = new List<TranslationLanguage>();

        public MainForm()
        {
            InitializeComponent();

            m_translationLanguages.Add(new TranslationLanguage() { tag = "eng", id = "en" });
            m_translationLanguages.Add(new TranslationLanguage() { tag = "ukr", id = "uk" });
            m_translationLanguages.Add(new TranslationLanguage() { tag = "pln", id = "pl" });
            m_translationLanguages.Add(new TranslationLanguage() { tag = "fra", id = "fr" });
            this.m_cbTranslationLanguage.DataSource = m_translationLanguages;
            this.m_cbTranslationLanguage.DisplayMember = "tag";
            this.m_cbTranslationLanguage.ValueMember = "id";
            this.m_cbTranslationLanguage.DropDownStyle = ComboBoxStyle.DropDownList;

            m_settings = Settings.Load();

            if (!m_settings.winSize.IsEmpty)
                this.ClientSize = m_settings.winSize;

            if (!m_settings.location.IsEmpty)
                this.Location = m_settings.location;

            if (m_settings.scd1 != 0)
            {
                splitContainer1.SplitterDistance = m_settings.scd1;
                splitContainer2.SplitterDistance = m_settings.scd2;
                splitContainer4.SplitterDistance = m_settings.scd4;
            }

            if (m_settings.fontSize != 0)
            {
                m_nudFontSize.Value = m_settings.fontSize;
            }

            if (m_settings.dstLang == null)
                m_settings.dstLang = "eng";

            m_cbTranslationLanguage.SelectedIndex = m_translationLanguages.FindIndex(language => language.tag.Equals(m_settings.dstLang, StringComparison.Ordinal));
            m_cbIngnoreIns.Checked = m_settings.ignoreIns;

            if (!string.IsNullOrWhiteSpace(m_settings.xmlLocation) && !string.IsNullOrWhiteSpace(m_settings.lastSelectedFile))
                LoadFile();
        }

        private void LoadFile()
        {
            if (m_stringTable != null && m_stringTable.HasModifications())
                StoreFile();

            m_stringTable = StringTable.Load(Path.Combine(m_settings.xmlLocation, m_settings.lastSelectedFile), TableStateChangedHandler, TableStatsChangedHandler);
            if (m_stringTable.items == null)
            {
                m_lbStringTable.DataSource = null;
                m_rtbRus.Text = "!!! wrong xml file format !!!";
                m_rtbTranslation.Text = "!!! wrong xml file format !!!";
                return;
            }

            Text = "S.T.A.L.K.E.R. Translator - " + m_settings.lastSelectedFile;

            m_lbStringTable.SelectedIndexChanged -= new EventHandler(m_lbStringTable_SelectedIndexChanged);
            m_lbStringTable.DataSource = m_stringTable.items.Where(x => x.usable).ToArray();
            m_lbStringTable.DisplayMember = "id";
            m_lbStringTable.SelectedIndex = -1;
            m_lbStringTable.SelectedIndexChanged += new EventHandler(m_lbStringTable_SelectedIndexChanged);

            if (m_settings.lastSelectedStringId != null && m_settings.lastSelectedStringId != "")
            {
                bool found = false;
                for (int i = 0; i < m_lbStringTable.Items.Count && !found; ++i)
                {
                    Item item = (Item)m_lbStringTable.Items[i];
                    if (item.id == m_settings.lastSelectedStringId)
                    {
                        m_lbStringTable.SelectedIndex = i;
                        found = true;
                    }
                }
                if (!found && m_lbStringTable.Items.Count > 0)
                {
                    m_lbStringTable.SelectedIndex = 0;
                }
            }
            else
            {
                m_lbStringTable.SelectedIndex = 0;
            }
            //TableStateChangedHandler(false);
        }

        private void StoreFile()
        {
            if (m_stringTable.HasModifications())
                m_stringTable.Store();
        }

        private void LoadStringId()
        {
            if (m_stringTable == null || m_lbStringTable.SelectedIndex < 0)
                return;

            Item item = (Item)m_lbStringTable.SelectedItem;
            m_settings.lastSelectedStringId = item.id;
            m_rtbRus.Text = item.get("rus");
            m_rtbTranslation.Text = item.get(m_settings.dstLang);

            m_rtbTranslation.SelectAll();
            m_rtbTranslation.SelectionBackColor = Color.White;
            m_rtbTranslation.DeselectAll();

            foreach(Match match in Regex.Matches(m_rtbTranslation.Text, m_settings.dstLang != "ukr" ? @"[а-яА-ЯъЪьЬёЁ]+" : @"[ыЫёЁъЪ]"))
            {
                m_rtbTranslation.Select(match.Index, match.Length);
                m_rtbTranslation.SelectionBackColor = Color.Red;
            }
            m_rtbTranslation.DeselectAll();
            m_rtbTranslation.Enabled = true;
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = m_settings.xmlLocation;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.DefaultExt = "*.xml";
                ofd.Filter = "S.T.A.L.K.E.R text files (*.xml)|*.xml";
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    m_settings.xmlLocation = Path.GetDirectoryName(ofd.FileName);
                    m_settings.lastSelectedFile = ofd.SafeFileName;
                    LoadFile();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StoreFile();
        }

        private void m_lbStringTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_lbStringTable.SelectedIndex != -1)
            {
                LoadStringId();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_settings.scd1 = splitContainer1.SplitterDistance;
            m_settings.scd2 = splitContainer2.SplitterDistance;
            m_settings.scd4 = splitContainer4.SplitterDistance;
            m_settings.winSize = this.ClientSize;
            m_settings.location = this.Location;
            m_settings.ignoreIns = m_cbIngnoreIns.Checked;

            Settings.Store(m_settings);

            if (m_stringTable != null)
                m_stringTable.Store();
        }

        private void m_nudFontSize_ValueChanged(object sender, EventArgs e)
        {
            m_rtbRus.Font = new Font(m_rtbRus.Font.FontFamily, (float)m_nudFontSize.Value, m_rtbRus.Font.Style);
            m_rtbTranslation.Font = new Font(m_rtbTranslation.Font.FontFamily, (float)m_nudFontSize.Value, m_rtbTranslation.Font.Style);
            m_settings.fontSize = m_nudFontSize.Value;
        }

        private void m_lbStringTable_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (e.Index < 0 || e.Index > lb.Items.Count)
                return;

            Item item = (Item)lb.Items[e.Index];

            e.DrawBackground();
            Graphics g = e.Graphics;

            // draw the background color you want
            // mine is set to olive, change it to whatever you want
            if (item.translationRequired(m_settings.dstLang))
            {
                if (e.State.HasFlag(DrawItemState.Selected))
                    g.FillRectangle(new SolidBrush(Color.DodgerBlue), e.Bounds);
                else
                    g.FillRectangle(new SolidBrush(Color.Olive), e.Bounds);
            }

            // draw the text of the list item, not doing this will only show
            // the background color
            // you will need to get the text of item to display
            g.DrawString(item.id, e.Font, new SolidBrush(e.ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));

            e.DrawFocusRectangle();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                m_ctrlPressed = true;

                if (e.KeyCode == Keys.S && m_stringTable.HasModifications())
                {
                    StoreFile();
                    e.Handled = true;
                }
                else if (e.Shift)
                {
                    m_shiftPressed = true;

                    if (e.KeyCode == Keys.Up)
                    {
                        e.Handled = true;
                        for (int i = m_lbStringTable.SelectedIndex - 1; i >= 0; --i)
                        {
                            Item item = (Item)m_lbStringTable.Items[i];
                            if (item.translationRequired(m_settings.dstLang))
                            {
                                m_lbStringTable.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.Down)
                    {
                        e.Handled = true;
                        for (int i = m_lbStringTable.SelectedIndex + 1; i < m_lbStringTable.Items.Count; ++i)
                        {
                            Item item = (Item)m_lbStringTable.Items[i];
                            if (item.translationRequired(m_settings.dstLang))
                            {
                                m_lbStringTable.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (e.KeyCode == Keys.Up && m_lbStringTable.SelectedIndex > 0)
                    {
                        m_lbStringTable.SelectedIndex = m_lbStringTable.SelectedIndex - 1;
                        e.Handled = true;
                    }
                    else if (e.KeyCode == Keys.Down && m_lbStringTable.SelectedIndex < (m_lbStringTable.Items.Count - 1))
                    {
                        m_lbStringTable.SelectedIndex = m_lbStringTable.SelectedIndex + 1;
                        e.Handled = true;
                    }
                }

                if (e.KeyCode == Keys.T || e.KeyCode == Keys.Tab)
                {
                    Translate();
                    e.Handled = true;
                }
            }
            else if (e.Shift)
            {
                m_shiftPressed = true;
            }
            else
            {
                if (m_cbIngnoreIns.Checked && !e.Shift && e.KeyCode == Keys.Insert)
                    e.Handled = true;
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Control)
            {
                m_ctrlPressed = false;
            }
            if (!e.Shift)
            {
                m_shiftPressed = false;
            }
        }

        private void m_rtbTranslation_TextChanged(object sender, EventArgs e)
        {
            Item item = (Item)m_lbStringTable.SelectedItem;
            item.set(m_settings.dstLang, m_rtbTranslation.Text);
            item.CheckTranslation();
        }

        private void TableStateChangedHandler(bool isModified)
        {
            btnSave.Enabled = isModified;
        }

        private void TableStatsChangedHandler(string lang, WordStats.Stats stats)
        {
            m_lWordCounter.Text = "Left: " + m_stringTable.getTranslationRequired(lang) + ", Letters: " + stats.letters + ", Digits: " + stats.digits + ", Whitespaces: " + stats.whites + ", Puncts: " + stats.puncts;
        }

        private void btnTranslate_Click(object sender, EventArgs e)
        {
            Translate();
        }

        private void Translate()
        {
            if (m_rtbTranslation.SelectionLength == 0)
            {
                m_rtbTranslation.Text = Translate(m_rtbRus.Text);
            }
            else
            {
                string src = m_rtbTranslation.SelectedText;
                string dst = Translate(src);
                m_rtbTranslation.Text = m_rtbTranslation.Text.Replace(src, dst);
            }
            m_rtbTranslation.SelectAll();
            m_rtbTranslation.SelectionBackColor = Color.White;
            m_rtbTranslation.DeselectAll();
            m_rtbTranslation.Select();
        }

        private string Translate(string src)
        {
            string dst = "";

            
            if (Regex.IsMatch(src, @"(.*?)(~|\\n|\t|\r|\n|\r\n|%+[a-z]\[\d+,\d+,\d+,\d+\]|%+[a-z]\[[a-z]+\]|%+[a-z])( *)"))
            {
                int lastIndex = -1;
                foreach (Match match in Regex.Matches(src, @"(.*?)( *)(~|\\n|\t|\r|\n|\r\n|%+[a-z]\[\d+,\d+,\d+,\d+\]|%+[a-z]\[[a-z]+\]|%+[a-z])( *)"))
                {
                    if (Regex.Matches(match.Groups[1].Value, @"[а-яА-ЯыЫъЪьЬёЁ]").Count > 0)
                    {
                        dst += TranslateBlock(match.Groups[1].Value);
                    }
                    else
                    {
                        dst += match.Groups[1].Value;
                    }
                    dst += match.Groups[2].Value + match.Groups[3].Value + match.Groups[4].Value;
                    lastIndex = match.Index + match.Length;
                }
                if (lastIndex != src.Length)
                    dst += TranslateBlock(src.Substring(lastIndex));
            }
            else
            {
                dst = TranslateBlock(src);
            }

            dst = dst.Replace("\\\"", "\"");
            dst = dst.Replace(" ...", "...");
            return dst;
        }

        private string TranslateBlock(string src)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "identity");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "utf-8");

            src = src.Replace("&", "%26");
            src = src.Replace("#", "%23");

            string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=ru&tl=" + (((TranslationLanguage)m_cbTranslationLanguage.SelectedItem).id) + "&dt=t&q=" + src;
            var response = httpClient.GetAsync(new Uri(url)).Result;

            if (response.IsSuccessStatusCode)
            {
                // by calling .Result you are performing a synchronous call
                var responseContent = response.Content;

                // by calling .Result you are synchronously reading the result
                string responseString = responseContent.ReadAsStringAsync().Result;

                // [[["Тестове повідомлення з \"лапками\".","Тестовое сообщение с \"кавычками\".",null,null,0]],null,"ru"]
                // [[["Бідні батьки! ","Бедные родители!",null,null,0],["Своєму начальству я доповім, що сержант загинув, борючись з мутантами, і тому є свідок ...","Своему начальству я доложу, что сержант погиб, сражаясь с мутантами, и тому есть свидетель...",null,null,0]],null,"ru"]
                string dst = "";
                foreach (Match match in Regex.Matches(responseString, "\\[\"(.*?)\",\".*?\",null,null,[0-9]+\\]"))
                {
                    string value = match.Groups[1].Value;
                    // & is replaced by \u0026, # by \u0023, they must be replaced back
                    value = value.Replace("\\u0026", "&");
                    value = value.Replace("\\u0023", "#");
                    value = Regex.Replace(value, @" ?~ ?", "~");
                    value = value.Replace(" & ", "&");
                    value = value.Replace(" ?!", "?!");
                    value = value.Replace("! ..", "!..");
                    value = value.Replace(" !", "!");
                    value = value.Replace("" + (char)0x200b, "");

                    // MP5/10 is modified to MP5 / 10, revert it
                    value = value.Replace(" / ", "/");

                    // Some usual ukrainian translation bugs
                    if (m_settings.dstLang == "ukr")
                    {
                        value = value.Replace("прицілом коліматора", "коліматорним прицілом");
                        value = value.Replace("стовбур", "ствол");
                        value = value.Replace("Боров", "Кабан");
                        value = value.Replace("ПДА", "КПК");
                        value = value.Replace("товбур", "твол");
                    }
                    dst += value;
                }

                if (Char.IsUpper(src[0]) && Char.IsLower(dst[0]))
                {
                    StringBuilder tmp = new StringBuilder(dst);
                    tmp[0] = Char.ToUpper(dst[0]);
                    dst = tmp.ToString();
                }
                else if (Char.IsLower(src[0]) && Char.IsUpper(dst[0]))
                {
                    StringBuilder tmp = new StringBuilder(dst);
                    tmp[0] = Char.ToLower(dst[0]);
                    dst = tmp.ToString();
                }

                return dst;
            }
            else
                return src;
        }

        private void m_rtbTranslation_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && m_rtbTranslation.Focused && m_rtbTranslation.SelectionLength > 0)
            {
                if (m_ctrlPressed)
                {
                    m_ctxMenuTranslation.Show(m_rtbTranslation, e.Location);
                    m_ctrlPressed = false;
                }
                else
                {
                    Point location = m_rtbTranslation.FindForm().PointToClient(m_rtbTranslation.Parent.PointToScreen(m_rtbTranslation.Location));
                    Point position = m_rtbTranslation.GetPositionFromCharIndex(m_rtbTranslation.SelectionStart);
                    Point formPosition = new Point(this.Location.X + e.Location.X + location.X + 16, this.Location.Y + e.Location.Y + location.Y + 30);
                    Completion completion = new Completion(formPosition, m_settings.contextReplacements, m_rtbTranslation);
                    completion.Show();
                }
            }
        }

        private void addToContextReplacementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_rtbTranslation.Focused && m_rtbTranslation.SelectionLength > 0)
            {
                if (!m_settings.contextReplacements.Contains(m_rtbTranslation.SelectedText.Trim()))
                {
                    m_settings.contextReplacements.Add(m_rtbTranslation.SelectedText.Trim());
                    m_settings.contextReplacements.Sort();
                }
            }
        }

        private void wordCounter_Click(object sender, EventArgs e)
        {

        }

        private void m_cbTranslationLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_settings == null || m_settings.dstLang == null)
                return;

//            TranslationLanguage lang = (TranslationLanguage);
            m_settings.dstLang = m_cbTranslationLanguage.Text;
            LoadStringId();
            m_lbStringTable.Invalidate();
            if (m_stringTable != null)
                TableStatsChangedHandler(m_settings.dstLang, m_stringTable.getFileStats(m_settings.dstLang));
            m_lbStringTable.Focus();
        }
    }
}
