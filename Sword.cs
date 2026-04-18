using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1;

internal class Sword : Weapon
{
    private int dmg;
    private int range = 50;
    private float attackTime = 5f;
    private float attackTimer = 0.5f;

    private float angleStart = -MathHelper.PiOver4;
    private float angleEnd = MathHelper.PiOver4;

    float currentAngle;
    bool isAttacking = false;

    // KC add on- carrot attack/weapon
    private Texture2D _carrotTexture;
    private List<CarrotSword> _carrotAttack = new();
    private const int CarrotFrames = 1;

    public Sword(Texture2D texture, Texture2D carrotTexture) : base(texture, 0.3f)
    {
        _carrotTexture = carrotTexture;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Jordan's part- sword swing
        if (isAttacking)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            attackTimer += time;
            float attackProg = attackTimer / attackTime;

            if (attackProg >= 1f)
                isAttacking = false;
            else
                currentAngle = MathHelper.Lerp(angleStart, angleEnd, attackProg);
        }

        // KC- carrot attack add on
        foreach (var p in _carrotAttack)
            p.Update(gameTime);
        _carrotAttack.RemoveAll(p => !p.Active);
    }

    public void StartAttacking()
    {
        if (isAttacking) return;
        isAttacking = true;
        attackTimer = 0f;
    }

    public override Rectangle? Attack(Vector2 playerPos, Vector2 facing) // KC
    {
        if (timer < cooldown) return null;
        timer = 0f;

        _carrotAttack.Add(new CarrotSword(playerPos, facing, _carrotTexture, CarrotFrames));
        
        float rotation = currentAngle + GetBaseRotation(facing);
        Vector2 offset = GetOffset(rotation, 5f);
        Vector2 attackDir = playerPos + offset;
        return new Rectangle((int)attackDir.X, (int)attackDir.Y, range, range);
    }

    public List<Rectangle> GetCarrotAttack()
    {
        var rects = new List<Rectangle>();
        foreach (var p in _carrotAttack)
            if (p.Active) rects.Add(p.Rect);
        return rects;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 playerPos, Vector2 facingDir)
    {
        // Jordan's part - sword draw
        float rotation = currentAngle + GetBaseRotation(facingDir);
        Vector2 offset = GetOffset(rotation, 5f);
        Vector2 drawingPos = playerPos + offset;

        spriteBatch.Draw(
            texture, drawingPos, null, Color.White, rotation,
            new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0f
        );

        // KC- carrot attack/weapon draw
        foreach (var p in _carrotAttack)
            p.Draw(spriteBatch);
    }
}

public class CarrotSword // KC's animation
{
    public Vector2 Position;
    public Vector2 Velocity;
    public bool Active = true;

    private float _lifetime = 0f;
    private const float MaxLifetime = 1.5f;

    private int _frame = 0;
    private float _frameTimer = 0f;
    private const float FrameDuration = 0.08f;
    private int _totalFrames;
    private Texture2D _texture;

    public Rectangle Rect => new Rectangle((int)Position.X, (int)Position.Y, 10, 10);

    public CarrotSword(Vector2 startPos, Vector2 direction, Texture2D texture, int totalFrames)
    {
        Position = startPos;
        Velocity = Vector2.Normalize(direction) * 500f;
        _texture = texture;
        _totalFrames = totalFrames;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += Velocity * dt;

        _lifetime += dt;
        if (_lifetime >= MaxLifetime) Active = false;

        _frameTimer += dt;
        if (_frameTimer >= FrameDuration)
        {
            _frameTimer = 0f;
            _frame = (_frame + 1) % _totalFrames;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!Active) return;

        int frameWidth = _texture.Width / _totalFrames;
        Rectangle sourceRect = new Rectangle(_frame * frameWidth, 0, frameWidth, _texture.Height);
        float rotation = (float)Math.Atan2(Velocity.Y, Velocity.X);

        spriteBatch.Draw(
            _texture, Position, sourceRect, Color.White, rotation,
            new Vector2(frameWidth / 2f, _texture.Height / 2f),
            1f, SpriteEffects.None, 0f
        );
    }
}
