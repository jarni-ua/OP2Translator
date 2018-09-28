using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace StalTran
{
    public partial class Completion : Form
    {
        List<string> content;
        RichTextBox textBox;
        string searchPredicate;
        bool delayedUpdate = false;

        public Completion(Point position, List<string> content, RichTextBox textBox)
        {
            InitializeComponent();

            this.content = content;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = position;
            this.textBox = textBox;

            m_lbProposals.Items.AddRange(content.ToArray());
            if (content.Count > 0)
                m_lbProposals.SelectedIndex = 0;
        }

        private void m_lbProposals_DoubleClick(object sender, EventArgs e)
        {
            if (m_lbProposals.SelectedIndex >= 0)
            {
                Replace();
                this.Close();
            }
        }

        private void Completion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (searchPredicate.Length != 0)
                {
                    searchPredicate = "";
                    e.Handled = true;
                    Filter();
                }
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                Replace();
                e.Handled = true;
                this.Close();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                content.Remove((string)m_lbProposals.SelectedItem);
                m_lbProposals.Items.RemoveAt(m_lbProposals.SelectedIndex);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Back)
            {
                if (searchPredicate.Length > 0)
                {
                    searchPredicate = searchPredicate.Substring(0, searchPredicate.Length - 1);
                    Filter();
                    e.Handled = true;
                }
            }
            else
            {
                string key = KeyCodeToUnicode(e.KeyCode);
                if (key.Length > 0)
                {
                    searchPredicate += key;
                    Filter();
                    e.Handled = true;
                }
            }
        }

        private void Completion_KeyUp(object sender, KeyEventArgs e)
        {
            if (delayedUpdate)
            {
                if (m_lbProposals.Items.Count > 0)
                    m_lbProposals.SelectedIndex = 0;
                m_lbProposals.EndUpdate();
                delayedUpdate = false;
            }
        }

        private void Completion_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Replace()
        {
            if (m_lbProposals.SelectedItem != null)
                textBox.SelectedText = (string)m_lbProposals.SelectedItem;
        }

        private void Filter()
        {
            List<string> filtered;
            if (searchPredicate.Length == 0)
                filtered = content;
            else
                filtered = content.FindAll(x => x.StartsWith(searchPredicate, StringComparison.CurrentCulture));

            m_lbProposals.BeginUpdate();
            m_lbProposals.Items.Clear();
            m_lbProposals.Items.AddRange(filtered.ToArray());
            if (filtered.Count > 0)
            {
                m_lbProposals.SelectedIndex = 0;
                delayedUpdate = true;
            }
            else
                m_lbProposals.EndUpdate();
        }

        private void m_lbProposals_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private string KeyCodeToUnicode(Keys key)
        {
            byte[] keyboardState = new byte[255];
            bool keyboardStateStatus = GetKeyboardState(keyboardState);

            if (!keyboardStateStatus)
            {
                return "";
            }

            uint virtualKeyCode = (uint)key;
            uint scanCode = MapVirtualKey(virtualKeyCode, 0);
            IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

            StringBuilder result = new StringBuilder();
            ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, result, (int)5, (uint)0, inputLocaleIdentifier);

            return result.ToString();
        }

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);
    }
}
