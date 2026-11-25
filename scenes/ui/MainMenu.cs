using System.Collections.Generic;
using Godot;
using PoIAna.scenes.autoload;

namespace PoIAna.scenes.ui;

public partial class MainMenu : TextureRect
{
    private record Locale(string Language, string Code);
    
    private readonly List<Locale> _locales = new()
    {
        new Locale("ENGLISH", "en_US"),
        new Locale("ITALIAN", "it_IT")
    };

    private GameGlobals _gameGlobals;
    
    public override void _Ready()
    {
        _gameGlobals = GetNode<GameGlobals>("/root/GameGlobals");
        
        var playButton = GetNode<Button>("%Play");
        AddButtonFeedback(playButton);
        playButton.ButtonUp += () =>
        {
            var gameScene = GD.Load<PackedScene>("res://scenes/ui/SelectorMenu.tscn");
            GetTree().ChangeSceneToPacked(gameScene);
        };
        var rulesButton = GetNode<Button>("%Rules");
        AddButtonFeedback(rulesButton);
        rulesButton.ButtonUp += () =>
        {
            var gameScene = GD.Load<PackedScene>("res://scenes/ui/Rules.tscn");
            GetTree().ChangeSceneToPacked(gameScene);
        };
        var optionsButton = GetNode<Button>("%Options");
        AddButtonFeedback(optionsButton);
        optionsButton.ButtonUp += () =>
        {
            var optionsScene = GD.Load<PackedScene>("res://scenes/ui/OptionsMenu.tscn");
            GetTree().ChangeSceneToPacked(optionsScene);
        };
        var quitButton = GetNode<Button>("%Quit");
        AddButtonFeedback(quitButton);
        quitButton.ButtonUp += () =>
        {
            GetTree().Quit();
        };
        
        var languages = GetNode<OptionButton>("%Language");
        SetLanguages(languages);
        languages.ItemSelected += indexPressed =>
        {
            var localeCode = _locales[(int)indexPressed].Code;
            TranslationServer.SetLocale(localeCode);
            _gameGlobals.SaveConfig();
        };
    }
    
    private void AddButtonFeedback(Button button)
    {
        button.MouseEntered += () =>
        {
            button.PivotOffset = button.Size / 2.0f;

            Tween tween = button.CreateTween();

            tween.TweenProperty(
                    button,
                    "scale",
                    Vector2.One * 1.04f,
                    0.12f
                ).SetTrans(Tween.TransitionType.Back)
                .SetEase(Tween.EaseType.Out);
        };

        button.MouseExited += () =>
        {
            Tween tween = button.CreateTween();

            tween.TweenProperty(
                button,
                "scale",
                Vector2.One,
                0.12f
            );
        };
    }

    private void SetLanguages(OptionButton languagesOptionButton)
    {
        var selectedIndex = -1;
        for (var i = 0; i < _locales.Count; i++)
        {
            var locale = _locales[i];
            languagesOptionButton.AddItem(locale.Language, i);
            languagesOptionButton.SetItemIcon(i, GD.Load<Texture2D>($"res://assets/locales/{locale.Code}.png"));
            if (TranslationServer.GetLocale() == locale.Code)
            {
                selectedIndex = i;
            }
        }
        languagesOptionButton.Selected = selectedIndex;
    }
}