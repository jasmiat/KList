using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace KListDemo1
{
    internal class Heart
    {
        // keeps track of time between spawns
        private static float spawnTimer = 0f;

        // how often a heart should spawn in seconds
        private static float spawnInterval = 10f;

        // called from Game1 to check if it's time to spawn a new heart
        public static bool ShouldSpawn(GameTime gameTime)
        {
            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // if enough time passed, reset timer and allow spawn
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                return true;
            }

            return false;
        }

        // original spawn position (used for floating effect)
        public Vector2 BasePosition;

        // determines if the heart should still exist in the game
        public bool Active = true;

        private Texture2D _texture;

        // time trackers
        private float time = 0f;       // floating animation
        private float lifetime = 0f;   // how long the heart has existed

        private const float maxLifetime = 10f; // disappears after 10 seconds

        // animation
        private int frame;
        private int frameCount;
        private float frameTimer;
        private float frameSpeed = 0.12f;

        private int frameWidth;
        private int frameHeight;

        // collision box (used for player pickup)
        public Rectangle Rect =>
            new Rectangle((int)Position.X, (int)Position.Y, frameWidth, frameHeight);

        public Heart(Texture2D texture, Vector2 position, int totalFrames)
        {
            _texture = texture;
            BasePosition = position;

            frameCount = totalFrames;

            // spritesheet -> frames
            frameWidth = texture.Width / frameCount;
            frameHeight = texture.Height;
        }

        // floaty motion
        public Vector2 Position
        {
            get
            {
                float floatOffset = (float)Math.Sin(time * 2f) * 10f;
                return new Vector2(BasePosition.X, BasePosition.Y + floatOffset);
            }
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            time += dt;
            lifetime += dt;

            // animation
            frameTimer += dt;
            if (frameTimer >= frameSpeed)
            {
                frame = (frame + 1) % frameCount;
                frameTimer = 0f;
            }

            // remove heart after lifetime max
            if (lifetime >= maxLifetime)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // don't draw 
            if (!Active)
                return;

            // blink after 5 seconds
            if (lifetime >= 5f)
            {
                float progress = (lifetime - 5f) / 5f;

                // blinking speeds up
                float blinkSpeed = 6f + (progress * 20f);

                // blink effect
                if (Math.Sin(lifetime * blinkSpeed) > 0)
                    return;
            }

            Rectangle source = new Rectangle(frame * frameWidth, 0, frameWidth, frameHeight);

            // draw heart at its floating position
            spriteBatch.Draw(_texture, Position, source, Color.White);
        }
    }
}
