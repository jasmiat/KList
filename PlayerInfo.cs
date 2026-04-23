using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1
{
    public class PlayerInfo
    {
        private Texture2D _pixel;
        private SpriteFont _font;
        private PlayerSprite _player;
        private Texture2D _swordTexture;
        private Texture2D _carrotTexture;

        private int _screenWidth;
        private int _screenHeight;


        public PlayerInfo(
            GraphicsDevice graphicsDevice,
            SpriteFont font,
            int width,
            int height,
            PlayerSprite player,
            Texture2D swordTexture,
            Texture2D carrotTexture)
        {
            _screenWidth = width;
            _screenHeight = height;
            _font = font;
            _player = player;
            
            _swordTexture = swordTexture;
            _carrotTexture = carrotTexture;

            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        // previously had from Alex and Jordan- KC and Jas modified
        
        public PlayerInfo(GraphicsDevice graphicsDevice, SpriteFont font, int width, int height, PlayerSprite player)
        {
            _screenWidth = width;
            _screenHeight = height;
            _font = font;
            _player = player;
        
            _pixel = new Texture2D(graphicsDevice, 1, 1); // KC NOTE: DO NOT COMMENT THIS OUT AGAIN, WE NEED THIS
            _pixel.SetData(new[] { Color.White }); // ^^^ as above, so below
        }

        public void Draw(SpriteBatch spriteBatch, int health, int maxHealth, WeaponType currentWeapon)
        {
            //size of dialogue box
            int hudHeight = _screenHeight / 6;
            int hudY = _screenHeight - hudHeight;

            // color background
            spriteBatch.Draw(_pixel,
                new Rectangle(0, hudY, _screenWidth, hudHeight),
                Color.White * 0.7f);

            // da border
            DrawBorder(spriteBatch, new Rectangle(0, hudY, _screenWidth, hudHeight), 1, Color.Black);

            DrawPlayer(spriteBatch, hudY);
            
            DrawHealthBar(spriteBatch, health, maxHealth, hudY);
            DrawWeaponIcon(spriteBatch, currentWeapon, hudY);
        }

        private void DrawPlayer(SpriteBatch spriteBatch, int hudY)
        {
            int iconSize = 100;
            int iconX = 20;
            int iconY = hudY + 20;

            // KC modified - bunny char animation
            Rectangle firstFrame = new Rectangle(0, 0, _player.FrameWidth, _player.FrameHeight);
            spriteBatch.Draw(_player.SpritesheetTexture, new Rectangle(iconX, iconY, iconSize, iconSize), firstFrame, Color.White);
            DrawBorder(spriteBatch, new Rectangle(iconX, iconY, iconSize, iconSize), 2, Color.Black);

            int textX = iconX + iconSize + 15;
            int textY = iconY + 5;

            spriteBatch.DrawString(_font, "Player Info", new Vector2(textX, textY), Color.PaleVioletRed); // KC modified
        }

        private void DrawHealthBar(SpriteBatch spriteBatch, int health, int maxHealth, int hudY) // Jas's part
        {
            int barWidth = 400;
            int barHeight = 20;

            int x = _screenWidth - 1360;
            int y = hudY + 80;
            
            // // health + mana bar
            // float hp = MathHelper.Clamp(_player.position.X / _screenWidth, 0f, 1f);
            // float mana = MathHelper.Clamp(_player.position.Y / _screenHeight, 0f, 1f);
            
            // da border
            spriteBatch.Draw(_pixel, new Rectangle(x - 3, y - 3, barWidth + 6, barHeight + 6), Color.Silver);

            // da background
            spriteBatch.Draw(_pixel, new Rectangle(x, y, barWidth, barHeight), Color.Black);

            float percent = (float)health / maxHealth;
            int currentWidth = (int)(barWidth * percent);

            // health color - KC modified from red to pink (cuter)
            spriteBatch.Draw(_pixel, new Rectangle(x, y, currentWidth, barHeight), Color.Pink);
            
            //hp bar health percentage - KC modified, add empty line above for cleaner look
            spriteBatch.DrawString(_font, $"Health: {health}/{maxHealth}", new Vector2(x, y + _font.LineSpacing), Color.MediumPurple);

        }
        private void DrawWeaponIcon(SpriteBatch spriteBatch, WeaponType currentWeapon, int hudY)
        {
            Texture2D texture = currentWeapon == WeaponType.Sword ? _swordTexture : _carrotTexture;

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

        // private void DrawBar(SpriteBatch spriteBatch, int x, int y, int width, int height, float percent, Color color)
        // {
        //     spriteBatch.Draw(_pixel, new Rectangle(x, y, width, height), Color.DarkGray);
        //     spriteBatch.Draw(_pixel, new Rectangle(x, y, (int)(width * percent), height), color);
        //     DrawBorder(spriteBatch, new Rectangle(x, y, width, height), 1, Color.Black);
        // }

        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, int thickness, Color color)
        {
            spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            spriteBatch.Draw(_pixel, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color);
            spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color);
        }
    }
}
