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
// diagonal board. So I'm following layout, drawing inspiration, etc. from http://www.ellsbury.com/bombe3.htm 

// In particular, the tiny Bombe only has an 8-symbol alphabet.  In my case, three 8-wire rotors and a reflector,
// and all scramblers are "hardwired" to always use the same mapping at each of their 512 rotor positions AAA - HHH.

namespace TinyBombe
{
    
    public partial class MainWindow : Window
    {

        // Layout is on a (conceptual) grid of rows and columns, the buses A,B,C ..H
        // run vertically in even numberd columns, the scramblers are placed in
        // odd number columns, and cross-connect to he buses.
        const int Rows = 12;
        const int Cols = 15;

        const int ColWidth = 70;
        const int RowHeight = 50;

        const int LeftMargin = 50;
        const int TopMargin = 20;

        const int WireChannelWidth = 7;

        Line[,] busWires;


        ScrollViewer theScroller;
        Canvas backLayer;
        Canvas overlay; // is a child of teh backLayer;
        public MainWindow()
        {
            InitializeComponent();
            theScroller = new ScrollViewer() { Margin = new Thickness(0,40,0,0)};
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
           
        }

        private void makeDiagonalBoard()
        {
            // There are 28 wires to place on the diagonal board.  Thanks to Sue for 
            // tediously producing this layout table.
            string[] layout = {"AH0", "BH1", "AG2",
                "CH3", "BG4", "AF5", "DH6", "CG7", "BF8", "AE9",
                "CF10", "EH11", "BE11", "DG12", "AD12", "BD13", "DF13", "FH13", "AC14", "CE14",
                "EG14", "AB15", "BC15", "CD15", "DE15", "EF15", "FG15", "GH15" 
             };


            foreach (string s in layout)
            {
                int src = s[0] - 'A';
                int dst = s[1] - 'A';
                int chan = int.Parse(s.Substring(2));

                double x0 = src * (2 * ColWidth) + dst * WireChannelWidth + LeftMargin;
                double x1 = dst * (2 * ColWidth) + src * WireChannelWidth + LeftMargin;

                double y = chan * WireChannelWidth + TopMargin;  // y soordinate of the channel

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
 
                int x0 = LeftMargin + 2 * ColWidth * bus;

                for (int wire = 0; wire < 8; wire++)
                {
                    Line p = new Line() { X1 = x0, X2 = x0, Y1 = botY, Y2 = topY, Stroke = Brushes.Blue };

                    busWires[bus, wire] = p;
                    Canvas.SetLeft(p, 0);
                    Canvas.SetTop(p, 0);
                    backLayer.Children.Add(p);
                    x0 += WireChannelWidth;
                  }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            busWires[2, 2].Y2 = 300;
        }
    }
}
