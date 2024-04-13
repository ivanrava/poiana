using System.Collections.Generic;
using Godot;

namespace PoIAna.scenes.ui;

public partial class Selector : PanelContainer
{
    private Button _previous;
    private Button _next;
    private Label _name;
    private Label _description;
    private int _idxOfSelected;
    private readonly List<ModelMeta> _modelMetas = new()
    {
        new ModelMeta("🤳🏻", "best_model", "DQN self-play", 0f),
        new ModelMeta("🌊", "amber-lake", "Long training with a sparse reward.", 0.45f),
        new ModelMeta("🌲", "toasty-pine", "Harsher penalties and higher bias.", 0.0f),
        new ModelMeta("🐦", "blooming-bird", "Best model with memory", 0.60f),
        new ModelMeta("🧨", "cosmic-firebrand", "Worst of the best. Quick training with fast stiffness and lr = 1e-4.", 0.25f),
        new ModelMeta("🥗", "dark-salad", "Long training with a bigger lr (3e-3)", 0.40f),
        new ModelMeta("🌃", "earnest-night", "Long training with 0.6 exploration rate. First decent model.", 0.41f),
        new ModelMeta("🕳️", "graceful-darkness", "Best model with penalties. Trained with all rewards activated.", 0.59f),
        new ModelMeta("☄️", "hardy-galaxy", "Smaller learning rate (1e-4) and gamma (0.9)", 0.30f),
        new ModelMeta("🎣", "laced-pond", "All rewards against the rules based agent.", 0f),
        new ModelMeta("🏔️", "rich-mountain", "Further training upon earnest-night, with fast exploration decay.", 0.41f),
        new ModelMeta("😌", "skilled-serenity", "Short training with little exploration.", 0.35f),
        new ModelMeta("🐲", "smart-dragon", "Long training with dense and sparse rewards. Update / 4 episodes.", 0.42f),
        new ModelMeta("🌨️", "snowy-shape", "Standard training, with penalties for suboptimal actions.", 0.58f),
        new ModelMeta("❄️", "spring-snowflake", "Further refinement of earnest-night, with a 256:256 ReLU network.", 0.40f),
        new ModelMeta("🌟", "true-star", "Best model so far, but no memory.", 0.58f),
        new ModelMeta("🛶", "warm-river", "Standard refinement upon an already trained model.", 0.40f),
    };

    private ProgressBar _winRate;

    public override void _Ready()
    {
        base._Ready();
        _previous = GetNode<Button>("%Previous");
        _next = GetNode<Button>("%Next");
        _name = GetNode<Label>("%Name");
        _description = GetNode<Label>("%Description");
        _winRate = GetNode<ProgressBar>("%WinRate");
        UpdateLabels(_modelMetas[_idxOfSelected]);
        _previous.Pressed += () => UpdateLabels(_modelMetas[--_idxOfSelected % _modelMetas.Count]);
        _next.Pressed += () => UpdateLabels(_modelMetas[++_idxOfSelected % _modelMetas.Count]);
    }

    private void UpdateLabels(ModelMeta selection)
    {
        _name.Text = selection.DisplayName;
        _description.Text = selection.Description;
        _winRate.Value = selection.WinRate * 100;

    }

    public ModelMeta SelectedModel()
    {
        return _modelMetas[_idxOfSelected % _modelMetas.Count];
    }
}

public record ModelMeta(string Icon, string Name, string Description, float WinRate)
{
    public string Filename => Name + ".onnx";
    public string DisplayName => Icon + " " + Name;
}