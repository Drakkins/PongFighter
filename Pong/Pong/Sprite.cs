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
    class                   Sprite
    {
        public int          Width
        {
            set { width = value; }
        }
        private int         width;

        public int          Height
        {
            set { height = value; }
        }
        private int         height;

        public Texture2D    Texture
        {
            get { return (texture); }
            set { texture = value; }
        }
        private Texture2D   texture;

        public Vector2      Position
        {
            get { return (position); }
            set { position = value; }
        }
        private Vector2     position;

        public Vector2      Direction
        {
            get { return (direction); }
            set { direction = value; }
        }
        private Vector2     direction;

        public float        Speed
        {
            get { return (speed); }
            set { speed = value; }
        }
        private float       speed;

        public virtual void initialize()
        {
            this.position = Vector2.Zero;
            this.direction = Vector2.Zero;
            this.speed = 0f;
        }

        public virtual void loadContent(ContentManager cm, String name)
        {
            this.texture = cm.Load<Texture2D>(name);
        }

        public virtual void update(GameTime time)
        {
            this.position += this.direction * this.speed * (float)time.ElapsedGameTime.TotalMilliseconds;
        }

        public virtual void handleInput(KeyboardState kb_state, MouseState ms_state)
        {
        }

        public virtual void draw(SpriteBatch sprite_batch, GameTime time)
        {
            sprite_batch.Draw(this.texture, this.position, Color.White);
        }
    }
}
