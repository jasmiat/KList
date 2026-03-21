
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

    public virtual void Update(GameTime gameTime)
    {
        timer +=(float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    
    

    public abstract Rectangle? Attack(Vector2 playerPos,  Vector2 facing);
    
    public virtual void Draw(SpriteBatch spriteBatch, Vector2 playerPos,Vector2 facingDir)
    {
        spriteBatch.Draw(texture, playerPos, Color.White);
    }
}
