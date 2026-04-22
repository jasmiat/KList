using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Test;

public enum GameState
{
    MainMenu,
    Playing
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private MainMenu _mainMenu;
    private GameState _currentState;

    private List<Enemy> _sprites;

    private int TILESIZE = 64;

    // Player
    private PlayerSprite _player;
    private PlayerInfo _playerInfo;
    private SpriteFont _font;

    // Weapon
    private Weapon _weapon;
    private List<Attack> _attacks = new();


    // Textures
    private Texture2D backgroundTexture;
    private Texture2D deathscreen;
    private Texture2D pixel;
    private Texture2D attackTex;
    private Texture2D textureAtlas;
    private Texture2D rectangleTexture;

    // Health
    private int health = 500;
    private bool dead = false;

    //Tilemaps 
    private Dictionary<Vector2, int> map;
    private Dictionary<Vector2, int> collisions;
    private List<Rectangle> textureStore;
    private List<Rectangle> intersections;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _mainMenu = new MainMenu();
        _currentState = GameState.MainMenu;
        map = LoadMap("C:\\Users\\jorda\\RiderProjects\\Test\\Test\\Content\\bin\\DesktopGL\\output_Tile Layer 1.csv");
        collisions =
            LoadMap("C:\\Users\\jorda\\RiderProjects\\Test\\Test\\Content\\bin\\DesktopGL\\output_Collisions.csv");
        intersections = new();
        //textureStore = new()
        // {
        // new Rectangle(0, 0, 16, 16),
        // new Rectangle(0, 16, 16, 16),
        //};
    }
    //TileMap Dictionary : Jordan 
    private Dictionary<Vector2, int> LoadMap(string filepath)
    {
        Dictionary<Vector2, int> result = new();
        
        StreamReader reader = new(filepath);


        int y = 0;
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] items = line.Split(',');
            for (int x = 0; x < items.Length; x++)
            {
                if (int.TryParse(items[x], out int value))
                {
                    if (value > -1)
                    {
                        result[new Vector2(x, y)] = value;
                    }
                }
            }

            y++;
        }
        return result;
    }

    protected override void Initialize()
    {
        _graphics.IsFullScreen = true;
        _graphics.PreferredBackBufferWidth = 1500;
        _graphics.PreferredBackBufferHeight = 1250;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _mainMenu.LoadContent(Content, GraphicsDevice);

        //player info part
        _font = Content.Load<SpriteFont>("MenuFont");

        Texture2D playerTexture = Content.Load<Texture2D>("SmallerPixel");
        Texture2D enemyTexture = Content.Load<Texture2D>("BadCat");
        Texture2D tankTexture = Content.Load<Texture2D>("BigBadCat");
        Texture2D weaponTexture = Content.Load<Texture2D>("weapon");
        
        rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
        rectangleTexture.SetData(new Color[] {new (255, 0, 0, 255)});

        backgroundTexture = Content.Load<Texture2D>("grass-background");
        deathscreen = Content.Load<Texture2D>("deathscreen");
        attackTex = Content.Load<Texture2D>("SwingAttack");
        
        textureAtlas = Content.Load<Texture2D>("textureAtlas");

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        // Enemies
        _sprites = new List<Enemy>


        {
            new Enemy(enemyTexture, new Vector2(400,200)),
            new Enemy(enemyTexture, new Vector2(400,300)),
            new Enemy(enemyTexture, new Vector2(400,400)),
            new TankEnemy(tankTexture, new Vector2(300,100))
        };

        _player = new PlayerSprite(playerTexture, new Vector2(200,200));

        _weapon = new Sword(weaponTexture);

        //Player info
        _playerInfo = new PlayerInfo(
            GraphicsDevice,
            _font,
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height,
            _player
        );
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        switch (_currentState)
        {
            case GameState.MainMenu:
                _mainMenu.Update(gameTime);

                if (_mainMenu.SelectedOption == MainMenu.MenuOption.Start)
                {
                    _currentState = GameState.Playing;
                }

                break;

            case GameState.Playing:
                if (!dead)
                    UpdateGameplay(gameTime);
                break;
        }
        
        //Player Collisions - Jordan
        intersections = getIntersections(_player.Rect);
        //Left Right Collisions 
        foreach (var rect in intersections)
        {
            if (collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int val))
            {
                
                Rectangle collision = new(
                    rect.X * TILESIZE,
                    rect.Y * TILESIZE,
                    TILESIZE,
                    TILESIZE
                );
                if (_player.Rect.Intersects(collision))
                {
                    if (_player.velocity.X > 0.0f)
                    {
                        _player.position.X = collision.Left - _player.Rect.Width;
                    }
                    else if (_player.velocity.X < 0.0f)
                    {
                        _player.position.X = collision.Right;
                    }
                }



            }
        }
        //Top Bottom Collisions
        foreach (var rect in intersections)
        {
            if (collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int val))
            {
                
                Rectangle collision = new(
                    rect.X * TILESIZE,
                    rect.Y * TILESIZE,
                    TILESIZE,
                    TILESIZE
                );
                if (_player.Rect.Intersects(collision))
                {
                    if (_player.velocity.Y > 0.0f)
                    {
                        _player.position.Y = collision.Top - _player.Rect.Height;
                    }
                    else if (_player.velocity.Y < 0.0f)
                    {
                        _player.position.Y = collision.Bottom;
                    }
                }



            }
        }
        
        base.Update(gameTime);
    }

    public List<Rectangle> getIntersections(Rectangle rect)
    {
        List<Rectangle> tiles = new();
        
        int left = rect.Left / TILESIZE;
        int right = (rect.Right -1) / TILESIZE;
        int top = rect.Top / TILESIZE;
        int bottom = (rect.Bottom - 1) / TILESIZE;

        for (int y = top; y <= bottom; y++)
        {
            for (int x = left; x <= right; x++)
            {
                tiles.Add(new Rectangle(x,y,1,1));
            }
        }

        return tiles;
    }

    //Jordan's part
    private void UpdateGameplay(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();
     
        


        // Weapon update
        
        _weapon.Update(gameTime);

        if (keyboard.IsKeyDown(Keys.Space) || Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            
            _attacks.Add(new Attack(attackTex, _player.position));
        }

        for (int i = _attacks.Count - 1; i >= 0; i--)
        {
            _attacks[i].Update(gameTime);
            if (!_attacks[i].IsAttacking)
            {
                _attacks.RemoveAt(i);
            }
        }

        foreach (var sprite in _sprites)
        {
            sprite.Update(gameTime, _player.position);
            

            sprite.position.X += sprite.velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            var tiles = getIntersections(sprite.Rect);

            foreach (var rect in tiles)
            {
                if (collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int val))
                {

                    Rectangle collision = new(
                        rect.X * TILESIZE,
                        rect.Y * TILESIZE,
                        TILESIZE,
                        TILESIZE
                    );
                    if (sprite.Rect.Intersects(collision))
                    {
                        if (sprite.velocity.X > 0.0f)
                        {
                            sprite.position.X = collision.Left - sprite.Rect.Width;
                        }
                        else if (sprite.velocity.X < 0.0f)
                        {
                            sprite.position.X = collision.Right;
                        }

                        sprite.velocity.X = 0;
                    }
                }
            } 
            sprite.position.Y += sprite.velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
            tiles = getIntersections(sprite.Rect);
            foreach (var rect in tiles)
            {
                if (collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int val))
                {

                    Rectangle collision = new(
                        rect.X * TILESIZE,
                        rect.Y * TILESIZE,
                        TILESIZE,
                        TILESIZE
                    );
                    if (sprite.Rect.Intersects(collision))
                    {
                        if (sprite.velocity.Y > 0.0f)
                        {
                            sprite.position.Y = collision.Top - sprite.Rect.Height;
                        }
                        else if (sprite.velocity.Y < 0.0f)
                        {
                            sprite.position.Y = collision.Bottom;
                        }
                        sprite.velocity.Y = 0;
                    }
                }
            }
            
        }

        List<Enemy> killList = new();
        //Jordan's part end
        //Alex's part
        foreach (var sprite in _sprites)
        {
            
            if (sprite.Rect.Intersects(_player.Rect))
            {
                health -= 1;

                if (health <= 0)
                {
                    health = 0;
                    dead = true;
                }
            }
        }

        foreach (var attack in _attacks)
        {
            foreach(var  sprite in _sprites)
            {
                if (sprite.Rect.Intersects(attack.Rectangle))
                {
                    killList.Add(sprite);
                }
            }
        }
        
            foreach (var sprite in killList)
                _sprites.Remove(sprite);

            _player.Update(gameTime);
        }

    public void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness)
    {
        spriteBatch.Draw(
            rectangleTexture,
            new Rectangle(
                rect.X,
                rect.Y,
                rect.Width,
                thickness
            ),
            Color.White
        );
        spriteBatch.Draw(
            rectangleTexture,
            new Rectangle(
                rect.X,
                rect.Bottom - thickness,
                rect.Width,
                thickness
            ),
            Color.White
        );
        spriteBatch.Draw(
            rectangleTexture,
            new Rectangle(
                rect.X,
                rect.Y,
                thickness,
                rect.Height
            ),
            Color.White
        );
        spriteBatch.Draw(
            rectangleTexture,
            new Rectangle(
                rect.Right - thickness,
                rect.Y,
                thickness,
                rect.Height
            ),
            Color.White
        );
    }
    

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gainsboro);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        switch (_currentState)
        {
            case GameState.MainMenu:
                _mainMenu.Draw(_spriteBatch);
                break;

            case GameState.Playing:
                int display_tilsize = 64;
                int num_tiles_per_row = 15;
                int pxl_tile = 8;
                foreach (var item in map)
                {
                    Rectangle dest = new(
                        (int)item.Key.X * display_tilsize,
                        (int)item.Key.Y * display_tilsize,
                        display_tilsize,
                        display_tilsize
                    );
                    
                    int x = item.Value % num_tiles_per_row;
                    int y = item.Value / num_tiles_per_row;

                    Rectangle source = new(
                        x * pxl_tile,
                        y * pxl_tile,
                        pxl_tile,
                        pxl_tile
                        );

                    _spriteBatch.Draw(textureAtlas, dest, source, Color.White);
                }
                foreach (var item in collisions)
                {
                    Rectangle dest = new(
                        (int)item.Key.X * display_tilsize,
                        (int)item.Key.Y * display_tilsize,
                        display_tilsize,
                        display_tilsize
                    );
                    
                    int x = item.Value % num_tiles_per_row;
                    int y = item.Value % num_tiles_per_row;

                    Rectangle source = new(
                        x * pxl_tile,
                        y * pxl_tile,
                        pxl_tile,
                        pxl_tile
                    );

                    _spriteBatch.Draw(textureAtlas, dest, source, Color.White);
                }

                // Background draw
               // _spriteBatch.Draw(backgroundTexture, GraphicsDevice.Viewport.Bounds, Color.White);
                foreach (var sprite in _sprites)
                    sprite.Draw(gameTime, _spriteBatch);

           
                
                // Healthbar draw
                _spriteBatch.Draw(pixel, new Rectangle(45, 45, 510, 105), Color.Silver);
                _spriteBatch.Draw(pixel, new Rectangle(50, 50, 500, 95), Color.Black);
                _spriteBatch.Draw(pixel, new Rectangle(50, 50, health, 95), Color.Red);

                // Player draw
                foreach (var rect in intersections)
                {
                    
                }
                _player.Draw(_spriteBatch);

                // // Weapon draw

                foreach (var attack in _attacks)
                {
                    attack.Draw(_spriteBatch);
                }
                
               
                _weapon.Draw(_spriteBatch, _player.position, _player.FacingDirection);
                
                
                // Player info draw
                _playerInfo.Draw(_spriteBatch);

                break;
        }
        
        //Rectangle draw

        foreach (var rect in intersections)
        {
            DrawRectHollow(_spriteBatch,  new Rectangle(
                rect.X * TILESIZE,
                rect.Y * TILESIZE,
                TILESIZE,
                TILESIZE
            ), 2);
        }
        //DrawRectHollow(_spriteBatch, _player.Rect, 4);

        _spriteBatch.End();

        // Death
        if (dead)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(deathscreen, GraphicsDevice.Viewport.Bounds, Color.White);
            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }
}
