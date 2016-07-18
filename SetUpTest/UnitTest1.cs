using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SetUpTest
{
    using MmsSetup;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PassTestMethod1()
        {
            var pri = new Privileges();
            var bvalue = pri.CheckExists("Y");
            Assert.AreEqual(true,bvalue);
        }


        [TestMethod]
        public void FailTestMethod1()
        {
            var pri = new Privileges();
            var bvalue = pri.CheckExists("Y");
            Assert.AreEqual(true, bvalue);
        }
    }
}
