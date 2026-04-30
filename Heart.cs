using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace KListDemo1
{
    internal class Heart
    {
        private static float spawnTimer = 0f;
        private static float spawnInterval = 8f;

        public static bool ShouldSpawn(GameTime gameTime)
        {
            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                return true;
            }

            return false;
        }

        public Vector2 BasePosition;
        public bool Active = true;

        private Texture2D _texture;

        private float time = 0f;
        private float lifetime = 0f;
        private const float maxLifetime = 10f;

        private int frame;
        private int frameCount;
        private float frameTimer;
        private float frameSpeed = 0.12f;

        private int frameWidth;
        private int frameHeight;

        public Rectangle Rect =>
            new Rectangle((int)Position.X, (int)Position.Y, frameWidth, frameHeight);

        public Heart(Texture2D texture, Vector2 position, int totalFrames)
        {
            _texture = texture;
            BasePosition = position;

            frameCount = totalFrames;
            frameWidth = texture.Width / frameCount;
            frameHeight = texture.Height;
        }

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

            frameTimer += dt;
            if (frameTimer >= frameSpeed)
            {
                frame = (frame + 1) % frameCount;
                frameTimer = 0f;
            }

            if (lifetime >= maxLifetime)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;

            if (lifetime >= 5f)
            {
                float progress = (lifetime - 5f) / 5f;
                float blinkSpeed = 6f + (progress * 20f);

                if (Math.Sin(lifetime * blinkSpeed) > 0)
                    return;
            }

            Rectangle source = new Rectangle(frame * frameWidth, 0, frameWidth, frameHeight);

            spriteBatch.Draw(_texture, Position, source, Color.White);
        }
    }
}
