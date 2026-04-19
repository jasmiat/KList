using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1
{
    public class PlayerInfo
    {
        private Texture2D _pixel;
        private Texture2D _swordTexture;
        private Texture2D _bowTexture;
        private SpriteFont _font;
        private PlayerSprite _player;

        private int _screenWidth;
        private int _screenHeight;

        public PlayerInfo(
            GraphicsDevice graphicsDevice,
            SpriteFont font,
            int width,
            int height,
            PlayerSprite player,
            Texture2D swordTexture,
            Texture2D bowTexture)
        {
            _screenWidth = width;
            _screenHeight = height;
            _font = font;
            _player = player;

            _swordTexture = swordTexture;
            _bowTexture = bowTexture;

            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }   //plz kill me

        public void Draw(SpriteBatch spriteBatch, int health, int maxHealth, WeaponType currentWeapon)
        {
            // backgroundd
            int hudHeight = _screenHeight / 5;
            int hudY = _screenHeight - hudHeight;

            spriteBatch.Draw(_pixel,
                new Rectangle(0, hudY, _screenWidth, hudHeight),
                Color.Gray * 0.7f);

            DrawBorder(spriteBatch, new Rectangle(0, hudY, _screenWidth, hudHeight), 3, Color.Black);
            
            DrawPlayer(spriteBatch, hudY);
            DrawHealthBar(spriteBatch, health, maxHealth, hudY);
            DrawWeaponIcon(spriteBatch, currentWeapon, hudY);
        }

        private void DrawPlayer(SpriteBatch spriteBatch, int hudY)
        {
            int iconSize = 100;
            int iconX = 20;
            int iconY = hudY + 20;

            spriteBatch.Draw(_player.texture, new Rectangle(iconX, iconY, iconSize, iconSize), Color.White);
            DrawBorder(spriteBatch, new Rectangle(iconX, iconY, iconSize, iconSize), 2, Color.Black);

            int textX = iconX + iconSize + 15;
            int textY = iconY + 5;

            spriteBatch.DrawString(_font, "Good Bunny", new Vector2(textX, textY), Color.Black);
        }

        private void DrawHealthBar(SpriteBatch spriteBatch, int health, int maxHealth, int hudY)
        {
            int barWidth = 400;
            int barHeight = 20;

            int x = _screenWidth - 1360;
            int y = hudY + 80;

            // da border
            spriteBatch.Draw(_pixel, new Rectangle(x - 3, y - 3, barWidth + 6, barHeight + 6), Color.Silver);

            // da background
            spriteBatch.Draw(_pixel, new Rectangle(x, y, barWidth, barHeight), Color.Black);

            float percent = (float)health / maxHealth;
            int currentWidth = (int)(barWidth * percent);

            // health color
            spriteBatch.Draw(_pixel, new Rectangle(x, y, currentWidth, barHeight), Color.Red);

            // hp gasp
            spriteBatch.DrawString(_font, $"HP: {health}/{maxHealth}", new Vector2(x, y - 1), Color.White);
        }

        private void DrawWeaponIcon(SpriteBatch spriteBatch, WeaponType currentWeapon, int hudY)
        {
            Texture2D texture = currentWeapon == WeaponType.Sword ? _swordTexture : _bowTexture;

            int size = 140;
            int x = _screenWidth - size - 30;
            int y = hudY + 20;

            // background
            spriteBatch.Draw(_pixel, new Rectangle(x - 5, y - 5, size + 10, size + 10), Color.White);

            if (texture != null)
            {
                spriteBatch.Draw(texture, new Rectangle(x, y, size, size), Color.White);
            }

            spriteBatch.DrawString(_font, currentWeapon.ToString(), new Vector2(x - 10, y + size + 5), Color.White);
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
