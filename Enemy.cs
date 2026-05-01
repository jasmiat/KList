using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1;

// Jordan made the base class with enemy movement and player tracking 
// KC and Jasmine add on for animation
// Jasmine add on for healthbar
// Alex helped w base w Jordan and debugging

public class Enemy
{
    public Vector2 position;
    public float speed;
    protected Texture2D texture;
    public Vector2 velocity; 
    
    // Jasmine - Enemy Healthbar
    public int Health = 350;
    public int MaxHealth = 350;
    
    // KC Note: add animination
    protected AnimatedTexture _animation;
    protected const int FrameCount = 3;
    protected const int FramesPerSec = 8;
    protected const int FrameWidth = 128;
    protected const int FrameHeight = 128;
    protected int _frame = 0;
    protected float _elapsed = 0f;
    protected float _timePerFrame = 1f / FramesPerSec;
    
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
        this.speed = 70f;
        this.Health = MaxHealth;

        _animation = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0f);
    }


    // Jasmine - Damage
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
        
        _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_elapsed >= _timePerFrame)
        {
            _frame = (_frame + 1) % FrameCount;
            _elapsed -= _timePerFrame;
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch,  Texture2D pixel)
    {
        Rectangle source = new Rectangle(_frame * FrameWidth, 0, FrameWidth, FrameHeight);
        spriteBatch.Draw(texture, position, source, Color.White);
        
        // Jasmine - Health Bar Draw
        int barWidth = texture.Width;
        int barHeight = 6;

        float healthPercent = (float)Health / MaxHealth;
        int currentWidth = (int)(barWidth * healthPercent);

        Vector2 barPosition = new Vector2(position.X, position.Y - 10);

        // Background (white)
        spriteBatch.Draw(pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.White);

        // Healthbar (red)
        spriteBatch.Draw(pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, currentWidth, barHeight), Color.Red);
    }
}
