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
        private List<Scud>      listScud;
        private Tank          p1;
        private Tank          p2;
        private SpriteFont      score_font;
        private Boolean         is_paused;
        private double time1 = 0.00;
        private double time2 = 0.00;
        bool fire1 = true;
        bool fire2 = true;

        bool explosionP1 = false;
        bool explosionP2 = false;
        private Texture2D explosionSpriteP1;
        private Texture2D explosionSpriteP2;
        private double elapsedTimeP1 = 0.0;
        private double elapsedTimeP2 = 0.0;
        private int p1PosX = 0;
        private int p1PosY = 0;
        private int p2PosX = 0;
        private int p2PosY = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            listScud = new List<Scud>();
            this.p1 = new Tank(Window.ClientBounds.Width, Window.ClientBounds.Height, 1);
            this.p2 = new Tank(Window.ClientBounds.Width, Window.ClientBounds.Height, 2);
            this.p1.initialize();
            this.p2.initialize();
            this.is_paused = true;
            explosionSpriteP1 = Content.Load<Texture2D>("explosion");
            explosionSpriteP2 = Content.Load<Texture2D>("explosion");
            base.Initialize();
        }

        protected override void     LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.p1.loadContent(Content, "green_tank_base");
            this.p2.loadContent(Content, "orange_tank_base");
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
            this.updateExplosion(gameTime);
            kb_state = Keyboard.GetState();
            this.addScuds(kb_state, gameTime);
            if (!this.is_paused)
            {
                this.p1.handleInput(kb_state, Mouse.GetState());
                this.p1.update(gameTime);
                this.p2.handleInput(kb_state, Mouse.GetState());
                this.p2.update(gameTime);
                this.checkIfBallOut();
            }
            else
            {
                if (kb_state.IsKeyDown(Keys.Space))
                    this.is_paused = false;
            }
            base.Update(gameTime);
        }

        private void drawScuds(GameTime gameTime)
        {
            int i = 0;
            while (i < listScud.Count)
            {
                listScud[i].draw(spriteBatch, gameTime);
                listScud[i].update(gameTime, this.p1.CollisionRectangle, this.p2.CollisionRectangle);
                if ((listScud[i].Direction.X < 0 && this.p1.CollisionRectangle.Contains((int)listScud[i].Position.X, (int)listScud[i].Position.Y + listScud[i].Texture.Height / 2)))
                {
                    explosionP1 = true;
                    listScud.Remove(listScud[i]);
                }
                else if ((listScud[i].Direction.X > 0 && this.p2.CollisionRectangle.Contains((int)listScud[i].Position.X + listScud[i].Texture.Width, (int)listScud[i].Position.Y + listScud[i].Texture.Height / 2)))
                {
                    explosionP2 = true;
                    listScud.Remove(listScud[i]);
                }
                i++;
            }
        }

        private void addScuds(KeyboardState kb_state, GameTime gameTime)
        {
            if (this.is_paused == false)
            {
                if ((kb_state.IsKeyDown(Keys.Space)) && fire1 == true)
                {
                    Scud scud;
                    scud = new Scud(Window.ClientBounds.Width, Window.ClientBounds.Height, p1.Position.X + 50, p1.Position.Y + 26, 0, p1.rotation);
                    scud.initialize();
                    scud.loadContent(Content, "bullet");
                    listScud.Add(scud);
                    fire1 = false;
                }
                else if (kb_state.IsKeyDown(Keys.Enter) && fire2 == true)
                {
                    Scud scud;
                    scud = new Scud(Window.ClientBounds.Width, Window.ClientBounds.Height, p2.Position.X - 25, p2.Position.Y + 26, 1, p2.rotation);
                    scud.initialize();
                    scud.loadContent(Content, "bullet_orange");
                    listScud.Add(scud);
                    fire2 = false;
                }
                if (fire1 == false)
                {
                    time1 += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (time1 >= 200)
                    {
                        fire1 = true;
                        time1 = 0.00;
                    }
                }
                if (fire2 == false)
                {
                    time2 += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (time2 >= 200)
                    {
                        fire2 = true;
                        time2 = 0.00;
                    }
                }
            }
        }

        private void updateExplosion(GameTime gameTime)
        {
            if (explosionP1 == true)
            {
                elapsedTimeP1 += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedTimeP1 >= 100)
                {
                    p1PosX += 107;

                    if (p1PosX == 642)
                    {
                        p1PosX = 0;
                        p1PosY += 82;
                    }
                    if (p1PosY == 164)
                    {
                        p1PosX = 0;
                        p1PosY = 0;
                        explosionP1 = false;
                    }
                    elapsedTimeP1 = 0.0;
                }
            }
            if (explosionP2 == true)
            {
                elapsedTimeP2 += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedTimeP2 >= 100)
                {
                    p2PosX += 107;

                    if (p2PosX == 642)
                    {
                        p2PosX = 0;
                        p2PosY += 82;
                    }
                    if (p2PosY == 164)
                    {
                        p2PosX = 0;
                        p2PosY = 0;
                        explosionP2 = false;
                    }
                    elapsedTimeP2 = 0.0;
                }
            }
        }

        private void drawExplosion(GameTime gameTime)
        {
            if (explosionP1 == true)
            {
                sspriteBatch.Draw(explosionSpriteP1, new Rectangle((int)p1.Position.X - 35, (int)p1.Position.Y - 25, 96, 96), new Rectangle(p1PosX, p1PosY, 96, 96), Color.White);
            }
            if (explosionP2 == true)
            {
                spriteBatch.Draw(explosionSpriteP2, new Rectangle((int)p2.Position.X - 35, (int)p2.Position.Y - 25, 96, 96), new Rectangle(p2PosX, p2PosY, 96, 96), Color.White);
            }
        }

        private void                checkIfBallOut()
        {
            /*if (this.ball.Position.X <= 0)
            {
                this.p2.Score++;
                //this.ball.initialize();
                //this.ball.Position = new Vector2(Window.ClientBounds.Width / 2 - this.ball.Texture.Width / 2, Window.ClientBounds.Height / 2 - this.ball.Texture.Height / 2);
                this.is_paused = true;
            }
            else if (this.ball.Position.X >= Window.ClientBounds.Width - this.ball.Texture.Width)
            {
                this.p1.Score++;
                this.ball.initialize();
                this.ball.Position = new Vector2(Window.ClientBounds.Width / 2 - this.ball.Texture.Width / 2, Window.ClientBounds.Height / 2 - this.ball.Texture.Height / 2);
                this.is_paused = true;
            }*/
        }

        protected override void     Draw(GameTime gameTime)
        {
            Vector2                 p1_score_size;
            Vector2                 p1_score_pos;
            Vector2                 p2_score_size;
            Vector2                 p2_score_pos;

            p1_score_size = this.score_font.MeasureString(this.p1.Name);
            p2_score_size = this.score_font.MeasureString(this.p2.Name);
            p1_score_pos = new Vector2(5, 5);
            p2_score_pos = new Vector2(Window.ClientBounds.Width - 5 - p2_score_size.X, 5);
            GraphicsDevice.Clear(Color.WhiteSmoke);
            spriteBatch.Begin();
            this.spriteBatch.DrawString(this.score_font, p1.Name, p1_score_pos, Color.Black);
            this.spriteBatch.DrawString(this.score_font, p2.Name, p2_score_pos, Color.Black);
            this.p1.draw(spriteBatch, gameTime);
            this.p2.draw(spriteBatch, gameTime);
            this.drawScuds(gameTime);
            this.drawExplosion(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
