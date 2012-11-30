using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using TestBed.Interface;
using Microsoft.Xna.Framework.Graphics;

namespace TestBed
{
    /// <summary>
    /// The blocks that should be rendered around an object
    /// ________________
    /// |    |    |    |
    /// |__0_|__1_|__2_|
    /// |    |Body|    |
    /// |__3_|__4_|__5_|
    /// |    |    |    |
    /// |__6_|__7_|__8_|
    /// </summary>
    public class BodyRenderKernel
    {
        const int TOP_LEFT = 0;
        const int TOP_MIDDLE = 1;
        const int TOP_RIGHT = 2;
        const int CENTER_LEFT = 3;
        const int CENTER_MIDDLE = 4;
        const int CENTER_RIGHT = 5;
        const int BOTTOM_LEFT = 6;
        const int BOTTOM_MIDDLE = 7;
        const int BOTTOM_RIGHT = 8;

        /// <summary>
        /// SpriteFont to render Debug text with.
        /// </summary>
        SpriteFont CourierNew;

        /// <summary>
        /// The body to render around.
        /// </summary>
        private Body m_trackingBody;

        /// <summary>
        /// The blocks that holds the tiles
        /// </summary>
        private List<List<ITile>> m_renderKernel;

        /// <summary>
        /// The height of the world this RenderKernal exists in (In Blocks)
        /// </summary>
        private int m_worldHeight;

        /// <summary>
        /// The width of the world this RenderKernel exists in (In Blocks)
        /// </summary>
        private int m_worldWidth;

        /// <summary>
        /// Font to render debug text with.
        /// </summary>
        SpriteFont m_font;

        public BodyRenderKernel(Body trackingBody, int blockSize, int worldWidth, int worldHeight, SpriteFont debugFont = null)
        {
            m_trackingBody = trackingBody;
            m_renderKernel = new List<List<ITile>>(){new List<ITile>(), new List<ITile>(), new List<ITile>(),
                                                     new List<ITile>(), new List<ITile>(), new List<ITile>(),
                                                     new List<ITile>(), new List<ITile>(), new List<ITile>()};
            m_worldHeight = worldHeight;
            m_worldWidth = worldWidth;
            m_font = debugFont;
            PreviousBlock = -1000;
        }

        /// <summary>
        /// The Block cache.
        /// </summary>
        public List<List<ITile>> BlockCache
        {
            get
            {
                return m_renderKernel;
            }
            private set
            {
                m_renderKernel = value;
            }
        }

        /// <summary>
        /// Converts the given blockID into a rneder kernel ID
        /// </summary>
        /// <param name="blockID">The Block ID you want to query</param>
        /// <returns>The ID of the render kernel section</returns>
        public int ConvertBlockIdToRenderKernelID(int blockID)
        {
            int sectionOne = (blockID - (CurrentBlock - m_worldWidth - 1)) % m_worldWidth;
            int sectionTwo = (int)((blockID - (CurrentBlock - m_worldWidth - 1)) / m_worldWidth) * 3;
            int returnID = sectionOne + sectionTwo;
            return returnID > 8 ? -1 : returnID;
        }
        /// <summary>
        /// The the block the Physics body was in, in the previous Update
        /// </summary>
        public int CurrentBlock { get; private set; }

        /// <summary>
        /// The block the Physics was previously in
        /// </summary>
        public int PreviousBlock { get; private set; }

        public void Update()
        {
            //Update the block number the PhysicsBody is in.
            int widthLocation = (int)Math.Floor(m_trackingBody.Position.X / 10) ;
            int heightLocation = (int)(m_trackingBody.Position.Y / 10);
            var currentBlock = widthLocation + (m_worldWidth * heightLocation);
            var tempBlock = 0;

            tempBlock = CurrentBlock;
            CurrentBlock = currentBlock;
            PreviousBlock = tempBlock;
        }

        public void Render(SpriteBatch spriteBatch)
        {
            var blockInfo = "Previous Block: " + PreviousBlock + "\nCurrent Block: " + CurrentBlock +
                "\nPosition: " + m_trackingBody.Position.X + ", " + m_trackingBody.Position.Y;
            Vector2 FontOrigin = m_font.MeasureString(blockInfo);
            spriteBatch.DrawString(m_font, blockInfo, ConvertUnits.ToDisplayUnits(m_trackingBody.Position), Color.Black);
        }
    }

    /// <summary>
    /// Object to handle the loading/disposing of objects in a level
    /// </summary>
    public class LevelAssetStreamer
    {

        internal class CachedTileInformation
        {
            /// <summary>
            /// The ID of the tile to load
            /// </summary>
            public int TileID { get; set; }

            /// <summary>
            /// The X Position of the Tile (Local to the Block)
            /// </summary>
            public int TilePosX { get; set; }

            /// <summary>
            /// The Y Position of the Tile (Local to the Block)
            /// </summary>
            public int TilePosY { get; set; }
        }

        const int TOP_LEFT = 0;
        const int TOP_MIDDLE = 1;
        const int TOP_RIGHT = 2;
        const int CENTER_LEFT = 3;
        const int CENTER_MIDDLE = 4;
        const int CENTER_RIGHT = 5;
        const int BOTTOM_LEFT = 6;
        const int BOTTOM_MIDDLE = 7;
        const int BOTTOM_RIGHT = 8;

        /// <summary>
        /// The path to the level file.
        /// </summary>
        private String m_levelFile;

        /// <summary>
        /// The IDs of each Tile in each Block in the world
        /// A Block is a collection of Tiles
        /// A Tile is the base component of the world.
        /// </summary>
        List<List<CachedTileInformation>> m_levelBlockCache;

        /// <summary>
        /// List of Render Kernals to render.
        /// </summary>
        BodyRenderKernel m_renderKernel;

        /// <summary>
        /// Factory for creating tiles.
        /// </summary>
        TileFactory m_tileFactory;

        /// <summary>
        /// Font to render debug text with.
        /// </summary>
        SpriteFont m_font;

        /// <summary>
        /// List of bools representing the render kernel blocks, and if they need updating.
        /// </summary>
        List<bool> m_renderKernelRequiresUpdate;

        /// <summary>
        /// Constructor for LevelAssetStreamer
        /// </summary>
        /// <param name="levelFile"></param>
        public LevelAssetStreamer(String levelFile, TileFactory tileFactory, SpriteFont debugFont = null)
        {
            m_levelFile = levelFile;
            m_levelBlockCache = new List<List<CachedTileInformation>>();
            m_renderKernelRequiresUpdate = new List<bool> { true, true, true, true, true, true, true, true, true };
            m_tileFactory = tileFactory;
            m_font = debugFont;
            TilesNotInUse = new List<ITile>();
            //saveTestWorld();
            LoadWorldIDsIntoCache();
        }


        /// <summary>
        /// Physics Body to stream the world around
        /// </summary>
        public void StreamForBody(Body physicsBody, object blockLock)
        {
            m_renderKernel = new BodyRenderKernel(physicsBody, TilesPerBlockWidth, WorldWidth, WorldHeight, m_font);
            m_renderKernel.Update();
            LoadBlock(blockLock);
        }

        /// <summary>
        /// The width of the world loaded from file (in Blocks not Tiles)
        /// </summary>
        public int WorldWidth { get; private set; }

        /// <summary>
        /// The height of the world loaded from file (in Blocks not Tiles)
        /// </summary>
        public int WorldHeight { get; private set; }

        /// <summary>
        /// The Amount of tiles wide a single Block is
        /// </summary>
        public int TilesPerBlockWidth { get; private set; }

        /// <summary>
        /// The Amount of tile High a single Block is.
        /// </summary>
        public int TilesPerBlockHeight { get; private set; }

        /// <summary>
        /// The amount of tiles per block (TilesWide * TilesHigh)
        /// </summary>
        public int TilesPerBlock { get; private set; }

        /// <summary>
        /// The size of a single tiles. Witdh == Height
        /// </summary>
        public int SizePerTile { get; private set; }

        /// <summary>
        /// A List of all tiles that have been loaded that are not currently in use.
        /// </summary>
        private List<ITile> TilesNotInUse
        {
            get;
            set;
        }

        /// <summary>
        /// Trys to find a Tile in the TilesNotInUse list.
        /// Creates a new Tile if there are none free exist.
        /// </summary>
        /// <returns></returns>
        public ITile GetTile(CollidableTileStructure tileStructure, Vector2 position, Rectangle rectangleSize)
        {
            var foundTile = TilesNotInUse.Find(tile => tile.TileID == tileStructure.TileID);

            if (foundTile == null)
            {
                return m_tileFactory.CreateCollidableTile(tileStructure, position, 0f, rectangleSize, 1f);
            }

            //Remove from the list so it wont' get re-used until it is free
            TilesNotInUse.Remove(foundTile);

            foundTile.Position = ConvertUnits.ToSimUnits(position);

            return foundTile;
        }

        List<ITile> m_testWorld = new List<ITile>();

        /// <summary>
        /// Saves the test world to a binary file.
        /// </summary>
        public void saveTestWorld()
        {
            using (var sw = new BinaryWriter(File.Open("Content/Levels/TestLevel.level", FileMode.Create)))
            {
                int worldWidth = 20;
                int worldHeight = 20;
                int tilesPerBlockWidth = 20;
                int tilesPerBlockHeight = 20;
                int sizeOfTile = 10;

                sw.Write(worldWidth); //Width of world in Blocks
                sw.Write(worldHeight); //Height of world in Blocks
                sw.Write(tilesPerBlockWidth); //Block Width
                sw.Write(tilesPerBlockHeight); //Block Height
                sw.Write(sizeOfTile); //The size of a tile.

                int requiredBlocks = worldWidth * worldHeight;

                var listOfBlocks = new List<List<CachedTileInformation>>();

                for (int block = 0; block < requiredBlocks; block++)
                {
                    listOfBlocks.Add(new List<CachedTileInformation>());

                    if (block > worldHeight * 2 - 1)
                    {
                        for (int tileY = 0; tileY < tilesPerBlockHeight; tileY++)
                        {
                            for (int tileX = 0; tileX < tilesPerBlockWidth; tileX++)
                            {
                                CachedTileInformation cacheTileInfo = new CachedTileInformation();
                                cacheTileInfo.TileID = 1;
                                cacheTileInfo.TilePosX = tileX;
                                cacheTileInfo.TilePosY = tileY;
                                listOfBlocks[block].Add(cacheTileInfo);
                            }
                        }
                    }
                }

                int blockID = 0;

                //Write the blocks to file
                listOfBlocks.ForEach(list =>
                {
                    sw.Write(blockID++);
                    sw.Write(list.Count); //Write number of tiles in the block

                    //Write tiles to the block
                    list.ForEach(entry =>
                    {
                        sw.Write(entry.TileID);
                        sw.Write(entry.TilePosX);
                        sw.Write(entry.TilePosY);
                    });
                });
            }
        }

        /// <summary>
        /// Updates the level
        /// </summary>
        /// <param name="blocklock"></param>
        public void Update(object blocklock)
        {
            m_renderKernel.Update();

            LoadBlock(blocklock);
        }

        /// <summary>
        /// Render all tiles that are currently streamed in.
        /// </summary>
        public void Render(bool debugInfo)
        {
            m_renderKernel.BlockCache.ForEach(tileList => tileList.ForEach(tile => tile.Draw()));

            if (debugInfo)
            {
                m_renderKernel.Render(m_tileFactory.TileFactorySpriteBatch);
            }
        }

        /// <summary>
        /// Unload all of the contents on the blocks
        /// </summary>
        private void UnloadBlocks()
        {

            m_renderKernel.BlockCache[CENTER_LEFT].Clear();
            m_renderKernel.BlockCache[CENTER_MIDDLE].Clear();
            m_renderKernel.BlockCache[CENTER_RIGHT].Clear();
            m_renderKernel.BlockCache[BOTTOM_LEFT].Clear();
            m_renderKernel.BlockCache[BOTTOM_MIDDLE].Clear();
            m_renderKernel.BlockCache[BOTTOM_RIGHT].Clear();
        }

        /// <summary>
        /// Moves the portions of the render kernal around depending on the director that the tracking body has moved
        /// </summary>
        private void BumpRenderKernel()
        {
            switch (m_renderKernel.PreviousBlock - m_renderKernel.CurrentBlock)
            {
                case -1: //Moved Right
                    BumpKernelLeft();
                    break;
                case 1: //Moved Left
                    BumpKernelRight();
                    break;
                case -20: //Moved Down
                    BumpKernelUp();
                    break;
                case 20: //Moved Up
                    BumpKernelDown();
                    break;
                //Four Other Cases Here, but they happen less often so won't cover them for now
                default:
                    m_renderKernelRequiresUpdate.ForEach(entry => entry = true);
                break;
            }
        }

        /// <summary>
        /// Moved all of the Blocks in the render kernel down.
        /// Leaving the top empty
        /// </summary>
        private void BumpKernelDown()
        {
            //Unload the Bottom
            //Add the files back to the TilesNotInUseList
            m_renderKernel.BlockCache[BOTTOM_LEFT].ForEach(tile => TilesNotInUse.Add(tile));
            m_renderKernel.BlockCache[BOTTOM_MIDDLE].ForEach(tile => TilesNotInUse.Add(tile));
            m_renderKernel.BlockCache[BOTTOM_RIGHT].ForEach(tile => TilesNotInUse.Add(tile));

            var bottomLeftTemp = m_renderKernel.BlockCache[BOTTOM_LEFT];
            var bottomMiddleTemp = m_renderKernel.BlockCache[BOTTOM_MIDDLE];
            var bottomRightTemp = m_renderKernel.BlockCache[BOTTOM_RIGHT];

            //Move the center to the bottom
            m_renderKernel.BlockCache[BOTTOM_LEFT] = m_renderKernel.BlockCache[CENTER_LEFT];
            m_renderKernel.BlockCache[BOTTOM_MIDDLE] = m_renderKernel.BlockCache[CENTER_MIDDLE];
            m_renderKernel.BlockCache[BOTTOM_RIGHT] = m_renderKernel.BlockCache[CENTER_RIGHT];

            //Move the top to the middle
            m_renderKernel.BlockCache[CENTER_LEFT] = m_renderKernel.BlockCache[TOP_LEFT];
            m_renderKernel.BlockCache[CENTER_MIDDLE] = m_renderKernel.BlockCache[TOP_MIDDLE];
            m_renderKernel.BlockCache[CENTER_RIGHT] = m_renderKernel.BlockCache[TOP_RIGHT];

            //Empty out the top for reloading
            m_renderKernel.BlockCache[TOP_LEFT] = bottomLeftTemp;
            m_renderKernel.BlockCache[TOP_MIDDLE] = bottomMiddleTemp;
            m_renderKernel.BlockCache[TOP_RIGHT] = bottomRightTemp;
            m_renderKernel.BlockCache[TOP_LEFT].Clear();
            m_renderKernel.BlockCache[TOP_MIDDLE].Clear();
            m_renderKernel.BlockCache[TOP_RIGHT].Clear();

            //Tag for updating
            m_renderKernelRequiresUpdate[TOP_LEFT] = true;
            m_renderKernelRequiresUpdate[TOP_MIDDLE] = true;
            m_renderKernelRequiresUpdate[TOP_RIGHT] = true;
        }

        /// <summary>
        /// Moved all of the Blocks in the render kernel up.
        /// Leaving the bottom empty
        /// </summary>
        private void BumpKernelUp()
        {
            //Unload the Top
            //Add the files back to the TilesNotInUseList
            m_renderKernel.BlockCache[TOP_LEFT].ForEach(tile => TilesNotInUse.Add(tile));
            m_renderKernel.BlockCache[TOP_MIDDLE].ForEach(tile => TilesNotInUse.Add(tile));
            m_renderKernel.BlockCache[TOP_RIGHT].ForEach(tile => TilesNotInUse.Add(tile));

            var topLeftTemp = m_renderKernel.BlockCache[TOP_LEFT];
            var topMiddleTemp = m_renderKernel.BlockCache[TOP_MIDDLE];
            var topRightTemp = m_renderKernel.BlockCache[TOP_RIGHT];


            //Move the center to the top
            m_renderKernel.BlockCache[TOP_LEFT] = m_renderKernel.BlockCache[CENTER_LEFT];
            m_renderKernel.BlockCache[TOP_MIDDLE] = m_renderKernel.BlockCache[CENTER_MIDDLE];
            m_renderKernel.BlockCache[TOP_RIGHT] = m_renderKernel.BlockCache[CENTER_RIGHT];

            //Move the bottom to the middle
            m_renderKernel.BlockCache[CENTER_LEFT] = m_renderKernel.BlockCache[BOTTOM_LEFT];
            m_renderKernel.BlockCache[CENTER_MIDDLE] = m_renderKernel.BlockCache[BOTTOM_MIDDLE];
            m_renderKernel.BlockCache[CENTER_RIGHT] = m_renderKernel.BlockCache[BOTTOM_RIGHT];

            //Empty out the bottom for reloading
            m_renderKernel.BlockCache[BOTTOM_LEFT] = topLeftTemp;
            m_renderKernel.BlockCache[BOTTOM_MIDDLE] = topMiddleTemp;
            m_renderKernel.BlockCache[BOTTOM_RIGHT] = topRightTemp;
            m_renderKernel.BlockCache[BOTTOM_LEFT].Clear();
            m_renderKernel.BlockCache[BOTTOM_MIDDLE].Clear();
            m_renderKernel.BlockCache[BOTTOM_RIGHT].Clear();

            m_renderKernelRequiresUpdate[BOTTOM_LEFT] = true;
            m_renderKernelRequiresUpdate[BOTTOM_MIDDLE] = true;
            m_renderKernelRequiresUpdate[BOTTOM_RIGHT] = true;
        }

        /// <summary>
        /// Moved all of the Blocks in the render kernel to the right.
        /// Leaving the far left empty
        /// </summary>
        private void BumpKernelRight()
        {
            //Unload the right
            //Add the files back to the TilesNotInUseList
            m_renderKernel.BlockCache[TOP_RIGHT].ForEach(tile => TilesNotInUse.Add(tile));
            m_renderKernel.BlockCache[CENTER_RIGHT].ForEach(tile => TilesNotInUse.Add(tile));
            m_renderKernel.BlockCache[BOTTOM_RIGHT].ForEach(tile => TilesNotInUse.Add(tile));

            var topRightTemp = m_renderKernel.BlockCache[TOP_RIGHT];
            var centreRightTemp = m_renderKernel.BlockCache[CENTER_RIGHT];
            var bottomRightTemp = m_renderKernel.BlockCache[BOTTOM_RIGHT];

            //Move the center to the right
            m_renderKernel.BlockCache[TOP_RIGHT] = m_renderKernel.BlockCache[TOP_MIDDLE];
            m_renderKernel.BlockCache[CENTER_RIGHT] = m_renderKernel.BlockCache[CENTER_MIDDLE];
            m_renderKernel.BlockCache[BOTTOM_RIGHT] = m_renderKernel.BlockCache[BOTTOM_MIDDLE];

            //Move the left to the center
            m_renderKernel.BlockCache[TOP_MIDDLE] = m_renderKernel.BlockCache[TOP_LEFT];
            m_renderKernel.BlockCache[CENTER_MIDDLE] = m_renderKernel.BlockCache[CENTER_LEFT];
            m_renderKernel.BlockCache[BOTTOM_MIDDLE] = m_renderKernel.BlockCache[BOTTOM_LEFT];

            //Null out the right to get ready for reload
            m_renderKernel.BlockCache[TOP_LEFT] = topRightTemp;
            m_renderKernel.BlockCache[CENTER_LEFT] = centreRightTemp;
            m_renderKernel.BlockCache[BOTTOM_LEFT] = bottomRightTemp;
            m_renderKernel.BlockCache[TOP_LEFT].Clear();
            m_renderKernel.BlockCache[CENTER_LEFT].Clear();
            m_renderKernel.BlockCache[BOTTOM_LEFT].Clear();

            m_renderKernelRequiresUpdate[TOP_LEFT] = true;
            m_renderKernelRequiresUpdate[CENTER_LEFT] = true;
            m_renderKernelRequiresUpdate[BOTTOM_LEFT] = true;
        }

        /// <summary>
        /// Moved all of the Blocks in the render kernel to the left.
        /// Leaving the far right empty
        /// </summary>
        private void BumpKernelLeft()
        {
            //Unload the left
            //Add the files back to the TilesNotInUseList
            m_renderKernel.BlockCache[TOP_LEFT].ForEach(tile => TilesNotInUse.Add(tile));
            m_renderKernel.BlockCache[CENTER_LEFT].ForEach(tile => TilesNotInUse.Add(tile));
            m_renderKernel.BlockCache[BOTTOM_LEFT].ForEach(tile => TilesNotInUse.Add(tile));

            //Store the left in a temp
            var topLeftTemp = m_renderKernel.BlockCache[TOP_LEFT];
            var centreLeftTemp = m_renderKernel.BlockCache[CENTER_LEFT];
            var bottomLeftTemp = m_renderKernel.BlockCache[BOTTOM_LEFT];

            //Move the center to the left
            m_renderKernel.BlockCache[TOP_LEFT]     = m_renderKernel.BlockCache[TOP_MIDDLE];
            m_renderKernel.BlockCache[CENTER_LEFT]  = m_renderKernel.BlockCache[CENTER_MIDDLE];
            m_renderKernel.BlockCache[BOTTOM_LEFT]  = m_renderKernel.BlockCache[BOTTOM_MIDDLE];

            //Move the right to the center
            m_renderKernel.BlockCache[TOP_MIDDLE]        = m_renderKernel.BlockCache[TOP_RIGHT];
            m_renderKernel.BlockCache[CENTER_MIDDLE]     = m_renderKernel.BlockCache[CENTER_RIGHT];
            m_renderKernel.BlockCache[BOTTOM_MIDDLE]     = m_renderKernel.BlockCache[BOTTOM_RIGHT];

            //Move left to the right and empty out
            m_renderKernel.BlockCache[TOP_RIGHT] = topLeftTemp;
            m_renderKernel.BlockCache[CENTER_RIGHT] = centreLeftTemp;
            m_renderKernel.BlockCache[BOTTOM_RIGHT] = bottomLeftTemp;

            m_renderKernel.BlockCache[TOP_RIGHT].Clear();
            m_renderKernel.BlockCache[CENTER_RIGHT].Clear();
            m_renderKernel.BlockCache[BOTTOM_RIGHT].Clear();

            m_renderKernelRequiresUpdate[TOP_RIGHT] = true;
            m_renderKernelRequiresUpdate[CENTER_RIGHT] = true;
            m_renderKernelRequiresUpdate[BOTTOM_RIGHT] = true;
        }

        private bool LoadBlock(object blockLock)
        {
            int blockWidth = TilesPerBlock;

            Rectangle rectangleSize = new Rectangle(0, 0, 10, 10);

            if (m_renderKernelRequiresUpdate.Any(entry => true))
            {
                lock (blockLock)
                {

                    BumpRenderKernel();

                    for (int i = 0; i < TilesPerBlockHeight; i++)
                    {
                        for (int j = 0; j < TilesPerBlockWidth; j++)
                        {
                            LoadBlockIntoRenderKernel(TOP_LEFT,     (m_renderKernel.CurrentBlock - 1) - WorldWidth, ref rectangleSize, i, j);    //Top Left
                            LoadBlockIntoRenderKernel(TOP_MIDDLE,   (m_renderKernel.CurrentBlock) - WorldWidth, ref rectangleSize, i, j);        //Top Centre
                            LoadBlockIntoRenderKernel(TOP_RIGHT,    (m_renderKernel.CurrentBlock + 1) - WorldWidth, ref rectangleSize, i, j);    //Top Right

                            LoadBlockIntoRenderKernel(CENTER_LEFT,  m_renderKernel.CurrentBlock - 1, ref rectangleSize, i, j);    //Centre Left
                            LoadBlockIntoRenderKernel(CENTER_MIDDLE,m_renderKernel.CurrentBlock, ref rectangleSize, i, j);        //Centre
                            LoadBlockIntoRenderKernel(CENTER_RIGHT, m_renderKernel.CurrentBlock + 1, ref rectangleSize, i, j);    //Centre Right
                            
                            LoadBlockIntoRenderKernel(BOTTOM_LEFT,  (m_renderKernel.CurrentBlock - 1) + WorldWidth, ref rectangleSize, i, j);    //Bottom Left
                            LoadBlockIntoRenderKernel(BOTTOM_MIDDLE,(m_renderKernel.CurrentBlock) + WorldWidth, ref rectangleSize, i, j);        //Bottom Centre
                            LoadBlockIntoRenderKernel(BOTTOM_RIGHT, (m_renderKernel.CurrentBlock + 1) + WorldWidth, ref rectangleSize, i, j);    //Bottom Right
                        }
                    }
                }

                m_renderKernelRequiresUpdate[TOP_RIGHT] = false;
                m_renderKernelRequiresUpdate[CENTER_RIGHT] = false;
                m_renderKernelRequiresUpdate[BOTTOM_RIGHT] = false;
                m_renderKernelRequiresUpdate[TOP_MIDDLE] = false;
                m_renderKernelRequiresUpdate[CENTER_MIDDLE] = false;
                m_renderKernelRequiresUpdate[BOTTOM_MIDDLE] = false;
                m_renderKernelRequiresUpdate[TOP_LEFT] = false;
                m_renderKernelRequiresUpdate[CENTER_LEFT] = false;
                m_renderKernelRequiresUpdate[BOTTOM_LEFT] = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CENTER_LEFT"></param>
        /// <param name="blockPositionLeft"></param>
        /// <param name="rectangleSize"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private void LoadBlockIntoRenderKernel(int renderKernelPosition, int blockPosition, ref Rectangle rectangleSize, int i, int j)
        {
            int blockPositionWidthModifier = (blockPosition % WorldWidth) * TilesPerBlockWidth;
            int blockPositionHeightModifier = (blockPosition / WorldHeight) * TilesPerBlockHeight;

            CachedTileInformation tileInfo = null;

            if (blockPosition > 0 && m_renderKernelRequiresUpdate[renderKernelPosition])
            {
                int searchLocation = i * WorldWidth + j;
                if (m_levelBlockCache[blockPosition].Count != 0 && m_levelBlockCache[blockPosition].Count > searchLocation)
                {
                    tileInfo = m_levelBlockCache[blockPosition][searchLocation];
                    m_renderKernel.BlockCache[renderKernelPosition].Add(
                        GetTile(
                        m_tileFactory.TileStructureCache
                        [
                            tileInfo.TileID
                        ] as CollidableTileStructure,
                        new Vector2(((blockPositionWidthModifier + tileInfo.TilePosX) * SizePerTile), (blockPositionHeightModifier + tileInfo.TilePosY) * SizePerTile),
                        rectangleSize));
                }
            }
        }

        public void RemoveTileAt(Vector2 position)
        {
            int widthLocation = (int)Math.Floor(position.X / 10f);
            int heightLocation = (int)Math.Floor(position.Y / 10f);

            //Figure out which block to remove this block from
            var currentBlock = widthLocation + (WorldWidth * (heightLocation));

            int positionX = (int)Math.Abs((Math.Round(position.X * 4) / 2)) % WorldWidth;
            int positionY = (int)Math.Abs((Math.Round(position.Y * 4) / 2)) % WorldHeight;

            try
            {
                var tileCache = m_levelBlockCache[currentBlock].Where(tile => tile.TilePosX == positionX && tile.TilePosY == positionY).Single();
                m_levelBlockCache[currentBlock].Remove(tileCache);

                int renderKernelID = m_renderKernel.ConvertBlockIdToRenderKernelID(currentBlock);

                //Need to clear the correct render kernel section .
                ITile returnTile = m_renderKernel.BlockCache[renderKernelID].First(
                    tile =>
                    {
                        return (tile.Position.X == position.X && tile.Position.Y == position.Y);
                    });

                //Re-Add it to the not in use list.
                returnTile.Position = new Vector2(-1000, -1000);
                TilesNotInUse.Add(returnTile);
                m_renderKernel.BlockCache[renderKernelID].Remove(returnTile);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Adds a tile to the LevelBlockCache and RenderKernel at the position given.
        /// </summary>
        /// <param name="position"></param>
        public void AddTileAt(Vector2 position)
        {
            int widthLocation = (int)Math.Floor(position.X / 10f);
            int heightLocation = (int)Math.Floor(position.Y / 10f);

            if (widthLocation < 0)
            {
                widthLocation = 0;
            }
            else if(widthLocation > WorldWidth)
            {
                widthLocation= WorldWidth;
            }

            if (heightLocation < 0)
            {
                heightLocation = 0;
            }
            else if (heightLocation > WorldHeight)
            {
                heightLocation = WorldHeight;
            }
            //Figure out which block to add this block to
            var currentBlock = widthLocation + (WorldWidth * (heightLocation));

            //Add it to the level block cache
            int positionX = (int)Math.Abs(((Math.Round(position.X * 4) / 2)) - widthLocation * (TilesPerBlockWidth));
            int positionY = (int)Math.Abs(((Math.Round(position.Y * 4) / 2)) - heightLocation * (TilesPerBlockHeight));

            m_levelBlockCache[currentBlock].Add(new CachedTileInformation() { TileID = 1, TilePosX = positionX, TilePosY = positionY });

            int renderKernelID = m_renderKernel.ConvertBlockIdToRenderKernelID(currentBlock);

            //Need to clear the correct render kernel section .
            m_renderKernelRequiresUpdate[renderKernelID] = true;
            m_renderKernel.BlockCache[renderKernelID].Clear();
            Rectangle rectangleSize = new Rectangle(0, 0, 10, 10);

            //Then reload that render kernel section
            for (int i = 0; i < TilesPerBlockHeight; i++)
            {
                for (int j = 0; j < TilesPerBlockWidth; j++)
                {
                    LoadBlockIntoRenderKernel(renderKernelID, currentBlock, ref rectangleSize, i, j);    //Top Left
                }
            }
            m_renderKernelRequiresUpdate[renderKernelID] = false;
        }

        /// <summary>
        /// Removes the given Tile from the block that owns it.
        /// </summary>
        /// <param name="tile">Tile to remove from a block</param>
        public void removeTileFromBlock(ITile tile)
        {
            int widthLocation = (int)Math.Floor(tile.Position.X / 10) ;
            int heightLocation = (int)Math.Floor(tile.Position.Y / 10);
            var currentBlock = widthLocation + (WorldWidth * heightLocation);

            //remove tile from the list
            var listThatContainsTile = m_renderKernel.BlockCache.First(list => list.Contains(tile));
            int positionInList = listThatContainsTile.IndexOf(tile);
            listThatContainsTile.Remove(tile);

            //Re-Add it to the not in use list.
            tile.Position = new Vector2(-1000, -1000);
            TilesNotInUse.Add(tile);

            //Remove it from the list we will load from
            m_levelBlockCache[currentBlock].RemoveAt(positionInList);
        }

        public void SaveWorldToFile()
        {
            using (var sw = new BinaryWriter(File.Open("Content/Levels/TestLevel.level", FileMode.Create)))
            {
                int worldWidth = WorldWidth;
                int worldHeight = WorldHeight;
                int tilesPerBlockWidth = TilesPerBlockHeight;
                int tilesPerBlockHeight = TilesPerBlockWidth;
                int sizeOfTile = SizePerTile;

                sw.Write(worldWidth); //Width of world in Blocks
                sw.Write(worldHeight); //Height of world in Blocks
                sw.Write(tilesPerBlockWidth); //Block Width
                sw.Write(tilesPerBlockHeight); //Block Height
                sw.Write(sizeOfTile); //The size of a tile.

                int requiredBlocks = worldWidth * worldHeight;

                int blockID = 0;

                m_levelBlockCache.ForEach(cachedTileInfoList =>
                {
                    sw.Write(blockID++);
                    sw.Write(cachedTileInfoList.Count); //Write number of tiles in the block

                    //Write tiles to the block
                    cachedTileInfoList.ForEach(entry =>
                    {
                        sw.Write(entry.TileID);
                        sw.Write(entry.TilePosX);
                        sw.Write(entry.TilePosY);
                    });
                });
            }
        }

        /// <summary>
        /// Loads the world file (the IDs of the tiles) into a collection and into their corresponding 'Blocks'
        /// </summary>
        private void LoadWorldIDsIntoCache()
        {
            using (var sr = new BinaryReader(File.Open(m_levelFile, FileMode.Open)))
            {
                //Read in the width of the world in Blocks
                WorldWidth = sr.ReadInt32();
                //Rad in the height of the world in Blocks
                WorldHeight = sr.ReadInt32();

                int totalNumberOfBlocks = WorldWidth * WorldHeight;

                //Read the width of a single Block
                TilesPerBlockWidth = sr.ReadInt32();
                //Read the height of a single Block
                TilesPerBlockHeight = sr.ReadInt32();

                //Size per tile
                SizePerTile = sr.ReadInt32();

                TilesPerBlock = TilesPerBlockWidth * TilesPerBlockHeight;     

                //Time to read in every single block
                for (int block = 0; block < totalNumberOfBlocks; block++)
                {
                    //Create a new spot in the cache for the block
                    m_levelBlockCache.Add(new List<CachedTileInformation>());

                    //Read the BlockID (Not currently used)
                    int blockID = sr.ReadInt32();

                    //Read in the number of tiles in the block
                    int numOfTileInBlock = sr.ReadInt32();

                    //Read in the 
                    for (int tile = 0; tile < numOfTileInBlock; tile++)
                    {
                        CachedTileInformation cacheTileInfo = new CachedTileInformation();
                        //Read all the info about the Tile.
                        cacheTileInfo.TileID = sr.ReadInt32();
                        cacheTileInfo.TilePosX = sr.ReadInt32();
                        cacheTileInfo.TilePosY = sr.ReadInt32();

                        m_levelBlockCache[block].Add(cacheTileInfo);
                    }
                }
            }
        }
    }
}
