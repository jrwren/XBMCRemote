using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XBMCRemote
{
    public class XbmcListRepsonseParser
    {
        //public Regex re = new Regex(@"<html>(?:\s*<li>([^;\s]+)[;\s]{,3})+</html>");
        public IList<string> GetItemsFromRepsonse(string response)
        {
            if (response.StartsWith("<html>"))
                response = response.Substring(6);
            if (response.EndsWith("</html>"))
                response = response.Substring(0,response.Length-7);
            var lines = response.Split(new[]{"\r\n","\n"},StringSplitOptions.RemoveEmptyEntries);
            return lines.Select(s => s.StartsWith("<li>")? 
                s.Substring(4) :
                s ).ToList();
            
        }
    }
}