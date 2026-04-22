using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test;

public class Enemy
{
    public Vector2 position;
    protected float speed;
    protected Texture2D texture;
    public Vector2 velocity; 
    
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
            velocity = direction * speed;
        }
        else
        {
            velocity = Vector2.Zero;
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position, Color.White);
    }
}
