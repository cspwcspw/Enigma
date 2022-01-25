using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class Helpers
    {

        public static List<List<T>> Permutations<T>(List<T> items)
        {
            List<List<T>> result = new List<List<T>>();
            if (items.Count == 0)
            {
                result.Add(new List<T>());
                return result; 
            }
            else
            {
                for (int i=0; i < items.Count; i++)
                {
                    T item = items[i];
                    List<T> clone = new List<T>(items); // Clone it
                    clone.RemoveAt(i);
                    List<List<T>> subResult = Permutations(clone);
                    foreach (List<T> subItem in subResult)
                    {
                        subItem.Insert(0, item);
                        result.Add(subItem);
                    }
                }
            }
            return result;
        }

        public static List<List<T>> ChooseFrom<T>(int numWanted, List<T> items)
        {
            List<List<T>> result = new List<List<T>>();
            if (numWanted == 0)
            {
                result.Add(new List<T>());
                return result;
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    T item = items[i];
                    List<T> clone = new List<T>(items); // Clone it
                    clone.RemoveAt(i);
                    List<List<T>> subResult = ChooseFrom(numWanted-1, clone);
                    foreach (List<T> subItem in subResult)
                    {
                        subItem.Insert(0, item);
                        result.Add(subItem);
                    }
                }
            }
            return result;
        }

        public static void TestPerms<T>(List<T> test)
        {

            List<List<T>> thePerms = Permutations(test);
            foreach (List<T> perm in thePerms)
            {
                foreach (T s in perm)
                {
                    Console.Write(s);
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
        }

        public static void TestChooseFrom<T>(int n, List<T> test)
        {
            List<List<T>> theChoices = ChooseFrom(n, test);
            foreach (List<T> choice in theChoices)
            {
                foreach (T s in choice)
                {
                    Console.Write(s);
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
        }
    }
}
