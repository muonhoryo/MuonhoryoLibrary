
using System;
using System.Text;

namespace MuonhoryoLibrary
{
    public static class StringExtensions
    {
        /// <summary>
        /// Removes all symbols in 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="removedSymbols"></param>
        /// <returns></returns>
        public static string FilterBy(this string input, char[] removedSymbols)
        {
            if(input==null)
                throw new ArgumentNullException("input");
            if (removedSymbols == null)
                throw new ArgumentNullException("removedSymbols");
            if (input.Length==0||
                removedSymbols.Length == 0)
                return input;

            for(int i=0; i< removedSymbols.Length; i++)
            {
                if (removedSymbols[i] == 0)
                    throw new ArgumentNullException("symbol by index " + i);
            }

            bool ComparionFunc(char sym)
            {
                foreach (char c in removedSymbols)
                {
                    if (sym == c) return false;
                }
                return true;
            }

            StringBuilder str = new StringBuilder(input.Length);
            foreach (var c in input)
            {

                if (ComparionFunc(c))
                {
                    str.Append(c);
                }
            }
            return str.ToString();
        }
    }
}
