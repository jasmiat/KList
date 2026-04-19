using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace KListDemo1;

public enum GameState
{
    MainMenu,
    Settings,
    Playing
}
public enum WeaponType
{
    Sword,
    Bow // *****NEW******* i added this - jasmine
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private MainMenu _mainMenu; // main menu - KC
    private Settings _settings; // settings - KC
    private GameState _currentState;

    private List<Enemy> _sprites;

    // Player
    private PlayerSprite _player;
    private PlayerInfo _playerInfo;
    private SpriteFont _font;

    //  *****NEW******* Weapon
    private Weapon _weapon;
    private Weapon _sword;
    private Weapon _bow;
    
    // *****NEW*******  Jasmine - Weapon toggle
    private WeaponType currentWeapon = WeaponType.Sword;
    private KeyboardState previousKeyboardState;

    //  *****NEW******* Textures
    private Texture2D backgroundTexture;
    private Texture2D deathscreen;
    private Texture2D pixel;
    private Texture2D swordTexture;
    private Texture2D bowTexture;
    

    // Health
    private int health = 500;
    private bool dead = false;
    
    // *****NEW******* Jasmine
    private KeyboardState _previousKeyboardState;
    

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _mainMenu = new MainMenu();
        _settings = new Settings();
        _currentState = GameState.MainMenu;
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
        Texture2D swordTexture = Content.Load<Texture2D>("weapon");
        Texture2D bowTexture = Content.Load<Texture2D>("bow");

        backgroundTexture = Content.Load<Texture2D>("background");
        deathscreen = Content.Load<Texture2D>("deathscreen");

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

        // *****NEW******* Weapon
        _player = new PlayerSprite(playerTexture, Vector2.Zero);
        _sword = new Sword(swordTexture);
        _bow = new Bow(bowTexture);
        _weapon = _sword; 
        //ermmm i might've took out a thing or two here - jasmine
        
        // *****NEW******* Player info
        _playerInfo = new PlayerInfo(
            GraphicsDevice,
            _font,
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height,
            _player,
            swordTexture,
            bowTexture
        );
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        switch (_currentState) // Kandace
        {
            case GameState.MainMenu:
                _mainMenu.Update(gameTime);

                if (_mainMenu.Confirmed)
                {
                    if (_mainMenu.SelectedOption == MainMenu.MenuOption.Start)
                        _currentState = GameState.Playing;
                    else if (_mainMenu.SelectedOption == MainMenu.MenuOption.Settings)
                        _currentState = GameState.Settings;
                }
                break;

            case GameState.Settings:
                KeyboardState keyboard = Keyboard.GetState();
                _settings.Update(keyboard, _previousKeyboardState);

                if (_settings.Change)
                {
                    _settings.Apply(_graphics);
                    _settings.Change = false;
                }
                
                _previousKeyboardState = keyboard;
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
        Rectangle? hitbox = null;

        
        // Weapon update
        _weapon.Update(gameTime);

        if (keyboard.IsKeyDown(Keys.Space))
        {
            hitbox = _weapon.Attack(_player.position, _player.FacingDirection);
        }
        
        //  *****NEW******* Weapon Toggle -Jasmine
        if (keyboard.IsKeyDown(Keys.D1) && previousKeyboardState.IsKeyUp(Keys.D1))
        {
            currentWeapon = WeaponType.Sword;
            _weapon = _sword;
        }

        if (keyboard.IsKeyDown(Keys.D2) && previousKeyboardState.IsKeyUp(Keys.D2))
        {
            currentWeapon = WeaponType.Bow;
            _weapon = _bow;
        }

        previousKeyboardState = keyboard;
        // Weapon Toggle End

        List<Enemy> killList = new();
        //Alex's part
        foreach (var sprite in _sprites)
        {
            sprite.Update(gameTime, _player.position);

            // Weapon kills enemy
            if (hitbox.HasValue && sprite.Rect.Intersects(hitbox.Value))
            {
                killList.Add(sprite);
                continue;
            }

            // Enemy damages player
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
        //Alex's part end
        
        foreach (var sprite in killList)
            _sprites.Remove(sprite);

        _player.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gainsboro);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        switch (_currentState) // Kandace's
        {
            case GameState.MainMenu:
                _mainMenu.Draw(_spriteBatch);
                break;
            
            case GameState.Settings:
                _settings.Draw(_spriteBatch, _font);
                break;

            case GameState.Playing:

                // Background draw
                _spriteBatch.Draw(backgroundTexture, GraphicsDevice.Viewport.Bounds, Color.White);
                foreach (var sprite in _sprites)
                    sprite.Draw(gameTime, _spriteBatch);
              
                // Player draw
                _player.Draw(_spriteBatch);

                // // Weapon draw
                // if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                
                KeyboardState keyboard = Keyboard.GetState();
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    _weapon.Draw(_spriteBatch, _player.position, _player.FacingDirection);
                }
                
                //  *****NEW******* Player info draw - Jasmine
                _playerInfo.Draw(_spriteBatch, health, 500, currentWeapon);
                
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
