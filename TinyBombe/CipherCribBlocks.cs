using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TinyBombe
{
    public class CipherCribBlocks: Canvas
    {
        double szH = 22;
        double szW = 14;
        int ClueLength;

        private string text = "";
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
                int n = text.Length;
                if (n > ClueLength)
                {
                    text = text.Substring(0, ClueLength);
                }
                for (int i = 0; i < ClueLength; i++)
                {
                    if (i < n)
                    {
                        Blocks[i].Content = text[i];
                    }
                    else
                    {
                        Blocks[i].Content = ' ';
                    }
                }
            }
        }

        Label[] Blocks;

        public CipherCribBlocks(int num) 
        {
            Text = "Hello";
            ClueLength = num;
            Blocks = new Label[ClueLength];
            for (int i = 0; i < ClueLength; i++)
            {
                Label lbl = new Label()
                {
                    Width = szW,
                    Height = szH,
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(0.5),
                    FontFamily = new FontFamily("Consolas"),
                    FontStyle = FontStyles.Normal,
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Content = (char)('A' + i),
                    Padding = new Thickness(0, 0, 0, 0),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,

                };
                lbl.KeyDown += Lbl_KeyDown;
                lbl.MouseDown += Lbl_MouseDown;
                SetTop(lbl, 0);
                SetLeft(lbl, szW * i);
                Children.Add(lbl);
                Blocks[i] = lbl;
            }
            Width = ClueLength * szW;
            Height = szH;
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;

            MouseDown += CrosswordBlocks_MouseDown;

            KeyDown += CrosswordBlocks_KeyDown;
            KeyUp += CrosswordBlocks_KeyUp;
        }

        private void CrosswordBlocks_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
        }

        private void CrosswordBlocks_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Key k = e.Key;
            char c = (char)k;
        }

        private void CrosswordBlocks_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
        }

        private void Lbl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Label lbl = sender as Label;
            if (lbl.Background != Brushes.Magenta)
            {
                lbl.Background = Brushes.Magenta;
            }
            else
            {
                lbl.Background = Brushes.Transparent;
            }
        }

        private void Lbl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Label lbl = sender as Label;
            if (lbl.BorderBrush != Brushes.Red)
            {
                lbl.BorderBrush = Brushes.Red;
            }
            else
            {
                lbl.BorderBrush = Brushes.Gray;
            }
        }

        internal void DealWithKeypress(object sender, KeyEventArgs e)
        {
            Key k = e.Key;
            char c = (char)k;
        }
    }
}
