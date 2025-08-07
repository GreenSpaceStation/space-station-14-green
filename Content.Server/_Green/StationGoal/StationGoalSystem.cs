using Content.Server.Fax;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.Fax.Components;
using Content.Shared.GameTicking;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Green.StationGoal;

public sealed class StationGoalSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly FaxSystem _fax = default!;

    public bool SendStationGoalOnRoundStart { get; set; }

    public override void Initialize()
    {
        SubscribeLocalEvent<StationDataComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestartCleanup);
    }

    private void OnMapInit(Entity<StationDataComponent> entity, ref MapInitEvent e)
    {
        List<StationGoalPrototype> goals = [];

        if (TryComp<StationGoalComponent>(entity, out var stationGoals))
            foreach (var goal in stationGoals.Goals)
                goals.Add(_prototype.Index(goal));
        else
            foreach (var goal in _prototype.EnumeratePrototypes<StationGoalPrototype>())
                if (goal.Implicit)
                    goals.Add(goal);

        SendStationGoal(entity, _random.Pick(goals));
    }

    private void OnRoundRestartCleanup(ref RoundRestartCleanupEvent e)
    {
        SendStationGoalOnRoundStart = true;
    }

    public void SendStationGoal(EntityUid station, StationGoalPrototype goal)
    {
        FaxPrintout printout = new("Test.", "test", null, null, "paper_stamp-centcom", [new() { StampedName = Loc.GetString("stamp-component-stamped-name-centcom"), StampedColor = Color.FromHex("#006600") }], true);

        var query = EntityQueryEnumerator<FaxMachineComponent>();
        while (query.MoveNext(out var entity, out var fax))
        {
            if (!fax.ReceiveAllStationGoals && !(fax.ReceiveStationGoal && _station.GetOwningStation(entity) == station))
                continue;

            _fax.Receive(entity, printout, component: fax);
        }
    }
}
