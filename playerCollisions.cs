using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Test
{
    public class PlayerSprite
    {
        public Texture2D texture;
        public Vector2 position;
        public Vector2 FacingDirection = Vector2.UnitX;
        public Vector2 velocity;
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
            velocity = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A)) velocity.X -= speed;
            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D)) velocity.X += speed;
            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W)) velocity.Y -= speed;
            if (keyboard.IsKeyDown(Keys.Down)  || keyboard.IsKeyDown(Keys.S)) velocity.Y += speed;

            position += velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
