using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nett.PerfTests
{
    /// <summary>
    /// Simplified parser that tries to do real parser like processing tasks. This simplified parsing
    /// is used to establish a base line for performance checks. This is a very simple first version...
    /// that is missing quite a few aspects from the real parser... but sould be good enought for a
    /// first try.
    /// Missing:
    /// - Dictionary Lookup
    /// - List traversal lookup
    /// - Collection allocations
    /// - Small collection copy
    /// - Reflection based property mapping
    /// </summary>
    internal sealed class BaselineParser
    {
        public object Parse(string s)
        {
            List<string> res = new List<string>();

            for (int i = 0; i < 11; i++) // To get baseline nearer to the real thing
            {
                HashSet<string> hs = new HashSet<string>();
                StringReader reader = new StringReader(s);

                for (int c = reader.Read(); c >= 0; c = reader.Read())
                {
                    StringBuilder sb = new StringBuilder();

                    char cc = (char)c;
                    if (cc == ' ')
                    {
                        hs.Add(sb.ToString());
                        sb = new StringBuilder();
                    }
                    else
                    {
                        sb.Append(cc);
                    }
                }

                res.AddRange(hs);
            }

            return res;
        }
    }
}
