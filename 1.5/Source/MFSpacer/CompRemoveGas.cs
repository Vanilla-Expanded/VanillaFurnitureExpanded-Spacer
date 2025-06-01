using HarmonyLib;
using RimWorld;
using Verse;

namespace MFSpacer;

public class CompRemoveGas : ThingComp
{
    protected static readonly AccessTools.FieldRef<GasGrid, uint[]> GasDensityField = AccessTools.FieldRefAccess<uint[]>(typeof(GasGrid), "gasDensity");

    private CompPowerTrader powerTrader;

    public CompProperties_RemoveGas Props => (CompProperties_RemoveGas)props;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        powerTrader = parent.GetComp<CompPowerTrader>();
    }

    public override void CompTick()
    {
        if (parent.Spawned && (powerTrader == null || powerTrader.PowerOn))
        {
            var gasGrid = GasDensityField(parent.Map.gasGrid);
            foreach (var pos in parent.OccupiedRect())
            {
                if (gasGrid[CellIndicesUtility.CellToIndex(parent.Position, parent.Map.Size.x)] > 0u)
                {
                    gasGrid[CellIndicesUtility.CellToIndex(parent.Position, parent.Map.Size.x)] &= Props.Mask;
                    parent.Map.mapDrawer.MapMeshDirty(pos, MapMeshFlagDefOf.Gas);
                }
            }
        }
    }
}