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

    public Sword(Texture2D texture) : base(texture,0.3f)
    {
        
    }

    public override Rectangle? Attack(Vector2 playerPos, Vector2 facing)
    {
        if (timer < cooldown)
        {
            return null;
        }

        Vector2 offset = facing * range;
        Vector2 attackDir = playerPos + offset;
        
        return new Rectangle((int)attackDir.X,(int)attackDir.Y,range,range);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 playerPos,  Vector2 facingDir)
    {

        Vector2 drawingPos = playerPos + facingDir * 50;
        spriteBatch.Draw(texture,drawingPos,Color.White);
    }
}
