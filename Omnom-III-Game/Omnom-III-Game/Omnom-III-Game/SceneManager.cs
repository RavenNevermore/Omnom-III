using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Omnom_III_Game.highscore;
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
        SceneActivationParameters nextSceneParams;
        String defaultScene;
        IScene activeScene;

        public void initialize(ContentUtil content, SceneActivationParameters parameters) {
            this.exit = false;
            this.content = content;
            this.scenes = new Dictionary<string,IScene>();
            this.defaultScene = "menu";

            MenuScene menu = new MenuScene();
            this.scenes["menu"] = menu;
            this.addDanceScene("tigerstep", menu);
            this.addDanceScene("eattherich", menu);
            this.addDanceScene("level_steaky", menu);
            menu.add(new MenuScene.MenuItem("highscore", "Highscore", new HighscoreParams("Foo Bar")));
            this.scenes["highscore"] = new HighscoreScene();

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
                SceneActivationParameters sceneParams = 
                    this.initSceneParamsUsingDefaults(this.nextSceneParams);

                this.activeScene = this.scenes[sceneParams.sceneName];
                this.activeScene.initialize(this.content, sceneParams);
            }

            this.activeScene.update(input);

            if (this.activeScene.wantsToExit()) {
                this.nextSceneParams = this.initSceneParamsUsingDefaults(this.activeScene.nextScene());
                this.activeScene.cleanup();
                
                if ("exit".Equals(this.nextSceneParams.sceneName.ToLower())) {
                    this.exit = true;
                } else {
                    this.activeScene = null;
                }
            }
        }

        private SceneActivationParameters initSceneParamsUsingDefaults(SceneActivationParameters sceneParams) {
            if (null == sceneParams)
                sceneParams = new SceneActivationParameters();
            if (null == sceneParams.sceneName)
                sceneParams.sceneName = this.defaultScene;
            return sceneParams;
        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            if (null != this.activeScene) {
                this.activeScene.draw(sprites, device);
            }
        }

        public SceneActivationParameters nextScene() {
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
