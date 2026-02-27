using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace KListDemo1;

internal class TankEnemy : Enemy

{
    private Color color; 
    public TankEnemy(Texture2D Texture, Vector2 startPos) : base(Texture, startPos)
    {
        speed = 20f;
    }
}
