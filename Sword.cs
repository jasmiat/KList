using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1;

internal class Sword : Weapon
{
    private int range = 125;
    private float attackTime = 0.5f;
    private float attackTimer = 0f;

    private float angleStart = -MathHelper.PiOver4;
    private float angleEnd = MathHelper.PiOver4;

    float currentAngle;
    bool isAttacking = false;

    public Sword(Texture2D texture) : base(texture, 0.3f)
    {
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (isAttacking)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            attackTimer += dt;

            float progress = attackTimer / attackTime;

            if (progress >= 1f)
            {
                isAttacking = false;
            }
            else
            {
                currentAngle = MathHelper.Lerp(angleStart, angleEnd, progress);
            }
        }
    }

    public void StartAttacking()
    {
        if (isAttacking) return;

        isAttacking = true;
        attackTimer = 0f;
    }

    public override Rectangle? Attack(Vector2 playerPos, Vector2 facing)
    {
        if (timer < cooldown) return null;

        timer = 0f;

        float rotation = currentAngle + GetBaseRotation(facing);
        Vector2 offset = GetOffset(rotation, 5f);
        Vector2 hitPos = playerPos + offset;

        return new Rectangle((int)hitPos.X, (int)hitPos.Y, range, range);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 playerPos, Vector2 facingDir)
    {
        float rotation = currentAngle + GetBaseRotation(facingDir);
        Vector2 offset = GetOffset(rotation, 125f);
        Vector2 drawPos = playerPos + offset;

        spriteBatch.Draw(
            texture,
            drawPos,
            null,
            Color.White,
            rotation,
            new Vector2(texture.Width / 2f, texture.Height),
            1f,
            SpriteEffects.None,
            0f
        );
    }
}
