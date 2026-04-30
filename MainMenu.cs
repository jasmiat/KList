using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace KListDemo1;

public class MainMenu // KC'S PART ENTIRELY - PLEASE DON'T TOUCH W/OUT TELLING ME!!
{
    public enum MenuOption
    {
        Start,
        Checkpoint,
        Settings,
        Exit,
        None
    }

    public MenuOption SelectedOption { get; private set; } = MenuOption.None;

    private enum MenuSettings
    {
        Main,
        Settings
    }

    private MenuSettings _settings = MenuSettings.Main;

    private string[] _menuItems = { "Start Game", "Last Checkpoint", "Settings", "Exit" };

    private int _selectedIndex = 0;

    private Texture2D _background;
    private Song _menuMusic;
    private SpriteFont _font;

    private bool _lockKey = true;

    // settings stuff
    private int _settingIndex = 0;
    private float _volume = 0.5f;
    private bool _isFullScreen = true; // start @ fullscreen
    private GraphicsDeviceManager _graphics;

    KeyboardState _previousKeyboardState;

    public void SetGraphicsManager(GraphicsDeviceManager graphics)
    {
        _graphics = graphics;
    }
    
    public void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();

        if (_lockKey)
        {
            _previousKeyboardState = keyboard;
            _lockKey = false;
            
            return;
        }

        switch (_settings)
        {
            case MenuSettings.Main:
                UpdateMain(keyboard);
                break;
            
            case MenuSettings.Settings:
                UpdateSettings(keyboard);
                break;
        }

        _previousKeyboardState = keyboard;
    }

    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, GraphicsDevice graphics)
    {
        _background = content.Load<Texture2D>("Second BackgroundKList");
        _menuMusic = content.Load<Song>("Futile Devices Instrumental");
        _font = content.Load<SpriteFont>("MenuFont");

        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = _volume;
        MediaPlayer.Play(_menuMusic);
    }

    private void UpdateMain(KeyboardState keyboard)
    {
        if (keyboard.IsKeyDown(Keys.Up) && _previousKeyboardState.IsKeyUp(Keys.Up))
        {
            _selectedIndex--;
            if (_selectedIndex < 0) _selectedIndex = _menuItems.Length - 1;
        }

        if (keyboard.IsKeyDown(Keys.Down) && _previousKeyboardState.IsKeyUp(Keys.Down))
        {
            _selectedIndex++;
            if (_selectedIndex >= _menuItems.Length) _selectedIndex = 0;
        }

        if (keyboard.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter))
        {
            switch (_selectedIndex)
            {
                case 0:
                    SelectedOption = MenuOption.Start;
                    MediaPlayer.Stop();
                    break;
                case 1:
                    SelectedOption = MenuOption.Checkpoint;
                    break;
                case 2:
                    _settings = MenuSettings.Settings;
                    _settingIndex = 0;
                    break;
                case 3:
                    Environment.Exit(0);
                    break;
            }
        }
    }

    private void UpdateSettings(KeyboardState keyboard)
    {
        if (keyboard.IsKeyDown(Keys.Up) && _previousKeyboardState.IsKeyUp(Keys.Up)) // KC NOTE: help handling for sticky keys
        {
            _settingIndex--;
            if (_settingIndex < 0) _settingIndex = 1;
        }

        if (keyboard.IsKeyDown(Keys.Down) && _previousKeyboardState.IsKeyUp(Keys.Down)) // ^^
        {
            _settingIndex++;
            if (_settingIndex > 1) _settingIndex = 0;
        }

        if (_settingIndex == 0)
        {
            if (keyboard.IsKeyDown(Keys.Left) && _previousKeyboardState.IsKeyUp(Keys.Left)) // 10% change - incr
            {
                _volume = MathHelper.Clamp(_volume - 0.1f, 0.0f, 1.0f); // Clamp to cap it at 0 and 1
                MediaPlayer.Volume = _volume;
            }

            if (keyboard.IsKeyDown(Keys.Right) && _previousKeyboardState.IsKeyUp(Keys.Right)) // 10% change - dec
            {
                _volume = MathHelper.Clamp(_volume + 0.1f, 0.0f, 1.0f);
                MediaPlayer.Volume = _volume;
            }
        }

        if (_settingIndex == 1)
        {
            if ((keyboard.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter)) || (keyboard.IsKeyDown(Keys.Left)  && _previousKeyboardState.IsKeyUp(Keys.Left))  || (keyboard.IsKeyDown(Keys.Right) && _previousKeyboardState.IsKeyUp(Keys.Right)))
            {
                _isFullScreen = !_isFullScreen;

                if (_graphics != null)
                {
                    _graphics.IsFullScreen = _isFullScreen;

                    if (_isFullScreen)
                    {
                        _graphics.PreferredBackBufferWidth  = 1500;
                        _graphics.PreferredBackBufferHeight = 1250;
                    }
                    else// this is what shows when its not fullscreen, random size for presentation (better visual)
                    {
                        _graphics.PreferredBackBufferWidth  = 1280; 
                        _graphics.PreferredBackBufferHeight = 720;
                    }

                    _graphics.ApplyChanges();
                }
            }
        }

        // esc to go back
        if (keyboard.IsKeyDown(Keys.Escape) && _previousKeyboardState.IsKeyUp(Keys.Escape))
        {
            _settings = MenuSettings.Main;
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_background, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.White);

        switch (_settings)
        {
            case MenuSettings.Main: // checkpoint and logic
                DrawMain(spriteBatch);
                break;
            case MenuSettings.Settings: // volume and screen adjustment
                DrawSettings(spriteBatch);
                break;
        }
    }

    private void DrawMain(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < _menuItems.Length; i++)
        {
            Color color = (i == _selectedIndex) ? Color.Pink : Color.White; // pink when it's the selected choice, white when its not

            Vector2 textSize = _font.MeasureString(_menuItems[i]); // KC NOTE: MeasureString helps makes it centralize the txt
            Vector2 position = new Vector2((spriteBatch.GraphicsDevice.Viewport.Width - textSize.X) / 2, (spriteBatch.GraphicsDevice.Viewport.Height / 2) + i * 60);

            spriteBatch.DrawString(_font, _menuItems[i], position, color);
        }
        
        if (SelectedOption == MenuOption.Checkpoint) // no checkpoints, msg pops up above options
        {
            string checkpointMessage = "NO CHECKPOINTS AVAILABLE - PLEASE SELECT ANOTHER OPTION <3";
            Vector2 checkpointSize = _font.MeasureString(checkpointMessage);
            spriteBatch.DrawString(_font, checkpointMessage, new Vector2((spriteBatch.GraphicsDevice.Viewport.Width - checkpointSize.X) / 2, (spriteBatch.GraphicsDevice.Viewport.Height / 2) - 80), Color.Pink);
        }
        
    }

    private void DrawSettings(SpriteBatch spriteBatch)
    {
        int viewW = spriteBatch.GraphicsDevice.Viewport.Width;
        int viewH = spriteBatch.GraphicsDevice.Viewport.Height;
        int centerX = viewW / 2; // KC NOTE: do not change!!!! will make txt go out of place!!!!!!
        int startY = viewH / 2 - 60; // ^^

        string settingTitle = "SETTING OPTIONS";
        Vector2 titleSize = _font.MeasureString(settingTitle);
        spriteBatch.DrawString(_font, settingTitle, new Vector2(centerX - titleSize.X / 2, startY - 70), Color.Pink);

        Color volumeColor = (_settingIndex == 0) ? Color.Blue : Color.White; // turns blue when its on this choice and white when its not

        // VOLUME OPTIONS
        int length = 10;
        int total = (int)Math.Round(_volume * length);
        string bar = VolumeBar(total, length);
        string volumeLabel = $"Volume:  -{bar}+";
        Vector2 volSize = _font.MeasureString(volumeLabel);
        spriteBatch.DrawString(_font, volumeLabel, new Vector2(centerX - volSize.X / 2, startY), volumeColor);

        // FullScreen OPTIONS
        Color fsColor = (_settingIndex == 1) ? Color.Blue : Color.White; // turns blue when its on this choice and white when its not
        string fsToggle = _isFullScreen ? "ON" : "OFF";
        string fsLabel = $"Fullscreen:  {fsToggle}";
        Vector2 fsSize = _font.MeasureString(fsLabel);
        spriteBatch.DrawString(_font, fsLabel, new Vector2(centerX - fsSize.X / 2, startY + 70), fsColor);

        // GO BACK TO MAIN MENU OPTION
        string goBack = "Press ESC to go Back to Main Menu";
        Vector2 goBackSize = _font.MeasureString(goBack);
        spriteBatch.DrawString(_font, goBack,new Vector2(centerX - goBackSize.X / 2, startY + 160), Color.Purple);
    }
    
    private string VolumeBar(int total, int length) // physical volume bar design
    {
        string left = new string('_', total);
        string right = new string('_', length - total);
        return $"{left}|{right}"; // where the volume is, the adjuster symbol
    }
    
    public void Restart()
    {
        SelectedOption = MenuOption.None;
        _selectedIndex = 0;
        _settings = MenuSettings.Main;
        _lockKey = true;
    }
}
