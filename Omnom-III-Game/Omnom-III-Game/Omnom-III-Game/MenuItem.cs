using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Omnom_III_Game.graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game {
    public class MenuItem {
        public static String FONTNAME = "menu/menufont";

        public String sceneName;
        public String title;
        public Object sceneParams;

        private MenuItem previous;
        private MenuItem next;
        private bool selected;

        protected Sound clickSound;
        protected Sound selectSound;

        private ScaledTexture texDeactivated;
        private ScaledTexture texActivated;

        public MenuItem(String sceneName, String title) : this(sceneName, title, null) { }

        public MenuItem(String sceneName, String title, Object sceneParams) {
            this.sceneName = sceneName;
            this.title = title;
            this.sceneParams = sceneParams;
        }

        public void setScene(MenuItem previous, MenuItem next) {
            this.previous = previous;
            previous.next = this;
            this.next = next;
            next.previous = this;
        }

        
        public virtual void initialize(Sound click, Sound select,
                    ScaledTexture texDeactivated, ScaledTexture texActivated) {

            this.clickSound = click;
            this.selectSound = select;
            this._nextScene = false;
            this.selected = false;
            this.texDeactivated = texDeactivated;
            this.texActivated = texActivated;
        }

        public void update(ExplicitInputState input) {
            if (!this.selected)
                return;

            updateState(input);
        }

        protected virtual void updateState(ExplicitInputState input) {
            if (input.isActive(InputState.Control.SELECT)) {
                this.perform();
            }
            if (input.isActive(InputState.Control.UP)) {
                this.selectSound.play();
                this.previous.select();
            }
            if (input.isActive(InputState.Control.DOWN)) {
                this.selectSound.play();
                this.next.select();
            }
        }

        public void perform() {
            this.clickSound.play();
            this._nextScene = true;
        }


        public void select() {
            this.selected = true;
            this.previous.selected = false;
            this.next.selected = false;
        }

        public bool isSelected() {
            return this.selected;
        }

        private bool _nextScene;
        public bool nextScene { get { return this._nextScene; } }

        /*public virtual void drawInLine(SpriteBatchWrapper sprites, int line) {
            sprites.drawTextCentered(this.title, line, this.selected ? Color.Orange : Color.GhostWhite);
        }*/

        public virtual void drawFromCenter(SpriteBatchWrapper sprites, int x, int y) {
            int width = sprites.getWidthOfText(this.title, 1.0f, FONTNAME);
            int textureWidth = this.texture.bounds.Width;
            if (textureWidth < width + 20) {
                textureWidth = width + 20;
            }

            sprites.drawTextureAt(
                this.texture.texture, textureWidth, this.texture.bounds.Height, 
                x - textureWidth/2, y);

            int textY = y + (int)(this.texture.bounds.Height * .3);

            sprites.drawTextAt(
                this.title, x - width / 2, textY, 1.0f,
                this.selected ? Color.Orange : Color.GhostWhite, FONTNAME);
        }

        protected ScaledTexture texture {
            get {
            return this.selected ? this.texActivated : this.texDeactivated;
        } }

        public virtual int getHeight() {
            return this.texture.bounds.Height;
        }

        /*public virtual int getLineSize() {
            return 1;
        }*/

        internal virtual string getSceneName() {
            return this.sceneName;
        }

        internal virtual object getSceneParams() {
            return this.sceneParams;
        }
    }
}
