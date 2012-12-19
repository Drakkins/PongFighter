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
    class Bonus : Sprite
    {
        public String bonusName = "";
        float posX;
        float posY;

        public Bonus(String _bonusName, float _posX, float _posY)
        {
            bonusName = _bonusName;
            posX = _posX;
            posY = _posY;
            this.Position = new Vector2(posX, posY);
            this.Direction = new Vector2(0, 1);
        }

        public Rectangle CollisionRectangle
        {
            get
            {
                return (new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height));
            }
        }

        public void launchBonus()
        {
            if (bonusName == "SPEED")
            {

            }
        }

        public void resetPosition()
        {
            this.Position = new Vector2(posX, posY);
        }

        public override void update(GameTime time)
        {
            this.Position += this.Direction;

            base.update(time);
        }

        public override void draw(SpriteBatch sprite_batch, GameTime time)
        {
            sprite_batch.Draw(Texture, Position, Color.White);
        }
    }
}
