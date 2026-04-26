using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1;

public class Enemy
{
    public Vector2 position;
    protected float speed;
    protected Texture2D texture;
    public Vector2 velocity; 
    
    //*****NEW****** - Jasmine - Enemy Healthbar
    public int Health = 20;
    public int MaxHealth = 20;
    
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
    
    //******NEW******* - Jasmine - Damage
    public void TakeDamage(int amount)
    {
        Health -= amount;
    }

    public bool IsDead()
    {
        return Health <= 0;
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

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch,  Texture2D pixel)
    {
        spriteBatch.Draw(texture, position, Color.White);
        
        // *****NEW***** - Jasmine - Health Bar Draw
        int barWidth = texture.Width;
        int barHeight = 6;

        float healthPercent = (float)Health / MaxHealth;
        int currentWidth = (int)(barWidth * healthPercent);

        Vector2 barPosition = new Vector2(position.X, position.Y - 10);

        // Background (black)
        spriteBatch.Draw(pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.Black);

        // Healthbar (red)
        spriteBatch.Draw(pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, currentWidth, barHeight), Color.Red);
    }
}
