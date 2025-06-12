using System.Collections.Generic;
using Verse;

namespace MFSpacer;

public class CompProperties_RepairStored : CompProperties
{
    public EffecterDef effecterDef;

    public int repairAmount = 1;
    public int repairInterval = 2500;
    
    public CompProperties_RepairStored() => compClass = typeof(CompRepairStored);

    public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
    {
        foreach (var error in base.ConfigErrors(parentDef))
            yield return error;

        if (repairAmount <= 0)
        {
            yield return $"{nameof(repairAmount)} must be more than 0, but it is {repairAmount}. Setting it to 1.";
            repairAmount = 1;
        }

        if (repairInterval <= 0)
        {
            yield return $"{nameof(repairInterval)} must be more than 0, but it is {repairInterval}. Setting it to 2500.";
            repairInterval = 2500;
        }
    }
}