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
        private List<Bonus>     listBonus;
        private int             currentBonusId = -1;
        private double          elapsedTimeBonus = 0.0;
        private Boolean         isBonus = false;
        private Boolean         bonusActived = false;
        private int             speedShot1 = 500;
        private int             speedShot2 = 500;
        Boolean                 shieldAnim = false;
        private Texture2D       life_p1;
        private Texture2D       life_p2;
        private Texture2D       ui;
        private Texture2D       background;
        private Texture2D       tree;
        private Texture2D       road;
        private Tank            p1;
        private Tank            p2;
        private SpriteFont      game_font;
        private Boolean         is_paused;
        private double          time1 = 0.00;
        private double          time2 = 0.00;
        bool                    fire1 = true;
        bool                    fire2 = true;
        private bool            end = false;
        bool                    explosionP1 = false;
        bool                    explosionP2 = false;
        private Texture2D       explosionSpriteP1;
        private Texture2D       explosionSpriteP2;
        private Texture2D       xbox_button_a;
        private Texture2D       key_r;
        private bool            xbox_controler = false;
        private double          elapsedTimeP1 = 0.0;
        private double          elapsedTimeP2 = 0.0;
        private int             p1PosX = 0;
        private int             p1PosY = 0;
        private int             p2PosX = 0;
        private int             p2PosY = 0;
        private int             width = 1024;
        private int             height = 600;
        SoundEffect             exp;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = this.width;
            this.graphics.PreferredBackBufferHeight = this.height;
            this.graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.listScud = new List<Scud>();
            this.listBonus = new List<Bonus>();
            this.p1 = new Tank(Window.ClientBounds.Width, Window.ClientBounds.Height, 1);
            this.p2 = new Tank(Window.ClientBounds.Width, Window.ClientBounds.Height, 2);
            this.p1.initialize();
            this.p2.initialize();
            this.is_paused = false;
            explosionSpriteP1 = Content.Load<Texture2D>("explosion");
            explosionSpriteP2 = Content.Load<Texture2D>("explosion");
            
            exp = Content.Load<SoundEffect>("soundExplosion");
            SoundEffect bgEffect;
            bgEffect = Content.Load<SoundEffect>("soundGame");
            SoundEffectInstance instance = bgEffect.CreateInstance();
            instance.IsLooped = true;
            bgEffect.Play(1f, 0.0f, 0.0f);
            base.Initialize();
        }

        private void            restartGame()
        {
            this.p1.Life = 100;
            this.p2.Life = 100;
            this.listScud.Clear();
        }

        protected override void     LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.p1.loadContent(Content, "green_tank_base");
            this.p2.loadContent(Content, "orange_tank_base");
            this.game_font = Content.Load<SpriteFont>("SquaredDisplay");
            this.ui = Content.Load<Texture2D>("ui_repeat");
            this.background = Content.Load<Texture2D>("background");
            this.road = Content.Load<Texture2D>("road");
            this.tree = Content.Load<Texture2D>("tree");
            this.life_p1 = Content.Load<Texture2D>("lifebar");
            this.life_p2 = Content.Load<Texture2D>("lifebar2");
            this.xbox_button_a = Content.Load<Texture2D>("Xbox A Button");
            this.key_r = Content.Load<Texture2D>("Key R");
            Bonus speed = new Bonus("SPEED", Window.ClientBounds.Width / 2, 0);
            speed.loadContent(Content, "speed");
            listBonus.Add(speed);
            speed = new Bonus("SHIELD", Window.ClientBounds.Width / 2, 0);
            speed.loadContent(Content, "shield");
            listBonus.Add(speed);
        }

        protected override void     UnloadContent()
        {
        }

        protected override void     Update(GameTime gameTime)
        {
            KeyboardState           kb_state;

            kb_state = Keyboard.GetState();
            if (kb_state.IsKeyDown(Keys.Escape))
                this.Exit();
            if (!this.end)
            {
                checkBonus(gameTime);
                this.updateExplosion(gameTime);
                this.addScuds(kb_state, gameTime);
                if (!this.is_paused)
                {
                    this.p1.handleInput(kb_state, Mouse.GetState());
                    this.p1.update(gameTime);
                    this.p2.handleInput(kb_state, Mouse.GetState());
                    this.p2.update(gameTime);
                }
                else
                {
                    if (kb_state.IsKeyDown(Keys.Space))
                        this.is_paused = false;
                }
                this.checkEnd();
            }
            else
            {
                if (kb_state.IsKeyDown(Keys.R))
                {
                    this.restartGame();
                    this.end = false;
                }
            }
            base.Update(gameTime);
        }

        private void        checkEnd()
        {
            if (this.p1.Life <= 0 || this.p2.Life <= 0)
                this.end = true;
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
                    this.p1.Life -= 10;
                    exp.Play();
                }
                else if ((listScud[i].Direction.X > 0 && this.p2.CollisionRectangle.Contains((int)listScud[i].Position.X + listScud[i].Texture.Width, (int)listScud[i].Position.Y + listScud[i].Texture.Height / 2)))
                {
                    explosionP2 = true;
                    this.p2.Life -= 10;
                    listScud.Remove(listScud[i]);
                    exp.Play();
                }
                i++;
            }
        }

        private void addScuds(KeyboardState kb_state, GameTime gameTime)
        {
            if (this.is_paused == false)
            {
                GamePadState        game_pad_state;
                GamePadState        game_pad_state_2;

                game_pad_state = GamePad.GetState(PlayerIndex.One);
                game_pad_state_2 = GamePad.GetState(PlayerIndex.Two);
                if (game_pad_state.IsConnected)
                    this.xbox_controler = true;
                if (game_pad_state.IsConnected && fire1 == true)
                {
                    if (game_pad_state.IsButtonDown(Buttons.LeftTrigger))
                    {
                        Scud scud;
                        scud = new Scud(Window.ClientBounds.Width, Window.ClientBounds.Height, p1.Position.X + 50, p1.Position.Y + 26, 0, p1.rotation);
                        scud.initialize();
                        scud.loadContent(Content, "bullet");
                        listScud.Add(scud);
                        fire1 = false;
                    }
                }
                else if ((kb_state.IsKeyDown(Keys.Space)) && fire1 == true)
                {
                    Scud scud;
                    scud = new Scud(Window.ClientBounds.Width, Window.ClientBounds.Height, p1.Position.X + 50, p1.Position.Y + 26, 0, p1.rotation);
                    scud.initialize();
                    scud.loadContent(Content, "bullet");
                    listScud.Add(scud);
                    fire1 = false;
                }
                if (game_pad_state_2.IsConnected && fire2 == true)
                {
                    if (game_pad_state_2.IsButtonDown(Buttons.LeftTrigger))
                    {
                        Scud scud;
                        scud = new Scud(Window.ClientBounds.Width, Window.ClientBounds.Height, p2.Position.X - 25, p2.Position.Y + 26, 1, p2.rotation);
                        scud.initialize();
                        scud.loadContent(Content, "bullet_orange");
                        listScud.Add(scud);
                        fire2 = false;
                    }
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
                    if (time1 >= speedShot1)
                    {
                        fire1 = true;
                        time1 = 0.00;
                    }
                }
                if (fire2 == false)
                {
                    time2 += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (time2 >= speedShot2)
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
            int x1 = 140;
            int y1 = 8;
            int x2 = 820;
            int y2 = 8;

            for (int i = 0; i < this.p1.Life; i += 10)
            {
                spriteBatch.Draw(this.life_p1, new Vector2(x1, y1), Color.White);
                x1 += 30;
            }
            for (int i = 0; i < this.p2.Life; i += 10)
            {
                spriteBatch.Draw(this.life_p2, new Vector2(x2, y2), Color.White);
                x2 -= 30;
            }
        }

        public void checkCollisionBonus()
        {
            int i = 0;

            while (i < listScud.Count)
            {
                if (listBonus[currentBonusId].CollisionRectangle.Contains((int)listScud[i].Position.X, (int)listScud[i].Position.Y + listScud[i].Texture.Height / 2))
                {
                    isBonus = false;
                    listBonus[currentBonusId].resetPosition();
                    bonusActived = true;
                    if ((listScud[i].player) == 0 && (listBonus[currentBonusId].bonusName == "SPEED"))
                        speedShot1 = 150;
                    else if ((listScud[i].player) == 1 && (listBonus[currentBonusId].bonusName == "SPEED"))
                        speedShot2 = 150;
                }
                i++;
            }
        }

        public void checkBonus(GameTime gameTime)
        {
            Random rand1 = new Random();
            int random1 = rand1.Next(0, 500);

            if (bonusActived == true)
            {
                elapsedTimeBonus += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTimeBonus >= 3000)
                {
                    elapsedTimeBonus = 0.0;
                    bonusActived = false;
                    speedShot2 = 500;
                    speedShot1 = 500;
                }
            }

            if (isBonus == false)
            {
                if (random1 == 250)
                {
                    currentBonusId += 1;
                    if (currentBonusId == 2)
                        currentBonusId = 0;
                    isBonus = true;
                }
            }
            else
            {
                checkCollisionBonus();
                if (listBonus[currentBonusId].Position.Y > Window.ClientBounds.Width)
                {
                    isBonus = false;
                    listBonus[currentBonusId].resetPosition();
                }
            }
        }

        public void drawBonus(GameTime gameTime)
        {
            if (isBonus == true)
            {
                listBonus[currentBonusId].draw(spriteBatch, gameTime);
                listBonus[currentBonusId].update(gameTime);
            }
        }

        private void                drawBackground()
        {
            int                     x = 0;
            int                     y = 50;

            while (y < this.height)
            {
                x = 0;
                while (x < this.width)
                {
                    this.spriteBatch.Draw(this.background, new Vector2(x, y), Color.White);
                    x += this.background.Width;
                }
                y += this.background.Height;
            }
            y = 50;            
            while (y < this.height)
            {
                this.spriteBatch.Draw(this.road, new Vector2(-2, y), Color.White);
                this.spriteBatch.Draw(this.road, new Vector2(this.width - this.road.Width + 10, y), Color.White);
                y += this.road.Height;
            }
            this.spriteBatch.Draw(this.tree, new Vector2(300, 250), Color.White);
            this.spriteBatch.Draw(this.tree, new Vector2(320, 450), Color.White);
            this.spriteBatch.Draw(this.tree, new Vector2(600, 150), Color.White);
            this.spriteBatch.Draw(this.tree, new Vector2(100, 350), Color.White);

        }

        protected override void     Draw(GameTime gameTime)
        {
            if (!this.end)
            {
                Vector2 p1_score_size;
                Vector2 p1_score_pos;
                Vector2 p2_score_size;
                Vector2 p2_score_pos;
                int y_score = 8;

                p1_score_size = this.game_font.MeasureString(this.p1.Name);
                p2_score_size = this.game_font.MeasureString(this.p2.Name);
                p1_score_pos = new Vector2(5, y_score);
                p2_score_pos = new Vector2(Window.ClientBounds.Width - 5 - p2_score_size.X, y_score);
                GraphicsDevice.Clear(Color.WhiteSmoke);
                spriteBatch.Begin();
                this.drawBackground();
                drawBonus(gameTime);
                this.p1.draw(spriteBatch, gameTime);
                this.p2.draw(spriteBatch, gameTime);
                this.drawScuds(gameTime);
                this.drawExplosion(gameTime);

                this.drawUI();
                drawLife();
                this.spriteBatch.DrawString(this.game_font, p1.Name, p1_score_pos, Color.White);
                this.spriteBatch.DrawString(this.game_font, p2.Name, p2_score_pos, Color.White);
                spriteBatch.End();
            }
            else
            {
                String              msg_restart_begin = "Press";
                String              msg_restart_end = "to restart the game";
                String              msg_win;
                float               x_msg_restart;
                Vector2             msg_win_size;
                Vector2             msg_restart_begin_size;
                Vector2             msg_restart_end_size;

                if (this.p1.isAlive())
                    msg_win = this.p1.Name;
                else
                    msg_win = this.p2.Name;
                msg_win += " won the game !";
                msg_win_size = this.game_font.MeasureString(msg_win);
                msg_restart_begin_size = this.game_font.MeasureString(msg_restart_begin);
                msg_restart_end_size = this.game_font.MeasureString(msg_restart_end);
                x_msg_restart = this.width / 2 - ((msg_restart_begin_size.X + this.xbox_button_a.Width + msg_restart_end_size.X) / 2);
                spriteBatch.Begin();
                this.spriteBatch.DrawString(this.game_font, msg_win, new Vector2(this.width / 2 - msg_win_size.X / 2, this.height / 2 - msg_win_size.Y / 2), Color.White);
                this.spriteBatch.DrawString(this.game_font, msg_restart_begin, new Vector2(x_msg_restart, this.height - msg_restart_begin_size.Y - 15), Color.White);
                if (this.xbox_controler)
                    this.spriteBatch.Draw(this.xbox_button_a, new Vector2(x_msg_restart + msg_restart_begin_size.X + 10, this.height - msg_restart_begin_size.Y - 20), Color.White);
                else
                    this.spriteBatch.Draw(this.key_r, new Vector2(x_msg_restart + msg_restart_begin_size.X + 10, this.height - msg_restart_begin_size.Y - 20), Color.White);
                this.spriteBatch.DrawString(this.game_font, msg_restart_end, new Vector2(x_msg_restart + msg_restart_begin_size.X + 20 + this.xbox_button_a.Width, this.height - msg_restart_begin_size.Y - 15), Color.White);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
