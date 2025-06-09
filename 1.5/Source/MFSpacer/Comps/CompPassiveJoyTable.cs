using RimWorld;
using Verse;

namespace MFSpacer;

public class CompPassiveJoyTable : ThingComp
{
    private const int JoyTickInterval = 10;

    private CompPowerTrader powerTrader;

    public override void CompTick()
    {
        if (parent.IsHashIntervalTick(JoyTickInterval) && (powerTrader == null || powerTrader.PowerOn))
            HandlePawnJoy();
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        powerTrader = parent.GetComp<CompPowerTrader>();
    }

    public void HandlePawnJoy()
    {
        if (parent.def.size is { x: 1, z: 1 })
        {
            // Using this method for a 1x1 building is faster than GenAdjFast.AdjacentCellsCardinal
            for (var index = 0; index < 4; index++)
                TryGivePawnJoy((parent.Position + GenAdj.CardinalDirections[index]).GetFirstPawn(parent.Map));
        }
        else
        {
            // This method is about as fast (or even very slightly faster) than caching offsets for a 2x2 building
            var cells = GenAdjFast.AdjacentCellsCardinal(parent);
            for (var index = 0; index < cells.Count; index++)
                TryGivePawnJoy(cells[index].GetFirstPawn(parent.Map));
        }
    }

    private void TryGivePawnJoy(Pawn pawn)
    {
        if (pawn is { health: not null, needs.joy: not null } && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
            pawn.needs.joy.GainJoy(parent.GetStatValue(StatDefOf.JoyGainFactor) * (JoyTickInterval * JoyTunings.BaseJoyGainPerHour / GenDate.TicksPerHour), parent.def.building.joyKind);
    }
}