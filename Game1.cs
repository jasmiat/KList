using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.IO;
using System;

namespace KListDemo1;

public enum GameState // KC
{
    MainMenu,
    Playing
}
public enum WeaponType // Jazz's
{
    Sword,
    Carrot
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Random _random = new Random();
    private MainMenu _mainMenu;
    private GameState _currentState;

    private List<Enemy> _enemies;

    private int TILESIZE = 64;

    // Player
    private PlayerSprite _player;
    private PlayerInfo _playerInfo;
    private SpriteFont _font;

    // Weapons
    private Weapon _weapon;
    private Sword _sword;
    private Carrot _carrot;
    private WeaponType _currentWeapon;

    // Textures
    private Texture2D backgroundTexture;
    private Texture2D deathscreen;
    private Texture2D pixel;
    
    private Texture2D _carrotTexture;
    private Texture2D _swordTexture;

    private Texture2D textureAtlas;
    private Texture2D rectangleTexture;
    
    // Tilemaps
    private Dictionary<Vector2, int> map;
    private Dictionary<Vector2, int> collisions;
    private List<Rectangle> textureStore;
    private List<Rectangle> intersections;
    private List<Rectangle> enemyIntersections;

    // Health
    private int health = 500;
    private bool dead = false;
    
    private KeyboardState _previousKeyboardState;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _mainMenu = new MainMenu();
        _mainMenu.SetGraphicsManager(_graphics);
        _currentState = GameState.MainMenu;
    }

    protected override void Initialize()
    {
        _graphics.IsFullScreen = true;
        _graphics.PreferredBackBufferWidth = 1500;
        _graphics.PreferredBackBufferHeight = 1250;
        _graphics.ApplyChanges();
        
        map = LoadMap(Path.Combine(Content.RootDirectory, "/Users/jazzy/RiderProjects/KListDemo1/Content/bin/DesktopGL/map.csv"));
        collisions = LoadMap(Path.Combine(Content.RootDirectory, "/Users/jazzy/RiderProjects/KListDemo1/Content/bin/DesktopGL/output_Collisions.csv"));
        
        intersections = new();

        base.Initialize();
    }
    
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

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _mainMenu.LoadContent(Content, GraphicsDevice);

        //player info part
        _font = Content.Load<SpriteFont>("MenuFont");

        Texture2D enemyTexture = Content.Load<Texture2D>("BadCat");
        Texture2D tankTexture = Content.Load<Texture2D>("BadCat");
        Texture2D weaponTexture = Content.Load<Texture2D>("weapon");

        backgroundTexture = Content.Load<Texture2D>("Second BackgroundKList");
        deathscreen = Content.Load<Texture2D>("deathscreen");
        
        textureAtlas = Content.Load<Texture2D>("textureAtlas");
        
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        // Enemies
        _enemies = new List<Enemy>
        
        {
            new Enemy(enemyTexture, new Vector2(400, 200)),
            new Enemy(enemyTexture, new Vector2(400, 300)),
            new Enemy(enemyTexture, new Vector2(400, 400)),
            new TankEnemy(tankTexture, new Vector2(300, 100))
        };
        
        // Kandace's animation
        _player = new PlayerSprite(Content, "BunnyChar-4", new Vector2(96, 96), 3, 8);

        _carrotTexture = Content.Load<Texture2D>("CarrotAttack-2"); // KC fix, needs to be the same
        _swordTexture = Content.Load<Texture2D>("weapon"); // KC fix, needs to be the same
        
        // KC modify - need current weapon state
        _sword = new Sword(_swordTexture);
        _carrot = new Carrot(_carrotTexture);

        _weapon = _sword;
        _currentWeapon = WeaponType.Sword;

        //Player info
        _playerInfo = new PlayerInfo(
            GraphicsDevice,
            _font,
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height,
            _player,
            _swordTexture,
            _carrotTexture
        );
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.Escape) && _currentState == GameState.Playing)
            Exit();

        // ALEX
        if (Keyboard.GetState().IsKeyDown(Keys.Q) && dead) // Alex's Part- restart w/ less health
        {
            _mainMenu.Update(gameTime);
            _currentState = GameState.MainMenu;
            dead = false;
            health = 350;
            UpdateGameplay(gameTime);
        }

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
        
        intersections = GetIntersections(_player.Rect);
        
        //enemyIntersections = getIntersections(Enemy.Rect);
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

                if (_player.velocity.X > 0.0f)
                {
                    _player.position.X = collision.Left - _player.Rect.Width;
                }else if (_player.velocity.X < 0.0f)
                {
                    _player.position.X = collision.Right;
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

                if (_player.velocity.Y > 0.0f)
                {
                    _player.position.Y = collision.Top - _player.Rect.Width;
                }else if (_player.velocity.Y < 0.0f)
                {
                    _player.position.Y = collision.Bottom;
                }
            }
        }

        base.Update(gameTime);
    }
    
    // Jordan - collisions
    public List<Rectangle> GetIntersections(Rectangle rect)
    {
        List<Rectangle> tiles = new();

        int left = rect.Left / TILESIZE;
        int right = (rect.Right -1) / TILESIZE;
        int top = rect.Top / TILESIZE;
        int bottom = (rect.Bottom - 1) / TILESIZE;

        for (int y = top; y < bottom; y++)
        {
            for (int x = left; x < right; x++)
            {
                tiles.Add(new Rectangle(x,y,1,1));
            }
        }

        return tiles;
    }
    
    //Jordan's part - KC modified
    private void UpdateGameplay(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();
        Rectangle? hitbox =  null;
        
        // KC and Jazz add on - switch space + 1 for sword, space + 2 for carrot
        if (keyboard.IsKeyDown(Keys.D1) && !_previousKeyboardState.IsKeyDown(Keys.D1))
        {
            _weapon = _sword;
            _currentWeapon = WeaponType.Sword;
        }

        if (keyboard.IsKeyDown(Keys.D2) && !_previousKeyboardState.IsKeyDown(Keys.D2))
        {
            _weapon = _carrot;
            _currentWeapon = WeaponType.Carrot;
        }
        
        // Updating weapons
        _weapon.Update(gameTime);

        if (keyboard.IsKeyDown(Keys.Space))
        {
            if (_weapon is Sword sword)
                sword.StartAttacking();

            hitbox = _weapon.Attack(_player.position, _player.FacingDirection);
        }
        
                foreach (var sprite in _enemies)
        {
            sprite.Update(gameTime, _player.position);
            

            sprite.position.X += sprite.velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            var tiles = GetIntersections(sprite.Rect);

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
            tiles = GetIntersections(sprite.Rect);
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
        
        //Alex's part
        foreach (var sprite in _enemies)
        {
            sprite.Update(gameTime, _player.position);

            // Weapon kills enemy
            if (hitbox.HasValue && sprite.Rect.Intersects(hitbox.Value))
            {
                sprite.TakeDamage(1); //****NEW**** - Jasmine - fixed instant death
                continue;
            }
            
            // KC modify- adding carrot attack
            var carrot = _weapon as Carrot;
            if (carrot != null)
            {
                foreach (var rect in carrot.Projectiles()) // KC Note: carrot attack as a rect too (ADD ON LAST MIN)
                {
                    if (sprite.Rect.Intersects(rect))
                    {
                        // killList.Add(sprite);
                        sprite.TakeDamage(2); //*****NEW***** - Jasmine - carrot damage 2
                        break;
                    }
                }
            }
            //*****NEW***** - Jasmine - enemy disappears after death
            if (sprite.IsDead())
            {
                killList.Add(sprite);
            }
            
            // Enemy damages player
            if (sprite.Rect.Intersects(_player.Rect))
            {
                // ALEX'S TELEPORT
                health -= 50; // decrease w each collision w enemy
                
                // Determine borders to teleport (may be changed when edges are set)
                int maxX = GraphicsDevice.Viewport.Width + 550;
                int maxY = GraphicsDevice.Viewport.Height - 550;
                
                sprite.position = new Vector2(_random.Next(0, maxX), _random.Next(0, maxY));

                if (health <= 0)
                {
                    health = 0;
                    dead = true;
                }
            }
        }
        //Alex's part end
        
        foreach (var sprite in killList)
            _enemies.Remove(sprite);

        _player.Update(gameTime);
        
        _previousKeyboardState = keyboard;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Pink);

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
                foreach (var sprite in _enemies)
                    sprite.Draw(gameTime, _spriteBatch, pixel);
                
                // KC- took out temp health bar
                // // Healthbar draw
                // _spriteBatch.Draw(pixel, new Rectangle(45, 45, 510, 105), Color.Silver);
                // _spriteBatch.Draw(pixel, new Rectangle(50, 50, 500, 95), Color.Black);
                // _spriteBatch.Draw(pixel, new Rectangle(50, 50, health, 95), Color.Red);

                // Player draw
                _player.Draw(_spriteBatch);

                // Weapon draw
                KeyboardState keyboard = Keyboard.GetState();
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    _weapon.Draw(_spriteBatch, _player.position, _player.FacingDirection);
                }
                
                // Player info draw
                _playerInfo.Draw(_spriteBatch,health,500,_currentWeapon);

                break;
        }

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
