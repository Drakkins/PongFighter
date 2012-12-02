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
        private List<Texture2D> listLifeP1;
        private List<Texture2D> listLifeP2;
        private Texture2D       ui;
        private Tank            p1;
        private Tank            p2;
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

        SoundEffect exp;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public void initLife()
        {
            int i = 0;

            while (i < 7)
            {
                listLifeP1.Add(Content.Load<Texture2D>("lifebar"));
                listLifeP2.Add(Content.Load<Texture2D>("lifebar2"));
                i++;
            }
        }

        protected override void Initialize()
        {
            listScud = new List<Scud>();
            listLifeP1 = new List<Texture2D>();
            listLifeP2 = new List<Texture2D>();
            this.p1 = new Tank(Window.ClientBounds.Width, Window.ClientBounds.Height, 1);
            this.p2 = new Tank(Window.ClientBounds.Width, Window.ClientBounds.Height, 2);
            this.p1.initialize();
            this.p2.initialize();
            this.is_paused = true;
            explosionSpriteP1 = Content.Load<Texture2D>("explosion");
            explosionSpriteP2 = Content.Load<Texture2D>("explosion");
            
            exp = Content.Load<SoundEffect>("soundExplosion");


            SoundEffect bgEffect;
            bgEffect = Content.Load<SoundEffect>("soundGame");
            SoundEffectInstance instance = bgEffect.CreateInstance();
            instance.IsLooped = true;
            bgEffect.Play(1f, 0.0f, 0.0f);

            initLife();

            base.Initialize();
        }

        protected override void     LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.p1.loadContent(Content, "green_tank_base");
            this.p2.loadContent(Content, "orange_tank_base");
            this.score_font = Content.Load<SpriteFont>("Impact");
            this.ui = Content.Load<Texture2D>("ui_repeat");
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
                    exp.Play();
                    if (listLifeP1.Count >= 1)
                        listLifeP1.RemoveAt(listLifeP1.Count - 1);
                }
                else if ((listScud[i].Direction.X > 0 && this.p2.CollisionRectangle.Contains((int)listScud[i].Position.X + listScud[i].Texture.Width, (int)listScud[i].Position.Y + listScud[i].Texture.Height / 2)))
                {
                    explosionP2 = true;
                    listScud.Remove(listScud[i]);
                    exp.Play();
                    if (listLifeP2.Count >= 1)
                        listLifeP2.RemoveAt(listLifeP2.Count - 1);
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
                spriteBatch.Draw(explosionSpriteP1, new Rectangle((int)p1.Position.X - 35, (int)p1.Position.Y - 25, 96, 96), new Rectangle(p1PosX, p1PosY, 96, 96), Color.White);
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

        private void        drawUI()
        {
            int             x = 0;

            while (x < Window.ClientBounds.Width)
            {
                spriteBatch.Draw(this.ui, new Vector2(x, 0), Color.White);
                x++;
            }
        }

        public void         drawLife()
        {
            int i = 0;
            int x1 = 110;
            int y1 = 10;
            int x2 = 640;
            int y2 = 10;

            while (i < listLifeP1.Count)
            {
                spriteBatch.Draw(listLifeP1[i], new Vector2(x1, y1), Color.White);
                i++;
                x1 += 30;
            }
            i = 0;
            while (i < listLifeP2.Count)
            {
                spriteBatch.Draw(listLifeP2[i], new Vector2(x2, y2), Color.White);
                i++;
                x2 -= 30;
            }
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
            
            this.p1.draw(spriteBatch, gameTime);
            this.p2.draw(spriteBatch, gameTime);
            this.drawScuds(gameTime);
            this.drawExplosion(gameTime);
            this.drawUI();
            drawLife();
            this.spriteBatch.DrawString(this.score_font, p1.Name, p1_score_pos, Color.White);
            this.spriteBatch.DrawString(this.score_font, p2.Name, p2_score_pos, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
