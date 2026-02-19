using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace KListDemo1;

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
        _graphics.PreferredBackBufferWidth = 1435;
        _graphics.PreferredBackBufferHeight = 760;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _mainMenu.LoadContent(Content, GraphicsDevice);

        
        Texture2D playerTexture = Content.Load<Texture2D>("Bunny1");
        Texture2D enemyTexture = Content.Load<Texture2D>("BadCat");

        _sprites = new List<Sprite>
        {
            new Sprite(enemyTexture, new Vector2(419, 450)),
            new Sprite(enemyTexture, new Vector2(621, 250)),
            new Sprite(enemyTexture, new Vector2(290, 192))
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
                killList.Add(sprite);
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
                foreach (var sprite in _sprites)
                    sprite.Draw(_spriteBatch);

                _player.Draw(_spriteBatch);
                break;
        }
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
