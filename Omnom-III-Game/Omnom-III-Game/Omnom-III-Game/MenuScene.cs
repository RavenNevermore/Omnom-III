using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Omnom_III_Game.highscore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game {
    public class MenuScene : IScene {

        List<MenuItem> items;
        bool next;
        bool exit;
        ExplicitInputState input;
        bool firstUpdate;
        Texture2D background;

        Sound clickSound;
        Sound selectSound;
        Sound backgroundSong;

        public MenuScene() {
            this.items = new List<MenuItem>();
        }

        public void buildMenuStructure(SceneManager manager) {
            this.add(new MenuItem("story_mode", "Story Mode", manager));

            MenuItem level1 = this.itemForDanceScene("level_01", manager);
            MenuItem level2 = this.itemForDanceScene("level_02", manager);
            MenuItem level3 = this.itemForDanceScene("level_03", manager);
            MenuItem level4 = this.itemForDanceScene("level_04", manager);
            SubMenu arcade = new SubMenu("arcade", "Arcade",
                level1, level2, level3, level4);
            this.add(arcade);

            this.add(new MenuItem("highscore", "Highscore", new HighscoreParams("T-Bone the Steak")));
            this.add(new MenuItem("exit", "Quit Game"));
        }

        private MenuItem itemForDanceScene(String name, SceneManager manager) {
            return new MenuItem(
                name,
                ((DanceScene)manager.getScene(name)).title);
        }

        public void add(MenuItem item) {
            this.items.Add(item);
            if (this.items.Count == 1){
                item.setScene(item, item);
            } else if (this.items.Count == 2){
                item.setScene(
                    this.items.ElementAt(0), 
                    this.items.ElementAt(0));
            } else {
                item.setScene(
                    this.items.ElementAt(this.items.Count - 2), 
                    this.items.ElementAt(0));
            }
        }

        public void initialize(ContentUtil content, SceneActivationParameters parameters) {
            
            this.input = new ExplicitInputState();
            this.next = false;
            this.exit = false;
            this.firstUpdate = true;
            this.background = content.load<Texture2D>("menu/background01");
            this.clickSound = new Sound("menu/click");
            this.selectSound = new Sound("menu/select");
            this.backgroundSong = new Sound("menu/backgroundsong");

            foreach (MenuItem item in this.items) {
                item.initialize(this.clickSound, this.selectSound);
            }
            this.items[0].select();
        }

        public void update(InputState currentInput) {
            if (!this.next && this.backgroundSong.stoppedPlaying())
                this.backgroundSong.play();

            this.input.update(currentInput);

            // We ignore the first update after initializing the scene,
            // to prevent older button presses carrying on.
            
            if (this.firstUpdate) {
                this.firstUpdate = false;
                return;
            }

            if (this.input.isActive(InputState.Control.EXIT)){
                this.next = true;
                this.exit = true;
                this.clickSound.play();
                this.backgroundSong.stop();
                return;
            }

            foreach (MenuItem item in this.items) {
                item.update(this.input);
                if (item.isSelected()) {
                    this.next = item.nextScene;
                }
            }

        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            sprites.drawBackground(background);
            int line = this.items.Count / 2 * -1;
            foreach (MenuItem item in this.items) {
                item.drawInLine(sprites, line);
                line += item.getLineSize();
            }
            /*
            for (int i = 0; i < this.items.Count; i++) {
                
                int line = i - this.items.Count / 2;
                this.items.ElementAt(i).drawInLine(sprites, line);
            }*/
        }

        public SceneActivationParameters nextScene() {
            SceneActivationParameters parameters = new SceneActivationParameters();

            MenuItem selected = this.getSelected();
            if (null == selected) {
                parameters.sceneName = "exit";
                parameters.parameters = null;
            } else {
                parameters.sceneName = selected.getSceneName();
                parameters.parameters = selected.getSceneParams();
            }
            return parameters;
        }

        private MenuItem getSelected() {
            if (this.exit) {
                return null;
            }
            foreach (MenuItem item in this.items) {
                if (item.isSelected())
                    return item;
            }
            return null;
        }

        public bool wantsToExit() {
            return this.next;
        }

        public void cleanup() {
            this.backgroundSong.stop();
        }
    }
}
