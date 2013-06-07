using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game {
    public class MenuScene : IScene {

        public class MenuItem {
            public String sceneName;
            public String title;

            public MenuItem(String sceneName, String title) {
                this.sceneName = sceneName;
                this.title = title;
            }
        }

        List<MenuItem> items;
        int selectedIndex;
        int exitIndex;
        bool next;

        public MenuScene() {
            this.items = new List<MenuItem>();
        }

        public void add(MenuItem item) {
            this.items.Add(item);
            if ("exit".Equals(item.sceneName)){
                this.exitIndex = this.items.IndexOf(item);
            }
        }

        public void initialize(ContentUtil content) {
            this.selectedIndex = 0;
            this.next = false;
        }

        public void update(InputState input) {
            if (input.isActive(InputState.Control.EXIT)){
                this.selectedIndex = exitIndex;
                this.next = true;
                return;
            }

            if (input.isActive(InputState.Control.SELECT)) {
                this.next = true;
                return;
            }

            if (input.isActive(InputState.Move.UP)){
                this.selectedIndex--;
                if (this.selectedIndex < 0) {
                    this.selectedIndex = this.items.Count - 1;
                }
            } else if (input.isActive(InputState.Move.DOWN)) {
                this.selectedIndex++;
                if (this.selectedIndex >= this.items.Count) {
                    this.selectedIndex = 0;
                }
            }

        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            sprites.fillWithColor(Color.DarkGray, 1.0f);
            for (int i = 0; i < this.items.Count; i++) {
                String title = this.items.ElementAt(i).title;
                bool active = selectedIndex == i;
                int line = i - this.items.Count / 2;
                sprites.drawTextCentered(title, line, active ? Color.Orange : Color.GhostWhite);
            }
        }

        public String nextScene() {
            return this.items[this.selectedIndex].sceneName;
        }

        public bool wantsToExit() {
            return this.next;
        }
    }
}
