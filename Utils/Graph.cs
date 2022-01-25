using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class Graph
    {
        
        Dictionary<char, List<Edge>> edgesDict;
        public Graph(string cipher, string crib)
        {
            int n = cipher.Length;
            Debug.Assert(n == crib.Length);
            Debug.Assert(n < 64);

            cipher = cipher.ToUpper();
            crib = crib.ToUpper();
            edgesDict = new Dictionary<char, List<Edge>>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                edgesDict[c] = new List<Edge>();
            }

            for (int i = 0; i < n; i++)
            {
                Debug.Assert(cipher[i] != crib[i]);
                char x = cipher[i];
                char y = crib[i];

                // If I already have an edge from X to Y I won't put it in again.
                if (!hasEdge(edgesDict[x], y))
                {

                    Edge e1 = new Edge() { src = x, dst = y, posn = i };
                    Edge e2 = new Edge() { src = y, dst = x, posn = i };
                    edgesDict[x].Add(e1);
                    edgesDict[y].Add(e2);
                }
            }

        }

        private bool hasEdge(List<Edge> edges, char y)
        {
            foreach(Edge e in edges)
            {
                if (e.dst == y) return true;
            }
            return false;
        }

        LoopGatherer loopsFound;
        public LoopGatherer findLoops()
        {

            loopsFound = new LoopGatherer();
            for (char x = 'A'; x <= 'Z'; x++)
            {
                depthFirstSearch(x, new GPath());
            }
            loopsFound.Sort();
            return loopsFound;
        }

        private void depthFirstSearch(char thisNode, GPath path)
        {
            int n = path.edges.Count;
            if (n > 1 && path.edges[0].src == path.edges[n - 1].dst)
            {
                GPath myClone = path.Clone();
                bool added = loopsFound.PerhapsAdd(myClone);
             //   if (added)
             //   {
             //       Console.WriteLine($"Took      {myClone}");
             //   }
             //   else
             //   {
             ////       Console.WriteLine($"Duplicate {myClone}");
             //   }
                return;
            }
            List<Edge> children = edgesDict[thisNode];
            foreach (Edge e in children)
            {   int pos = e.posn;
                if (!path.visited[pos]) // if we have not already used this edge in the path
                {
                    path.visited[pos] = true;  // use it
                    path.edges.Add(e);
                    // look further
                    depthFirstSearch(e.dst, path);
                    // Reverse the damage we did to follow this child
                    path.visited[pos] = false;  
                    path.edges.RemoveAt(n);
                }
            }
        }
    }

    public class GPath:IComparable, IEquatable<GPath>
    {
        public BitArray visited = new BitArray(64);
        public List<Edge> edges { get; set; } = new List<Edge>();

        internal GPath Clone()
        {
            GPath result = new GPath();
            result.visited = (BitArray) this.visited.Clone();
            foreach (Edge e in edges)
            {
                result.edges.Add(e);
            }
            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"(sz={edges.Count}) ");
            foreach(Edge e in edges)
            {
                sb.Append($"{e.ToString()} ");
            }
            ulong fingerprint = visitedAsUlong();
            sb.Append(fingerprint);


            return sb.ToString();
        }

        public ulong visitedAsUlong()
        {
            ulong result = 0;
            for (int i = 63; i >= 0; i--)
            {
                result = (result << 1) + (ulong)(visited[i] ? 1 : 0);
            }
            return result;
        }

        public int CompareTo(object? obj)
        {  
           
            GPath other = obj as GPath;
            ulong xx = visitedAsUlong();
            ulong yy = other.visitedAsUlong();

            return xx.CompareTo(yy);

            //int result = edges.Count.CompareTo(other.edges.Count);
            //if (result != 0)
            //{
            //    Debug.Assert(xx != yy);
            //    return result;
            //}
            //for (int i = 0; i < edges.Count; i++)
            //{
            //    int r2 = edges[i].CompareTo(other.edges[i]);
            //    if (r2 != 0)
            //    {
            //        Debug.Assert(xx != yy);
            //        return r2;
            //    }
            //}
            //They are equal
            //if (xx != yy)
            //{
            //    Console.WriteLine(this);
            //    Console.WriteLine(other);
            //}
            //return 0;
        }

        public bool Equals(GPath? other)
        {
            return this.CompareTo(other) == 0;
        }
    }

    public class LoopGatherer
    {
        public List<GPath> TheLoops { get; set; } = new List<GPath> ();

        public bool PerhapsAdd(GPath myClone)
        {
   
            int minp = 10000;
            int minpIndx = -1;

            // Find the index of the minimum position
            for (int i = 0; i < myClone.edges.Count; i++)
            {
                Edge e = myClone.edges[i];
                if (e.posn < minp)
                {
                    minp = e.posn;
                    minpIndx = i;
                }
            }
            // Rotate the edges path so that it starts with the minimum
            List<Edge> leftPart = myClone.edges.GetRange(0, minpIndx);
            List<Edge> rightPart = myClone.edges.GetRange(minpIndx, myClone.edges.Count - minpIndx);
            rightPart.AddRange(leftPart);
         //   Console.WriteLine($"Normalized loop {myClone}");
            myClone.edges = rightPart;
        //    Console.WriteLine($"............... {myClone}");

            if (TheLoops.Contains(myClone))
            {
                return false;
            }
            TheLoops.Add(myClone);
            return true;
        }

        internal void Sort()
        {
            TheLoops.Sort((GPath a, GPath b) => a.edges.Count.CompareTo(b.edges.Count));
           
        }
    }

    public struct Edge:IComparable<Edge>

    {
        public char src { get; set; }
        public char dst { get; set; }
        public int posn { get; set; }

        public int CompareTo(Edge other)
        {
            int result = this.src.CompareTo(other.src);
            if (result != 0) return result;
            return this.dst.CompareTo(other.dst);
        }

        public override string ToString()
        {
            return $"{src}{dst}.{posn.ToString("D2")}";
        }
    }
}
