using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
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

    private string CSVFilePath = "C:\\Users\\jorda\\RiderProjects\\Test\\Test\\Content\\bin\\DesktopGL\\map.csv";

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

    // Health
    private int health = 500;
    private bool dead = false;

    private Dictionary<Vector2, int> tilemap;
    private List<Rectangle> textureStore;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _mainMenu = new MainMenu();
        _currentState = GameState.MainMenu;
        tilemap = LoadMap(CSVFilePath);
        textureStore = new()
        {
            new Rectangle(0, 0, 16, 16),
            new Rectangle(0, 16, 16, 16),
        };
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
                    if (value > 0)
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

        Texture2D playerTexture = Content.Load<Texture2D>("Bunny1");
        Texture2D enemyTexture = Content.Load<Texture2D>("BadCat");
        Texture2D tankTexture = Content.Load<Texture2D>("BigBadCat");
        Texture2D weaponTexture = Content.Load<Texture2D>("weapon");

        backgroundTexture = Content.Load<Texture2D>("grass-background");
        deathscreen = Content.Load<Texture2D>("deathscreen");
        attackTex = Content.Load<Texture2D>("SwingAttack");
        
        textureAtlas = Content.Load<Texture2D>("textureAtlas");

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        // Enemies
        _sprites = new List<Enemy>


        {
            new Enemy(enemyTexture, new Vector2(600, 600)),
            new Enemy(enemyTexture, new Vector2(900, 900)),
            new Enemy(enemyTexture, new Vector2(1200, 1200)),
            new TankEnemy(tankTexture, new Vector2(750, 750))
        };

        _player = new PlayerSprite(playerTexture, Vector2.Zero);

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

        base.Update(gameTime);
    }

    //Jordan's part
    private void UpdateGameplay(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();
        Vector2 move = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.Left)) move.X -= 5;
        if (keyboard.IsKeyDown(Keys.Right)) move.X += 5;
        if (keyboard.IsKeyDown(Keys.Up)) move.Y -= 5;
        if (keyboard.IsKeyDown(Keys.Down)) move.Y += 5;

        _player.position += move;
        


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
        }

        List<Enemy> killList = new();
        //Jordan's part end
        //Alex's part
        foreach (var sprite in _sprites)
        {
            sprite.Update(gameTime, _player.position);
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
                
                foreach (var item in tilemap)
                {
                    Rectangle dest = new(
                        (int)item.Key.X * 64,
                        (int)item.Key.Y * 64,
                        64,
                        64
                    );

                    Rectangle source = textureStore[item.Value - 1];

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
