//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
//using TestBed.Interface;
//using TestBed.Tiles;
//using FarseerPhysics.Dynamics;
//using FarseerPhysics.Dynamics.Contacts;
//using System.IO;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.ComponentModel;

//namespace TestBed
//{

//    /// <summary>
//    /// This is the main type for your game
//    /// </summary>
//    public class Game1 : Microsoft.Xna.Framework.Game
//    {
//        DeferredRenderer m_deferredRenderer;

//        /*
//        String EditorMode = null;
//        GamePadState state = GamePad.GetState(PlayerIndex.One);
//        GraphicsDeviceManager graphics;
//        SpriteBatch spriteBatch;
//        Camera2D camera;
//        Input input;
//        Vector2 forceVector = new Vector2(0.2f, 0.0f);
//        TileFactory tileFactory;
//        LevelAssetStreamer levelAssetStreamer;
//        SpriteFont CourierNew;
//        Vector2 playerToCursorVector;
//        Texture2D blank;
//        BackgroundWorker levelStreamer;
//        int oldBlock = 0;

//        private Texture2D Green;
//        static object RenderLock = new object();

//        private DynamicCollidableTile dynamicTileOne;
//        private DynamicCollidableTile projecTile = null; //Lol Pun!
//        private StaticCollidableTile staticTileOne;

//        private World physicsWorld;
//        */

//        public Game1()
//        {
//            m_deferredRenderer = new DeferredRenderer(this);
//            Components.Add(m_deferredRenderer);
//            /*
//            graphics = new GraphicsDeviceManager(this);
//            graphics.PreferMultiSampling = true;
//            graphics.PreferredBackBufferWidth = 1280;
//            graphics.PreferredBackBufferHeight = 720;
//            ConvertUnits.SetDisplayUnitToSimUnitRatio(20.0f);
//            IsFixedTimeStep = true;
//            graphics.IsFullScreen = false;
//            Content.RootDirectory = "Content";
//            this.IsMouseVisible = true;
//            levelStreamer = new BackgroundWorker();
//            //levelStreamer.DoWork += new DoWorkEventHandler(levelStreamer_DoWork);
//            //levelStreamer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(levelStreamer_RunWorkerCompleted);
//            tileFactory = new TileFactory();
//            tileFactory.LoadTileStructure(@"Content\TileScripts");
//             */
//        }

//        //void levelStreamer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        //{
//        //    if (!levelStreamer.IsBusy)
//        //    {
//        //        levelStreamer.RunWorkerAsync();
//        //    }
//        //}

//        //void levelStreamer_DoWork(object sender, DoWorkEventArgs e)
//        //{
//        //    LoadBlock(dynamicTileOne.Position);
//        //}

//        protected override void OnExiting(Object sender, EventArgs args)
//        {
//            //levelAssetStreamer.SaveWorldToFile();
//            base.OnExiting(sender, args);
//            // Stop the threads
//        }

//        /// <summary>
//        /// Allows the game to perform any initialization it needs to before starting to run.
//        /// This is where it can query for any required services and load any non-graphic
//        /// related content.  Calling base.Initialize will enumerate through any components
//        /// and initialize them as well.
//        /// </summary>
//        protected override void Initialize()
//        {
//            // TODO: Add your initialization logic here

//            base.Initialize();
//        }

//        /// <summary>
//        /// LoadContent will be called once per game and is the place to load
//        /// all of your content.
//        /// </summary>
//        protected override void LoadContent()
//        {
//            /*
//            // Create a new SpriteBatch, which can be used to draw textures.
//            spriteBatch = new SpriteBatch(GraphicsDevice);
//            CourierNew = Content.Load<SpriteFont>("Text");
//            EditorMode = "Edit";
//            camera = new Camera2D(graphics.GraphicsDevice);
//            input = new Input(this);

//            //Create a world for physics
//            physicsWorld = new World(new Vector2(0, 20));

//            //Initialize that biatch
//            tileFactory.Initialize(spriteBatch, physicsWorld, this);

//            levelAssetStreamer = new LevelAssetStreamer(@"Content\Levels\TestLevel.level", tileFactory, CourierNew);
//            //levelAssetStreamer.saveTestWorld();

//            camera.Zoom = 1f;

//            Green = Content.Load<Texture2D>("Images/Green_Front");
//            dynamicTileOne = new DynamicCollidableTile(spriteBatch, Green, new Vector2(250.0f, 150.0f), new Rectangle(0, 0, 5, 5), 0.0f, physicsWorld);
//            staticTileOne = new StaticCollidableTile(spriteBatch, Green, new Vector2(250.0f, 180.0f), new Rectangle(0, 0, 5, 5), 0.0f, physicsWorld);

//            blank = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
//            blank.SetData(new[] { Color.White });

//            //dynamicTileTwo = new DynamicCollidableTile(spriteBatch, Green, new Vector2(250.0f, -20.0f), new Rectangle(0, 0, 5, 5), 0.0f, physicsWorld);
            
//            levelAssetStreamer.StreamForBody(dynamicTileOne.PhysicsBody, RenderLock);

//            camera.TrackingBody = ((DynamicCollidableTile)dynamicTileOne).PhysicsBody;*/
//            #region bindings

//        //    input.BindChordToAction(new Chord { Keys = new List<Keys> { Keys.D }, State = ChordState.Pressed }, () =>
//        //    {
//        //        var body = dynamicTileOne;
//        //        if (body.PhysicsBody.LinearVelocity.X < 5f)
//        //        {
//        //            body.ApplyForce(new Vector2(0.1f, 0f));
//        //        }
//        //    });

//        //    input.BindChordToAction(new Chord { Keys = new List<Keys> { Keys.A }, State = ChordState.Pressed }, () =>
//        //    {
//        //        var body = dynamicTileOne;
//        //        if (body.PhysicsBody.LinearVelocity.X > -5f)
//        //        {
//        //            body.ApplyForce(new Vector2(-0.1f, 0f));
//        //        }
//        //    });

//        //    input.BindChordToAction(new Chord { Keys = new List<Keys> { Keys.Space }, State = ChordState.Pressed }, () =>
//        //    {
//        //        var body = dynamicTileOne;
//        //        if (Math.Abs(body.PhysicsBody.LinearVelocity.Y) < 0.00001f)
//        //        {
//        //            body.ApplyForce(new Vector2(0f, -.5f));
//        //        }
//        //    });

//        //    input.BindChordToAction(new Chord { Keys = new List<Keys> { Keys.Tab }, State = ChordState.Pressed }, () =>
//        //    {
//        //       if(String.Compare(EditorMode, "Edit") == 0)
//        //       {
//        //           EditorMode = "Play";
//        //       }
//        //       else
//        //       {
//        //           EditorMode = "Edit";
//        //       }
//        //    });

//        //    input.BindMouseButtonToAction(MouseButton.Right, ButtonState.Pressed, () =>
//        //    {
//        //        if(String.Compare(EditorMode, "Edit") == 0)
//        //        {
//        //            Vector2 cameraInput = camera.ConvertScreenToWorld(input.MousePosition());
//        //            cameraInput.X = (float)Math.Abs((Math.Round(cameraInput.X * 2) / 2));
//        //            cameraInput.Y = (float)Math.Abs((Math.Round(cameraInput.Y * 2) / 2));
//        //            var fixture = physicsWorld.TestPoint(cameraInput);

//        //            if (fixture == null)
//        //            {
//        //                levelAssetStreamer.AddTileAt(cameraInput);
//        //                levelAssetStreamer.Update(RenderLock);
//        //            }
//        //        }
//        //        else
//        //        {}
//        //    });

//        //    input.BindMouseButtonToAction(MouseButton.Left, ButtonState.Pressed, () =>
//        //    {
//        //        if (String.Compare(EditorMode, "Edit") == 0)
//        //        {
//        //            Vector2 cameraInput = camera.ConvertScreenToWorld(input.MousePosition());
//        //            cameraInput.X = (float)Math.Abs((Math.Round(cameraInput.X * 2) / 2));
//        //            cameraInput.Y = (float)Math.Abs((Math.Round(cameraInput.Y * 2) / 2));
//        //            var fixture = physicsWorld.TestPoint(cameraInput);

//        //            if (fixture != null)
//        //            {
//        //                levelAssetStreamer.RemoveTileAt(cameraInput);
//        //            }
//        //        }
//        //        else 
//        //        {
//        //            playerToCursorVector = ConvertUnits.ToDisplayUnits(camera.ConvertScreenToWorld(input.MousePosition()));

//        //            projecTile = new DynamicCollidableTile(spriteBatch,
//        //                Green,
//        //                ConvertUnits.ToDisplayUnits(dynamicTileOne.Position),
//        //                new Rectangle(0, 0, 3, 3),
//        //                0,
//        //                physicsWorld);
//        //            projecTile.PhysicsBody.IsBullet = true;
//        //            projecTile.PhysicsBody.IgnoreCollisionWith(dynamicTileOne.PhysicsBody);
//        //            Vector2 direction = ConvertUnits.ToDisplayUnits(camera.ConvertScreenToWorld(input.MousePosition())) - 
//        //                ConvertUnits.ToDisplayUnits(dynamicTileOne.Position);
//        //            direction.Normalize();
//        //            projecTile.PhysicsBody.LinearVelocity = direction * 30 ;

//        //            projecTile.PhysicsBody.OnCollision += DislodgeBlock;
//        //        }
//        //    });

//        //    input.BindMouseButtonToAction(MouseButton.Left, ButtonState.Released, () =>
//        //    {
//        //        if (String.Compare(EditorMode, "Edit") == 0)
//        //        {
//        //        }
//        //        else
//        //        {
//        //            playerToCursorVector = Vector2.Zero;
//        //        }
//        //    });
//            #endregion
//        }


//        //public bool DislodgeBlock(Fixture f1, Fixture f2, Contact contact)
//        //{

//        //    f2.Body.BodyType = BodyType.Dynamic;
//        //    f2.Body.SleepingAllowed = true;
//        //    projecTile.PhysicsBody.Dispose();
//        //    projecTile = null;
//        //    return true;
//        //} 


//        private void WriteBlockToFile(int blockID, List<int> blockToWrite)
//        {
//            //LevelBlockCache[blockID] = blockToWrite;
//            //using (var sw = new BinaryWriter(File.Open("Content/Levels/TestLevel.level", FileMode.Create)))
//            //{
//            //    Int32 seekDistance = 0;
                
//            //    //Need to seek
//            //    // (2 * Int32) Which is the definition of the block sizes.
//            //    seekDistance += 2 * sizeof(Int32);

//            //    // (100 (amount of tiles in a block) * Int32) * blockID
//            //    seekDistance += (100 * sizeof(Int32)) * blockID;

//            //    sw.Seek(seekDistance, SeekOrigin.Begin);

//            //    blockToWrite.ForEach(tile => sw.Write(tile.TileID));
//            //}
//        }

//        private void WriteWorldToFile()
//        {
//            //using (var sw = new BinaryWriter(File.Open("Content/Levels/TestLevel.level", FileMode.Create)))
//            //{
//            //    sw.Write(10); //Block Width
//            //    sw.Write(10); //Block Height

//            //    //Find out how many 'blocks' there will be
//            //    int tileCount = 0;

//            //    foreach (var list in EarthTileBox.Values)
//            //    {
//            //        tileCount += list.Count;
//            //    }

//            //    int requiredBlocks = tileCount / 100; // Tiles / (Width * Height)
//            //    var listOfBlocks = new List<List<int>>();

//            //    for (int i = 0; i < requiredBlocks; i++)
//            //    {
//            //        listOfBlocks.Add(new List<int>());
//            //    }

//            //    //EarthTileBox.ForEach(l => listOfBlocks[((int)l.Position.X) % 5].Add(l.TileID));
//            //    foreach (var list in EarthTileBox.Values)
//            //    {
//            //        foreach (ITile tile in list)
//            //        {
//            //            listOfBlocks[(int)(tile.Position.X / 5) - 1].Add(tile.TileID);
//            //        }
//            //    }


//            //    //Write the blocks to file
//            //    listOfBlocks.ForEach(list => list.ForEach(entry => sw.Write(entry)));
//            //}
//        }

//        /// <summary>
//        /// UnloadContent will be called once per game and is the place to unload
//        /// all content.
//        /// </summary>
//        protected override void UnloadContent()
//        {
//            // TODO: Unload any non ContentManager content here
//            //levelAssetStreamer.SaveWorldToFile();
//        }

//        /// <summary>
//        /// Allows the game to run logic such as updating the world,
//        /// checking for collisions, gathering input, and playing audio.
//        /// </summary>
//        /// <param name="gameTime">Provides a snapshot of timing values.</param>
//        protected override void Update(GameTime gameTime)
//        {
//            /*
//            // Allows the game to exit
//            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
//                this.Exit();

//            levelAssetStreamer.Update(RenderLock);

//            // TODO: Add your update logic here
//            lock(RenderLock)
//                physicsWorld.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 10f)));
        
//            camera.Update(gameTime);
//            input.Update();
//            forceVector = RotateVector2(forceVector, dynamicTileOne.RotationAngle);

//            //camera.Zoom += 0.001f;
//            base.Update(gameTime);
//             * */
//        }

//        public static Vector2 RotateVector2(Vector2 point, float radians)
//        {
//            float cosRadians = (float)Math.Cos(radians);
//            float sinRadians = (float)Math.Sin(radians);

//            return new Vector2(
//                point.X * cosRadians - point.Y * sinRadians,
//                point.X * sinRadians + point.Y * cosRadians);
//        }
//        /// <summary>
//        /// This is called when the game should draw itself.
//        /// </summary>
//        /// <param name="gameTime">Provides a snapshot of timing values.</param>
//        protected override void Draw(GameTime gameTime)
//        {

//            /*GraphicsDevice.Clear(Color.CornflowerBlue);

//            spriteBatch.Begin(0, null, null, null, null, null, camera.BatchViewMatrix);

//            lock (RenderLock)
//            {
//                if (tileFactory.Tiles != null)
//                {
//                    levelAssetStreamer.Render(true);
//                }
//            }

//            dynamicTileOne.Draw();
//            staticTileOne.Draw();
//            if(projecTile != null)
//                projecTile.Draw();

//            var mouseInfo = "Mouse Pos:" + (int)Math.Abs((Math.Round(camera.ConvertScreenToWorld(input.MousePosition()).X * 4) / 2)) + " "
//                                         + (int)Math.Abs((Math.Round(camera.ConvertScreenToWorld(input.MousePosition()).Y * 4) / 2)) + "\n Mode:"
//                                         + EditorMode ;
//            Vector2 FontOrigin = CourierNew.MeasureString(mouseInfo);
//            //spriteBatch.DrawString(CourierNew, mouseInfo, ConvertUnits.ToDisplayUnits(camera.ConvertScreenToWorld(input.MousePosition())), Color.Black);

//            //if(playerToCursorVector != Vector2.Zero)
//            //    DrawLine(spriteBatch, blank, 2, Color.Green, ConvertUnits.ToDisplayUnits(dynamicTileOne.Position), playerToCursorVector);

//            spriteBatch.End();

//            base.Draw(gameTime);*/
//        }

//        void DrawLine(SpriteBatch batch, Texture2D blank, float width, Color color, Vector2 point1, Vector2 point2)
//        {
//            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
//            float length = Vector2.Distance(point1, point2);

//            batch.Draw(blank, point1, null, color,
//                       angle, Vector2.Zero, new Vector2(length, width),
//                       SpriteEffects.None, 0);
//        }
//    }
//}
