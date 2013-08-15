using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Omnom_III_Game.highscore;
using Microsoft.Xna.Framework;
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

            this.scenes["story_01"] = new VideoScene("video/story_01");
            this.scenes["story_02"] = new VideoScene("video/story_02");
            this.scenes["story_03"] = new VideoScene("video/story_03");
            this.scenes["story_04"] = new VideoScene("video/story_04");
            this.scenes["credits"] = new VideoScene("video/credits");

            SceneChain storyMode = new SceneChain();
            storyMode.addToChain(
                "story_01",
                "level_01",
                "story_02",
                "level_02",
                "story_03",
                "level_03",
                "story_04",
                "level_04",
                "credits");
            this.scenes["story_mode"] = storyMode;

            MenuScene menu = new MenuScene();
            this.scenes["menu"] = menu;
            //menu.add(new MenuItem("story_mode", "Story Mode", this));

            this.addDanceScene("level_01", menu);
            this.addDanceScene("level_02", menu);
            this.addDanceScene("level_03", menu);
            this.addDanceScene("level_04", menu);
            //menu.add(new MenuItem("highscore", "Highscore", new HighscoreParams("T-Bone the Steak")));
            this.scenes["highscore"] = new HighscoreScene();

            //menu.add(new MenuItem("exit", "Quit Game"));

            menu.buildMenuStructure(this);

            this.initIntroScenes();
        }

        private void initIntroScenes() {
            FadeInPictureScene sceneHairware = new FadeInPictureScene("intro/hairware", Color.White, 2000);
            FadeInPictureScene sceneGA = new FadeInPictureScene("intro/games_academy", Color.White, 2000);
            FadeInPictureScene sceneOmnom = new FadeInPictureScene("intro/omnom", Color.White, 4000);
            this.scenes["intro_ga"] = sceneGA;
            this.scenes["intro_hairware"] = sceneHairware;
            this.scenes["intro_omnom"] = sceneOmnom;

            MusicalSceneChain chain = new MusicalSceneChain("intro/introsound", 120f);
            chain.addToChain("intro_ga", "intro_hairware", "intro_omnom");
            this.scenes["intro"] = chain;

            this.nextSceneParams = new SceneActivationParameters("intro", this);
        }

        private void addDanceScene(String scriptname, MenuScene menu) {
            DanceScene scene = new DanceScene(scriptname);
            this.scenes[scriptname] = scene;
            //menu.add(new MenuItem(scriptname, scene.title));
        }

        public void update(InputState input) {
            if (this.exit)
                return;

            if (null == this.activeScene) {
                SceneActivationParameters sceneParams = 
                    this.initSceneParamsUsingDefaults(this.nextSceneParams);

                this.activeScene = this.initScene(sceneParams);
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

        public IScene getNextSceneFor(IScene scene) {
            SceneActivationParameters parameters = scene.nextScene();
            if (null == parameters) {
                return null;
            }
            return this.initScene(parameters);
        }

        public IScene initScene(String sceneName) {
            return this.initScene(
                this.getDefaultActivationParameters(sceneName));
        }

        private IScene initScene(SceneActivationParameters parameters) {
            IScene nextScene = this.scenes[parameters.sceneName];
            nextScene.initialize(this.content, parameters);
            return nextScene;
        }

        private SceneActivationParameters getDefaultActivationParameters(String sceneName) {
            SceneActivationParameters paramters = this.initSceneParamsUsingDefaults(null);
            paramters.sceneName = sceneName;
            return paramters;
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

        internal IScene getScene(string name) {
            return this.scenes[name];
        }
    }
}
