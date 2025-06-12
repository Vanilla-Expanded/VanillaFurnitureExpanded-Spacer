using RimWorld;
using Verse;

namespace MFSpacer;

public class Building_RepairStored : Building_Storage
{
    private CompRepairStored repairComp;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);

        repairComp = GetComp<CompRepairStored>();
        if (repairComp == null)
            Log.Error($"{nameof(Building_RepairStored)} requires {nameof(CompRepairStored)} to work. {nameof(Building_RepairStored)} is also fully optional and only exists to notify the comp that an item was added/removed to/from storage.");
    }

    public override void Notify_ReceivedThing(Thing newItem)
    {
        base.Notify_ReceivedThing(newItem);

        repairComp.Notify_ReceivedThing(newItem);
    }

    public override void Notify_LostThing(Thing newItem)
    {
        base.Notify_LostThing(newItem);

        repairComp.Notify_LostThing(newItem);
    }
}