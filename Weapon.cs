using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KListDemo1;

public abstract class Weapon
{
    protected Vector2 position;
    protected Texture2D texture;
    protected float cooldown;
    protected float timer;

    public Weapon(Texture2D texture,float cooldown)
    {
        this.texture = texture;
        this.cooldown = cooldown;
        
        
    }

    protected float GetBaseRotation(Vector2 facing)
    {
        return (float)Math.Atan2(facing.Y, facing.X);
    }

    protected Vector2 GetOffset(float rotation, float radius)
    {
        return new Vector2((float)Math.Cos(rotation) * radius, (float)Math.Sin(rotation) * radius);
    }

    public virtual void Update(GameTime gameTime)
    {
        timer +=(float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    
    public abstract Rectangle? Attack(Vector2 playerPos,  Vector2 facing);
    public abstract void Draw(SpriteBatch spriteBatch, Vector2 playerPos, Vector2 facingDir);

}
