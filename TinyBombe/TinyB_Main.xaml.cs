using System;
using System.Collections.Generic;
using System.Windows;
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

        // Layout is on a (conceptual) grid of rows and columns, the buses A,B,C ..H
        // run vertically in even numberd columns, the scramblers are placed in
        // odd number columns, and cross-connect to the buses.
        const int Rows = 10;
        const int Cols = 8;

        const int ColWidth = 140;
        const int RowHeight = 76;

        const int WireChannelWidth = 7;
        const int ScramblerSize = 8*WireChannelWidth;

        const int LeftMargin = 50;
        const int TopMargin = 20;

        List<Scrambler> Scramblers =  new List<Scrambler>();
        List<Canvas> ScramblerCanvases = new List<Canvas>();

        Line[,] busWires;

        Brush[] palette = { Brushes.LightPink, Brushes.Lavender, Brushes.Khaki, Brushes.AliceBlue, Brushes.LightSalmon,
           Brushes.LightSteelBlue,  Brushes.LightGray, Brushes.LightCyan, Brushes.LightSkyBlue, Brushes.Linen};

        Connections Joints = new Connections();

        ScrollViewer theScroller;
        Canvas backLayer;
        Canvas overlay; // is a child of the backLayer;
        int  VccAttachesAt = 34;
        int actualRows; 
        List<int> scramblerRows;

        public MainWindow()
        {
            InitializeComponent();

            // BEACHHEAD -> GBEECECBC at BFG
            // BEACHHEAD -> GBEECBGFH at CAA
            // Test case on first five-letter crib should succeed at 128(CAA) and give false stop at 110(BFG)
            // BEACH -> GBEEC  
            Cipher.Text = "GBEECBGFH";
            Crib.Text = "BEACHHEAD";
            tbWindow.Text = "BFF";

            ResetToInitialState();          
        }


        void ResetToInitialState()
        {
            scramblerRows = getScramblerRowsNeeded(Crib.Text, Cipher.Text);
            actualRows = 0;
            foreach (int r in scramblerRows)
            {
                if (r > actualRows)
                {
                    actualRows = r;
                }
            }
             
            Scramblers.Clear();
            ScramblerCanvases.Clear();
            Joints.Clear();
            
            theScroller = new ScrollViewer() { Margin = new Thickness(0, 40, 0, 0) };
            backLayer = new Canvas()
            {
                Background = Brushes.LightSkyBlue,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            theScroller.Content = backLayer;
            mainGrid.Children.Add(theScroller);

            busWires = new Line[8, 8];

            makeBuses();
            makeDiagonalBoard();
            addScramblers(Crib.Text, Cipher.Text);
            addVoltageSource();
        }

        private void addVoltageSource()
        {
            // Add the VCC source
            int botY = RowHeight * Rows;
            PointCollection pc = new PointCollection() { new Point(0, 0), new Point(-10, 20), new Point(0, 14), new Point(10, 20), new Point(0, 0) };
            Polygon VccTag = new Polygon() { Points = pc, Fill = Brushes.Blue, Stroke = Brushes.Blue };
            VccTag.MouseUp += Tag_MouseUp;
            int busA = VccAttachesAt / 8;
            int wireA = VccAttachesAt % 8;

            Canvas.SetLeft(VccTag, xFor(busA, wireA));
            Canvas.SetTop(VccTag, botY);

            backLayer.Children.Add(VccTag);
            Joints.Join(VccTag, busWires[busA, wireA]);
        }

        private void addScrambler(Brush b, int row, int leftBus, int col, int rightBus, int stepOffset)
        {
            Scramblers.Add(new Scrambler(stepOffset,leftBus, rightBus));
            Canvas c = new Canvas() { Background = b, Width = ScramblerSize, Height = ScramblerSize };
            ScramblerCanvases.Add(c);
            placeScrambler(c, row, leftBus, col, rightBus);         
        }

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

        private void addScramblers(string crib, string cipher)
        {
            Placements places = new Placements();
 
             for (int i = 0; i < crib.Length; i++)
                {
                int leftBus = crib[i] - 'A';
                int rightBus = cipher[i] - 'A';
                if (leftBus > rightBus) // wrong way around?
                { int temp = leftBus;
                    leftBus = rightBus;
                    rightBus = temp;
                }
                int col = (leftBus + rightBus) / 2;
                int row = places.PlaceIt(leftBus, col, rightBus);
                // Find first available row where we don't bump into existing scrambler placements
                addScrambler(Brushes.Tan, row, leftBus, col, rightBus, i);
            }
            updateScramblerCrossConnects();
        }

        int xFor(int bus, int wire)
        {
           int x0 = bus * ColWidth + (wire+1) * WireChannelWidth + LeftMargin;
           return x0;
        }

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

                double y = chan * WireChannelWidth + TopMargin;  // y coordinate of the channel

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
            int botY = RowHeight * Rows;
            int topY = 15 * WireChannelWidth + TopMargin;
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

                // Label the bus
                Label name = new Label() { Foreground = Brushes.Black, Content = (char)('A' + bus), FontFamily = new FontFamily("Consolas"), FontSize = 20, FontWeight = FontWeights.Bold };
                Canvas.SetLeft(name, xFor(bus, 2));
                Canvas.SetTop(name, botY + 30);
                backLayer.Children.Add(name);

            }

        }

        private void Tag_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Shape p = sender as Shape;
            if (p.Stroke == Brushes.Red)
            {
                Joints.DisconnectAllVoltages();
            }
            else
            {
                Joints.MakeItLive(p);
            }
         }

        void placeScrambler(Canvas scramblerCanvas, int row, int leftBus, int posCol, int rightBus)
        {
 
            double xMid = (xFor(posCol, 7) + xFor(posCol+1, 0)) / 2; // middle of channel
            double x0 = xMid - scramblerCanvas.Width / 2;
            Canvas.SetLeft(scramblerCanvas, x0);
            double y0 = row * RowHeight;
            Canvas.SetTop(scramblerCanvas,y0);
            backLayer.Children.Add(scramblerCanvas);

            // Plug the scrambler into the buses on either side
            double w0 = xFor(leftBus, 0);
            double y = y0 + WireChannelWidth/2;
            double dotSize = WireChannelWidth-2;
            double halfDotSz = dotSize / 2;
            for (int i=0; i < 8; i++)
            {
                Line p = new Line() { X1 = w0, X2 = x0, Y1 = y, Y2 = y, Stroke = Brushes.Blue };
                backLayer.Children.Add(p);
                Ellipse theDot = new Ellipse() { Width = dotSize, Height = dotSize, Stroke = Brushes.Blue, Fill = Brushes.Blue };
                Canvas.SetTop(theDot, y - halfDotSz);
                Canvas.SetLeft(theDot, w0 - halfDotSz);
                backLayer.Children.Add(theDot);
                y += WireChannelWidth;
                w0 += WireChannelWidth;
                Joints.Join(busWires[leftBus,i],p);
                Joints.Join(theDot, p);
            }

            double w1 = xFor(rightBus, 0);
            y = row*RowHeight + WireChannelWidth / 2;
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            addScramblers("BEACHHEAD", Cipher.Text);   
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            ResetToInitialState();
        }

        private void updateScramblerCrossConnects()
        {
            string txt = tbWindow.Text.ToUpper().Trim();
            if (txt.Length != 3) {
                MessageBox.Show("Window settings must be three letters. Only A-G allowed");
                return;
            }
            for (int i=0; i < 3; i++)
            {
                char c = txt[i];
                if (c < 'A' || c > 'H')
                {
                    MessageBox.Show("Window settings invalid. Only A-G allowed");
                    return;
                }
            }
            int baseIndex = Scrambler.FromWindowView(txt);
            for (int i=0; i < Scramblers.Count; i++)
            {
                Canvas cnvs = ScramblerCanvases[i];
                int stepsAhead = Scramblers[i].StepOffsetInMenu;
                int myIndex = (baseIndex + stepsAhead) % 512 ;
                string map = Scramblers[i][myIndex];
                string myWindowText = $"{Scrambler.ToWindowView(myIndex)} (+{stepsAhead})";
                cnvs.Children.Clear();
                Label lbl = new Label() { Content = myWindowText,
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
                
                for (int w=0; w < 8; w++) // map each wire
                {
                    double y1 = WireChannelWidth / 2 + WireChannelWidth * w;
                    double y2 = WireChannelWidth / 2 + WireChannelWidth * (map[w]-'A');
                    Line wire = new Line() { X1 = 0, X2 = cnvs.Width, Y1 = y1, Y2 = y2, Stroke = Brushes.Blue };
                    cnvs.Children.Add(wire);
                    Joints.Join(busWires[leftBus, w], wire);
                    Joints.Join(busWires[rightBus, map[w] - 'A'], wire);
                }
                
            }
        }
    }
}
