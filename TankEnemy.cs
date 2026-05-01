using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1;

// Jordan and Alex base
// Jasmine added health
// Kandace added animation
internal class TankEnemy : Enemy
{
    public TankEnemy(Texture2D texture, Vector2 startPos) : base(texture, startPos)
    {
        speed = 140f;
        Health = 1000;
        MaxHealth = 1000;
        _timePerFrame = 1f / 10;
        this.Health = MaxHealth;
    }

    public new void Update(GameTime gameTime, Vector2 playerPos)
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

    public new void Draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D pixel)
    {
        Rectangle source = new Rectangle(_frame * FrameWidth, 0, FrameWidth, FrameHeight);
        spriteBatch.Draw(texture, position, source, Color.OrangeRed);

        int barWidth = FrameWidth;
        int barHeight = 6;
        float healthPercent = (float)Health / MaxHealth;
        int currentWidth = (int)(barWidth * healthPercent);
        Vector2 barPos = new Vector2(position.X, position.Y - 10);

        spriteBatch.Draw(pixel, new Rectangle((int)barPos.X, (int)barPos.Y, barWidth, barHeight), Color.Black);
        spriteBatch.Draw(pixel, new Rectangle((int)barPos.X, (int)barPos.Y, currentWidth, barHeight), Color.DarkRed);
    }
}
