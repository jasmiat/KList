using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KListDemo1;

public class Enemy
{
    Vector2 position;
    float speed;
    Texture2D texture;
    
    public Rectangle Rect
    {
        get
        {
            return new Rectangle((int)position.X, (int)position.Y, 100, 200);
        }
    }

    public Enemy(Texture2D Texture, Vector2 startPos, float speed)
    {

        this.texture = Texture;
        this.position = startPos;
        this.speed = speed;
    }

    public void Update(GameTime gameTime, Vector2 playerPos)
    {
        Vector2 direction = playerPos - position;

        if (direction != Vector2.Zero)
        {
            direction.Normalize();
            
            position += direction *speed*(float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position, Color.White);
    }
    
    public Vector2 Postion => position;
}
