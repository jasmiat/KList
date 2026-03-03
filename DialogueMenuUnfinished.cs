using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject2
{
    public class DialogueMenu
    {
        private Texture2D texture;
        private SpriteFont font;

        public string PlayerName = "PLAYER NAME";
        public int MaxHealth = 5;
        public int CurrentHealth = 5;
        public int MaxMana = 5;
        public int CurrentMana = 5;

        public Texture2D PlayerIcon;

        //weaponstuff
        public Texture2D EquippedWeapon;
        public Texture2D[] InventorySlots = new Texture2D[3];

        //layout
        private Rectangle gameArea;
        private Rectangle bottomPanel;
        private Rectangle weaponSlot;

        public DialogueMenu(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _font = font;
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            //unfinished, idk how to set screen width or height
            screenHeight
            screenWidth

                //gameplay rectangle thanggg
                gameArea = new Rectangle(50, 30, screenWidth - 100, screenHeight - 250);

            //dialogue panel
            bottomPanel = new Rectangle(0, screenHeight - 200, screenWidth, 200);

            //weapon slot
            weaponSlot = new Rectangle(screenWidth - 220, screenHeight - 180, 150, 150);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            //dialogue menu drawing thing
            spriteBatch.Draw(texture, bottomPanel, Color.DarkSlateGray);

            DrawPlayerInfo(spriteBatch);
            DrawWeaponSection(spriteBatch);

        }
        private void DrawPlayerInfo(SpriteBatch spriteBatch)
        {
            int startX = 40;
            int startY = bottomPanel.Y + 20;

            //player icon
            if (PlayerIcon != null)
            {
                spriteBatch.Draw(PlayerIcon,
                    new Rectangle(startX, startY, 100, 100),
                    Color.White);
            }

            //player name
            spriteBatch.DrawString(font, PlayerName,
                new Vector2(startX + 120, startY),
                Color.White);

            //health bar
            for (int i = 0; i < MaxHealth; i++)
            {
                Color heartColor = i < CurrentHealth ? Color.Red : Color.Gray;

                spriteBatch.Draw(texture, new Rectangle(startX + 120 + (i * 30), startY + 40, 20, 20),
                    heartColor);
            }

            //mana bar
            for (int i = 0; i < MaxMana; i++)
            {
                Color manaColor = i < CurrentMana ? Color.LimeGreen : Color.DarkGreen;

                spriteBatch.Draw(texture, new Rectangle(startX + 120 + (i * 30), startY + 80, 20, 20),
                    manaColor);
            }
        }
    }
}
