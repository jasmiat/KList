using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1;

public class WaveManager // Alex's Part
{
    private Random _random = new Random();

    private Texture2D _enemyTexture;
    private Texture2D _tankTexture;
    private GraphicsDevice _graphics;
    private Rectangle _playableArea;

    // KC add on to delay the new waves by 5 secs and display a countdown timer w/ msg
    private const float countdown = 5f;
    private const float messageTimer = 2f;
    private bool countdownDoneMessage = false;
    private float _messageWaitTime = 0f;
    private float _countdownTimer = 0f;
    private bool _itsComing = false;
    public bool NewWave => _itsComing || countdownDoneMessage;
    public bool ReadyToSpawn { get; private set; } = false;

    public int CurrentWave { get; private set; } = 1;
    public const int MaxWaves = 5; // KC add on for credits so theres a limit to spawning waves, no infinite waves

    public WaveManager(Texture2D enemyTexture, Texture2D tankTexture, GraphicsDevice graphics, Rectangle playableArea)
    {
        _enemyTexture = enemyTexture;
        _tankTexture  = tankTexture;
        _graphics     = graphics;
        _playableArea = playableArea;
    }
    
    
    public void StartNewWave()
    {
        if (_itsComing || IsLastWave()) return;
        _itsComing = true;
        ReadyToSpawn = false;
        _countdownTimer = 0f;
    }

    public void Update(float deltaTime)
    {
        if (countdownDoneMessage)
        {
            _messageWaitTime += deltaTime;
            if (_messageWaitTime >= messageTimer)
            {
                countdownDoneMessage = false;
                ReadyToSpawn = true;
            }

            return;
        }
        
        if (!_itsComing) return;

        _countdownTimer += deltaTime;

        if (_countdownTimer >= countdown)
        {
            _itsComing = false;
            countdownDoneMessage = true;
            _messageWaitTime = 0f;
        }
    }

    public void Check()
    {
        ReadyToSpawn = false;
    }

    public void ShowMessage(SpriteBatch spriteBatch, SpriteFont font, Rectangle screenBounds)
    {
        if (!_itsComing && !countdownDoneMessage)
            return;

        int secondsLeft = (int)Math.Ceiling(countdown - _countdownTimer);

        string line1 = "Next Wave Coming In...";
        string line2 = countdownDoneMessage ? "Keep fighting your way out the Dark Valley!" : secondsLeft.ToString();

        Vector2 center = new Vector2(screenBounds.Width / 2f, screenBounds.Height / 2f);

        Vector2 size1 = font.MeasureString(line1);
        Vector2 size2 = font.MeasureString(line2);

        spriteBatch.DrawString(font, line1, center - new Vector2(size1.X / 2f, size1.Y + 4), Color.White);
        spriteBatch.DrawString(font, line2, center - new Vector2(size2.X / 2f, -4), Color.Yellow);
    }

    public List<Enemy> GenerateWave()
    {
        List<Enemy> enemies = new();

        // Enemy count currently is linear but can be manually set up another way later on
        int enemyCount = 3 * (int)Math.Pow(2, CurrentWave - 1);
        int tankCount  = 1 * (int)Math.Pow(2, CurrentWave - 1);
        
        // Spawn enemies not in the border but a little away
        int centerX = _playableArea.Left + _playableArea.Width  / 2;
        int centerY = _playableArea.Top  + _playableArea.Height / 2;
        
        // First wave enemy count
        if (CurrentWave == 1)
        {
            enemies.Add(new Enemy(_enemyTexture, new Vector2(centerX - 100, centerY)));
            enemies.Add(new Enemy(_enemyTexture, new Vector2(centerX + 100, centerY)));
            enemies.Add(new TankEnemy(_tankTexture, new Vector2(centerX, centerY)));

            return enemies;
        }
        
        for (int i = 0; i < enemyCount; i++)
        {
            // Spawn in enemy for each enemy count
            enemies.Add(new Enemy(_enemyTexture, new Vector2((centerX),(centerY))));
        }
        
        for (int i = 0; i < tankCount; i++)
        {
            // Spawn in tank enemy for each tankCount 
            enemies.Add(new TankEnemy(_tankTexture, new Vector2(_playableArea.Left + _playableArea.Width / 4, centerY)));
        }

        return enemies;
    }
    
    
    public bool IsLastWave() => CurrentWave >= MaxWaves;
    
    public void NextWave() => CurrentWave++;

    public void Restart()
    {
        CurrentWave = 1;
        _itsComing = false;
        ReadyToSpawn = false;
        _countdownTimer = 0f;
        countdownDoneMessage = false;
        _messageWaitTime = 0f;
    }
}
