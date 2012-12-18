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

namespace                       Pong
{
    class                       Scud : Sprite
    {
        private int             screen_width;
        private int             screen_height;
        private int player;
        public float rotation;
        private float posX;
        private float posY;
        private int             y_min = 50;

        public                  Scud(int width, int height, float _posX, float _posY, int _player, float rotation)
        {
            this.screen_width = width;
            this.screen_height = height;
            this.posX = _posX;
            this.posY = _posY;
            player = _player;
            this.rotation = rotation;
        }

        public override void    initialize()
        {
            base.initialize();
            if (this.player == 0)
                this.Direction = new Vector2(1, 0);
            else if (this.player == 1)
                this.Direction = new Vector2(-1, 0);
            Matrix rotMatrix = Matrix.CreateRotationZ(this.rotation);
            Direction = Vector2.Transform(Direction, rotMatrix);
            this.Speed = 0.2f;
        }

        public override void loadContent(ContentManager cm, string name)
        {
            base.loadContent(cm, name);
            this.Position = new Vector2(posX, posY);
        }

        public void update(GameTime time, Rectangle p1_rect, Rectangle p2_rect)
        {
            if ((Position.Y <= y_min && Direction.Y < 0) || (Position.Y > this.screen_height - this.Texture.Height && Direction.Y > 0))
            {
                Direction = new Vector2(Direction.X, -Direction.Y);
                Speed += 0.05f;
                this.rotation = -this.rotation;
            }
            base.update(time);
        }

        public override void draw(SpriteBatch sprite_batch, GameTime time)
        {
            sprite_batch.Draw(Texture, Position, null, Color.White, this.rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), 1.0f, SpriteEffects.None, 0f);
        }
    }
}
