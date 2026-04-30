using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KListDemo1;

public class WaveManager // Alex's Part, KC add-ons to show message in between. KC + Alex: countdown code. KC + Jordan: enemies spawning debugging
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

    public void ShowMessage(SpriteBatch spriteBatch, SpriteFont font, Rectangle screenBounds) // KC add-on: to show msg between waves 
    {
        if (!_itsComing && !countdownDoneMessage)
            return;

        int secondsLeft = (int)Math.Ceiling(countdown - _countdownTimer); // math for cooldown

        string line1 = "Next Wave Coming In...";
        string line2 = countdownDoneMessage ? "Keep fighting your way out the Dark Valley!" : secondsLeft.ToString();

        Vector2 center = new Vector2(screenBounds.Width / 2f, screenBounds.Height / 2f);

        Vector2 size1 = font.MeasureString(line1); // center txt
        Vector2 size2 = font.MeasureString(line2); // ^^

        spriteBatch.DrawString(font, line1, center - new Vector2(size1.X / 2f, size1.Y + 4), Color.White);// first msg font color
        spriteBatch.DrawString(font, line2, center - new Vector2(size2.X / 2f, -4), Color.Yellow); // second msg font color
    }

    public List<Enemy> GenerateWave() // 
    {
        List<Enemy> enemies = new();

        // Spawn enemies not in the border but a little away
        int centerX = _playableArea.Left + _playableArea.Width / 2;
        int centerY = _playableArea.Top + _playableArea.Height / 2;

        // First wave enemy count
        // ^^ KC NOTE: I'M EDITING THIS SO ITS 4 ENEMIES IN THE BEGINNING...
        // ^^ ....gonna debug specifics for how many enemies we want so the math is less intense
        if (CurrentWave == 1)
        {
            for (int i = 0; i < 4; i++)
                enemies.Add(new Enemy(_enemyTexture, new Vector2(centerX + (i - 2) * 80, centerY)));

            return enemies;
        }

        // KC Note: 2nd wave to add tank enemy and add 2 more enemies
        // ^^ 1st wave only has regular enemy
        // Jordan: redid math (debugged)
        if (CurrentWave == 2)
        {
            for (int i = 0; i < 6; i++)
                enemies.Add(new Enemy(_enemyTexture, new Vector2(centerX + (i - 3) * 70, centerY)));
            enemies.Add(new TankEnemy(_tankTexture,
                new Vector2(_playableArea.Left + _playableArea.Width / 4, centerY)));

            return enemies;
        }

        // KC NOTE: 3-last wave:
        // ^^ - double tank enemies
        // ^^ - add 2 more enemies than last
        // ^^ - increase speed by 15%ltiplier;
        
        // Jordan: redid math (debugged)
        if (!IsLastWave())
        {
            int tankBase = 2;
            int enemyBase = 8;
            float speedMultiplier = 1.15f; // 15% inc, use 1.xxf bc 0.xxf slows it by xx% instead

            // Each wave past 3 doubles tanks, adds 2 enemies, multiplies speed by
            for (int k = 3; k < CurrentWave; k++)
            {
                tankBase *= 2; // double tank enemies
                enemyBase += 2;  // add 2 enemies
                speedMultiplier *= 1.15f; // increase speed by 15%
            }

            for (int i = 0; i < enemyBase; i++)
            {
                var enemy = new Enemy(_enemyTexture, new Vector2(_playableArea.Left + _playableArea.Width / 4 + (i % 3) * 100, centerY + (i / 3) * 80));
                enemy.speed *= speedMultiplier;
                enemies.Add(enemy);
            }

            for (int i = 0; i < tankBase; i++)
            {
                var tank = new TankEnemy(_tankTexture, new Vector2(_playableArea.Left + _playableArea.Width / 4 + (i % 3) * 100, centerY + (i / 3) * 80));
                tank.speed *= speedMultiplier;
                enemies.Add(tank);
            }

            return enemies;
        }
        
        // KC Note: last wave
        // No enemies, nust tank enemies
        // increase speed by 25%
        int finalTank = 2;
        float finalSpeed = 1.25f;
        
        for (int k = 3; k < CurrentWave; k++)
        {
            finalTank *= 2;
            finalSpeed *= 1.25f;
        }

        finalSpeed *= 1.25f; // increase x 25%

        for (int i = 0; i < finalTank; i++) // Jordan: redid math (debugged)
        {
            var tank = new TankEnemy(_tankTexture, new Vector2(centerX + (i% 4 -2) * 100, centerY + (i / 4) * 80));
            tank.speed *= finalSpeed;
            enemies.Add(tank);
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
