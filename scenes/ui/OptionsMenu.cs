using Godot;
using System;
using PoIAna.scenes.autoload;

public partial class OptionsMenu : TextureRect
{
    private CheckButton _fatality;
    private CheckButton _audioEffects;
    private GameGlobals _autoload;
    
    public override void _Ready()
    {
        base._Ready();
        _fatality = GetNode<CheckButton>("%EnableFatality");
        _audioEffects = GetNode<CheckButton>("%EnableAudio");
        _autoload = GetNode<GameGlobals>("/root/GameGlobals");

        _fatality.ButtonPressed = _autoload.IsFatalityEnabled;
        _audioEffects.ButtonPressed = _autoload.IsAudioEnabled;

        _fatality.Toggled += on => _autoload.IsFatalityEnabled = on;
        _audioEffects.Toggled += on => _autoload.IsAudioEnabled = on;
    }
}
