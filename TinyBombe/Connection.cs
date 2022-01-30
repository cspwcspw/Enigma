using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TinyBombe
{

    /// <summary>
    /// Represents an electrical connection between two things (wires, pads, scrambler pin, etc). 
    /// Electrical components can all be Red or Blue.  We don't keep Model state info
    /// separate from View (e.g. MVC).  Here the Shape Stroke and Fill colours keep 
    /// track of the electrial "hot or not".
    /// Having a collection of bindings allows us to propagate a voltage around
    /// the TinyBombe.
    /// </summary>
    public class Connections : List<Connection>
    {
        public Connections() : base()
        { }

        public void Join(Shape a, Shape b)
        {
            this.Add(new Connection(a, b));
        }

        internal void MakeItLive(Shape p)
        {
           p.Stroke = Brushes.Red;
           p.Fill = Brushes.Red;
           propagateLiveThroughJoints(p);
        }
        bool isLive(Shape p)
        {
            return p.Stroke == Brushes.Red;
        }
        private void propagateLiveThroughJoints(Shape p)
        {
            foreach (Connection c in this)
            {             
                if (p.Equals(c.A))
                {
                    Shape s = c.B;
                    if (! isLive(s))
                    {
                        MakeItLive(s);
                        propagateLiveThroughJoints(s);
                    }
                }
                else if (p.Equals(c.B))
                {
                    Shape s = c.A;
                    if (!isLive(s))
                    {
                        MakeItLive(s);
                        propagateLiveThroughJoints(s);
                    }
                }
            }
        }

        internal void DisconnectAllVoltages()
        {
           foreach (Connection c in this)
            {
                c.A.Stroke = Brushes.Blue;
                c.A.Fill = Brushes.Blue;
                c.B.Stroke = Brushes.Blue;
                c.B.Fill = Brushes.Blue;
            }
        }
    }

    public class Connection
    {
        public Shape A { get; set; }
        public Shape B { get; set; }

        public Connection(Shape a, Shape b)
        {
            A = a;
            B = b;
        }
    }
}
