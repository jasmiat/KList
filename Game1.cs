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
    private GameState _currentState;
    private List<Sprite> _sprites;
    private PlayerSprite _player;
    
    Texture2D backgroundTexture;
    private Texture2D deathscreen;

    private Texture2D death;
    Texture2D pixel;
    
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
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();

        base.Initialize();
    }
    

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _mainMenu.LoadContent(Content, GraphicsDevice);


        Texture2D playerTexture = Content.Load<Texture2D>("Stick-Man-Transparent");
        Texture2D enemyTexture = Content.Load<Texture2D>("Stick-Man-Transparent");
        backgroundTexture = Content.Load<Texture2D>("grass-background");
        deathscreen = Content.Load<Texture2D>("death-screen");
        
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        _sprites = new List<Sprite>
        {
            new Sprite(enemyTexture, new Vector2(681, 450)),
            new Sprite(enemyTexture, new Vector2(883, 250)),
            new Sprite(enemyTexture, new Vector2(1031, 600)),
            new Sprite(enemyTexture, new Vector2(1200, 500)),
            new Sprite(enemyTexture, new Vector2(1400, 800)),
        };

        _player = new PlayerSprite(playerTexture, Vector2.Zero);
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
                UpdateGameplay(gameTime);
                break;
        }

        base.Update(gameTime);
    }

    private void UpdateGameplay(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();
        Vector2 move = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.Left)) move.X -= 5;
        if (keyboard.IsKeyDown(Keys.Right)) move.X += 5;
        if (keyboard.IsKeyDown(Keys.Up)) move.Y -= 5;
        if (keyboard.IsKeyDown(Keys.Down)) move.Y += 5;

        _player.position += move;

        List<Sprite> killList = new();
        foreach (var sprite in _sprites)
        {
            sprite.Update(gameTime);
            if (sprite.Rect.Intersects(_player.Rect))
            {
                health -= 100;
                killList.Add(sprite);
                if (health == 0)
                {
                    dead = true;
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
        
        _spriteBatch.Begin();

        switch (_currentState)
        {
            case GameState.MainMenu:
                _mainMenu.Draw(_spriteBatch);
                break;
            case GameState.Playing:
                // Background Drawn
                _spriteBatch.Draw(backgroundTexture, GraphicsDevice.Viewport.Bounds, Color.White);
                
                // Healthbar drawn
                _spriteBatch.Draw(pixel, new Rectangle(45, 45, 510, 105), Color.Silver);
                _spriteBatch.Draw(pixel, new Rectangle(50, 50, 500, 95), Color.Black);
                _spriteBatch.Draw(pixel, new Rectangle(50, 50, health, 95), Color.Red);
                
                // Draw Starting Sprites
                foreach (var sprite in _sprites)
                    sprite.Draw(_spriteBatch);

                _player.Draw(_spriteBatch);
                break;
        }
        _spriteBatch.End();

        if (dead)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(deathscreen, GraphicsDevice.Viewport.Bounds, Color.White);
            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }
}