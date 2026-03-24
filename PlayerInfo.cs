using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1
{
    public class PlayerInfo
    {
        private Texture2D _pixel;
        private SpriteFont _font;
        private PlayerSprite _player;

        private int _screenWidth;
        private int _screenHeight;

        public PlayerInfo(GraphicsDevice graphicsDevice, SpriteFont font, int width, int height, PlayerSprite player)
        {
            _screenWidth = width;
            _screenHeight = height;
            _font = font;
            _player = player;

            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //size of dialogue box
            int hudHeight = _screenHeight / 6;
            int hudY = _screenHeight - hudHeight;

            // color background
            spriteBatch.Draw(_pixel,
                new Rectangle(0, hudY, _screenWidth, hudHeight),
                Color.Gray * 0.7f);

            // da border
            DrawBorder(spriteBatch, new Rectangle(0, hudY, _screenWidth, hudHeight), 3, Color.Black);

            DrawPlayer(spriteBatch, hudY);
            DrawInventory(spriteBatch, hudY);
        }

        private void DrawPlayer(SpriteBatch spriteBatch, int hudY)
        {
            int iconSize = 100;
            int iconX = 20;
            int iconY = hudY + 20;

            // bunny icon
            spriteBatch.Draw(_player.texture, new Rectangle(iconX, iconY, iconSize, iconSize), Color.White);
            DrawBorder(spriteBatch, new Rectangle(iconX, iconY, iconSize, iconSize), 2, Color.Black);

            int textX = iconX + iconSize + 15;
            int textY = iconY + 5;

            spriteBatch.DrawString(_font, "Player", new Vector2(textX, textY), Color.Black);

            // health + mana bar
            float hp = MathHelper.Clamp(_player.position.X / _screenWidth, 0f, 1f);
            float mana = MathHelper.Clamp(_player.position.Y / _screenHeight, 0f, 1f);

            DrawBar(spriteBatch, textX, textY + 50, 150, 12, hp, Color.Red);
            DrawBar(spriteBatch, textX, textY + 100, 150, 12, mana, Color.Blue);
        }

        private void DrawInventory(SpriteBatch spriteBatch, int hudY)
        {
            int startX = _screenWidth - 260;
            int startY = hudY + 20;

            int size = 50;
            int spacing = 10;

            for (int i = 0; i < 4; i++)
            {
                Rectangle slot = new Rectangle(startX + i * (size + spacing), startY, size, size);

                spriteBatch.Draw(_pixel, slot, Color.White);
                DrawBorder(spriteBatch, slot, 2, Color.Black);
            }
        }

        private void DrawBar(SpriteBatch spriteBatch, int x, int y, int width, int height, float percent, Color color)
        {
            spriteBatch.Draw(_pixel, new Rectangle(x, y, width, height), Color.DarkGray);
            spriteBatch.Draw(_pixel, new Rectangle(x, y, (int)(width * percent), height), color);
            DrawBorder(spriteBatch, new Rectangle(x, y, width, height), 1, Color.Black);
        }

        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, int thickness, Color color)
        {
            spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            spriteBatch.Draw(_pixel, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color);
            spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color);
        }
    }
}
