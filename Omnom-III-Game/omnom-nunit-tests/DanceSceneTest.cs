using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using Microsoft.Xna.Framework.Graphics;
using Omnom_III_Game;
using Omnom_III_Game.util;

namespace omnom_nunit_tests {

    [TestFixture]
    class DanceSceneTest {



        [Test]
        public void draw_should_draw_textures() {
            PresentationParameters windowParams = new PresentationParameters();
            windowParams.DeviceWindowHandle = new IntPtr(1);
            GraphicsAdapter.UseNullDevice = true;
            GraphicsDevice device = new GraphicsDevice(
                GraphicsAdapter.DefaultAdapter,
                GraphicsProfile.Reach,
                windowParams);
            var texture = new Mock<Texture2D>(device, 50, 50);
            var content = new Mock<ContentUtil>();
            content.Setup(x => x.load<Texture2D>(It.IsAny<String>())).Returns(texture.Object);

            DanceScene scene = new DanceScene();
            scene.initialize(content.Object);

            
        }

    }
}
