using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pong
{
    public class                Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager   graphics;
        SpriteBatch             spriteBatch;
        private Tank          p1;
        private Tank          p2;
        private Ball            ball;
        private SpriteFont      score_font;
        private Boolean         is_paused;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.p1 = new Tank(Window.ClientBounds.Width, Window.ClientBounds.Height, 1);
            this.p2 = new Tank(Window.ClientBounds.Width, Window.ClientBounds.Height, 2);
            this.ball = new Ball(Window.ClientBounds.Width, Window.ClientBounds.Height);
            this.p1.initialize();
            this.p2.initialize();
            this.ball.initialize();
            this.is_paused = true;
            base.Initialize();
        }

        protected override void     LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.p1.loadContent(Content, "green_tank_base");
            this.p2.loadContent(Content, "orange_tank_base");
            this.ball.loadContent(Content, "ball");
            this.score_font = Content.Load<SpriteFont>("Impact");
        }

        protected override void     UnloadContent()
        {
        }

        protected override void     Update(GameTime gameTime)
        {
            KeyboardState       kb_state;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            kb_state = Keyboard.GetState();
            if (!this.is_paused)
            {
                this.p1.handleInput(kb_state, Mouse.GetState());
                this.p1.update(gameTime);
                this.p2.handleInput(kb_state, Mouse.GetState());
                this.p2.update(gameTime);
                this.ball.update(gameTime, this.p1.CollisionRectangle, this.p2.CollisionRectangle);
                this.checkIfBallOut();
            }
            else
            {
                if (kb_state.IsKeyDown(Keys.Space))
                    this.is_paused = false;
            }
            base.Update(gameTime);
        }

        private void                checkIfBallOut()
        {
            if (this.ball.Position.X <= 0)
            {
                this.p2.Score++;
                this.ball.initialize();
                this.ball.Position = new Vector2(Window.ClientBounds.Width / 2 - this.ball.Texture.Width / 2, Window.ClientBounds.Height / 2 - this.ball.Texture.Height / 2);
                this.is_paused = true;
            }
            else if (this.ball.Position.X >= Window.ClientBounds.Width - this.ball.Texture.Width)
            {
                this.p1.Score++;
                this.ball.initialize();
                this.ball.Position = new Vector2(Window.ClientBounds.Width / 2 - this.ball.Texture.Width / 2, Window.ClientBounds.Height / 2 - this.ball.Texture.Height / 2);
                this.is_paused = true;
            }
        }

        protected override void     Draw(GameTime gameTime)
        {
            Vector2                 p1_score_size;
            Vector2                 p1_score_pos;
            Vector2                 p2_score_size;
            Vector2                 p2_score_pos;

            p1_score_size = this.score_font.MeasureString(this.p1.Score.ToString());
            p2_score_size = this.score_font.MeasureString(this.p2.Score.ToString());
            p1_score_pos = new Vector2((Window.ClientBounds.Width / 4 - p1_score_size.X / 2), p1_score_size.Y);
            p2_score_pos = new Vector2((3 * Window.ClientBounds.Width / 4 - p2_score_size.X / 2), p2_score_size.Y);
            GraphicsDevice.Clear(Color.WhiteSmoke);
            spriteBatch.Begin();
            this.spriteBatch.DrawString(this.score_font, p1.Score.ToString(), p1_score_pos, Color.Black);
            this.spriteBatch.DrawString(this.score_font, p2.Score.ToString(), p2_score_pos, Color.Black);
            this.p1.draw(spriteBatch, gameTime);
            this.p2.draw(spriteBatch, gameTime);
            this.ball.draw(spriteBatch, gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
