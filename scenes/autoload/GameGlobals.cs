using System.Collections.Generic;
using Godot;
using PoIAna.scenes.cards;
using PoIAna.scenes.ui;

namespace PoIAna.scenes.autoload;

public partial class GameGlobals : Node
{
    public CardData Briscola;
    public ModelMeta ModelMeta;
    public List<ModelMeta> AllModels;

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
        AllModels =
        [
            new ModelMeta("🤳🏻", "selfplay-best", 0.568933f),
            new ModelMeta("🌊", "amber-lake", 0.578433f),
            new ModelMeta("🍂", "autumn-night", 0.680767f),
            new ModelMeta("🦦", "mild-aardvark", 0.653067f),
            new ModelMeta("💫", "lively-cosmos", 0.633833f),
            new ModelMeta("🍃", "bumbling-leaf", 0.615367f),
            new ModelMeta("🪨", "easy-shape", 0.612867f),
            new ModelMeta("🌲", "toasty-pine", 0.680800f),
            new ModelMeta("🐦", "blooming-bird", 0.682033f),
            new ModelMeta("📄", "devout-paper", 0.656833f),
            new ModelMeta("🧨", "cosmic-firebrand", 0.450967f),
            new ModelMeta("🥗", "dark-salad", 0.572400f),
            new ModelMeta("🌃", "earnest-night", 0.588133f),
            new ModelMeta("🕳️", "graceful-darkness", 0.681433f),
            new ModelMeta("☄️", "hardy-galaxy", 0.482300f),
            new ModelMeta("🎣", "laced-pond", 0.676333f),
            new ModelMeta("🏔️", "rich-mountain", 0.588133f),
            new ModelMeta("😌", "skilled-serenity", 0.538800f),
            new ModelMeta("🐲", "smart-dragon", 0.532100f),
            new ModelMeta("🌨️", "snowy-shape", 0.650300f),
            new ModelMeta("❄️", "spring-snowflake", 0.575500f),
            new ModelMeta("🌟", "true-star", 0.680267f),
            new ModelMeta("🛶", "warm-river", 0.576833f)
        ];
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
        
        TranslationServer.SetLocale((string)_configFile.GetValue("options", "locale", "en_US"));
    }

    internal void SaveConfig()
    {
        _configFile.Clear();
        _configFile.SetValue("options", "fatality", IsFatalityEnabled);
        _configFile.SetValue("options", "audio", IsAudioEnabled);
        _configFile.SetValue("options", "locale", TranslationServer.GetLocale());
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