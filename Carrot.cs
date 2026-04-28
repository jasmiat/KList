using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1;

internal class Carrot : Weapon // KC added secondary weapon as a projectile for more animation
{
    private List<CarrotAttack> _projectiles = new(); // THIS IS THE WEAPON
    private const int CarrotFrames = 1;

    public Carrot(Texture2D texture) : base(texture, 0.2f)
    {
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        foreach (var p in _projectiles) // movement of animation
            p.Update(gameTime);

        _projectiles.RemoveAll(p => !p.Active); // error handling, if no more carrots available (doesnt matter bc theres no supply limit)
    }

    public override Rectangle? Attack(Vector2 playerPos, Vector2 facing)
    {
        if (timer < cooldown) // KC NOTE: DO NOT DELETE!! WILL CRASH GAME, NEEDS TO MAKE SURE IT DOESN'T OVERLOAD W TOO MANY CARROTS
            return null;

        timer = 0f;

        _projectiles.Add(new CarrotAttack(playerPos, facing, texture, CarrotFrames));

        return null;
    }
    
    public List<Rectangle> Projectiles() // needed for game1 to keep weapon as a RECT
    {
        var rects = new List<Rectangle>();

        foreach (var p in _projectiles)
        {
            if (p.Active)
                rects.Add(p.Rect);
        }

        return rects;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 playerPos, Vector2 facingDir)
    {
        foreach (var p in _projectiles)
            p.Draw(spriteBatch);
    }
}

internal class CarrotAttack
{
    public Vector2 Position;
    public Vector2 Velocity;
    public bool Active = true;

    private float carrotSupply = 0f;
    private const float maxCarrot = 0.7f; // how long to last, do not put more than 1 bc of the spritesheet size

    private Texture2D _texture;

    public Rectangle Rect => new Rectangle((int)Position.X, (int)Position.Y, 10, 10);

    public CarrotAttack(Vector2 startPos, Vector2 direction, Texture2D texture, int totalFrames) // actual attack movement
    {
        Position = startPos;

        if (direction != Vector2.Zero)
            Velocity = Vector2.Normalize(direction) * 350f; // speed
        else
            Velocity = Vector2.Zero;

        _texture = texture;
    }

    public void Update(GameTime gameTime)
    {
        float stopTime = (float)gameTime.ElapsedGameTime.TotalSeconds; // prevents extras

        Position += Velocity * stopTime; 

        carrotSupply += stopTime; // error handling, stops excess carrot throwing (no more after # of seconds)
        if (carrotSupply >= maxCarrot)
            Active = false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!Active)
            return;

        spriteBatch.Draw(_texture, Position, Color.White); // its Carrot time 
    }
}
