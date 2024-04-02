using System.Collections.Generic;
using Godot;

namespace PoIAna.scenes.ui;

public partial class Selector : HBoxContainer
{
    private Button _previous;
    private Button _next;
    private Label _name;
    private Label _description;
    private int _idxOfSelected;
    private readonly List<ModelMeta> _modelMetas = new()
    {
        new ModelMeta("🌊", "amber-lake", "Long training with a sparse reward."),
        new ModelMeta("🐦", "blooming-bird", "Best model with memory"),
        new ModelMeta("🧨", "cosmic-firebrand", "Worst of the best. Quick training with fast stiffness and lr = 1e-4."),
        new ModelMeta("🥗", "dark-salad", "Long training with a bigger lr (3e-3)"),
        new ModelMeta("🌃", "earnest-night", "Long training with 0.6 exploration rate. First decent model."),
        new ModelMeta("🕳️", "graceful-darkness", "Best model with penalties. Trained with all rewards activated."),
        new ModelMeta("☄️", "hardy-galaxy", "Smaller learning rate (1e-4) and gamma (0.9)"),
        new ModelMeta("🏔️", "rich-mountain", "Further training upon earnest-night, with fast exploration decay."),
        new ModelMeta("😌", "skilled-serenity", "Short training with little exploration."),
        new ModelMeta("🐲", "smart-dragon", "Long training with both dense and sparse rewards. Update every 4 episodes."),
        new ModelMeta("🌨️", "snowy-shape", "Standard training, with penalties for suboptimal actions."),
        new ModelMeta("❄️", "spring-snowflake", "Further refinement of earnest-night, with a 256:256 ReLU network."),
        new ModelMeta("🌟", "true-star", "Best model so far, but no memory."),
        new ModelMeta("🛶", "warm-river", "Standard refinement upon an already trained model."),
    };
    
    public override void _Ready()
    {
        base._Ready();
        _previous = GetNode<Button>("Previous");
        _next = GetNode<Button>("Next");
        _name = GetNode<Label>("%Name");
        _description = GetNode<Label>("%Description");
        UpdateLabels(_modelMetas[_idxOfSelected]);
        _previous.Pressed += () => UpdateLabels(_modelMetas[--_idxOfSelected % _modelMetas.Count]);
        _next.Pressed += () => UpdateLabels(_modelMetas[++_idxOfSelected % _modelMetas.Count]);
    }

    private void UpdateLabels(ModelMeta selection)
    {
        _name.Text = selection.DisplayName;
        _description.Text = selection.Description;
    }

    public ModelMeta SelectedModel()
    {
        return _modelMetas[_idxOfSelected % _modelMetas.Count];
    }
}

public record ModelMeta(string Icon, string Name, string Description)
{
    public string Filename => Name + ".onnx";
    public string DisplayName => Icon + " " + Name;
}