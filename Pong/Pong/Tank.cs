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
    class                   Tank : Sprite
    {
        private int         screen_width;
        private int         screen_height;
        private int         player_number;
        private Texture2D   texture_canon;
        public float        rotation;
        private Vector2     origin;
        private int         y_min = 50;

        public Rectangle CollisionRectangle
        {
            get
            {
                return (new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height));
            }
        }

        private String name;
        public String Name
        {
            get { return name; }
            set { name = value; }
        }


        public                  Tank(int width, int height, int nb)
        {
            this.screen_width = width;
            this.screen_height = height;
            this.player_number = nb;
            this.name = "Player " + nb;
            this.rotation = 0;
        }

        public override void initialize()
        {
            base.initialize();
        }

        public override void loadContent(ContentManager cm, string name)
        {
            base.loadContent(cm, name);
            if (this.player_number == 1)
            {
                this.Position = new Vector2(10, this.screen_height / 2 - this.Texture.Height / 2);
                this.texture_canon = cm.Load<Texture2D>("green_tank_canon");
                origin.X = this.texture_canon.Width / 2 - 10;
            }
            else
            {
                this.Position = new Vector2(this.screen_width - this.Texture.Width - 10, this.screen_height / 2 - this.Texture.Height / 2);
                this.texture_canon = cm.Load<Texture2D>("orange_tank_canon");
                origin.X = this.texture_canon.Width / 2 + 10;
            }

            origin.Y = this.texture_canon.Height / 2;
        }

        public override void handleInput(KeyboardState kb_state, MouseState ms_state)
        {
            base.handleInput(kb_state, ms_state);
            if (this.player_number == 1)
            {
                if (!xboxController())
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
                    if (kb_state.IsKeyDown(Keys.D) && this.rotation < 1f)
                        this.rotation += 0.1f;
                    else if (kb_state.IsKeyDown(Keys.A) && this.rotation > -1f)
                        this.rotation -= 0.1f;
                }
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
                if (kb_state.IsKeyDown(Keys.Right) && this.rotation < 1f)
                    this.rotation += 0.1f;
                else if (kb_state.IsKeyDown(Keys.Left) && this.rotation > -1f)
                    this.rotation -= 0.1f;
            }
        }

        private Boolean         xboxController()
        {
            GamePadState        game_pad_state;

            game_pad_state = GamePad.GetState(PlayerIndex.One);
            if (game_pad_state.IsConnected)
            {
                if (game_pad_state.IsButtonUp(Buttons.LeftThumbstickUp) || game_pad_state.IsButtonUp(Buttons.LeftThumbstickDown))
                    this.Speed = 0f;
                if (game_pad_state.IsButtonDown(Buttons.LeftThumbstickUp))
                {
                    this.Direction = -Vector2.UnitY;
                    this.Speed = 0.3f;
                }
                else if (game_pad_state.IsButtonDown(Buttons.LeftThumbstickDown))
                {
                    this.Direction = Vector2.UnitY;
                    this.Speed = 0.3f;
                }
                if (game_pad_state.ThumbSticks.Right.X > 0 && game_pad_state.ThumbSticks.Right.Y >= 0 && this.rotation > -1f)
                    this.rotation -= 0.1f;
                else if (game_pad_state.ThumbSticks.Right.X > 0 && game_pad_state.ThumbSticks.Right.Y <= 0 &&  this.rotation < 1f)
                    this.rotation += 0.1f;
                return (true);
            }
            return (false);
        }

        public override void    update(GameTime time)
        {
            if ((this.Position.Y <= this.y_min && this.Direction .Y < 0) || (this.Position.Y >= this.screen_height - this.Texture.Height && this.Direction.Y > 0))
                this.Speed = 0;
            base.update(time);
        }

        public override void    draw(SpriteBatch sprite_batch, GameTime time)
        {
            Vector2             position_canon;

            base.draw(sprite_batch, time);
            if (this.player_number == 1)
                position_canon = new Vector2(Position.X + 20, Position.Y + 30);
            else
                position_canon = new Vector2(Position.X + 23, Position.Y + 30);
            sprite_batch.Draw(this.texture_canon, position_canon, null, Color.White, this.rotation, this.origin, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
