using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KListDemo1
{
    public class PlayerSprite
    {
        public Texture2D texture;
        public Vector2 position;
        public float speed = 5f;

        public PlayerSprite(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            position = startPosition;
        }

        public Rectangle Rect
        {
            get { return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height); }
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            Vector2 move = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A)) move.X -= speed;
            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D)) move.X += speed;
            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W)) move.Y -= speed;
            if (keyboard.IsKeyDown(Keys.Down)  || keyboard.IsKeyDown(Keys.S)) move.Y += speed;

            position += move;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
