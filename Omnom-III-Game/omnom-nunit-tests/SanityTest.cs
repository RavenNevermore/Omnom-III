using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace omnom_nunit_tests
{
    [TestFixture]
    class SanityTest
    {
        [Test]
        public void testSantiy() {
            Assert.AreEqual(4, 2+2);
        }

        [Test]
        public void testTest() {
            Assert.True(true);
        }
    }
}
