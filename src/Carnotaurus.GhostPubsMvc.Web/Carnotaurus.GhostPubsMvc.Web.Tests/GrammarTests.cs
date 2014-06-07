using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carnotaurus.GhostPubsMvc.Web.Extensions;

namespace Carnotaurus.GhostPubsMvc.Web.Tests
{
    [TestClass]
    public class GrammarTests
    {
        [TestMethod]
        public void TestThatResultContainsAnAnd()
        {
            var test = new List<String> { "Manchester", "Chester", "Bolton" };

            var oxbridgeAnd = test.OxbridgeAnd();
           
            Assert.IsTrue( oxbridgeAnd.Contains(", and"));
        }
    }
}
