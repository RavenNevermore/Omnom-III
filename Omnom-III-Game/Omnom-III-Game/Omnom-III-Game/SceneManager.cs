using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game {
    public class SceneManager : IScene {

        public IScene this[String name] {
            get {
                return this.scenes[name];
            }
        }

        Dictionary<String, IScene> scenes;
        ContentUtil content;
        bool exit;
        String nextSceneName;
        String defaultScene;
        IScene activeScene;

        public void initialize(ContentUtil content) {
            this.exit = false;
            this.content = content;
            this.scenes = new Dictionary<string,IScene>();
            this.defaultScene = "menu";

            MenuScene menu = new MenuScene();
            this.scenes["menu"] = menu;
            this.addDanceScene("tigerstep", menu);
            this.addDanceScene("eattherich", menu);
            menu.add(new MenuScene.MenuItem("exit", "Quit Game"));
        }

        private void addDanceScene(String scriptname, MenuScene menu) {
            DanceScene scene = new DanceScene(scriptname);
            this.scenes[scriptname] = scene;
            menu.add(new MenuScene.MenuItem(scriptname, scene.title));
        }

        public void update(InputState input) {
            if (this.exit)
                return;

            if (null == this.activeScene) {
                this.activeScene = this.scenes[null == this.nextSceneName ?
                    this.defaultScene : this.nextSceneName];
                this.activeScene.initialize(this.content);
            }

            this.activeScene.update(input);

            if (this.activeScene.wantsToExit()) {
                this.nextSceneName = this.activeScene.nextScene();
                this.activeScene.cleanup();
                if (null == this.nextSceneName)
                    this.nextSceneName = defaultScene;

                if ("exit".Equals(this.nextSceneName.ToLower())) {
                    this.exit = true;
                } else {
                    this.activeScene = null;
                }
            }
        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            if (null != this.activeScene) {
                this.activeScene.draw(sprites, device);
            }
        }

        public String nextScene() {
            return null;
        }

        public bool wantsToExit() {
            return this.exit;
        }

        public void cleanup() {
            foreach (KeyValuePair<String, IScene> scene in this.scenes) {
                scene.Value.cleanup();
            }
        }
    }
}
