using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MFSpacer;

public class CompProperties_RemoveGas : CompProperties
{
    public bool requiresIndoors = false;
    public List<GasType> gasTypesToRemove = null;

    public bool removeAllThingDefGases = false;
    public List<ThingDef> gasThingDefsToRemove = null;

    public uint Mask { get; protected set; } = uint.MaxValue;

    public CompProperties_RemoveGas() => compClass = typeof(CompRemoveGas);

    public override void ResolveReferences(ThingDef parentDef)
    {
        base.ResolveReferences(parentDef);

        if (Mask == uint.MaxValue && !gasTypesToRemove.NullOrEmpty())
        {
            var mask = 0u;

            // Set all the bytes corresponding to a specific gas to 1's
            for (var i = 0; i < gasTypesToRemove.Count; i++)
                mask |= (uint)byte.MaxValue << (short)gasTypesToRemove[i];

            // Invert the mask, since we'll want to set those specific bits to 0 to clear the gas
            Mask = ~mask;
        }
    }

    public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
    {
        foreach (var error in base.ConfigErrors(parentDef))
            yield return error;

        if (!gasTypesToRemove.NullOrEmpty())
        {
            var distinct = gasTypesToRemove.Distinct().ToList();
            if (distinct.Count != gasTypesToRemove.Count)
            {
                gasTypesToRemove = distinct;
                distinct.TrimExcess();
                yield return $"{parentDef.defName} has {nameof(CompProperties_RemoveGas)} with {gasTypesToRemove} that has repeated gas types.";
            }
        }

        if (!gasThingDefsToRemove.NullOrEmpty())
        {
            if (removeAllThingDefGases)
            {
                yield return $"{parentDef.defName} has {nameof(CompProperties_RemoveGas)} with {nameof(removeAllThingDefGases)} set to true and non-empty {nameof(gasThingDefsToRemove)}, only one of those should be used.";
            }

            for (var i = gasThingDefsToRemove.Count - 1; i >= 0; i--)
            {
                var gasDef = gasThingDefsToRemove[i];
                if (gasDef == null)
                {
                    gasThingDefsToRemove.RemoveAt(i);
                    yield return $"{parentDef.defName} has {nameof(CompProperties_RemoveGas)} with {nameof(gasThingDefsToRemove)} that has an empty/null entry.";
                }
                else if (gasDef.category != ThingCategory.Gas)
                {
                    gasThingDefsToRemove.RemoveAt(i);
                    yield return $"{parentDef.defName} has {nameof(CompProperties_RemoveGas)} with {nameof(gasThingDefsToRemove)} that contains a non-gas element ({nameof(ThingDef)}.{nameof(ThingDef.category)} is {nameof(gasDef.category)}, expected {ThingCategory.Gas}).";
                }
            }
        }
    }
}