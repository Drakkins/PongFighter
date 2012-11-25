using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pong
{
    class           Racket : Sprite
    {
        private int screen_width;
        private int screen_height;
        private int player_number;

        public Rectangle CollisionRectangle
        {
            get
            {
                return (new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height));
            }
        }

        private int score;
        public int Score
        {
            get { return score; }
            set { score = value; }
        }


        public                  Racket(int width, int height, int nb)
        {
            this.screen_width = width;
            this.screen_height = height;
            this.player_number = nb;
        }

        public override void initialize()
        {
            base.initialize();
            this.score = 0;
        }

        public override void loadContent(ContentManager cm, string name)
        {
            base.loadContent(cm, name);
            if (this.player_number == 1)
                this.Position = new Vector2(10, this.screen_height / 2 - this.Texture.Height / 2);
            else
                this.Position = new Vector2(this.screen_width - this.Texture.Width - 10, this.screen_height / 2 - this.Texture.Height / 2);
        }

        public override void handleInput(KeyboardState kb_state, MouseState ms_state)
        {
            base.handleInput(kb_state, ms_state);
            if (this.player_number == 1)
            {
                if (kb_state.IsKeyUp(Keys.S) || kb_state.IsKeyUp(Keys.W))
                    this.Speed = 0f;
                if (kb_state.IsKeyDown(Keys.S))
                {
                    this.Direction = Vector2.UnitY;
                    this.Speed = 0.3f;
                }
                else if (kb_state.IsKeyDown(Keys.W))
                {
                    this.Direction = -Vector2.UnitY;
                    this.Speed = 0.3f;
                }
                else
                    this.Speed = 0f;
            }
            else
            {
                if (kb_state.IsKeyDown(Keys.Down))
                {
                    this.Direction = Vector2.UnitY;
                    this.Speed = 0.3f;
                }
                else if (kb_state.IsKeyDown(Keys.Up))
                {
                    this.Direction = -Vector2.UnitY;
                    this.Speed = 0.3f;
                }
                else
                    this.Speed = 0f;
            }
        }

        public override void update(GameTime time)
        {
            if ((this.Position.Y <= 0 && this.Direction .Y < 0) || (this.Position.Y >= this.screen_height - this.Texture.Height && this.Direction.Y > 0))
                this.Speed = 0;
            base.update(time);
        }
    }
}
