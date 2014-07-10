using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Carnotaurus.GhostPubsMvc.Tests
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
