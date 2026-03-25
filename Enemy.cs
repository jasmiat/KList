using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1;

public class Enemy
{
    protected Vector2 position;
    protected float speed;
    protected Texture2D texture;
    
    public Rectangle Rect
    {
        get
        {
            return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }
    }

    public Enemy(Texture2D Texture, Vector2 startPos)
    {
        this.texture = Texture;
        this.position = startPos;
        this.speed = 60f;
    }

    public void Update(GameTime gameTime, Vector2 playerPos)
    {
        Vector2 direction = playerPos - position;

        if (direction != Vector2.Zero)
        {
            direction.Normalize();
            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position, Color.White);
    }
}
