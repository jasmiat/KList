using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1;

internal class TankEnemy : Enemy
{
    public TankEnemy(Texture2D Texture, Vector2 startPos) : base(Texture, startPos)
    {
        speed = 20f;
    }
}
