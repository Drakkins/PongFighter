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

        private float posX;
        private float posY;

        public                  Scud(int width, int height, float _posX, float _posY, int _player)
        {
            this.screen_width = width;
            this.screen_height = height;
            this.posX = _posX;
            this.posY = _posY;
            player = _player;
        }

        public override void    initialize()
        {
            base.initialize();
            if (this.player == 0)
                this.Direction = new Vector2(1, 0);
            else if (this.player == 1)
                this.Direction = new Vector2(-1, 0);
            this.Speed = 0.2f;
        }

        public override void loadContent(ContentManager cm, string name)
        {
            base.loadContent(cm, name);
            this.Position = new Vector2(posX, posY);
        }

        public void update(GameTime time, Rectangle p1_rect, Rectangle p2_rect)
        {
            if ((Position.Y <= 0 && Direction.Y < 0) || (Position.Y > this.screen_height - this.Texture.Height && Direction.Y > 0))
                Direction = new Vector2(Direction.X, -Direction.Y);
            if ((Direction.X < 0 && p1_rect.Contains((int)Position.X, (int)Position.Y + Texture.Height / 2)) || (Direction.X > 0 && p2_rect.Contains((int)Position.X + Texture.Width, (int)Position.Y + Texture.Height / 2)))
            {
                Direction = new Vector2(-Direction.X, Direction.Y);
                Speed += 0.05f;
            }
            base.update(time);
        }
    }
}
