using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Test;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    List<Sprite> sprites;
    List<Sprite> killList;

    PlayerSprite player;
    

    Texture2D texture;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferHeight = 2480;
        _graphics.PreferredBackBufferWidth = 2480;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        sprites = new();

        // TODO: use this.Content to load your game content here
        Texture2D texture = Content.Load<Texture2D>("Stick-Man");
        Texture2D badGuyTex = Content.Load<Texture2D>("Stick-Man");

        
        
        sprites.Add(new Sprite(badGuyTex, new Vector2(400,400))); 
        sprites.Add(new Sprite(badGuyTex, new Vector2(600,600))); 
        sprites.Add(new Sprite(badGuyTex, new Vector2(700,700))); 
        player = new PlayerSprite(texture, Vector2.Zero);
        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        List<Sprite> killList = new();
        foreach (var sprite in sprites)
        {
            sprite.Update(gameTime);
            if (sprite.Rect.Intersects(player.Rect))
            {
                killList.Add(sprite);
            }
        }

        foreach (var sprite in killList)
        {
            sprites.Remove(sprite);
        }

        
        player.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();

        foreach (var sprite in sprites)
        {
            sprite.Draw(_spriteBatch);
        }
        player.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
