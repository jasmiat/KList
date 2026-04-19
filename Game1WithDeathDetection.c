using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace SpriteTest;

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
    public GameState _currentState;

    private List<Enemy> _sprites;

    // Player
    private PlayerSprite _player;
    private PlayerInfo _playerInfo;
    private SpriteFont _font;

    // Weapon
    private Weapon _weapon;

    // Textures
    private Texture2D backgroundTexture;
    private Texture2D deathscreen;
    private Texture2D pixel;

    // Health
    private int health = 500;
    private bool dead = false;

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
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
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

        backgroundTexture = Content.Load<Texture2D>("background");
        deathscreen = Content.Load<Texture2D>("death-screen");
        
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
        // Alex part
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        // If click "retry", reset gamestate and stats.
        if (Keyboard.GetState().IsKeyDown(Keys.Q) && dead)
        {
          // Basic stats that can be adjusted to pre death.
            _mainMenu.Update(gameTime);
            _currentState = GameState.MainMenu;
            dead = false;
            health = 500;
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
                if (!dead && Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    
                }
                break;
        }

        base.Update(gameTime);
    }
    // Jordan's part
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

        List<Enemy> killList = new();
        //Jordan's part end
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
                
                // Healthbar draw
                _spriteBatch.Draw(pixel, new Rectangle(45, 45, 510, 105), Color.Silver);
                _spriteBatch.Draw(pixel, new Rectangle(50, 50, 500, 95), Color.Black);
                _spriteBatch.Draw(pixel, new Rectangle(50, 50, health, 95), Color.Red);

                // Player draw
                _player.Draw(_spriteBatch);

                // // Weapon draw
                // if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                
                KeyboardState keyboard = Keyboard.GetState();
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    _weapon.Draw(_spriteBatch, _player.position, _player.FacingDirection);
                }
                
                // Player info draw
                _playerInfo.Draw(_spriteBatch);

                break;
        }

        _spriteBatch.End();

        // Death
        // Alex part - If dead, draw the death screen for the player.
        if (dead)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(deathscreen, GraphicsDevice.Viewport.Bounds, Color.White);
            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }
}
