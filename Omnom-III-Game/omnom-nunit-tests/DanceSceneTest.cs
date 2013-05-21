using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omnom_III_Game;
using Omnom_III_Game.util;

namespace omnom_nunit_tests {

    [TestFixture]
    class DanceSceneTest {



        [Test]
        public void draw_should_draw_textures() {
            DanceScene scene = initializeDanceScene();

            
            var sprites = new Mock<SpriteBatchWrapper>();
            var device = new Mock<GraphicsDevice>();

            sprites.Setup(x => x.drawFromCenter(
                It.IsAny<Texture2D>(), 
                It.IsAny<Rectangle>(), 
                It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<Color>()));
                
            scene.draw(sprites.Object, device.Object);
                //sprites.Object, new Rectangle(0, 0, 800, 600));

            sprites.Verify();
        }

        private static DanceScene initializeDanceScene() {
            GraphicsDevice device = setupGraphicsDevice();

            var content = new Mock<ContentUtil>();
            var texture = new Mock<Texture2D>(device, 50, 50);
            content.Setup(x => x.load<Texture2D>(It.IsAny<String>())).Returns(texture.Object);

            DanceScene scene = new DanceScene();
            scene.initialize(content.Object);
            return scene;
        }

        private static GraphicsDevice setupGraphicsDevice() {
            PresentationParameters windowParams = new PresentationParameters();
            windowParams.DeviceWindowHandle = new IntPtr(1);
            GraphicsAdapter.UseNullDevice = true;
            GraphicsDevice device = new GraphicsDevice(
                GraphicsAdapter.DefaultAdapter,
                GraphicsProfile.Reach,
                windowParams);
            return device;
        }

    }
}
