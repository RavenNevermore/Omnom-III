using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Omnom_III_Game;
using Omnom_III_Game.util;

namespace omnom_nunit_tests {

    [TestFixture]
    class SongTest {

        [Test]
        public void testBpm() {
            Song song = new Song();
            song.bpm = 120;
            Assert.AreEqual(120, song.bpm);
        }
    }
}
