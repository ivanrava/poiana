using Godot;
using PoIAna.scenes.cards;
using PoIAna.scenes.ui;

namespace PoIAna.scenes.autoload;

public partial class GameGlobals : Node
{
    public CardData Briscola;
    public ModelMeta ModelMeta;
    public bool IsAudioEnabled = true;
    public bool IsFatalityEnabled = true;
    private bool _fullscreen = true;

    private void SetWindowMode()
    {
        DisplayServer.WindowSetMode(_fullscreen
            ? DisplayServer.WindowMode.Fullscreen
            : DisplayServer.WindowMode.Windowed);
    }

    public override void _Ready()
    {
        SetWindowMode();
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!@event.IsAction("toggle_fullscreen") || !@event.IsPressed()) return;
        
        _fullscreen = !_fullscreen;
        SetWindowMode();
        GetViewport().SetInputAsHandled();
    }
}