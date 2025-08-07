using Robust.Shared.Prototypes;

namespace Content.Server._Green.StationGoal;

[Prototype]
public sealed partial class StationGoalPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public string? Text;

    [DataField]
    public bool Implicit;
}
