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

namespace DriveFast
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        private Texture2D mCar;
        private Texture2D mCar2;
        private Texture2D mBackground;
        private Texture2D mRoad;
        private Texture2D mRoad2;
        private Texture2D mHazard;
        private Texture2D mApple;
        private Texture2D mPineapple;
        private Song backgroundMusic;
        private SoundEffect ohno;
        private SoundEffect crash;
        private SoundEffect gameover;
        private SoundEffect get;
        private SoundEffect cheer;
        private Song start;

        private KeyboardState mPreviousKeyboardState;
        private Vector2 mCarPosition = new Vector2(280, 440);
        private int mMoveCarX = 5;
        private int mVelocityY;
        private double mNextHazardAppearsIn;
        private double mNextAppleAppearsIn;
        private double mNextPineappleAppearsIn;
        private int mCarsRemaining;
        private int mHazardsPassed;
        private int mApplesPassed;
        private int score;
        private int mIncreaseVelocity;
        private double mExitCountDown = 10;

        private int[] mRoadY = new int[2];
        private List<Hazard> mHazards = new List<Hazard>();
        private List<Apple> mApples = new List<Apple>();
        private List<pineapple> mPineapples = new List<pineapple>();

        private int CarSelect = 1;
        private Boolean existApple = false;
        private Boolean existPineapple = false; 
        // 定义随机数 - 比方用来表示障碍物的位置
        private Random mRandom = new Random();
        private SpriteFont mFont;

        //----------------------- Feng ---------------------
        // 自定义枚举类型，表明不同的游戏状态
        private enum State
        {
            TitleScreen,     // 初始片头
            intro,
            Running,
            Crash,           // 碰撞
            GameOver,
            Success
        }
        //--------------------- Tian --------------------------


        private State mCurrentState = State.TitleScreen;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // 定义游戏窗口大小
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 760;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mCar = Content.Load<Texture2D>("Images/Car");
            mCar2 = Content.Load<Texture2D>("Images/Car2");
            mBackground = Content.Load<Texture2D>("Images/Background");
            mRoad = Content.Load<Texture2D>("Images/Road");
            mRoad2 = Content.Load<Texture2D>("Images/Road2");
            mHazard = Content.Load<Texture2D>("Images/Hazard");
            mApple = Content.Load<Texture2D>("Images/apple");
            mPineapple = Content.Load<Texture2D>("Images/pineapple");
            backgroundMusic = Content.Load<Song>("Music/backgroundMusic");
            ohno = Content.Load<SoundEffect>("Music/ohno");
            crash = Content.Load<SoundEffect>("Music/crash");
            gameover = Content.Load<SoundEffect>("Music/gameover");
            get = Content.Load<SoundEffect>("Music/get");
            cheer = Content.Load<SoundEffect>("Music/cheer");
            start = Content.Load<Song>("Music/start");
            MediaPlayer.Volume = 0.4f;
            MediaPlayer.Play(start);
            // 定义字体
            mFont = Content.Load<SpriteFont>("MyFont");
            //mBigFont = Content.Load<SpriteFont>("MyBigFont");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        protected void introGame()
        {
            mCurrentState = State.intro;
        }
        protected void StartGame()
        {
            MediaPlayer.Pause();
            MediaPlayer.Play(backgroundMusic);
            mRoadY[0] = 0;
            mRoadY[1] = -1 * mRoad.Height;
            mHazardsPassed = 0;
            mApplesPassed = 0;
            score = 0;
            if (CarSelect == 1)
            {
                mCarsRemaining = 3; // 所剩车辆的数量
                mVelocityY = 6;
            }
            else if (CarSelect == 2)
            {
                mCarsRemaining = 5;
                mVelocityY = 4;
            }
            mNextHazardAppearsIn = 1;
            mNextAppleAppearsIn = 3.1;
            mNextPineappleAppearsIn = 5.2;
            mIncreaseVelocity = 5;  // 速度递增

            mHazards.Clear();
            mApples.Clear();

            mCurrentState = State.Running;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();

            //Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                aCurrentKeyboardState.IsKeyDown(Keys.Escape) == true)
            {
                this.Exit();
            }
            //减小音量F3
            // TODO
            if (aCurrentKeyboardState.IsKeyDown(Keys.F3) == true && mPreviousKeyboardState.IsKeyDown(Keys.F3) == false)
            {
                MediaPlayer.Volume -= 0.05f;
            }
            //增大音量F4
            if (aCurrentKeyboardState.IsKeyDown(Keys.F4) == true && mPreviousKeyboardState.IsKeyDown(Keys.F4) == false)
            {
                MediaPlayer.Volume += 0.05f;
            }
            //介绍页面
            if (aCurrentKeyboardState.IsKeyDown(Keys.M))
            {
                introGame();
            }
            switch (mCurrentState)
            {
                case State.TitleScreen:
                    {
                        if (aCurrentKeyboardState.IsKeyDown(Keys.Space) == true/*&& mPreviousKeyboardState.IsKeyDown(Keys.Space) == false*/)
                        {
                            introGame();
                        }
                        break;
                    }
                case State.intro:
                case State.Success:
                case State.GameOver:
                    {
                        ExitCountdown(gameTime);
                        //选择不同的车
                        if (aCurrentKeyboardState.IsKeyDown(Keys.A) == true && mPreviousKeyboardState.IsKeyDown(Keys.A) == false)
                        {   
                            mMoveCarX = 6;
                            CarSelect = 1;
                            mVelocityY = 5;
                            StartGame();
                        }
                        if (aCurrentKeyboardState.IsKeyDown(Keys.S) == true && mPreviousKeyboardState.IsKeyDown(Keys.S) == false)
                        {
                            mMoveCarX = 3;
                            CarSelect = 2;
                            mVelocityY = 4;
                            StartGame();
                        }
                        break;
                    }

                case State.Running:
                    {
                        if (CarSelect == 1)
                            if (mVelocityY > 15) mVelocityY = 15;
                        if (CarSelect == 2)
                            if (mVelocityY > 10) mVelocityY = 10;
                        //If the user has pressed the Spacebar, then make the Car switch lanes
                        if (aCurrentKeyboardState.IsKeyDown(Keys.Left) == true/*&& mPreviousKeyboardState.IsKeyDown(Keys.Space) == false*/)
                        {
                            mCarPosition.X -= mMoveCarX;
                        }
                        if (aCurrentKeyboardState.IsKeyDown(Keys.Right) == true/*&& mPreviousKeyboardState.IsKeyDown(Keys.Space) == false*/)
                        {
                            mCarPosition.X += mMoveCarX;
                        }
                        if (mCarPosition.X >= 600 || mCarPosition.X <= 50)
                        {
                            mCurrentState = State.Crash;
                            mCarsRemaining--;
                            crash.Play();
                            if (mCarsRemaining < 0)
                            {
                                mCurrentState = State.GameOver;
                                gameover.Play();
                                MediaPlayer.Pause();
                                mExitCountDown = 10;
                                if (mCarPosition.X <= 50) mCarPosition.X += 5;
                                if (mCarPosition.X >= 600) mCarPosition.X -= 5;
                            }
                        }
                        ScrollRoad();
                        foreach (Hazard aHazard in mHazards)
                        {
                            if (CheckCollision(aHazard) == true)
                            {
                                break;
                            }
                            MoveHazard(aHazard);
                        }
                        foreach (Apple aApple in mApples)
                        {
                            if (CheckAppleCollision(aApple) == 1)
                            {
                                break;
                            }
                            MoveApple(aApple);
                        }
                        foreach (pineapple apineapple in mPineapples)
                        {
                            if (CheckPineappleCollision(apineapple) == 1)
                            {
                                break;
                            }
                            MovePineapple(apineapple);
                        }
                        UpdateHazards(gameTime);
                        UpdateApples(gameTime);
                        UpdatePineapples(gameTime);
                        break;
                    }
                case State.Crash:
                    {
                        //If the user has pressed the Space key, then resume driving
                        if (aCurrentKeyboardState.IsKeyDown(Keys.Space) == true && mPreviousKeyboardState.IsKeyDown(Keys.Space) == false)
                        {
                            mHazards.Clear();
                            mApples.Clear();
                            existApple = false;
                            foreach(Apple apple in  mApples)
                            {
                                apple.isEated = false;
                                apple.Visible = true;
                            }
                            foreach (pineapple pineapple in mPineapples)
                            {
                                pineapple.isEated = false;
                                pineapple.Visible = true;
                            }
                            existApple = false;
                            existPineapple = false;
                            if (mCarPosition.X <= 50) mCarPosition.X += 20;
                            if (mCarPosition.X >= 600) mCarPosition.X -= 20;
                            mCurrentState = State.Running;
                        }

                        break;
                    }
            }
            mPreviousKeyboardState = aCurrentKeyboardState;

            base.Update(gameTime);
        }

        //----------------------- Feng ---------------------
        // 让路面向后移动（使车辆看起来在往前行）
        private void ScrollRoad()
        {
            //Move the scrolling Road
            for (int aIndex = 0; aIndex < mRoadY.Length; aIndex++)
            {
                if (mRoadY[aIndex] >= this.Window.ClientBounds.Height) // 检测路面有没有移出游戏窗口
                {
                    int aLastRoadIndex = aIndex;
                    for (int aCounter = 0; aCounter < mRoadY.Length; aCounter++)
                    {
                        if (mRoadY[aCounter] < mRoadY[aLastRoadIndex])
                        {
                            aLastRoadIndex = aCounter;
                        }
                    }
                    mRoadY[aIndex] = mRoadY[aLastRoadIndex] - mRoad.Height; // 改变Y坐标，让路移动
                }
            }

            for (int aIndex = 0; aIndex < mRoadY.Length; aIndex++)
            {
                mRoadY[aIndex] += mVelocityY;
            }
        }
        //----------------------- Tian ---------------------

        private void MoveHazard(Hazard theHazard)
        {
            theHazard.Position.Y += mVelocityY;
            if (theHazard.Position.Y > graphics.GraphicsDevice.Viewport.Height && theHazard.Visible == true)
            {
                theHazard.Visible = false;
                mHazardsPassed += 1;
                score += 5;
                if (mHazardsPassed >= 200) // 如果超过200个障碍物，通关游戏
                {
                    MediaPlayer.Pause();
                    cheer.Play();
                    mCurrentState = State.Success;
                    mExitCountDown = 10;
                }

                mIncreaseVelocity -= 1;
                if (mIncreaseVelocity < 0)
                {
                    mIncreaseVelocity = 5;
                    mVelocityY += 1;
                }
            }
        }

        private void MoveApple(Apple theApple)
        {
            theApple.Position.Y += mVelocityY;
            if (theApple.Position.Y > graphics.GraphicsDevice.Viewport.Height && theApple.Visible == true)
            {
                theApple.Visible = false;
                theApple.isEated = true;
                existApple = false;
                if (score >= 1000)  //第二种通关的可能就是score超过1000
                {
                    MediaPlayer.Pause();
                    cheer.Play();
                    mCurrentState = State.Success;
                    mExitCountDown = 10;
                }
            }
        }
        private void MovePineapple(pineapple thepineapple)
        {
            thepineapple.Position.Y += mVelocityY;
            if (thepineapple.Position.Y > graphics.GraphicsDevice.Viewport.Height && thepineapple.Visible == true)
            {
                thepineapple.Visible = false;
                thepineapple.isEated = true;
                existPineapple = false;
                if (score >= 1000)
                {
                    MediaPlayer.Pause();
                    cheer.Play();
                    mCurrentState = State.Success;
                    mExitCountDown = 10;
                }
            }
        }
        private void UpdateHazards(GameTime theGameTime)
        {
            mNextHazardAppearsIn -= theGameTime.ElapsedGameTime.TotalSeconds; // 游戏运行的时间
            if (mNextHazardAppearsIn < 0)
            {
                int aLowerBound = 24 - (mVelocityY * 2);
                int aUpperBound = 30 - (mVelocityY * 2);

                if (mVelocityY > 10)
                {
                    aLowerBound = 6;
                    aUpperBound = 8;
                }

                // 控制障碍物出现的位置（随机）
                mNextHazardAppearsIn = (double)mRandom.Next(aLowerBound, aUpperBound) / 10;
                AddHazard();
            }
        }

        private void UpdateApples(GameTime theGameTime)
        {
            mNextAppleAppearsIn -= theGameTime.ElapsedGameTime.TotalSeconds; // 游戏运行的时间
            if (mNextAppleAppearsIn < 0)
            {
                int aLowerBound = 24 - (mVelocityY * 2);
                int aUpperBound = 30 - (mVelocityY * 2);

                if (mVelocityY > 10)
                {
                    aLowerBound = 6;
                    aUpperBound = 8;
                }

                // 控制障碍物出现的位置（随机）
                if (!existApple)
                {
                    mNextAppleAppearsIn = (double)mRandom.Next(aLowerBound, aUpperBound) / 10;
                    AddApple();
                    existApple = true;
                }
            }
        }
        private void UpdatePineapples(GameTime theGameTime)
        {
            mNextPineappleAppearsIn -= theGameTime.ElapsedGameTime.TotalSeconds; // 游®?戏¡¤运?行D的Ì?时º¡À间?
            if (mNextPineappleAppearsIn < 0)
            {
                int aLowerBound = 24 - (mVelocityY * 2);
                int aUpperBound = 30 - (mVelocityY * 2);

                if (mVelocityY > 10)
                {
                    aLowerBound = 6;
                    aUpperBound = 8;
                }
                if (!existPineapple)
                {
                    mNextPineappleAppearsIn = (double)mRandom.Next(aLowerBound, aUpperBound) / 10;
                    AddPineapple();
                    existPineapple = true;
                }
            }
        }
        private void AddHazard()
        {
            int aRoadPosition = mRandom.Next(1, 8);
            int aPosition = 90 + mRandom.Next(-40,40);
            if (aRoadPosition == 2)
            {
                aPosition = 265 + mRandom.Next(-40, 40);
            }
            else if (aRoadPosition == 3 )
            {
                aPosition = 455 + mRandom.Next(-40, 40);
            }
            else if (aRoadPosition == 4)
            {
                aPosition = 610 + mRandom.Next(-40, 40);
            }
            else if (aRoadPosition == 5)
            {
                aPosition = 180 + mRandom.Next(-20, 20);
            }
            else if (aRoadPosition == 6 )
            {
                aPosition = 530 + mRandom.Next(-20, 20);
            }
            else if (aRoadPosition == 7)
            {
                aPosition = 360 + mRandom.Next(-20, 20);
            }
            bool aAddNewHazard = true;
            foreach (Hazard aHazard in mHazards)
            {
                if (aHazard.Visible == false)
                {
                    aAddNewHazard = false;
                    aHazard.Visible = true;
                    aHazard.Position = new Vector2(aPosition, -mHazard.Height);
                    break;
                }
            }

            if (aAddNewHazard == true)
            {
                //Add a hazard to the left side of the Road
                Hazard aHazard = new Hazard();
                aHazard.Position = new Vector2(aPosition, -mHazard.Height);

                mHazards.Add(aHazard);
            }
        }

        private void AddApple()
        {
            int aRoadPosition = mRandom.Next(1, 8);
            int aPosition = 85 - mRandom.Next(1, 11); ;
            if (aRoadPosition == 2)
            {
                aPosition = 120+mRandom.Next(1,11);
            }
            else if (aRoadPosition == 3)
            {
                aPosition = 240 - mRandom.Next(1, 11);
            }
            else if (aRoadPosition == 4)
            {
                aPosition = 280 + mRandom.Next(1, 11);
            }
            else if (aRoadPosition == 5)
            {
                aPosition = 410 - mRandom.Next(1, 11);
            }
            else if (aRoadPosition == 6)
            {
                aPosition = 450 + mRandom.Next(1, 11);
            }
            else if (aRoadPosition == 7)
            {
                aPosition = 560 - mRandom.Next(1, 11);
            }
            bool aAddNewApple = true;
            foreach (Apple aApple in mApples)
            {
                if (aApple.Visible == false)
                {
                    aAddNewApple = false;
                    aApple.Visible = true;
                    aApple.isEated = false;
                    aApple.Position = new Vector2(aPosition, -mApple.Height);
                    break;
                }
            }
            if (aAddNewApple == true)
            {
                //Add a Apple to the left side of the Road
                Apple aApple = new Apple();
                aApple.Position = new Vector2(aPosition, -mApple.Height);

                mApples.Add(aApple);
            }
        }
        private void AddPineapple()
        {
            int aRoadPosition = mRandom.Next(1,5);
            int aPosition = 130;
            if (aRoadPosition == 2)
            {
                aPosition = 180;
            }
            else if (aRoadPosition == 3)
            {
                aPosition = 350;
            }
            else if (aRoadPosition == 4)
            {
                aPosition = 500;
            }
            bool aAddNewPineapple = true;
            foreach (pineapple aPineapple in mPineapples)
            {
                if (aPineapple.Visible == false)
                {
                    aAddNewPineapple = false;
                    aPineapple.Visible = true;
                    aPineapple.isEated = false;
                    aPineapple.Position = new Vector2(aPosition, -mPineapple.Height);
                    break;
                }
            }
            if (aAddNewPineapple == true)
            {
                //Add a Pineapple to the left side of the Road
                pineapple aPineapple = new pineapple();
                aPineapple.Position = new Vector2(aPosition, -mPineapple.Height);

                mPineapples.Add(aPineapple);
            }
        }



        //----------------------- Feng ------------------------------------------------
        // 检测车辆是否碰到了障碍物
        private bool CheckCollision(Hazard theHazard)
        {
            // 分别计算并使用封闭（包裹）盒给障碍物和车
            BoundingBox aHazardBox = new BoundingBox(new Vector3(theHazard.Position.X, theHazard.Position.Y, 0), new Vector3(theHazard.Position.X + (mHazard.Width * .4f), theHazard.Position.Y + ((mHazard.Height - 50) * .4f), 0));
            BoundingBox aCarBox = new BoundingBox(new Vector3(mCarPosition.X, mCarPosition.Y, 0), new Vector3(mCarPosition.X + (mCar.Width * .2f), mCarPosition.Y + (mCar.Height * .2f), 0));

            if (aHazardBox.Intersects(aCarBox) == true) // 碰上了吗?
            {
                crash.Play();
                mCurrentState = State.Crash;
                mCarsRemaining -= 1;
                if (mCarsRemaining < 0)
                {
                    mCurrentState = State.GameOver;
                    mExitCountDown = 10;
                    gameover.Play();
                    MediaPlayer.Pause();
                }
                return true;
            }

            return false;
        }
        private int CheckAppleCollision(Apple theApple)
        {
            // 分别计算并使用封闭（包裹）盒给障碍物和车
            BoundingBox aAppleBox = new BoundingBox(new Vector3(theApple.Position.X, theApple.Position.Y, 0), new Vector3(theApple.Position.X + (mApple.Width * .4f), theApple.Position.Y + ((mApple.Height - 50) * .4f), 0));
            BoundingBox aCarBox = new BoundingBox(new Vector3(mCarPosition.X, mCarPosition.Y, 0), new Vector3(mCarPosition.X + (mCar.Width * .2f), mCarPosition.Y + (mCar.Height * .2f), 0));

            if (aAppleBox.Intersects(aCarBox) == true && theApple.isEated == false)
            {
                get.Play();
                MoveApple(theApple);
                theApple.Visible = false;
                theApple.isEated = true;
                existApple = false;
                score += 15;
                if(CarSelect == 1)if (mVelocityY >= 14) mVelocityY--;
                if (CarSelect == 2) if (mVelocityY >= 9) mVelocityY--;
                return 1;
            }
            return 0;
        }
        private int CheckPineappleCollision(pineapple thePineapple)
        {
            BoundingBox aPineappleBox = new BoundingBox(new Vector3(thePineapple.Position.X, thePineapple.Position.Y, 0), new Vector3(thePineapple.Position.X + (mPineapple.Width * .4f), thePineapple.Position.Y + ((mPineapple.Height - 50) * .4f), 0));
            BoundingBox aCarBox = new BoundingBox(new Vector3(mCarPosition.X, mCarPosition.Y, 0), new Vector3(mCarPosition.X + (mCar.Width * .2f), mCarPosition.Y + (mCar.Height * .2f), 0));

            if (aPineappleBox.Intersects(aCarBox) == true && thePineapple.isEated == false)
            {
                ohno.Play();
                MovePineapple(thePineapple);
                thePineapple.Visible = false;
                thePineapple.isEated = true;
                existPineapple = false;
                score -= 10;
                mVelocityY++;
                return 1;
            }
            return 0;
        }



        //----------------------- Tian ------------------------------------------------------

        private void ExitCountdown(GameTime theGameTime)
        {
            mExitCountDown -= theGameTime.ElapsedGameTime.TotalSeconds;
            if (mExitCountDown < 0 && mCurrentState != State.intro)
            {
                this.Exit();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(mBackground, new Rectangle(graphics.GraphicsDevice.Viewport.X, graphics.GraphicsDevice.Viewport.Y, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);

            switch (mCurrentState)
            {
                case State.TitleScreen:
                    {
                        //Draw the display text for the Title screen
                        DrawTextCentered("Welcome To DriveFast Game!", 200);
                        DrawTextCentered("Press 'Space' to begin", 260);
                        DrawTextCentered("Exit in " + ((int)mExitCountDown).ToString(), 475);
                        ExitCountdown(gameTime);
                        break;
                    }
                case State.intro:
                    {
                        DrawTextCentered("Hi, Here are some introductions about this game:      ", 50);
                        DrawTextCentered("Goals: get 1000 scores or pass 200 Hazards            ", 100);
                        DrawTextCentered("===================================================", 130);
                        DrawTextCentered("Press A or S to choose different Car                   ", 160);
                        DrawTextCentered("Press <- and -> to control Car to avoid hazards        ", 190);
                        DrawTextCentered("Press M to enter the introduction page                 ", 220);
                        DrawTextCentered("Use F3 and F4 to control volume                        ", 250);
                        DrawTextCentered("===================================================", 280);
                        DrawTextCentered("                  About the Cars                   ", 310);
                        DrawTextCentered("If you press A, the car is easier to control,with 3     ", 340);
                        DrawTextCentered("cars, but at a higher top speed;                        ", 370);
                        DrawTextCentered("If you press S, the car is less flexiable,but it is     ", 400);
                        DrawTextCentered("with 5 cars, and at a lower top speed.                  ", 430);
                        DrawTextCentered("===================================================", 460);
                        DrawTextCentered("                   ATTENTION                       ", 490);
                        DrawTextCentered("Each apple will GIVE you 10 scores,While each pineapple ", 520);
                        DrawTextCentered("will DECREASE 10 scores and increase the SPEED.         ", 550);
                        break;
                    }
                default:
                    {
                        DrawRoad();
                        DrawHazards();
                        DrawApples();
                        DrawPineapples();
                        if (CarSelect == 1)
                        {
                            spriteBatch.Draw(mCar, mCarPosition, new Rectangle(0, 0, mCar.Width, mCar.Height), Color.White, 0, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);
                            spriteBatch.DrawString(mFont, "Cars:", new Vector2(28, 520), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                            for (int aCounter = 0; aCounter < mCarsRemaining; aCounter++)
                            {
                                spriteBatch.Draw(mCar, new Vector2(25 + (30 * aCounter), 550), new Rectangle(0, 0, mCar.Width, mCar.Height), Color.White, 0, new Vector2(0, 0), 0.05f, SpriteEffects.None, 0);
                            }
                        }
                        else if (CarSelect == 2)
                        {
                            spriteBatch.Draw(mCar2, mCarPosition, new Rectangle(0, 0, mCar.Width, mCar.Height), Color.White, 0, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);
                            spriteBatch.DrawString(mFont, "Cars:", new Vector2(28, 520), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                            for (int aCounter = 0; aCounter < mCarsRemaining; aCounter++)
                            {
                                spriteBatch.Draw(mCar2, new Vector2(25 + (30 * aCounter), 550), new Rectangle(0, 0, mCar.Width, mCar.Height), Color.White, 0, new Vector2(0, 0), 0.05f, SpriteEffects.None, 0);
                            }
                        }

                        spriteBatch.DrawString(mFont, "Hazards: " + mHazardsPassed.ToString(), new Vector2(5, 25), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

                        spriteBatch.DrawString(mFont, "Apples: " + mApplesPassed.ToString(), new Vector2(5, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

                        spriteBatch.DrawString(mFont, "Speed: " + mVelocityY, new Vector2(5, 75), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

                        spriteBatch.DrawString(mFont, "Score: " + score.ToString(), new Vector2(5, 100), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

                        if (mCurrentState == State.Crash)
                        {
                            DrawTextDisplayArea();
                            DrawTextCentered("Crash!", 200);
                            DrawTextCentered("Press 'Space' to continue driving.", 260);
                        }
                        else if (mCurrentState == State.GameOver)
                        {
                            DrawTextDisplayArea();
                            DrawTextCentered("Game Over!", 200);
                            DrawTextCentered("Try again? Press 'A' for Car or 'S' for Truck.", 260);
                            DrawTextCentered("Exit in " + ((int)mExitCountDown).ToString(), 400);
                            existApple = false;
                        }
                        else if (mCurrentState == State.Success)
                        {
                            DrawTextDisplayArea();
                            DrawTextCentered("Congratulations!", 200);
                            DrawTextCentered("play again? Press 'A' for Car or 'S' for Truck.", 260);
                            DrawTextCentered("Exit in " + ((int)mExitCountDown).ToString(), 400);
                        }

                        break;
                    }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawRoad()
        {
            for (int aIndex = 0; aIndex < mRoadY.Length; aIndex++)
            {
                if (mRoadY[aIndex] > mRoad.Height * -1 && mRoadY[aIndex] <= this.Window.ClientBounds.Height)
                {
                    if(CarSelect == 1)spriteBatch.Draw(mRoad2, new Rectangle((int)((this.Window.ClientBounds.Width - mRoad.Width) / 2 - 18), mRoadY[aIndex], mRoad.Width, mRoad.Height + 5), Color.White);
                    else spriteBatch.Draw(mRoad, new Rectangle((int)((this.Window.ClientBounds.Width - mRoad.Width) / 2 - 18), mRoadY[aIndex], mRoad.Width, mRoad.Height + 5), Color.White);
                }
            }
        }

        private void DrawHazards()
        {
            foreach (Hazard aHazard in mHazards)
            {
                if (aHazard.Visible == true)
                {
                    spriteBatch.Draw(mHazard, aHazard.Position, new Rectangle(0, 0, mHazard.Width, mHazard.Height), Color.White, 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                }
            }
        }

        private void DrawApples()
        {
            foreach (Apple aApple in mApples)
            {
                if (aApple.Visible == true)
                {
                    spriteBatch.Draw(mApple, aApple.Position, new Rectangle(0, 0, mApple.Width, mApple.Height), Color.White, 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                }
            }
        }
        private void DrawPineapples()
        {
            foreach (pineapple aPineapple in mPineapples)
            {
                if (aPineapple.Visible == true)
                {
                    spriteBatch.Draw(mPineapple, aPineapple.Position, new Rectangle(0, 0, mPineapple.Width, mPineapple.Height), Color.White, 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                }
            }
        }
        private void DrawTextDisplayArea()
        {
            int aPositionX = (int)((graphics.GraphicsDevice.Viewport.Width / 2) - (450 / 2));
            spriteBatch.Draw(mBackground, new Rectangle(aPositionX, 75, 450, 400), Color.White);
        }

        private void DrawTextCentered(string theDisplayText, int thePositionY)
        {
            Vector2 aSize = mFont.MeasureString(theDisplayText);
            int aPositionX = (int)((graphics.GraphicsDevice.Viewport.Width / 2) - (aSize.X / 2));

            spriteBatch.DrawString(mFont, theDisplayText, new Vector2(aPositionX, thePositionY), Color.Green, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.DrawString(mFont, theDisplayText, new Vector2(aPositionX + 1, thePositionY + 1), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
        }
    }
}
