using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using PoIAna.scenes.autoload;

namespace PoIAna.scenes.ui;

public partial class SelectorMenu : TextureRect
{
    private List<ModelMeta> _modelMetas;
    private ModelMeta _selectedModel;
    private Button _selectedButton;

    private readonly Dictionary<Button, Tween> _buttonTweens = new();
    
    private void ScaleButton(Button button, float scale)
    {
        if (_buttonTweens.TryGetValue(button, out var previous)
            && previous.IsRunning())
        {
            previous.Kill();
        }

        button.PivotOffset = button.Size / 2.0f;

        Tween tween = button.CreateTween();

        _buttonTweens[button] = tween;

        tween.TweenProperty(
                button,
                "scale",
                Vector2.One * scale,
                0.10f
            ).SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
    }
    
    private void AddButtonJuice(Button button)
    {
        button.MouseEntered += () =>
            ScaleButton(button, button == _selectedButton ? 1.06f : 1.035f);

        button.MouseExited += () =>
            ScaleButton(button, button == _selectedButton ? 1.05f : 1.0f);

        button.ButtonDown += () =>
            ScaleButton(button, 0.97f);
    }

    private void UpdateModelDescription(ModelMeta modelMeta)
    {
        GetNode<Label>("%DisplayName").Text =
            modelMeta.DisplayName;

        GetNode<Label>("%Description").Text =
            modelMeta.Description;

        GetNode<RichTextLabel>("%WinRate").Text =
            modelMeta.IsSpecial
                ? Tr("WINRATE-UNTESTED")
                : String.Format(
                    Tr("WINRATE"),
                    modelMeta.WinRate * 100
                );
    }
    
    private void SelectModel(ModelMeta modelMeta, Button button)
    {
        if (_selectedButton != null &&
            _selectedButton != button)
        {
            ScaleButton(_selectedButton, 1.0f);
        }

        _selectedButton = button;
        ScaleButton(button, 1.05f);

        if (_selectedModel == null)
        {
            UpdateModelDescription(modelMeta);
        }
        else
        {
            var infoContainer = GetNode<Control>("%InfoContainer");
            Tween tween = infoContainer.CreateTween();
            tween.TweenProperty(
                infoContainer,
                "modulate:a",
                0.0f,
                0.08f
            );

            tween.TweenCallback(Callable.From(() => UpdateModelDescription(modelMeta)));

            tween.TweenProperty(
                infoContainer,
                "modulate:a",
                1.0f,
                0.14f
            );
        }
        

        GetNode<Button>("%Play").Show();

        _selectedModel = modelMeta;
    }
    
    private void ModulateStyleBox(Button button, string styleBoxName, Color color)
    {
        var normal = button.GetThemeStylebox(styleBoxName) as StyleBoxTexture;
        StyleBoxTexture copy = new StyleBoxTexture();
        AtlasTexture texture = new AtlasTexture();
        texture.Atlas = GD.Load<CompressedTexture2D>("res://assets/ui_sheet_red_minimal.png");
        texture.Region = ((AtlasTexture)normal!.Texture).Region;
        copy.Texture = texture;
        copy.TextureMarginBottom = normal.TextureMarginBottom;
        copy.TextureMarginTop = normal.TextureMarginTop;
        copy.TextureMarginLeft = normal.TextureMarginLeft;
        copy.TextureMarginRight = normal.TextureMarginRight;
        copy.RegionRect = normal.RegionRect;
        copy.ModulateColor = color;
        button.AddThemeStyleboxOverride(styleBoxName, copy);
    }
    
    public override void _Ready()
    {
        base._Ready();
        _modelMetas = GetNode<GameGlobals>("/root/GameGlobals").AllModels;
        
        foreach (var meta in _modelMetas)
        {
            meta.Description = Tr(meta.Name.ToUpper());
        }
        
        _modelMetas.Sort((meta1, meta2) => meta2.WinRate.CompareTo(meta1.WinRate));
        var container = GetNode<HFlowContainer>("%HFlowContainer");
        var playButton = GetNode<Button>("%Play");
        playButton.ButtonUp += Play;
        AddButtonJuice(playButton);
        
        container.AddChild(AddVotingStrategyButton(5));
        container.AddChild(AddVotingStrategyButton(10));
        container.AddChild(AddVotingStrategyButton(23));

        float min = _modelMetas.Select(meta => meta.WinRate).Min();
        float max = _modelMetas.Select(meta => meta.WinRate).Max();
        foreach (ModelMeta modelMeta in _modelMetas)
        {
            Button button = new Button();
            AddButtonJuice(button);
            button.Text = modelMeta.DisplayName;
            button.AddThemeFontSizeOverride("font_size", 24);
            container.AddChild(button);

            var color = Colors.Green.Lerp(Colors.MediumPurple, (modelMeta.WinRate - min) / (max - min));
            ModulateStyleBox(button, "normal", color);
            ModulateStyleBox(button, "focus", color);
            ModulateStyleBox(button, "pressed", color);
            ModulateStyleBox(button, "hover", color);
            
            button.ButtonUp += () => SelectModel(modelMeta, button);
        }
    }

    private Button AddVotingStrategyButton(int howmany)
    {
        var votingMeta = new ModelMeta("🗳️", "voting-"+howmany, 0f);
        votingMeta.Description = Tr(votingMeta.Name.ToUpper());
        Button moeButton = new Button();
        moeButton.Text = votingMeta.DisplayName;
        moeButton.AddThemeFontSizeOverride("font_size", 24);

        moeButton.ButtonUp += () =>
            SelectModel(votingMeta, moeButton);
        
        AddButtonJuice(moeButton);
        
        return moeButton;
    }

    private void Play()
    {
        var gameScene = GD.Load<PackedScene>("res://scenes/screens/Table.tscn");
        GetNode<GameGlobals>("/root/GameGlobals").ModelMeta = _selectedModel;
        GetTree().ChangeSceneToPacked(gameScene);
    }
}