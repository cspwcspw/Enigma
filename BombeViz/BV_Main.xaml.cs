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
using System.Windows.Threading;
using Utils;

namespace BombeViz
{

    public partial class MainWindow : Window
    {

        public const int NCubed = Enigma.NCubed;

        public const int padHeight = 18;
        public const int padWidth = padHeight * 3;

        public const int lineShadowWidth = 7;

        const int topMargin = 20;
        const int leftMargin = 60;
        const double CoreHeight = 600;
        const double CoreWidth = 240;
        const double vPadSpacing = CoreHeight / 27;

        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
       
        const int feedbackChannelWidth = lineShadowWidth + 4;

        const int feedbackChannelAreaWidth = 13 * feedbackChannelWidth;
        const int leftMarginPlusChannel = leftMargin + feedbackChannelAreaWidth;
        const int topMarginPlusChannel = topMargin + feedbackChannelAreaWidth;

   

        Enigma theMachine;
        int numCores;

        double[] topYPad;    // Pre-computed layout coordinates
        double[] midYPad;

        Canvas[] coreCanvases;  // Array of background canvases one per core, they live on the BackLayer

        Label[,] pads;          // each core has 26 pads, they live on the topLayer

        PointCollection[] wireRoutes;  // Each wireRoute has points to move through the cores, exit, and feed back.
                                       // The routes are used for polylines.  Single polylines live on the middleLayer.
                                       // Fat animated polylines follow the same routes but live on the backLayer

        Polyline[] shadows = new Polyline[26];

        int[] groupOutputMap = new int[26]; // How each pad maps from input to output across the whole group of Enigmas

        int[] coreStepOffsets;            
        string[] coreMaps;

        const char arrow = '\u2192';

        Label[] titleLabels;

        List<Polyline> animatables = new List<Polyline>();

        Brush padNormalBackground = Brushes.Beige;
        Brush padHighlightBackground = Brushes.Yellow;


        Brush[] palette = { Brushes.LightPink, Brushes.Lavender, Brushes.Khaki, Brushes.AliceBlue, Brushes.LightSalmon,
           Brushes.LightSteelBlue,  Brushes.LightGray, Brushes.LightCyan, Brushes.LightSkyBlue, Brushes.Linen};


        DispatcherTimer animator;

        public MainWindow()
        {
            InitializeComponent();
            BackLayer.Background = Brushes.LightGreen;
            MiddleLayer.Background = Brushes.Transparent;
            FrontLayer.Background = Brushes.Transparent;

            animator = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(100), IsEnabled = true };
            animator.Tick += Animator_Tick;

            theMachine = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.III, new Plugboard());


            RotorWindows.Text = theMachine.VisibleInWindows;
            MachineDescription.Content = theMachine.FullDescription;


            coreStepOffsets = new int[] { 1, 12, 14, 15 };

            numCores = coreStepOffsets.Length;
            coreMaps = new string[numCores];
            setupBombeGroup();
            refreshRotorView();
            theMachine.ChangeRotorsToShow("SUE");
            Apply_Click(new object(), new RoutedEventArgs());
        }

        private void refreshRotorView()
        {
            RotorWindows.Text = theMachine.VisibleInWindows;
        }

        // Polyline yellowPolyline;
        private void Animator_Tick(object? sender, EventArgs e)
        {
            foreach (Polyline pl in animatables) {
                pl.StrokeDashOffset += 9;
            }
        }

        double leftOfCore(int c)
        {
            return leftMarginPlusChannel + c * (CoreWidth + padWidth / 2);
        }

        double rightOfCore(int c)
        {
            return leftOfCore(c) + CoreWidth;
        }


        void setupBombeGroup()
        {
            
            coreCanvases = new Canvas[numCores];

            this.Width = (CoreWidth+padWidth)*numCores + 2 * feedbackChannelAreaWidth + 2 * leftMargin;
            this.Height = 720 + 2 * feedbackChannelAreaWidth;
            titleLabels = new Label[numCores];
            for (int c=0; c < numCores; c++)
            {
                Canvas cnvs = new Canvas() { Background = Brushes.LightGray, Width=CoreWidth, Height=CoreHeight};
                Label title = new Label() { Content = $"Offset={coreStepOffsets[c]}",FontSize=14, FontWeight=FontWeights.Bold  };
                titleLabels[c] = title;
                Canvas.SetLeft(title, 60);
                Canvas.SetTop(title, -4);
                cnvs.Children.Add(title);

                Canvas.SetLeft(cnvs, leftOfCore(c));
                Canvas.SetTop(cnvs, topMarginPlusChannel);
                coreCanvases[c] = cnvs;
                BackLayer.Children.Add(cnvs);
            }

            // Set up some easy arrays for consistent positioning
            topYPad = new double[26];
            midYPad = new double[26];
            for (int i = 0; i < 26; i++)
            {
                topYPad[i] = topMarginPlusChannel + (i+1) * vPadSpacing;
                midYPad[i] = topYPad[i] + padHeight / 2;
            }

            // Now create an entry pad for each position on each core Enigma
            pads = new Label[numCores, 26];
            for (int c = 0; c < numCores; c++)
            {
                for (int i = 0; i < 26; i++)
                {
                    Label e = new Label()
                    {
                        Height = padHeight,
                        Width = padWidth,
                        Content = $"{alphabet[i]}{arrow}??",
                        Background = padNormalBackground,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        FontFamily = new FontFamily("Consolas"),
                        FontStyle = FontStyles.Normal,
                        FontSize = 14,
                        Padding = new Thickness(4, 0, 0, 0),
                    };

                    Canvas.SetLeft(e, Canvas.GetLeft(coreCanvases[c])  -padWidth / 2);
                    Canvas.SetTop(e, topYPad[i]); // topMargin + i * vSpacing);
                    FrontLayer.Children.Add(e);
                    pads[c,i] = e;

                    // Some special treatment for leftmost set of pads: make them clickable
                    if (c == 0)
                    {
                        e.Tag = i;
                        e.MouseUp += E_MouseUp;
                    }
                }
            }

            // Add some more minor labelling, to the exits of the last core

            for (int i = 0; i < 26; i++)
            {
                Label e = new Label()
                {
                    Height = padHeight,
                    Width = padWidth,
                    Content = $"{alphabet[i]}",
                  //  Background = Brushes.Beige,
                 //   BorderBrush = Brushes.Black,
                 //   BorderThickness = new Thickness(1),
                    FontFamily = new FontFamily("Consolas"),
                    FontStyle = FontStyles.Normal,
                    FontSize = 14,
                    Padding = new Thickness(4, 0, 0, 0),
                };

                Canvas.SetLeft(e, rightOfCore(numCores-1));
                Canvas.SetTop(e, topYPad[i]); 
                FrontLayer.Children.Add(e);
            }

        }

        // Respond to the mouseClick on one of the leftmost pads
        private void E_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Label thePad = sender as Label;
            if (thePad == null)
            {
                return;
            }
            int padNum = (int) thePad.Tag;
            Polyline theShadow = shadows[padNum];
            if (animatables.Contains(theShadow))
            {
                theShadow.Stroke = Brushes.SkyBlue;
                animatables.Remove(theShadow);
            }
            else
            {
                theShadow.Stroke = Brushes.Red;
                animatables.Add(theShadow);
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            int indx = Enigma.GetIndexFrom(RotorWindows.Text.ToUpper().Trim());
            updateWiring(indx);
        }

        private void StepBack_Click(object sender, RoutedEventArgs e)
        {
            int indx = (theMachine.CoreIndex + NCubed - 1) % NCubed;
            updateWiring(indx);
        }

        private void StepFwd_Click(object sender, RoutedEventArgs e)
        {
            int indx = (theMachine.CoreIndex + 1) % NCubed;
            updateWiring(indx);
        }

        private void updateWiring(int indx)
        {
            theMachine.ChangeRotorsToShow(indx);
            refreshRotorView();  // Put the current rotor setting into the GUI textbox

            for (int c = 0; c < numCores; c++)
            {
                int ofs = coreStepOffsets[c];
                string s = Enigma.IndexToView(indx + ofs);
                string t = $"Offset {ofs}:  [{s}]";
                titleLabels[c].Content = t;
            }

            // Update text at all the machine entry labels to show how each enigma maps the character
            for (int c = 0; c < numCores; c++)
            {
                int coreRow = (coreStepOffsets[c] + indx) % NCubed;
                coreMaps[c] = theMachine[coreRow];
            }

            wireRoutes = new PointCollection[26];
            MiddleLayer.Children.Clear();


            // Remove all polylines from the backlayer.  Leave the other stuff.
            for (int i = BackLayer.Children.Count-1; i >= 0; i--)
            {
                Polyline pl = BackLayer.Children[i] as Polyline;
                if (pl != null)
                {
                    BackLayer.Children.RemoveAt(i);
                }
            }

            animatables.Clear();
            DoubleCollection dd = new DoubleCollection() { 6, 1, 1, 1, 1, 1 };  // For line dots and dashes

            for (int i = 0; i < 26; i++)
            {
                wireRoutes[i] = new PointCollection();

                PointCollection points = wireRoutes[i];
                int padNum = i;
                Point startAt = new Point(leftOfCore(0), midYPad[padNum]);
                points.Add(startAt);
                points.Add(new Point(leftOfCore(0) + padWidth, midYPad[padNum]));
                for (int c = 0; c < numCores; c++)
                {
                    padNum = coreMaps[c][padNum] - 'A';
                    Point pt = new Point(rightOfCore(c) - padWidth / 2, midYPad[padNum]);
                    points.Add(pt);
                    points.Add(new Point(leftOfCore(c + 1) + padWidth, midYPad[padNum]));
                }

                addFeedbackWiring(points, padNum);

                // Remember the "group mapping" of a number of serialized enigmas
                groupOutputMap[i] = padNum;

                Polyline pl = new Polyline() { Stroke = Brushes.SlateBlue, StrokeThickness = 1, Points = points };
                MiddleLayer.Children.Add(pl);

                // Now build an underlying shadow current-flow path on the back layer
                Polyline shadow = new Polyline() { Stroke = Brushes.SkyBlue, StrokeThickness = 3, StrokeDashArray = dd, Points = points };
                BackLayer.Children.Add(shadow);
                shadows[i] = shadow;
            }

            // Update text at all the machine entry labels to show how each enigma maps the character
            for (int c = 0; c < numCores; c++)
            {
                for (int i = 0; i < 26; i++)
                {
                    if (c == 0)
                    {  
                        pads[c, i].Content = $"{alphabet[i]}{arrow}{coreMaps[c][i]}➠{(char)('A' + groupOutputMap[i])}";
                      //  pads[c, i].Content = $"{alphabet[i]}{arrow}{coreMaps[c][i]}➾{(char)('A' + groupOutputMap[i])}";
                        // The single isolated cycle we are looking for.  Hightlight the label.
                        pads[c, i].Background = (i == groupOutputMap[i] ? padHighlightBackground : padNormalBackground);
                    }
                    else
                    {
                        pads[c, i].Content = $"{alphabet[i]}{arrow}{coreMaps[c][i]}";
                    }
                }
            }
        }

        private void addFeedbackWiring(PointCollection points, int padNum)
        {
            // Create points to wrap the wires from the output of the last machine back to the corresponding iput of the first machine.

            Point p = points[points.Count - 1];

            if (padNum < 13)
            {
                double x1 = p.X + padNum * feedbackChannelWidth;
                Point extensionRight = new Point(x1, p.Y);
                points.Add(extensionRight);

                double upY = (14-padNum) * feedbackChannelWidth;
                points.Add(new Point(x1, upY));
                double x0  = (14-padNum) * feedbackChannelWidth;
                points.Add(new Point(x0, upY));
                points.Add(new Point(x0, p.Y));
                points.Add(new Point(leftOfCore(0), p.Y));
            }
            else
            {
                double x1 = p.X + (25-padNum) * feedbackChannelWidth;
                Point extensionRight = new Point(x1, p.Y);
                points.Add(extensionRight);

                double bot = Canvas.GetTop(coreCanvases[0]) + CoreHeight;
                double y0 = bot  + (26-padNum) * feedbackChannelWidth;
                points.Add(new Point(x1, y0));
                double x0 = (padNum - 11) * feedbackChannelWidth;
                points.Add(new Point(x0, y0));
                points.Add(new Point(x0, p.Y));
                points.Add(new Point(leftOfCore(0), p.Y));
            }
        }
    }
}
