using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KListDemo1
{
    public class PlayerSprite
    {
        private AnimatedTexture animatedTexture; // KC - animation portions
        public Texture2D texture;
        public Vector2 position;
        public Vector2 FacingDirection = Vector2.UnitX;
        public Vector2 velocity;
        
        public float speed = 5f;

        private int frameWidth;
        private int frameHeight;

        public Texture2D SpritesheetTexture { get; private set; }
        public int FrameWidth => frameWidth;
        public int FrameHeight => frameHeight;

        public PlayerSprite(ContentManager content, string assetName,
                            Vector2 startPosition, int frameCount, int framesPerSec)
        {
            animatedTexture = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0f);
            animatedTexture.Load(content, assetName, frameCount, framesPerSec);

            Texture2D tex = content.Load<Texture2D>(assetName);
            SpritesheetTexture = tex;
            frameWidth = tex.Width / frameCount;
            frameHeight = tex.Height;

            position = startPosition;
        }

        public Rectangle Rect
        {
            get { return new Rectangle((int)position.X, (int)position.Y, frameWidth, frameHeight); }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            animatedTexture.UpdateFrame(elapsed);

            KeyboardState keyboard = Keyboard.GetState();
            velocity = Vector2.Zero;
            
            if (keyboard.IsKeyDown(Keys.Left)  || keyboard.IsKeyDown(Keys.A)) velocity.X -= speed;
            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D)) velocity.X += speed;
            if (keyboard.IsKeyDown(Keys.Up)    || keyboard.IsKeyDown(Keys.W)) velocity.Y -= speed;
            if (keyboard.IsKeyDown(Keys.Down)  || keyboard.IsKeyDown(Keys.S)) velocity.Y += speed;
            position += velocity;
        } 

        public void Draw(SpriteBatch spriteBatch)
        {
            animatedTexture.DrawFrame(spriteBatch, position);
        }
    }
}
