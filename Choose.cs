using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KListDemo1;

public class Choose
{
    private string[] _characters = { "Jesella", "Kathryn", "Count Alaric", "James" };
    private string[] _characterDisplayNames = {
        "Jesella the Witch",
        "Kathryn the Wolf",
        "Count Alaric the Vampire",
        "James the Centaur"
    };
    private int _selectedIndex = -1;

    private SpriteFont _font;
    private KeyboardState _previousKeyboard;

    private bool _selectionDone = false;
    public bool HasSelectedCharacter { get; private set; } = false;
    public string SelectedCharacter { get; private set; }

    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, GraphicsDevice graphics)
    {
        _font = content.Load<SpriteFont>("MenuFont");
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Up) && _previousKeyboard.IsKeyUp(Keys.Up))
        {
            if (_selectedIndex == -1) _selectedIndex = 0;
            else _selectedIndex--;
            if (_selectedIndex < 0) _selectedIndex = _characters.Length - 1;
        }

        if (keyboard.IsKeyDown(Keys.Down) && _previousKeyboard.IsKeyUp(Keys.Down))
        {
            if (_selectedIndex == -1) _selectedIndex = 0;
            else _selectedIndex++;
            if (_selectedIndex >= _characters.Length) _selectedIndex = 0;
        }

        if (keyboard.IsKeyDown(Keys.Enter) && _previousKeyboard.IsKeyUp(Keys.Enter))
        {
            if (_selectedIndex != -1 && !HasSelectedCharacter)
            {
                SelectedCharacter = _characters[_selectedIndex];
                HasSelectedCharacter = true;
            }
            else if (HasSelectedCharacter)
            {
                _selectionDone = true;
            }
        }

        _previousKeyboard = keyboard;
    }

    public bool IsConfirmed
    {
        get
        {
            return _selectionDone;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_font,
            "Which Hero Are You?",
            new Microsoft.Xna.Framework.Vector2(100, 50),
            Microsoft.Xna.Framework.Color.Cyan);

        for (int i = 0; i < _characters.Length; i++)
        {
            Color color = (i == _selectedIndex) ? Microsoft.Xna.Framework.Color.Yellow : Microsoft.Xna.Framework.Color.White;

            Vector2 textSize = _font.MeasureString(_characterDisplayNames[i]);
            Vector2 position = new Microsoft.Xna.Framework.Vector2(
                (spriteBatch.GraphicsDevice.Viewport.Width - textSize.X) / 2,
                150 + i * 60
            );

            spriteBatch.DrawString(_font, _characterDisplayNames[i], position, color);
        }

        if (HasSelectedCharacter)
        {
            spriteBatch.DrawString(_font,
                "Selected: " + _characterDisplayNames[_selectedIndex] + " (Press Enter to confirm)",
                new Microsoft.Xna.Framework.Vector2(100, 400),
                Microsoft.Xna.Framework.Color.Green);
        }
    }
}
