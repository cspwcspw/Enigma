using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        const int Rows = 9;
        const int Cols = 8;

        const int ColWidth = 140;
        const int RowHeight = 70;

        const int WireChannelWidth = 7;
        const int ScramblerSize = 8*WireChannelWidth;

        const int LeftMargin = 50;
        const int TopMargin = 20;

        List<Scrambler> Scramblers =  new List<Scrambler>();
        List<Canvas> ScramberCanvases = new List<Canvas>();

        Line[,] busWires;

        Brush[] palette = { Brushes.LightPink, Brushes.Lavender, Brushes.Khaki, Brushes.AliceBlue, Brushes.LightSalmon,
           Brushes.LightSteelBlue,  Brushes.LightGray, Brushes.LightCyan, Brushes.LightSkyBlue, Brushes.Linen};



        ScrollViewer theScroller;
        Canvas backLayer;
        Canvas overlay; // is a child of teh backLayer;
        public MainWindow()
        {
            InitializeComponent();
            theScroller = new ScrollViewer() { Margin = new Thickness(0, 40, 0, 0) };
            backLayer = new Canvas()
            {
                Background = Brushes.LightSkyBlue,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            overlay = new Canvas()
            {
                Background = Brushes.Pink,
                Width = 100,
                Height = 100,
                Margin = new Thickness(20),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            backLayer.Children.Add(overlay);
            theScroller.Content = backLayer;
            mainGrid.Children.Add(theScroller);

            busWires = new Line[8, 8];

            makeBuses();
            makeDiagonalBoard();

            addScrambler(Brushes.SandyBrown, 8, 2, 2, 3, 0);
            addScrambler(Brushes.Tan, 8, 3, 4, 7, 1);
            addScrambler(Brushes.Cyan, 4, 2, 3, 5, 2);
            addScrambler(Brushes.LightSalmon, 5, 0, 3, 5, 3);
            addScrambler(Brushes.LightYellow, 2, 2, 3, 4, 4);
            addScrambler(Brushes.LightPink, 7, 5, 6, 7, 5);
            updateScramblerVisuals();


        }

        private void addScrambler(Brush b, int row, int leftBus, int col, int rightBus, int stepOffset)
        {
            Scramblers.Add(new Scrambler(stepOffset));
            Canvas c = new Canvas() { Background = b, Width = ScramblerSize, Height = ScramblerSize };
            ScramberCanvases.Add(c);
            placeScrambler(c, row, leftBus, col, rightBus);         
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
            }
        }

        void makeBuses()
        {
            int botY = RowHeight * Rows;
            int topY = 15*WireChannelWidth + TopMargin;
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
            }
        }  

        private void button_Click(object sender, RoutedEventArgs e)
        {
         
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            updateScramblerVisuals();
        }

        private void updateScramblerVisuals()
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
                Canvas cnvs = ScramberCanvases[i];
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
                
                for (int w=0; w < 8; w++) // map each wire
                {
                    double y1 = WireChannelWidth / 2 + WireChannelWidth * w;
                    double y2 = WireChannelWidth / 2 + WireChannelWidth * (map[w]-'A');
                    Line wire = new Line() { X1 = 0, X2 = cnvs.Width, Y1 = y1, Y2 = y2, Stroke = Brushes.Blue };
                    cnvs.Children.Add(wire);
                }
                
            }
        }
    }
}
