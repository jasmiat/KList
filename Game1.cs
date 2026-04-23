using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
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

    private List<Enemy> _sprites;

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

        Texture2D enemyTexture = Content.Load<Texture2D>("BadCat");
        Texture2D tankTexture = Content.Load<Texture2D>("BigBadCat");
        Texture2D weaponTexture = Content.Load<Texture2D>("weapon");

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
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
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

        base.Update(gameTime);
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
            
            // KC modify- adding carrot attack
            var carrot = _weapon as Carrot;
            if (carrot != null)
            {
                foreach (var rect in carrot.Projectiles()) // KC Note: carrot attack as a rect too (ADD ON LAST MIN)
                {
                    if (sprite.Rect.Intersects(rect))
                    {
                        killList.Add(sprite);
                        break;
                    }
                }
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
            _sprites.Remove(sprite);

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

                // Background draw
                _spriteBatch.Draw(backgroundTexture, GraphicsDevice.Viewport.Bounds, Color.White);
                foreach (var sprite in _sprites)
                    sprite.Draw(gameTime, _spriteBatch);
                
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
