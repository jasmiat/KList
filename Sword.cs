using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace KListDemo1;

internal class Sword : Weapon
{
    private int dmg; //can be added after we figure out enemy health and stuff
    private int range = 100;
    private float attackTime = .2f;
    private float attackTimer = 0f;
    
    
    private float angleStart = -MathHelper.PiOver4;
    private float angleEnd = MathHelper.PiOver4;
    
    float currentAngle;
    
    bool isAttacking = false;

    public Sword(Texture2D texture) : base(texture,0.3f)
    {
        
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (!isAttacking) return;
        
        float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
        attackTimer += time;

        float attackProg = attackTimer / attackTime;
        
        if(attackProg >= 1f)
        {
            isAttacking = false;
            return;
        }

        currentAngle = MathHelper.Lerp(angleStart, angleEnd, attackProg);
    }

    public void startAttacking()
    {
        if (isAttacking) return;
        
        isAttacking = true;
        attackTimer = 0f;
    }

    public override Rectangle? Attack(Vector2 playerPos, Vector2 facing)
    {
      
        float rotation = currentAngle + GetBaseRotation(facing);


        Vector2 offset = GetOffset(rotation, 40f);

        
        Vector2 attackDir = playerPos + offset;
        
        return new Rectangle((int)attackDir.X,(int)attackDir.Y,range,range);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 playerPos,  Vector2 facingDir)
    {
        float rotation = currentAngle + GetBaseRotation(facingDir);
        
        Vector2 offset = GetOffset(rotation, 40f);

        Vector2 drawingPos = playerPos + offset;
        
        spriteBatch.Draw(
            texture, drawingPos, null, Color.White, rotation, new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0f
        );
        
    }
}
