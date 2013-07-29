using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game {
    public class MenuItem {
        public String sceneName;
        public String title;
        public Object sceneParams;

        private MenuItem previous;
        private MenuItem next;
        private bool selected;

        protected Sound clickSound;
        protected Sound selectSound;

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

        
        public virtual void initialize(Sound click, Sound select){
            this.clickSound = click;
            this.selectSound = select;
            this._nextScene = false;
            this.selected = false;
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
            if (input.isActive(InputState.Move.UP)) {
                this.selectSound.play();
                this.previous.select();
            }
            if (input.isActive(InputState.Move.DOWN)) {
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

        public virtual void drawInLine(SpriteBatchWrapper sprites, int line) {
            sprites.drawTextCentered(this.title, line, this.selected ? Color.Orange : Color.GhostWhite);
        }

        public virtual int getLineSize() {
            return 1;
        }

        internal virtual string getSceneName() {
            return this.sceneName;
        }

        internal virtual object getSceneParams() {
            return this.sceneParams;
        }
    }
}
