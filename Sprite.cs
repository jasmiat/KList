using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KListDemo1
{
    public class Sprite
    {
        public Texture2D texture;
        public Vector2 postion;

        public Rectangle Rect
        {
            get
            {
                return new Rectangle((int)postion.X, (int)postion.Y, 100, 200);
            }
        }

        public Sprite(Texture2D texture, Vector2 Position)
        {
            this.texture = texture;
            this.postion = Position;
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rect, Color.White);
        }
    }

}
