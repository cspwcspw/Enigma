using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Utils;

// Purpose of this program is to help us animate and understand the Bombe, particularly the impact of Welchman's
// diagonal board. So I'm drawing inspiration, etc. from http://www.ellsbury.com/bombe3.htm 

// In particular, the tiny Bombe here only has an 8-symbol alphabet.  In my case, three 8-wire rotors and a reflector,
// and all scramblers are "hardwired" to always use the same mapping at each of their 512 rotor positions AAA - HHH.

namespace TinyBombe
{

    public partial class MainWindow : Window
    {

        // Layout is on a (conceptual) grid of rows and columns.  Each column contains a bus of wires,
        // and a channel to the right for scramblers.
        // Row numbering excludes the area at the top for the diagonal board.

        // Layout is mainly lots of hacks and magic coordinates until I think it looks pretty.
        // It is easier than thinking the deep conceptual stuff about why the Bombe worked and the
        // actual crypto issues.

        const int WindowTopMargin = 40; // reserve a bit of space for menus, buttons, etc above the canvas.

        const int ColWidth = 140;
        const int RowHeight = 76;

        const int WireChannelWidth = 7;
        const int ScramblerSize = 8*WireChannelWidth;

        const int LeftMargin = 30;
       // The top margin is space on the canvas above the rows is used for layout/wiring of the diagonal board cross-connects.

        const int TopCanvasMargin = 18 * WireChannelWidth;  

        List<Scrambler> Scramblers =  new List<Scrambler>();

        Line[,] busWires;

        Connections Joints = new Connections();

        ScrollViewer theScroller;
        Canvas backLayer;
        int VccAttachesAt = 34; // this is a packed Column*8+Wire telling us where to apply the voltage
        bool VccIsHot = true;   // Persist whether the user wants voltage applied when we advance to other wheel positions

        List<int> scramblerRows;

        int botY;  // Calculated depending on crib length and how many scrambler layout rows are needed

        public MainWindow()
        {
            InitializeComponent();

            // BEACHHEADBEACHHEADBEACHHEADBEACHHEAD->GBEECECBCHCBECFHHEGBEECBGFHCAEAGFFBE at BFG
            // BEACHHEADBEACHHEADBEACHHEADBEACHHEAD->GBEECBGFHCAEAGFFBEAAFFABFDEHHDBADBHB at CAA
            // Test case on first five-letter crib should succeed at 128(CAA) and give false stop at 110(BFG)
            // BEACH -> GBEEC  
            Cipher.Text = "GBEECBGFHCAEAGFFBEAAFFABFDEHHDBADBHB";
            Crib.Text = "BEACH"; // HEAD";
            tbWindow.Text = "BFF";
            ResetToInitialState();
            // And this time only, change the Window size. 
            this.Height = botY + 140;
        

        }

        int xFor(int bus, int wire)  // General helper for x layout
        {
            int x0 = bus * ColWidth + (wire + 1) * WireChannelWidth + LeftMargin;
            return x0;
        }

        // Trying to untangle connections, make new cross-connects in the scrambers, etc.
        // when the rotors move is too messy. Just rebuild the whole GUI from scratch on each step.
        void ResetToInitialState()
        {
            scramblerRows = getScramblerRowsNeeded(Crib.Text, Cipher.Text);
            int maxRow = scramblerRows.Max();
            botY = TopCanvasMargin + (maxRow+1) * RowHeight;
             
            Scramblers.Clear();
            Joints.Clear();

            theScroller = new ScrollViewer() { Margin = new Thickness(0, WindowTopMargin, 0, 0),
                                            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto };
            backLayer = new Canvas()
            {
                Background = Brushes.LightSkyBlue,
                //HorizontalAlignment = HorizontalAlignment.Stretch,
                //  VerticalAlignment = VerticalAlignment.Stretch
                Height = botY + 50,
                Width = ColWidth * 8 + LeftMargin
            };

            theScroller.Content = backLayer;
            mainGrid.Children.Add(theScroller);

            busWires = new Line[8, 8];

            makeBuses();
            makeDiagonalBoard();
            addScramblers(Crib.Text, Cipher.Text, scramblerRows);
            addVoltageSource();
            RecoverMessage();
         
        }

        private void RecoverMessage()
        {
            Scrambler sc = new Scrambler(0, 0, 0);
            sc.Index = Scrambler.FromWindowView(tbWindow.Text);
            string cp = Cipher.Text;
            string plain = sc.EncryptText(cp);
            Recovered.Content = plain;
        }

        private void addVoltageSource()
        {
            // Add the VCC source
            PointCollection pc = new PointCollection() { new Point(0, 0), new Point(-10, 20), new Point(0, 14), new Point(10, 20), new Point(0, 0) };
            Polygon VccTag = new Polygon() { Points = pc, Fill = Brushes.Blue, Stroke = Brushes.Blue };
            VccTag.MouseUp += Tag_MouseUp;
            int busA = VccAttachesAt / 8;
            int wireA = VccAttachesAt % 8;

            Canvas.SetLeft(VccTag, xFor(busA, wireA));
            Canvas.SetTop(VccTag, botY);

            backLayer.Children.Add(VccTag);
            Joints.Join(VccTag, busWires[busA, wireA]);
            if (VccIsHot) // Push the voltage through the wiring
            {
                Joints.MakeItLive(VccTag);
            }
        }

        #region Scramblers - decide what is needed, create them with crib offsets, lay them out, plug them into buses, etc.
        private List<int> getScramblerRowsNeeded(string crib, string cipher)
        {
            List<int> rows = new List<int>();
            Placements places = new Placements();
            for (int i = 0; i < crib.Length; i++)
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
            return rows;
        }

        private void addScramblers(string crib, string cipher, List<int> scramblerRow)
        {
             for (int i = 0; i < crib.Length; i++)
            {
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
            double y0 = row * RowHeight + TopCanvasMargin + 2 * WireChannelWidth;
            Canvas.SetTop(scramblerCanvas, y0);
            backLayer.Children.Add(scramblerCanvas);

            // Plug the scrambler into the buses on either side
            double w0 = xFor(leftBus, 0);
            double y = y0 + WireChannelWidth / 2;
            double dotSize = WireChannelWidth - 2;
            double halfDotSz = dotSize / 2;
            for (int i = 0; i < 8; i++)
            {
                Line p = new Line() { X1 = w0, X2 = x0, Y1 = y, Y2 = y, Stroke = Brushes.Blue };
                backLayer.Children.Add(p);
                Ellipse theDot = new Ellipse() { Width = dotSize, Height = dotSize, Stroke = Brushes.Blue, Fill = Brushes.Blue };
                Canvas.SetTop(theDot, y - halfDotSz);
                Canvas.SetLeft(theDot, w0 - halfDotSz);
                backLayer.Children.Add(theDot);
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
                Line p = new Line() { X1 = x0, X2 = w1, Y1 = y, Y2 = y, Stroke = Brushes.Blue };
                backLayer.Children.Add(p);
                Ellipse theDot = new Ellipse() { Width = dotSize, Height = dotSize, Stroke = Brushes.Blue, Fill = Brushes.Blue };
                Canvas.SetTop(theDot, y - halfDotSz);
                Canvas.SetLeft(theDot, w1 - halfDotSz);
                backLayer.Children.Add(theDot);
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



        private void makeDiagonalBoard()
        {
            // There are 28 wires to place on the diagonal board.  Thanks to Sue for 
            // tediously producing this layout table.
            string[] layout = {"AE9",
                    "AH0", "BH1", "AG2",
                "CH3", "BG4", "AF5", "DH6", "CG7", "BF8",
                "CF10", "EH11", "BE11", "DG12", "AD12", "BD13", "DF13", "FH13", "AC14", "CE14",
                "EG14", "AB15", "BC15", "CD15", "DE15", "EF15", "FG15", "GH15"
             };

            foreach (string s in layout)
            {
                int src = s[0] - 'A';
                int dst = s[1] - 'A';
                int chan = int.Parse(s.Substring(2));

                double x0 = xFor(src, dst);
                double x1 = xFor(dst, src);

                double y = (chan+2) * WireChannelWidth;  // y coordinate of the channel

                // There is a cheat here.The vertical bus wires are extended or cut to exactly terminate at the crosswire channel.
                busWires[src, dst].Y2 = y;
                busWires[dst, src].Y2 = y;

                Line p = new Line() { X1 = x0, X2 = x1, Y1 = y, Y2 = y, Stroke = Brushes.Blue };
                Canvas.SetLeft(p, 0);
                Canvas.SetTop(p, 0);
                backLayer.Children.Add(p);
                Joints.Join(busWires[src, dst], p);
                Joints.Join(busWires[dst, src], p);
            }
        }

        void makeBuses()
        {
            int topY = TopCanvasMargin;
            for (int bus = 0; bus < 8; bus++)
            {
                for (int wire = 0; wire < 8; wire++)
                {
                    int x0 = xFor(bus, wire);
                    Line p = new Line() { X1 = x0, X2 = x0, Y1 = botY, Y2 = topY, Stroke = Brushes.Blue };

                    busWires[bus, wire] = p;
                    Canvas.SetLeft(p, 0);
                    Canvas.SetTop(p, 0);
                    backLayer.Children.Add(p);
                }

                // Labels near the bottom of each bus
                Label name = new Label() { Foreground = Brushes.Black, Content = (char)('A' + bus), FontFamily = new FontFamily("Consolas"), FontSize = 20, FontWeight = FontWeights.Bold };
                Canvas.SetLeft(name, xFor(bus, -3));
                Canvas.SetTop(name, botY -14);
                backLayer.Children.Add(name);
            }
        }

        private void Tag_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Shape p = sender as Shape;
            if (p.Stroke == Brushes.Red)
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
           // addScramblers("BEACHHEAD", Cipher.Text);   
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            ResetToInitialState();
        }


    }
}
