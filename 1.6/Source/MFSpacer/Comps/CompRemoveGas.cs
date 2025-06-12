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
        if (parent.Spawned && (powerTrader == null || powerTrader.PowerOn) && (!Props.requiresIndoors || !parent.IsOutside()))
        {
            uint[] gasGrid = null;
            object[] parallelGasGrids = null;

            if (PerformanceFishCompatibility.IsPerformanceFishActive)
                parallelGasGrids = (object[])PerformanceFishCompatibility.ParallelGasGridsMethod(null, parent.Map.gasGrid);
            else
                gasGrid = GasDensityField(parent.Map.gasGrid);

            foreach (var pos in parent.OccupiedRect())
            {
                var cellIndex = CellIndicesUtility.CellToIndex(parent.Position, parent.Map.Size.x);

                // PerformanceFish replaces vanilla gas system, so we need to use it to clear the vanilla gases.
                // On top of that, this will work with non-vanilla gases.
                if (parallelGasGrids != null)
                {
                    if (Props.removeAllGasDefGases)
                    {
                        for (var i = 0; i < parallelGasGrids.Length; i++)
                            PerformanceFishCompatibility.SetDirectMethod(parallelGasGrids[i], cellIndex, (byte)0);
                    }
                    else if (Props.gasDefGasesToRemove != null)
                    {
                        for (var i = 0; i < Props.gasDefGasesToRemove.Count; i++)
                            PerformanceFishCompatibility.SetDirectMethod(parallelGasGrids[Props.gasDefGasesToRemove[i].index], cellIndex, (byte)0);
                    }
                }
                else if (gasGrid != null && gasGrid[cellIndex] > 0u)
                {
                    gasGrid[cellIndex] &= Props.Mask;
                    parent.Map.mapDrawer.MapMeshDirty(pos, MapMeshFlagDefOf.Gas);
                }

                if (Props.removeAllThingDefGases)
                {
                    pos.GetGas(parent.Map)?.DeSpawn();
                }
                else if (Props.thingDefGasesToRemove != null)
                {
                    var gas = pos.GetGas(parent.Map);
                    if (gas != null && Props.thingDefGasesToRemove.Contains(gas.def))
                        gas.DeSpawn();
                }
            }
        }
    }
}