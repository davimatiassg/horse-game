using Godot;
using System;

public partial class Clock : RichTextLabel
{
    // Total elapsed time (seconds)
    private float _totalTime = 0f;

    // Interval accumulator
    private float _intervalTimer = 0f;

    [Export]
    public float IntervalSeconds = 30f;

    // Optional event
    public event Action OnIntervalReached;

    public override void _Process(double delta)
    {
        float deltaTime = (float)delta;

        _totalTime += deltaTime;
        _intervalTimer += deltaTime;

        // Update label text
        Text = FormatTime(_totalTime);

        // Check interval
        if (_intervalTimer >= IntervalSeconds)
        {
            _intervalTimer -= IntervalSeconds;
            HandleIntervalReached();
        }
    }

    private void HandleIntervalReached()
    {
		EnemyManager.Singleton.difficulty += 3;
        OnIntervalReached?.Invoke();
    }

    private string FormatTime(float timeInSeconds)
    {

        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);

        return $"{minutes:00}:{seconds:00}";
    }
}