using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using PoIAna.scenes.autoload;
using PoIAna.scenes.cards;
using PoIAna.scenes.ui;

public partial class SelectorMenu : TextureRect
{
    private readonly List<ModelMeta> _modelMetas = new()
    {
        new ModelMeta("🤳🏻", "selfplay-best", "DQN self-play", 0.568933f),
        new ModelMeta("🌊", "amber-lake", "Long training with a sparse reward.", 0.578433f),
        new ModelMeta("🍂", "autumn-night", "Very long training against the greedy agent.", 0.680767f),
        new ModelMeta("🦦", "mild-aaldvark", "Only the sparse reward. Greedy agent and quick training.", 0.653067f),
        new ModelMeta("💫", "lively-cosmos", "Only the sparse reward, with a 256:256 ReLU network.", 0.633833f),
        new ModelMeta("🍃", "bumbling-leaf", "Only the sparse reward, with a 256:256 ReLU network.", 0.615367f),
        new ModelMeta("🪨", "easy-shape", "Only sparse reward and a 256:256 ReLU network. Slow annealing.", 0.612867f),
        new ModelMeta("🌲", "toasty-pine", "Long training with harsher penalties. Pretty hard, less errors.", 0.707933f),
        new ModelMeta("🐦", "blooming-bird", "Best model with memory", 0.682033f),
        new ModelMeta("📄", "devout-paper", "Harsher penalties and quick training.", 0.6811f),
        new ModelMeta("🧨", "cosmic-firebrand", "Worst of the best. Quick training with fast stiffness and lr = 1e-4.", 0.450967f),
        new ModelMeta("🥗", "dark-salad", "Long training with a bigger lr (3e-3)", 0.572400f),
        new ModelMeta("🌃", "earnest-night", "Long training with 0.6 exploration annealing. First good model.", 0.588133f),
        new ModelMeta("🕳️", "graceful-darkness", "Best model with penalties. Trained with all rewards activated.", 0.681433f),
        new ModelMeta("☄️", "hardy-galaxy", "Smaller learning rate (1e-4) and gamma (0.9)", 0.482300f),
        new ModelMeta("🎣", "laced-pond", "All rewards against the rules based agent.", 0.676333f),
        new ModelMeta("🏔️", "rich-mountain", "Further training upon earnest-night, with fast exploration decay.", 0.588133f),
        new ModelMeta("😌", "skilled-serenity", "Short training with little exploration.", 0.538800f),
        new ModelMeta("🐲", "smart-dragon", "Long training with dense and sparse rewards. Update / 4 episodes.", 0.532100f),
        new ModelMeta("🌨️", "snowy-shape", "Standard training, with penalties for suboptimal actions.", 0.650300f),
        new ModelMeta("❄️", "spring-snowflake", "Further refinement of earnest-night, with a 256:256 ReLU network.", 0.575500f),
        new ModelMeta("🌟", "true-star", "Second good model obtained. Strong in the second move.", 0.680267f),
        new ModelMeta("🛶", "warm-river", "Standard refinement upon an already trained model.", 0.576833f),
    };
    private ModelMeta _selectedModel;
    
    private void ModulateStyleBox(Button button, string styleBoxName, Color color)
    {
        var normal = button.GetThemeStylebox(styleBoxName) as StyleBoxTexture;
        StyleBoxTexture copy = new StyleBoxTexture();
        copy.Texture = normal!.Texture;
        copy.TextureMarginBottom = normal.TextureMarginBottom;
        copy.TextureMarginTop = normal.TextureMarginTop;
        copy.TextureMarginLeft = normal.TextureMarginLeft;
        copy.TextureMarginRight = normal.TextureMarginRight;
        copy.ModulateColor = color;
        copy.RegionRect = normal.RegionRect;
        button.AddThemeStyleboxOverride(styleBoxName, copy);
    }
    
    public override void _Ready()
    {
        base._Ready();
        _modelMetas.Sort((meta1, meta2) => meta2.WinRate.CompareTo(meta1.WinRate));
        var container = GetNode<HFlowContainer>("%HFlowContainer");
        GetNode<Button>("%Back").ButtonUp += () =>
        {
            var gameScene = GD.Load<PackedScene>("res://scenes/ui/MainMenu.tscn");
            GetTree().ChangeSceneToPacked(gameScene);
        };
        GetNode<Button>("%Play").ButtonUp += () =>
        {
            Play();
        };

        float min = _modelMetas.Select(meta => meta.WinRate).Min();
        float max = _modelMetas.Select(meta => meta.WinRate).Max();
        foreach (ModelMeta modelMeta in _modelMetas)
        {
            Button button = new Button();
            button.Text = modelMeta.DisplayName;
            button.AddThemeFontSizeOverride("font_size", 24);
            container.AddChild(button);

            var color = Colors.Purple.Lerp(Colors.Green, (modelMeta.WinRate - min) / (max - min));
            ModulateStyleBox(button, "normal", color);
            ModulateStyleBox(button, "focus", color);
            ModulateStyleBox(button, "pressed", color);
            ModulateStyleBox(button, "hover", color);
            
            button.ButtonUp += () =>
            {
                GetNode<Label>("%DisplayName").Text = modelMeta.DisplayName;
                GetNode<Label>("%Description").Text = modelMeta.Description;
                GetNode<RichTextLabel>("%WinRate").Text = $"[center]This model has an averaged [b]{modelMeta.WinRate * 100}%[/b] win rate against easy to hard heuristic agents.[/center]";
                GetNode<Button>("%Play").Show();
                _selectedModel = modelMeta;
            };
        }
    }

    private void Play()
    {
        var gameScene = GD.Load<PackedScene>("res://scenes/screens/Table.tscn");
        GetNode<GameGlobals>("/root/GameGlobals").ModelMeta = _selectedModel;
        GetTree().ChangeSceneToPacked(gameScene);
    }
}
