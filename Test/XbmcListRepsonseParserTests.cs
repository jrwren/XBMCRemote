using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class XbmcListRepsonseParserTests
    {
        [Test]
        public void Test()
        {
            var blah = @"<html>

<li>smb://xbox:omgxbox@utonium/d/cd images/
<li>smb://xbox:omgxbox@utonium/d/Documents/
<li>smb://xbox:omgxbox@utonium/d/ForLilly/
<li>smb://xbox:omgxbox@utonium/d/fsharp/
<li>smb://xbox:omgxbox@utonium/d/MSOCache/</html>"
                ;
            XBMCRemote.XbmcListRepsonseParser xbmcListRepsonseParser = new XBMCRemote.XbmcListRepsonseParser();
            var items = xbmcListRepsonseParser.GetItemsFromRepsonse(blah);
            Assert.AreEqual("smb://xbox:omgxbox@utonium/d/cd images/", items[0]);
        }
    }
}
