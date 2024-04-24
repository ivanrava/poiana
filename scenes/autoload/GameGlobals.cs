using Godot;
using PoIAna.scenes.cards;
using PoIAna.scenes.ui;

namespace PoIAna.scenes.autoload;

public partial class GameGlobals : Node
{
    public CardData Briscola;
    public ModelMeta ModelMeta;

    private bool _isAudioEnabled;
    public bool IsAudioEnabled
    {
        get => _isAudioEnabled;
        set
        {
            _isAudioEnabled = value;
            SaveConfig();
        }
    }

    private bool _isFatalityEnabled;

    public bool IsFatalityEnabled
    {
        get => _isFatalityEnabled;
        set
        {
            _isFatalityEnabled = value;
            SaveConfig();
        }
    }
    private bool _fullscreen;

    private ConfigFile _configFile;
    private string _configLocation = "user://settings.cfg";

    public override void _Ready()
    {
        LoadConfig();
        SetWindowMode();
    }

    private void LoadConfig()
    {
        _configFile = new ConfigFile();
        Error err = _configFile.Load(_configLocation);
        if (err != Error.Ok)
        {
            return;
        }

        _isFatalityEnabled = (bool)_configFile.GetValue("options", "fatality", true);
        _isAudioEnabled    = (bool)_configFile.GetValue("options", "audio", true);
        _fullscreen       = (bool)_configFile.GetValue("graphics", "fullscreen", true);
    }

    private void SaveConfig()
    {
        _configFile.Clear();
        _configFile.SetValue("options", "fatality", IsFatalityEnabled);
        _configFile.SetValue("options", "audio", IsAudioEnabled);
        _configFile.SetValue("graphics", "fullscreen", _fullscreen);

        _configFile.Save(_configLocation);
    }

    private void SetWindowMode()
    {
        DisplayServer.WindowSetMode(_fullscreen
            ? DisplayServer.WindowMode.Fullscreen
            : DisplayServer.WindowMode.Windowed);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!@event.IsAction("toggle_fullscreen") || !@event.IsPressed()) return;
        
        _fullscreen = !_fullscreen;
        SetWindowMode();
        SaveConfig();
        GetViewport().SetInputAsHandled();
    }
}