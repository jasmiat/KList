using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace KListDemo1;

public class MainMenu
{
    public enum MenuOption { Start, Checkpoint, Settings, Exit, None }
    public MenuOption SelectedOption { get; private set; } = MenuOption.None;

    private string[] _menuItems = { "Start Game", "Last Checkpoint", "Settings", "Exit" };
    private int _selectedIndex = 0;

    private Texture2D _background;
    private Song _menuMusic;
    private SpriteFont _font;
    
    KeyboardState _previousKeyboardState;

    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, GraphicsDevice graphics)
    {
        _background = content.Load<Texture2D>("KlistBackground");
        _menuMusic = content.Load<Song>("Massive Attack - Angel (Instrumental Original)");
        _font = content.Load<SpriteFont>("MenuFont");

        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(_menuMusic);
        
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();   
        if (keyboard.IsKeyDown(Keys.Up) &&  _previousKeyboardState.IsKeyUp(Keys.Up))  
        {
            _selectedIndex--;
            if (_selectedIndex < 0) _selectedIndex = _menuItems.Length - 1;
        }

        if (keyboard.IsKeyDown(Keys.Down) &&  _previousKeyboardState.IsKeyUp(Keys.Down))
        {
            _selectedIndex++;
            if (_selectedIndex >= _menuItems.Length) _selectedIndex = 0;
        }

        if (keyboard.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter))
        {
            switch (_selectedIndex)
            {
                case 0: SelectedOption = MenuOption.Start;
                    if (keyboard.IsKeyDown(Keys.Enter)) MediaPlayer.Stop(); 
                    break;
                case 1: SelectedOption = MenuOption.Checkpoint; break;
                case 2: SelectedOption = MenuOption.Settings; break;
                case 3: Environment.Exit(0); break;
            }
        }
        _previousKeyboardState = keyboard;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_background, new Rectangle(0, 0,
            spriteBatch.GraphicsDevice.Viewport.Width,
            spriteBatch.GraphicsDevice.Viewport.Height), Color.White);

        for (int i = 0; i < _menuItems.Length; i++)
        {
            Color color = (i == _selectedIndex) ? Color.Yellow : Color.White;

            Vector2 textSize = _font.MeasureString(_menuItems[i]);

            Vector2 position = new Vector2(
                (spriteBatch.GraphicsDevice.Viewport.Width - textSize.X) / 2,
                (spriteBatch.GraphicsDevice.Viewport.Height / 2) + i * 60
            );

            spriteBatch.DrawString(_font, _menuItems[i], position, color);
        }
    }
}
