
using Utils;

namespace EnigmaGUI
{
    public partial class Enigma_GUI : Form
    {
        Enigma theMachine = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.III, new Plugboard());

        Label[] windows;
        Label[] details;
        Label[] notches;

        public Enigma_GUI()
        {
            InitializeComponent();
            windows = new Label[3] { lblWin1, lblWin2, lblWin3 };
            details = new Label[3] { lr1, lr2, lr3 };
            notches = new Label[3] { null, lblMedmTurnover, lblFastTurnover };

            foreach (string s in Enigma.PossibleRingSettings)
            {
                changeRingSettingToolStripMenuItem.DropDownItems.Add(s, null, RingSettingChangeRequest);
            }

            foreach (string s in Rotor.KnownRotors)
            {
                swapRotorToolStripMenuItem.DropDownItems.Add(s, null, RotorSelectionChangeRequest);
            }                
        }


        int txtOut_groupsOnLine = 0;
        int txtOut_lettersInGroup = 0;
        void outputChar(char ch)
        {
            if (txtOut_lettersInGroup == 5)
            {
                txtOut_groupsOnLine++;
                if (txtOut_groupsOnLine >= 4)
                {
                    txtOut.Text += Environment.NewLine;
                    txtOut_groupsOnLine = 0;
                    txtOut_lettersInGroup = 0;
                }
                else
                {
                    txtOut.Text += ' ';
                    txtOut_lettersInGroup = 0;
                }
            }
            txtOut.Text += ch;
            txtOut_lettersInGroup++;
        }

        private void textIn_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = char.ToUpper(e.KeyChar);
            if (char.IsLetter(c))
            {
                outputChar(theMachine.Encode(c));
                RefreshView();
            }
 
            else if (e.KeyChar == (char)Keys.Back) // I cannot backspace the enigma, so don't allow it here.
            {
                e.Handled = true;  
            }
        }

        private void RotorSelectionChangeRequest(object? sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            string captionOnMenu = item.ToString();
            Label lbl = contextMenuRotor.SourceControl as Label;
            int rotorNum = Array.IndexOf(details, lbl);
            Rotor r = Rotor.ByName(captionOnMenu);
            theMachine.ReplaceRotor(rotorNum, r);
            RefreshView();
        }

        private void RingSettingChangeRequest(object? sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            string captionOnMenu = item.ToString();
            Label lbl = contextMenuRotor.SourceControl as Label;
            int rotorNum = Array.IndexOf(details, lbl);
            Rotor r = theMachine.Rotors[rotorNum];
            int zeroBased = int.Parse(captionOnMenu.Substring(0, 2)) - 1;
            r.SetRing0(zeroBased);
            RefreshView();
        }

        private void Enigma_GUI_Shown(object sender, EventArgs e)
        {
            RefreshView();
        }

        void createTheMachine(Reflector reflect, Rotor r1, Rotor r2, Rotor r3, Plugboard pb)
        {
            theMachine = new Enigma(reflect,r1,r2,r3,pb);
            RefreshView();
        }

        private void mainMnu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void iIIIIIABCDEFToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            createTheMachine(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.III, new Plugboard());
            theMachine.ChangeRotorsToShow("ABC");
        }

        private void btnSetWindows_Click(object sender, EventArgs e)
        {
            theMachine.PlugB.SetWirings(txtPlugs.Text);

            string rotors = txtRotors.Text.ToUpper();
            string[] parts = rotors.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i=0; i < 3; i++)
            {
                string[] subParts = parts[i].Split('/');
                Rotor r = Rotor.ByName(subParts[0]);
                r.SetRing0(subParts[1][0] - 'A');
                theMachine.ReplaceRotor(i, r);
            }

            string contents = txtRotorInit.Text.ToUpper();
            parts = contents.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            try {
                if (parts.Length == 1)
                {
                    // Assume it is letter-denoted, of the form XYZ
                    for (int i = 0; i < 3; i++)
                    {
                        theMachine.ChangeRotorToLetter(i,parts[0][i]);
                    }
                }
                else
                {  // Assume it is three numbers, 1-based 
                    for (int i = 0; i < 3; i++)
                    {
                        theMachine.Rotors[i].PosAtWin0 = int.Parse(parts[i]) - 1;
                    }
                }
            }
            catch (Exception ex) {
              MessageBox.Show(ex.Message);           
            }
            RefreshView();
        }

        private void RefreshView()
        {
          //  string inWindows = theMachine.VisibleInWindows;
            for (int i=0; i < 3; i++)
            { Rotor r = theMachine.Rotors[i];
                windows[i].Text = Enigma.ToUserView(r.PosAtWin0);
                if (i > 0)
                {
                    windows[i].ForeColor = (r.IsAtNotch) ? Color.Maroon : Color.Black ;
                    notches[i].Visible = r.IsAtNotch;
                }

                string tover = $"Notch={(r.TurnoverPosition0+1).ToString("D2")}({(char)('A'+r.TurnoverPosition0)})";
                if (i==0)
                {
                    tover = "Not used";
                }
                details[i].Text = $"Rotor={r.RotorName}\nRing={Enigma.ToUserView(r.RingSetting0)}\n{tover}";
            }
        }

        private void txtRotorInit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                btnSetWindows_Click(sender, e);
                btnSetWindows.Focus();
            }
        }

        private void btnAdvance_Click(object sender, EventArgs e)
        {
            theMachine.AdvanceRotors();
            RefreshView();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnSetWindows_Click(sender,e);
            string result = theMachine.EncodeText(txtPlain.Text);
            txtCipher.Text = result;
            RefreshView();
        }

        private void btnClearReset_Click(object sender, EventArgs e)
        {
            txtIn.Clear();
            txtOut.Clear();
            btnSetWindows_Click(sender, e);
            txtOut_groupsOnLine = 0;
            txtOut_lettersInGroup = 0;
        }
    }
}