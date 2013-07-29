using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game {
    public class SceneChain : IScene {

        public String[] chainedScenes;
        private IScene activeScene;
        private int currentSceneIndex;
        private SceneManager manager;

        public SceneChain() {
            this.chainedScenes = this.prepareChain();
        }

        internal virtual string[] prepareChain() {
            return new string[] { };
        }

        public void addToChain(params String[] sceneNames) {
            this.chainedScenes = this.chainedScenes.Concat(sceneNames).ToArray();
        }

        public virtual void initialize(ContentUtil content, SceneActivationParameters parameters) {
            this.manager = (SceneManager) parameters.parameters;
            this.currentSceneIndex = -1;
            this.activateNext();
        }

        public virtual void update(InputState input) {
            if (this.wantsToExit()) {
                return;
            }

            activeScene.update(input);
            if (activeScene.wantsToExit()) {
                IScene nextScene = this.manager.getNextSceneFor(activeScene);
                activeScene.cleanup();
                
                if (null == nextScene) {
                    this.activateNext();
                } else {
                    this.activeScene = nextScene;
                }
            }
        }

        private void activateNext() {
            this.currentSceneIndex++;
            if (this.wantsToExit()) {
                return;
            } else {
                this.activeScene = this.manager.initScene(
                    this.chainedScenes[this.currentSceneIndex]);
            }
        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            if (this.wantsToExit()) {
                return;
            }
            this.activeScene.draw(sprites, device);
        }

        public virtual void cleanup() {
            if (null != this.activeScene)
                this.activeScene.cleanup();
            this.activeScene = null;
            this.currentSceneIndex = 0;
        }

        public SceneActivationParameters nextScene() {
            return null;
        }

        public bool wantsToExit() {
            return this.currentSceneIndex >= this.chainedScenes.Length;
        }
    }
}
