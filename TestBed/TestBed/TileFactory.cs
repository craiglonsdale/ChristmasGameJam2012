using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestBed.Interface;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TestBed.Tiles;
using System.IO;
using FarseerPhysics.Factories;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace TestBed
{
    public interface ITileStructure
    {
        int TileID { get; set;}
        String TileType { get; set;}
        String Texture { get; set;}
    }

    public class TileStructure : ITileStructure
    {
        public int TileID { get; set; }
        public String TileType { get; set; }
        public String Texture { get; set; }
    }

    public class CollidableTileStructure : ITileStructure
    {
        public int TileID { get; set; }
        public String TileType { get; set; }
        public BodyType CollidableType { get; set; }
        public String Texture { get; set; }
        public int Density { get; set; }
    }

    public class TileFactory
    {
        /// <summary>
        /// The 'World' object representing from the Physics Engine
        /// </summary>
        private World m_physicsWorld = null;

        /// <summary>
        /// Spriet Batch for the game, used to load textures from file.
        /// </summary>
        private SpriteBatch m_spriteBatch = null;

        /// <summary>
        /// Refers to the current game.
        /// </summary>
        private Game m_game = null;

        /// <summary>
        /// Constructor for TileFactory.
        /// </summary>
        public TileFactory()
        {
            //Cache to store the Created Tiles in. If they exist 
            Tiles = new List<ITile>();
            CollidableTiles = new List<ITile>();
            NonCollidableTiles = new List<ITile>();

            TextureCache = new Dictionary<String, Texture2D>();
            TileStructureCache = new ConcurrentDictionary<int, ITileStructure>();
            PhysicsBodyCache = new List<Body>();
        }

        /// <summary>
        /// Initializes the Tile Factory.
        /// </summary>
        /// <param name="spriteBatch">Sprite Batch for the current game.</param>
        /// <param name="physicsWorld">World object from the Physics Engine for the current game</param>
        /// <param name="game">Game object...for the current game</param>
        public void Initialize(SpriteBatch spriteBatch, World physicsWorld, Game game)
        {
            if (m_spriteBatch == null)
            {
                m_spriteBatch = spriteBatch;
            }

            if (m_physicsWorld == null)
            {
                m_physicsWorld = physicsWorld;
            }

            if (m_game == null)
            {
                m_game = game;
            }
        }

        public SpriteBatch TileFactorySpriteBatch
        {
            get
            {
                return m_spriteBatch;
            }
            private set
            {
                m_spriteBatch = value;
            }
        }

        /// <summary>
        /// A Cache of all TileStructures that have been loaded from file.
        /// </summary>
        public ConcurrentDictionary<int, ITileStructure> TileStructureCache
        {
            get;
            set;
        }

        /// <summary>
        /// A Cache of all textures that have been loaded from file.
        /// </summary>
        private Dictionary<String, Texture2D> TextureCache
        {
            get;
            set;
        }

        /// <summary>
        /// A Cache of all Physics Bodies that have been created by the Physics Engine and aren't currently in use.
        /// </summary>
        private List<Body> PhysicsBodyCache
        {
            get;
            set;
        }

        /// <summary>
        /// Publicly accessible list of all the Tiles in the TileFactory.
        /// </summary>
        public List<ITile> Tiles
        {
            get;
            private set;
        }

        /// <summary>
        /// List of all NonCollidable tiles.
        /// All Tiles in this list will be available publicly through the Tiles list
        /// </summary>
        private List<ITile> NonCollidableTiles
        {
            get;
            set;
        }

        /// <summary>
        /// List of all Collidable tiles.
        /// All Tiles in this list will be available publicly through the Tiles list
        /// </summary>
        private List<ITile> CollidableTiles
        {
            get;
            set;
        }

        /// <summary>
        /// Loads a Tiles Structure from the given folder into the TileStructureCache
        /// </summary>
        /// <param name="tileFile">The folder that holds the tile scripts.</param>
        public void LoadTileStructure(String folder)
        {
            Parallel.ForEach(Directory.GetFiles(folder).ToList(),
                (fileName) =>
                {
                    var fileContents = File.ReadAllLines(fileName);

                    var typeDictionary = fileContents.Select(l => l.Split(':')).ToDictionary(e => e[0], e => e[1]);

                    LoadTileStructure(typeDictionary);
                });

        }

        /// <summary>
        /// Loads the dictionary containing the data into a TileStructureCache, so once we have a file loaded we can access
        /// the via TileID instead of Re-Reading the files.
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns>True is successfully added, or already exists in cache</returns>
        private bool LoadTileStructure(Dictionary<String, String> fileData)
        {
            int fileID = Int16.Parse(fileData["TileID"]);

            if (!TileStructureCache.Keys.Contains(fileID))
            {
                switch (fileData["TileType"])
                {
                    case "Tile":
                        TileStructureCache.TryAdd(fileID, new TileStructure()
                        {
                            TileID = fileID,
                            TileType = "Earth",
                            Texture = fileData["Texture"]
                        });
                        break;
                    case "Collidable":
                        TileStructureCache.TryAdd(fileID, new CollidableTileStructure()
                        {
                            TileID = fileID,
                            TileType = "Collidable",
                            CollidableType = BodyType.Static,
                            Density = Int16.Parse(fileData["Density"]),
                            Texture = fileData["Texture"]
                        });
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds a tile that has the given Unique ID
        /// </summary>
        /// <param name="uniqueID">The GUID that needs to be found.</param>
        /// <returns>ITile - Null if none found</returns>
        public ITile FindTileWithID(Guid uniqueID)
        {
            ITile foundTile = null;
            try
            {
                foundTile = CollidableTiles.First(tile =>
                {
                    return tile.UniqueID == uniqueID;
                });
            }
            catch (Exception e)
            {
                //Oh noes that doesn't exist...
                //Swallow the exception BITCH!
            }

            return foundTile;
        }

        /// <summary>
        /// Creates a basic non-collidable tile.
        /// </summary>
        /// <param name="texture">Texture for the tile.</param>
        /// <param name="position">Position of the tile.</param>
        /// <param name="tileSize">Size of the file.</param>
        /// <param name="rotation">Rotation of the tile.</param>
        /// <returns>The created non collidable tile</returns>
        public ITile CreateNonCollidableTile(Texture2D texture, Vector2 position, Rectangle tileSize, float rotation)
        {
            var tile = new Tiles.Tile(m_spriteBatch, texture, position, tileSize, rotation);
            NonCollidableTiles.Add(tile);
            Tiles.Add(tile);
            
            return tile;
        }

        /// <summary>
        /// Creates a basic non-collidable tile.
        /// </summary>
        /// <param name="tile">Structure of the tile.</param>
        /// <param name="position">Position of the tile.</param>
        /// <param name="rotation">Rotation of the tile.</param>
        /// <param name="tileSize">Size of the file.</param>
        /// <returns>The created non collidable tile</returns>
        public ITile CreateNonCollidableTile(ITileStructure tile, Vector2 position, float rotation, Rectangle tileSize)
        {
            var frontTexture = LoadTexture(tile.Texture);

            var returnTile = CreateNonCollidableTile(frontTexture, position, tileSize, rotation);
            returnTile.TileID = tile.TileID;

            return returnTile;
        }

        /// <summary>
        /// Creates a Collidable tile, Dynamic or Static.
        /// </summary>
        /// <param name="bodyType"></param>
        /// <param name="texture">Texture for the tile.</param>
        /// <param name="position">Position of the tile.</param>
        /// <param name="tileSize">Size of the file.</param>
        /// <param name="rotation">Rotation of the tile.</param>
        /// <param name="density">Density of it?</param>
        /// <returns>ICollidableTile</returns>
        public ICollidableTile CreateCollidableTile(BodyType bodyType, Texture2D texture, Vector2 position, Rectangle tileSize, float rotation, float density)
        {
            ICollidableTile collidableTile;
            Body physicsBody = null;

            if (PhysicsBodyCache.Count == 0)
            {
                //If our cache is empty make a brand spankin' new one
                physicsBody = BodyFactory.CreateRectangle(m_physicsWorld,
                                ConvertUnits.ToSimUnits(tileSize.Width),
                                ConvertUnits.ToSimUnits(tileSize.Height),
                                density);
            }
            else
            {
                //If we have some phyasics bodies in our cache just grab the first one, they are all pretty much the same.
                physicsBody = PhysicsBodyCache[0];

                //and remove it so we don't try to use it again
                PhysicsBodyCache.Remove(physicsBody);
            }

            collidableTile = new Tiles.StaticCollidableTile(m_spriteBatch, texture, position, physicsBody, tileSize.Height, tileSize.Width, rotation, m_physicsWorld);
            CollidableTiles.Add(collidableTile);
            Tiles.Add(collidableTile);

            return collidableTile;
        }

        /// <summary>
        /// Creates a basic collidable tile.
        /// </summary>
        /// <param name="tile">Structure of the tile.</param>
        /// <param name="position">Position of the tile.</param>
        /// <param name="rotation">Rotation of the tile.</param>
        /// <param name="tileSize">Size of the file.</param>
        /// <param name="density">Density of the tile.</param>
        /// <returns>The created collidable tile</returns>
        public ITile CreateCollidableTile(CollidableTileStructure tile, Vector2 position, float rotation, Rectangle tileSize, float density)
        {
            ITile returnTile;
 
            var frontTexture = LoadTexture(tile.Texture);

            returnTile = CreateCollidableTile(tile.CollidableType, frontTexture, position, tileSize, rotation, tile.Density);
            returnTile.TileID = tile.TileID;

            return returnTile;
        }

        /// <summary>
        /// Loads a texture from file.
        /// If Texture has been loaded before it will pull it from a Texture Cache
        /// </summary>
        /// <param name="textureFile">Path to the file.</param>
        /// <returns>Texture from a TextureCache</returns>
        private Texture2D LoadTexture(String textureFile)
        {
            if (!TextureCache.Keys.Contains(textureFile))
            {
                TextureCache.Add(textureFile, m_game.Content.Load<Texture2D>(textureFile));
            }

            return TextureCache[textureFile];
        }


    }
}
