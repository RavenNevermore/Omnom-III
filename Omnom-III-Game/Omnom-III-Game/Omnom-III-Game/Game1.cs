using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Omnom_III_Game.util;

namespace Omnom_III_Game {
    
    public class Game1 : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        IScene scene;
        SpriteFont defaultFont;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            ContentUtil contentUtil = new ContentUtil(this.Content);

            //this.scene = new DanceScene("eattherich");
            this.scene = new SceneManager();
            this.scene.initialize(contentUtil);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.defaultFont = Content.Load<SpriteFont>("default");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() { }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            //this.checkForExitSignals();

            
            GamePadDPad dpad = GamePad.GetState(PlayerIndex.One).DPad;
            GamePadButtons buttons = GamePad.GetState(PlayerIndex.One).Buttons;
            
            InputState input = new InputState();
            input.set(InputState.Move.UP, dpad.Up == ButtonState.Pressed 
                || buttons.Y == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.W));
            input.set(InputState.Move.DOWN, dpad.Down == ButtonState.Pressed
                || buttons.A == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.S));
            input.set(InputState.Move.LEFT, dpad.Left == ButtonState.Pressed
                || buttons.X == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.A));
            input.set(InputState.Move.RIGHT, dpad.Right == ButtonState.Pressed
                || buttons.B == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.D));

            input.set(InputState.Control.EXIT, buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape));
            input.set(InputState.Control.SELECT, buttons.Start == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Enter));


            this.scene.update(input);

            if (this.scene.wantsToExit())
                this.Exit();

            base.Update(gameTime);
        }

        private void checkForExitSignals() {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            SpriteBatchWrapper wrapper = new SpriteBatchWrapper(
                this.spriteBatch, this.GraphicsDevice, this.defaultFont);
            this.scene.draw(wrapper, this.GraphicsDevice);
            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
