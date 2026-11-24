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
        GetNode<Button>("%Play").ButtonUp += () =>
        {
            Play();
        };
        
        container.AddChild(AddVotingStrategyButton(5));
        container.AddChild(AddVotingStrategyButton(10));
        container.AddChild(AddVotingStrategyButton(23));

        float min = _modelMetas.Select(meta => meta.WinRate).Min();
        float max = _modelMetas.Select(meta => meta.WinRate).Max();
        foreach (ModelMeta modelMeta in _modelMetas)
        {
            Button button = new Button();
            button.Text = modelMeta.DisplayName;
            button.AddThemeFontSizeOverride("font_size", 24);
            container.AddChild(button);

            var color = Colors.Green.Lerp(Colors.MediumPurple, (modelMeta.WinRate - min) / (max - min));
            ModulateStyleBox(button, "normal", color);
            ModulateStyleBox(button, "focus", color);
            ModulateStyleBox(button, "pressed", color);
            ModulateStyleBox(button, "hover", color);
            
            button.ButtonUp += () =>
            {
                GetNode<Label>("%DisplayName").Text = modelMeta.DisplayName;
                GetNode<Label>("%Description").Text = modelMeta.Description;
                GetNode<RichTextLabel>("%WinRate").Text = String.Format(Tr("WINRATE"), modelMeta.WinRate * 100);
                GetNode<Button>("%Play").Show();
                _selectedModel = modelMeta;
            };
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
        {
            GetNode<Label>("%DisplayName").Text = votingMeta.DisplayName;
            GetNode<Label>("%Description").Text = votingMeta.Description;
            GetNode<RichTextLabel>("%WinRate").Text = Tr("WINRATE-UNTESTED");
            GetNode<Button>("%Play").Show();
            _selectedModel = votingMeta;
        };
        
        return moeButton;
    }

    private void Play()
    {
        var gameScene = GD.Load<PackedScene>("res://scenes/screens/Table.tscn");
        GetNode<GameGlobals>("/root/GameGlobals").ModelMeta = _selectedModel;
        GetTree().ChangeSceneToPacked(gameScene);
    }
}