using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omnom_III_Game.graphics;

namespace Omnom_III_Game {
    public class SubMenu : MenuItem {

        private List<MenuItem> items;
        private int selectedIndex = 0;
        private bool active;

        public SubMenu(String sceneName, String title,
                        params MenuItem[] items)
                : base(sceneName, title) {
            this.items = new List<MenuItem>();
            for (int i = 0; i < items.Length; i++) {
                MenuItem item = items[i];
                MenuItem prev = i > 0 ? items[i - 1] : items[items.Length - 1];
                MenuItem next = i + 2 < items.Length ? items[i + 1] : items[0];
                this.items.Add(item);
                item.setScene(prev, next);
            }
        }

        public override void initialize(Sound click, Sound select,
                    ScaledTexture texDeactivated, ScaledTexture texActivated) {

            base.initialize(click, select, texDeactivated, texActivated);
            this.active = false;
            this.selectedIndex = 0;
            foreach (MenuItem item in this.items) {
                item.initialize(click, select, texDeactivated, texActivated);
            }
        }

        private MenuItem selectedItem {
            get {
                return this.items[this.selectedIndex];
            }
        }

        private void next() {
            this.selectedIndex++;
            if (this.selectedIndex >= this.items.Count){
                this.selectedIndex = 0;
            }
        }

        private void prev() {
            this.selectedIndex--;
            if (this.selectedIndex < 0) {
                this.selectedIndex = this.items.Count - 1;
            }
        }

        protected override void updateState(ExplicitInputState input) {
            if (this.active) {
                if (input.isActive(InputState.Move.UP) ||
                        input.isActive(InputState.Move.DOWN)){

                    this.active = false;
                    this.clickSound.play();
                    base.updateState(input);
                } else if (input.isActive(InputState.Control.EXIT)) {
                    this.active = false;
                    this.clickSound.play();
                } else if (input.isActive(InputState.Move.RIGHT)) {
                    this.selectSound.play();
                    this.next();
                    this.selectedItem.select();
                } else if (input.isActive(InputState.Move.LEFT)) {
                    this.selectSound.play();
                    this.prev();
                    this.selectedItem.select();
                } else if (input.isActive(InputState.Control.SELECT)) {
                    this.clickSound.play();
                    this.selectedItem.select();
                    this.selectedItem.perform();
                    this.perform();
                }

            } else if (input.isActive(InputState.Control.SELECT)) {
                this.active = true;
                this.selectedIndex = 0;
                this.selectedItem.select();
                this.clickSound.play();
            } else {
                base.updateState(input);
            }
        }

        /*public override void drawInLine(util.SpriteBatchWrapper sprites, int line) {
            base.drawInLine(sprites, line);
            if (this.active) {
                int y = sprites.getYForTextLine(line + 1);
                int x = 75;
                foreach (MenuItem item in this.items) {
                    sprites.drawTextAt(item.title, x, y, .8f, 
                        item.isSelected() ? Color.Orange : Color.GhostWhite);
                    x += sprites.getWidthOfText(item.title, .8f);
                    x += 15;
                }
            }
        }*/

        /*public override int getLineSize() {
            int size = base.getLineSize();
            if (this.active) {
                size += 2;
            }
            return size;
        }*/

        public override void drawFromCenter(util.SpriteBatchWrapper sprites, int x, int y) {
            base.drawFromCenter(sprites, x, y);
            if (this.active) {
                int width = 0;
                foreach (MenuItem item in this.items) {
                    width += sprites.getWidthOfText(item.title, .8f);
                    width += 15;
                }
                width -= 15;

                int submenuX = x - width / 2;
                int submenuY = y + this.texture.bounds.Height;

                int textHeight = sprites.getHeightOfText("FOO", .8f);
                int backdropY = submenuY - 3;
                sprites.drawColorAt(Color.Black, .3f, width +10, textHeight + 6, submenuX - 5, backdropY);

                foreach (MenuItem item in this.items) {
                    sprites.drawTextAt(item.title, submenuX, submenuY, .8f,
                        item.isSelected() ? Color.Orange : Color.GhostWhite);
                    submenuX += sprites.getWidthOfText(item.title, .8f);
                    submenuX += 15;
                }
            }
        }

        public override int getHeight() {
            int height = base.getHeight();
            if (this.active) {
                height += 30;
            }
            return height;
        }

        internal override string getSceneName() {
            return this.selectedItem.getSceneName();
        }

        internal override object getSceneParams() {
            return this.selectedItem.getSceneParams();
        }
    }
}
