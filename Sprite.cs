using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Alex's part
namespace KListDemo1
{
    public class Sprite
    {
        public Texture2D texture;
        public Vector2 position;

        public Rectangle Rect
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, 100, 200);
            }
        }

        public Sprite(Texture2D texture, Vector2 Position)
        {
            this.texture = texture;
            this.position = Position;
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rect, Color.White);
        }
    }
}
