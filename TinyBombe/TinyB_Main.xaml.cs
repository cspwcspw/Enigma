﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Text;
using System.Windows.Threading;

// Pete, Jan/Feb 2022.

// Purpose of this program is to help us animate and understand the Bombe, particularly the impact of Welchman's
// diagonal board. So I'm drawing inspiration, etc. from http://www.ellsbury.com/bombe3.htm 

// In particular, the tiny Bombe here only has an 8-symbol alphabet.  In my case, three 8-wire rotors and a reflector,
// and all scramblers are "hardwired" to always use the same mapping at each of their 512 rotor positions AAA - HHH.

namespace TinyBombe
{
    public partial class MainWindow : Window
    {

        #region Fields and constant declarations that are used for positioning things on the Bombe's Canvas
        // Layout is on a (conceptual) grid of rows and columns.  Each column contains a bus of wires,
        // and a channel to the right for scramblers.
        // Row numbering excludes the area at the top for the diagonal board.

        // Layout is mainly lots of hacks and magic coordinates until I think it looks pretty.
        // It is easier than pondering the deep conceptual stuff about why the Bombe worked and the
        // actual crypto issues.

        const int WindowTopMargin = 50; // reserve area at top for menus, buttons, etc above the canvas.
        const int ColWidth = 140;
        const int RowHeight = 76;
        const int WireChannelWidth = 7;
        const int ScramblerSize = 8*WireChannelWidth;
        const double WireThickness = 1.0;
        const int LeftMargin = 30;
        
        // This margin is space on the canvas above the rows is used for layout/wiring of the diagonal board cross-connects.
        const int DiagonalBoardMargin = 18 * WireChannelWidth;
        const char leftRightArrow = '\u2194';  //https://www.fileformat.info/info/unicode/char/search.htm

        List<Scrambler> Scramblers =  new List<Scrambler>();
        Line[,] busWires;
        Brush HotBrush = Brushes.IndianRed;
        Connections Joints;  // This keeps track of electrical joints between Shapes on the GUI, so that we can propagate Hot voltages.
        Dictionary<string, bool>? diagonalSwitchClosed = null;   // Persistent across calls to ResetToInitialState, built on first call to makeDiagonalBoard
        ScrollViewer theScroller;  // The WPF designer and I are not particularly good friends, so a lot of GUI stuff is built in code. 
        Canvas bombeCanvas;

        int VccAttachesAt = 34; // this is a packed Column*8+Wire telling us which bus and wire to apply the VCC source voltage to.
        bool VccIsHot = true;   // Persist whether the user wants voltage applied when we advance to other wheel positions

        int botY;  // Calculated depending on crib length and how many scrambler layout rows are needed, used in a couple of places for layout and sizing.

        string uppers = "ABCDEFGH";
        string lowers = "abcdefgh";

        int cribOffsetWithinIntercept = 0;

        DispatcherTimer stepTimer;

        #endregion

        #region Constructor and Rebuilding Bombe Canvas when anything changes 
        public MainWindow()
        {
            InitializeComponent();

            stepTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(3), IsEnabled = false };
            stepTimer.Tick += StepTimer_Tick;

            AddSamplesToMenu();

            Joints = new Connections(HotBrush);

            // BEAD ACE BEACH BABE FED DEAD DAD BAD FEED EGGHEAD EGGED BEG BEGGED CAGED AGED FACED
            // DEADHEAD BEHEAD EACH DEED FEE EGGHEAD GAFF

            // BEACHHEADGEACHGBEACHGBABEGFEDGDADGBEAD->GBEECECBCCCBECCGBHFGCFBHGCGGGCBCCHADBG at BFG
            // BEACHHEADGEACHGBEACHGBABEGFEDGDADGBEAD->GBEECBGFHEAEAGADFFECBAHHFBGFGHEHHDCCHC at CAA
            // This test case on first five-letter crib should succeed at 128(CAA) and give a false stop at 110(BFG)
            // BEACH -> GBEEC  
            Cipher.Text = "GBEECBGFHEAEAGADFFECBAHHFBGFGHEHHDCCHC";
            Crib.Text = "??ACHHEAD";  
            tbWindow.Text = "BFF";
            RebuildBombe();
            // And first time only, change the Window size. 
            this.Height = botY + 140;
        }

        private void AddSamplesToMenu()
        {
             // ToDo
        }

        int xFor(int bus, int wire)  // General helper for x layout
        {
            int x0 = bus * ColWidth + (wire + 1) * WireChannelWidth + LeftMargin;
            return x0;
        }

        // Trying to untangle connections, make new cross-connects in the scrambers, etc.
        // when the rotors move is too messy. So I just rebuild the whole GUI from scratch on each step.
        void RebuildBombe()
        {
            normalizeCrib();

            List<int> scramblerRows = getScramblerRowsNeeded(Crib.Text, Cipher.Text);
            int maxRow = scramblerRows.Max();
            botY = DiagonalBoardMargin + (maxRow + 1) * RowHeight;

            Scramblers.Clear();
            Joints.Clear();
            if (bombeCanvas != null)
            {
                bombeCanvas.Children.Clear();
            }

            theScroller = new ScrollViewer()
            {
                Margin = new Thickness(0, WindowTopMargin, 0, 0),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            bombeCanvas = new Canvas()
            {
                Background = Brushes.PowderBlue,  // powderblue, lightskyblue
                Height = botY + 50,
                Width = ColWidth * 8 + LeftMargin
            };

            theScroller.Content = bombeCanvas;
            mainGrid.Children.Add(theScroller);

            busWires = new Line[8, 8];

            makeBuses();

            if ((bool)cbUseDiagonalBoard.IsChecked)
            {
                makeDiagonalBoard();
            }
            addScramblers(Crib.Text, Cipher.Text, scramblerRows);
            addVoltageSource();
            RecoverMessage();
            isTestRegisterTriggered();
        }

        private void normalizeCrib()
        {
            string s = Crib.Text.ToUpper().Trim();
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (c >= 'A' && c <= 'H')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append('?');
                }
            }

            Crib.Text = sb.ToString();
        }

        private void RecoverMessage()
        {
            Scrambler sc = new Scrambler(0, 0, 0);
            sc.Index = Scrambler.FromWindowView(tbWindow.Text);
            sc.Index = (sc.Index + 512 - cribOffsetWithinIntercept) % 512;
            string cp = Cipher.Text;
            string plain = sc.EncryptText(cp);
            if ((bool)cbReplaceSpaces.IsChecked)
            {
                plain = plain.Replace("G", " ");
            }
            Recovered.Content = plain;
        }

        #endregion

        #region Vcc Source Tag.  Put voltage on the tag, test Steckering hypothesis
        private void addVoltageSource()
        {
            // Add the VCC source
            PointCollection pc = new PointCollection() { new Point(0, 0), new Point(-10, 20), new Point(0, 14), new Point(10, 20), new Point(0, 0) };
            Polygon VccTag = new Polygon() { Points = pc, Fill = Brushes.Blue, Stroke = Brushes.Blue };
            VccTag.MouseUp += VccTag_MouseUp;
            int attachedBus = VccAttachesAt / 8;
            int attachedWire = VccAttachesAt % 8;

            string tip = $"Tests the hypothesis that {uppers[attachedBus]} is steckered to {uppers[attachedWire]}. If so, no other wire in this bus can light up.";
            VccTag.ToolTip = tip;
            Canvas.SetLeft(VccTag, xFor(attachedBus, attachedWire));
            Canvas.SetTop(VccTag, botY);

            bombeCanvas.Children.Add(VccTag);
            Joints.Join(VccTag, busWires[attachedBus, attachedWire]);
            if (VccIsHot) // Push the voltage through the wiring
            {
                Joints.MakeItLive(VccTag);
            }

            ContextMenu mnu = new ContextMenu();
            for (char bus='A'; bus <= 'H'; bus++)
            {
                MenuItem theItem = new MenuItem() { Header = bus };
                int p = mnu.Items.Add(theItem);
                for (int wire = 0; wire < 8; wire++)
                {
                    MenuItem subItem = new MenuItem() { Header = $"{bus} {leftRightArrow} {(char)('a'+wire)}", Tag = 8*(bus-'A')+wire };
                    subItem.Click += VccSubItem_Click;
                    theItem.Items.Add(subItem);
                }
            }       
            VccTag.ContextMenu = mnu;
        }

        private void VccTag_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            Shape p = sender as Shape;
            if (p.Stroke == HotBrush)
            {
                Joints.DisconnectAllVoltages();
                VccIsHot = false;
            }
            else
            {
                VccIsHot = true;
                Joints.MakeItLive(p);
            }
        }

        private void VccSubItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            int t = (int) item.Tag;
            VccAttachesAt = t;
            RebuildBombe();
            e.Handled = true;
        }

#endregion

        #region Scramblers - decide what is needed, create them with crib offsets, lay them out, plug them into buses, etc.
        private List<int> getScramblerRowsNeeded(string crib, string cipher)
        {
            List<int> rows = new List<int>();
            Placements places = new Placements();
            for (int i = 0; i < crib.Length; i++)
            {
                if (crib[i] == '?')    // step over wildcard don't-cares im crib, 
                {
                    rows.Add(-1);
                }
                else
                {
                    int leftBus = crib[i] - 'A';
                    int rightBus = cipher[i] - 'A';
                    if (leftBus > rightBus) // wrong way around?
                    {
                        int temp = leftBus;
                        leftBus = rightBus;
                        rightBus = temp;
                    }
                    int col = (leftBus + rightBus) / 2;
                    int row = places.PlaceIt(leftBus, col, rightBus);
                    rows.Add(row);
                }
             }
            return rows;
        }

        private void addScramblers(string crib, string cipher, List<int> scramblerRow)
        {
            for (int i = 0; i < crib.Length; i++)
            {
                if (crib[i] == '?') continue;  // ignore wildcard don't-cares in crib
                int leftBus = crib[i] - 'A';
                int rightBus = cipher[i] - 'A';
                if (leftBus > rightBus) // wrong way around?
                {
                    // Fortunately scramblers and Enigmas are symmetrical so we can call either side left or right
                    int temp = leftBus;
                    leftBus = rightBus;
                    rightBus = temp;
                }
                int col = (leftBus + rightBus) / 2;
                int row = scramblerRow[i];

                Canvas cnvs = new Canvas() { Background = Brushes.Tan, Width = ScramblerSize, Height = ScramblerSize };
                Scrambler sc = new Scrambler(i, leftBus, rightBus, cnvs);
                Scramblers.Add(sc);
                placeAndPlugup(sc, row, leftBus, col, rightBus);
            }
            updateScramblerCrossConnects();
        }

        void placeAndPlugup(Scrambler sc, int row, int leftBus, int posCol, int rightBus)
        {
            Canvas scramblerCanvas = sc.Tag as Canvas;
            double xMid = (xFor(posCol, 7) + xFor(posCol + 1, 0)) / 2; // middle of channel
            double x0 = xMid - scramblerCanvas.Width / 2;
            Canvas.SetLeft(scramblerCanvas, x0);
            double y0 = row * RowHeight + DiagonalBoardMargin + 2 * WireChannelWidth;
            Canvas.SetTop(scramblerCanvas, y0);
            bombeCanvas.Children.Add(scramblerCanvas);

            // Plug the scrambler into the buses on either side
            double w0 = xFor(leftBus, 0);
            double y = y0 + WireChannelWidth / 2;
            double dotSize = WireChannelWidth - 2;
            double halfDotSz = dotSize / 2;
            for (int i = 0; i < 8; i++)
            {
                Line p = new Line() { X1 = w0, X2 = x0, Y1 = y, Y2 = y, Stroke = Brushes.Blue, StrokeThickness = WireThickness };
                bombeCanvas.Children.Add(p);
                Ellipse theDot = new Ellipse() { Width = dotSize, Height = dotSize, Stroke = Brushes.Blue, Fill = Brushes.Blue };
                Canvas.SetTop(theDot, y - halfDotSz);
                Canvas.SetLeft(theDot, w0 - halfDotSz);
                bombeCanvas.Children.Add(theDot);
                y += WireChannelWidth;
                w0 += WireChannelWidth;
                Joints.Join(busWires[leftBus, i], p);
                Joints.Join(theDot, p);
            }

            double w1 = xFor(rightBus, 0);
            y = y0 + WireChannelWidth / 2;
            x0 = xMid + scramblerCanvas.Width / 2; // Right edge of scramber icon
            for (int i = 0; i < 8; i++)
            {
                Line p = new Line() { X1 = x0, X2 = w1, Y1 = y, Y2 = y, Stroke = Brushes.Blue, StrokeThickness = WireThickness };
                bombeCanvas.Children.Add(p);
                Ellipse theDot = new Ellipse() { Width = dotSize, Height = dotSize, Stroke = Brushes.Blue, Fill = Brushes.Blue };
                Canvas.SetTop(theDot, y - halfDotSz);
                Canvas.SetLeft(theDot, w1 - halfDotSz);
                bombeCanvas.Children.Add(theDot);
                y += WireChannelWidth;
                w1 += WireChannelWidth;
                Joints.Join(busWires[rightBus, i], p);
                Joints.Join(theDot, p);
            }
        }

        private void updateScramblerCrossConnects()
        {
            string txt = tbWindow.Text.ToUpper().Trim();
            if (txt.Length != 3)
            {
                MessageBox.Show("Window settings must be three letters. Only A-G allowed");
                return;
            }
            for (int i = 0; i < 3; i++)
            {
                char c = txt[i];
                if (c < 'A' || c > 'H')
                {
                    MessageBox.Show("Window settings invalid. Only A-G allowed");
                    return;
                }
            }
            int baseIndex = Scrambler.FromWindowView(txt);
            for (int i = 0; i < Scramblers.Count; i++)
            {
                Canvas cnvs = Scramblers[i].Tag as Canvas;
                int stepsAhead = Scramblers[i].StepOffsetInMenu;
                int myIndex = (baseIndex + stepsAhead) % 512;
                string map = Scramblers[i][myIndex];
                string myWindowText = $"{Scrambler.ToWindowView(myIndex)} (+{stepsAhead})";
                cnvs.Children.Clear();
                Label lbl = new Label()
                {
                    Content = myWindowText,
                    FontFamily = new FontFamily("Consolas"),
                    FontStyle = FontStyles.Normal,
                    FontWeight = FontWeights.Bold,
                    FontSize = 12,
                };
                Canvas.SetLeft(lbl, -2);
                Canvas.SetTop(lbl, -17);
                cnvs.Children.Add(lbl);

                int leftBus = Scramblers[i].LeftBus;
                int rightBus = Scramblers[i].RightBus;

                for (int w = 0; w < 8; w++) // map each wire
                {
                    double y1 = WireChannelWidth / 2 + WireChannelWidth * w;
                    double y2 = WireChannelWidth / 2 + WireChannelWidth * (map[w] - 'A');
                    Line wire = new Line() { X1 = 0, X2 = cnvs.Width, Y1 = y1, Y2 = y2, Stroke = Brushes.Blue };
                    cnvs.Children.Add(wire);
                    Joints.Join(busWires[leftBus, w], wire);
                    Joints.Join(busWires[rightBus, map[w] - 'A'], wire);
                }

            }
        }
        #endregion

        #region Diagonal Board:  Build the diagonal board wiring, provide switches to selectively disconnect lines, etc.

        // Why the diagonal board works and is justified is an tricky concept for me.  So a lot of 
        // tedious over-engineered effort around this feature to provide an interactive playground. 

        private void makeDiagonalBoard()
        {
            // There are 28 wires to place on the diagonal board. We add a switch to each.  
            // Thanks to Sue for tediously producing this layout table.
            string[] layout = {
                "AB15",
                "AC14",
                "AD12",
                "AE9",
                "AF5",
                "AG2",
                "AH0",
                "BC15",
                "BD13",
                "BE11",
                "BF8",
                "BG4",
                "BH1",
                "CD15",
                "CE14",
                "CF10",
                "CG7",
                "CH3",
                "DE15",
                "DF13",
                "DG12",
                "DH6",
                "EF15",
                "EG14",
                "EH11",
                "FG15",
                "FH13",
                "GH15"
            };
            if (diagonalSwitchClosed == null) // this code done in First time only, after this the table is established and updated
            {
                diagonalSwitchClosed = new Dictionary<string, bool>();
                foreach (string s in layout)
                {
                    string wireName = s.Substring(0,2);
                    diagonalSwitchClosed.Add(wireName, true);
                }
            }

            // Put a couple of quick-change buttons on the canvas:



            foreach (string s in layout)
            {
                string wireName = s.Substring(0, 2);
                bool isClosed = diagonalSwitchClosed[wireName];
                int src = s[0] - 'A';
                int dst = s[1] - 'A';

                int chan = int.Parse(s.Substring(2));
                int switchPos = xFor(src, 12);

                double x0 = xFor(src, dst);
                double x4 = xFor(dst, src);

                double y = (chan + 2) * WireChannelWidth;  // y coordinate of the channel



                string toolTip = $"{uppers[src]}.{lowers[dst]} {leftRightArrow} {uppers[dst]}.{lowers[src]} ";
                Canvas theSwitch = makeSwitch(switchPos, y, isClosed, toolTip, wireName);

                Brush bWire = isClosed ? Brushes.Blue : Brushes.Gray;

                // There is a cheat here.The vertical bus wires are extended or cut to exactly terminate at the crosswire channel.
                busWires[src, dst].Y2 = y;
                busWires[dst, src].Y2 = y;


                double leftedge = Canvas.GetLeft(theSwitch);
                double rightEdge = leftedge + theSwitch.Width;
                Line p = new Line() { X1 = x0, X2 = leftedge, Y1 = y, Y2 = y, Stroke = bWire, StrokeThickness = WireThickness };
                Canvas.SetLeft(p, 0);
                Canvas.SetTop(p, 0);
                bombeCanvas.Children.Add(p);

                Line pRight = new Line() { X1 = rightEdge, X2 = x4, Y1 = y, Y2 = y, Stroke = bWire, StrokeThickness = WireThickness };
                Canvas.SetLeft(pRight, 0);
                Canvas.SetTop(pRight, 0);
                bombeCanvas.Children.Add(pRight);
               
                if (isClosed)
                {
                    Joints.Join(busWires[src, dst], p);
                    Joints.Join(busWires[dst, src], pRight);
                    Joints.Join(p, pRight);
                }


                //if (isClosed)
                //{
                //    // There is a cheat here.The vertical bus wires are extended or cut to exactly terminate at the crosswire channel.
                //    busWires[src, dst].Y2 = y;
                //    busWires[dst, src].Y2 = y;


                //    double leftedge = Canvas.GetLeft(theSwitch);
                //    double rightEdge = leftedge + theSwitch.Width;
                //    Line p = new Line() { X1 = x0, X2 = leftedge, Y1 = y, Y2 = y, Stroke = Brushes.Blue, StrokeThickness = WireThickness };
                //    Canvas.SetLeft(p, 0);
                //    Canvas.SetTop(p, 0);
                //    bombeCanvas.Children.Add(p);
                //    Joints.Join(busWires[src, dst], p);

                //    Line pRight = new Line() { X1 = rightEdge, X2 = x4, Y1 = y, Y2 = y, Stroke = Brushes.Blue, StrokeThickness = WireThickness };
                //    Canvas.SetLeft(pRight, 0);
                //    Canvas.SetTop(pRight, 0);
                //    bombeCanvas.Children.Add(pRight);
                //    Joints.Join(busWires[dst, src], pRight);
                //    if (isClosed)
                //    {
                //        Joints.Join(p, pRight);
                //    }
                //}
                bombeCanvas.Children.Add(theSwitch); // Put it down last so it stays foremost.
            }
        }

        private Canvas makeSwitch(int x, double y, bool isClosed, string myToolTip, string wireName)
        {   double cWidth = 16;
            double cHeight = 6;
            double delta = isClosed ? cHeight : 2.5;
            double y2 = y;
            if (!isClosed)
            {
                y2 += delta;
            }
            Canvas cnvs = new Canvas() { Width = cWidth, Height = cHeight, Background = Brushes.Transparent };
            Line sw = new Line() { X1 = 0, X2 = cWidth, Y1 = cHeight-2, Y2 = delta-2, Stroke = Brushes.Black, StrokeThickness = WireThickness };
            Canvas.SetLeft(sw, 0);
            Canvas.SetTop(sw, 0);
            cnvs.Children.Add(sw);
            double eSz = 5;
            Ellipse eLeft = new Ellipse() { Width = eSz, Height = eSz, Stroke = Brushes.Black, Fill = Brushes.Black };
            Canvas.SetLeft(eLeft, 0-eSz);
            Canvas.SetTop(eLeft, cHeight-eSz/2-2);
            cnvs.Children.Add(eLeft);

            Ellipse eRight = new Ellipse() { Width = eSz, Height = eSz, Stroke = Brushes.Black, Fill = Brushes.Black };
            Canvas.SetLeft(eRight, cWidth);
            Canvas.SetTop(eRight,  cHeight-eSz/2-2);
            cnvs.Children.Add(eRight);

            Canvas.SetLeft(cnvs, x);
            Canvas.SetTop(cnvs, y - cHeight+2);
            cnvs.ToolTip = myToolTip;
            cnvs.Tag = wireName;
            cnvs.MouseUp += DiagonalSwitch_MouseUp;
            cnvs.Cursor = Cursors.Hand;
            return cnvs;
        }

        private void DiagonalSwitch_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Canvas elem = sender as Canvas;
            string wireName = elem.Tag as String;
            diagonalSwitchClosed[wireName] = !diagonalSwitchClosed[wireName];
            RebuildBombe();
        }

        private void btnCloseAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (string wireName in diagonalSwitchClosed.Keys)
            {
                diagonalSwitchClosed[wireName] = true;
            }
            RebuildBombe();
        }

        private void btnToggleAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (string wireName in diagonalSwitchClosed.Keys)
            {
                diagonalSwitchClosed[wireName] = !diagonalSwitchClosed[wireName]; 
            }
            RebuildBombe();
        }

        private void btnOpenAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (string wireName in diagonalSwitchClosed.Keys)
            {
                diagonalSwitchClosed[wireName] = false;
            }
            RebuildBombe();
        }

        private void cbUseDiagonalBoard_Click(object sender, RoutedEventArgs e)
        {
            RebuildBombe();
        }

        #endregion

        #region Buses:   Build the bus wiring
        void makeBuses()
        {
            int topY = DiagonalBoardMargin;
            for (int bus = 0; bus < 8; bus++)
            {
                for (int wire = 0; wire < 8; wire++)
                {
                    int x0 = xFor(bus, wire);
                    Line p = new Line() { X1 = x0, X2 = x0, Y1 = botY, Y2 = topY, Stroke = Brushes.Blue, StrokeThickness = WireThickness };

                    busWires[bus, wire] = p;
                    Canvas.SetLeft(p, 0);
                    Canvas.SetTop(p, 0);
                    bombeCanvas.Children.Add(p);
                }

                // Labels near the bottom of each bus
                Label name = new Label() { Foreground = Brushes.Black, Content = (char)('A' + bus), FontFamily = new FontFamily("Consolas"), FontSize = 20, FontWeight = FontWeights.Bold };
                Canvas.SetLeft(name, xFor(bus, -3));
                Canvas.SetTop(name, botY -14);
                bombeCanvas.Children.Add(name);

                Label minorNames = new Label() { Foreground = Brushes.Black, Content = "abcdefgh", FontFamily = new FontFamily("Consolas"), FontSize =13, FontWeight = FontWeights.Bold };
                Canvas.SetLeft(minorNames, xFor(bus, -1) -2);
                Canvas.SetTop(minorNames, botY - 7);
                bombeCanvas.Children.Add(minorNames);
            }
        }

        #endregion

        #region Other GUI handlers to single-step the scramblers, etc.
        private void btnFwd_Click(object sender, RoutedEventArgs e)
        {
            int win = Scrambler.FromWindowView(tbWindow.Text);
            win = (win + 1) % 512;
            tbWindow.Text = Scrambler.ToWindowView(win);
            RebuildBombe();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            int win = Scrambler.FromWindowView(tbWindow.Text);
            win = (512 + win - 1) % 512;
            tbWindow.Text = Scrambler.ToWindowView(win);
            RebuildBombe();
        }

        private void cbReplaceSpaces_Click(object sender, RoutedEventArgs e)
        {
            RecoverMessage();
        }

        private void tbWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RebuildBombe();
            }
        }

        private void Crib_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RebuildBombe();
            }
        }

        private void Cipher_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RebuildBombe();
            }
        }

        #endregion

        #region // Automatic timer stepping through all possibilities, stopping on candidate solutions


        private void isTestRegisterTriggered()
        {
            // VccSource is part of my test register, so we test where attaches
            int hotCount = 0;
            int testBus = VccAttachesAt / 8;
            for (int wire = 0; wire < 8; wire++)
            {
                if (Joints.IsLive(busWires[testBus, wire]))
                {
                    hotCount++;
                }
            }
            if (hotCount ==1 || hotCount == 7)
            {
                stepTimer.IsEnabled = false;
            }
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            tbWindow.Text = "AAA";
            stepTimer.IsEnabled = true;
        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {
            stepTimer.IsEnabled = true;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            stepTimer.IsEnabled = false;
        }

        private void StepTimer_Tick(object? sender, EventArgs e)
        {
            int index = Scrambler.FromWindowView(tbWindow.Text);
            index = (index + 1) % 256;
            tbWindow.Text = Scrambler.ToWindowView(index);
            if (index == 0)
            {
                stepTimer.IsEnabled = false;
            }
            RebuildBombe();
        }
        #endregion
    }
}
