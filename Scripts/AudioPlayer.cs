using Godot;
using System;

public partial class AudioPlayer : Node
{

    public static AudioPlayer Instance;

    public static Random rng;

    [Export]
    public Godot.Collections.Dictionary<String, AudioStream> sounds = new();

	public override void _Ready()
    {
        base._Ready();
        if (Instance == null) Instance = this;
        else if (Instance != this) QueueFree();
        rng = new Random();


        Play("music", true, 1, -1);
        
    }

    

    public static void Play(string sound, bool loop = false, float pitch = 1, float volume = 0)
    {
        AudioStream soundStream = (AudioStream)Instance.sounds[sound];
        var streamPlayer = new AudioStreamPlayer();
        Instance.AddChild(streamPlayer);
        streamPlayer.Stream = soundStream;
        streamPlayer.PitchScale = pitch;
        streamPlayer.VolumeDb = volume;
        streamPlayer.Play();

        if(loop) streamPlayer.Finished += () => streamPlayer.Play();
        else streamPlayer.Finished += () => streamPlayer.QueueFree();
    }

    public static void PlayRandomPitch(string sound, bool loop = false, float pitch = 1, float variation = 0.4f, float volume = 0)
    {
        float newPitch = pitch + (float)(((rng.NextDouble() * 2) - 1) * variation);
        Play(sound, loop, newPitch, volume);
    }

    
}
