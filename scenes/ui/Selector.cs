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

    private ProgressBar _winRate;

    public override void _Ready()
    {
        base._Ready();
        _previous = GetNode<Button>("%Previous");
        _next = GetNode<Button>("%Next");
        _name = GetNode<Label>("%Name");
        _description = GetNode<Label>("%Description");
        _winRate = GetNode<ProgressBar>("%WinRate");
        
        _modelMetas.Sort((meta1, meta2) => meta1.WinRate.CompareTo(meta2.WinRate));
        
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