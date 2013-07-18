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
            public Object sceneParams;

            public MenuItem(String sceneName, String title) : this(sceneName, title, null) {}

            public MenuItem(String sceneName, String title, Object sceneParams) {
                this.sceneName = sceneName;
                this.title = title;
                this.sceneParams = sceneParams;
            }
        }

        List<MenuItem> items;
        int selectedIndex;
        int exitIndex;
        bool next;
        ExplicitInputState input;
        bool firstUpdate;

        public MenuScene() {
            this.items = new List<MenuItem>();
        }

        public void add(MenuItem item) {
            this.items.Add(item);
            if ("exit".Equals(item.sceneName)){
                this.exitIndex = this.items.IndexOf(item);
            }
        }

        public void initialize(ContentUtil content, SceneActivationParameters parameters) {
            this.selectedIndex = 0;
            this.input = new ExplicitInputState();
            this.next = false;
            this.firstUpdate = true;
        }

        public void update(InputState currentInput) {
            this.input.update(currentInput);

            // We ignore the first update after initializing the scene,
            // to prevent older button presses carrying on.
            
            if (this.firstUpdate) {
                this.firstUpdate = false;
                return;
            }

            if (this.input.isActive(InputState.Control.EXIT)){
                this.selectedIndex = exitIndex;
                this.next = true;
                return;
            }

            if (this.input.isActive(InputState.Control.SELECT)) {
                this.next = true;
                return;
            }

            if (this.input.isActive(InputState.Move.UP)) {
                this.selectedIndex--;
                if (this.selectedIndex < 0) {
                    this.selectedIndex = this.items.Count - 1;
                }
            } else if (this.input.isActive(InputState.Move.DOWN)) {
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

        public SceneActivationParameters nextScene() {
            SceneActivationParameters parameters = new SceneActivationParameters();
            parameters.sceneName = this.items[this.selectedIndex].sceneName;
            parameters.parameters = this.items[this.selectedIndex].sceneParams;
            return parameters;
        }

        public bool wantsToExit() {
            return this.next;
        }

        public void cleanup() { }
    }
}
