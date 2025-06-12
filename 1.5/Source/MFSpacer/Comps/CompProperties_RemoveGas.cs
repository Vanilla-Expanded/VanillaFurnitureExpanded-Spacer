using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MFSpacer;

public class CompProperties_RemoveGas : CompProperties
{
    public bool requiresIndoors = false;
    public List<GasType> gasTypesToRemove = null;

    public bool removeAllThingDefGases = false;
    public List<ThingDef> thingDefGasesToRemove = null;

    // Performance Fish
    public bool removeAllGasDefGases = false;
    public List<Def> gasDefGasesToRemove = null;

    public uint Mask { get; protected set; } = uint.MaxValue;

    public CompProperties_RemoveGas() => compClass = typeof(CompRemoveGas);

    public override void ResolveReferences(ThingDef parentDef)
    {
        base.ResolveReferences(parentDef);

        // If Performance Fish is active, convert gases from gasTypesToRemove list
        // to gasDefGasesToRemove, unless removeAllGasDefGases is true.
        if (PerformanceFishCompatibility.IsPerformanceFishActive)
        {
            if (!removeAllGasDefGases && !gasTypesToRemove.NullOrEmpty())
            {
                gasDefGasesToRemove ??= [];

                for (var i = 0; i < gasTypesToRemove.Count; i++)
                {
                    if (PerformanceFishCompatibility.OfGasTypeMethod(null, gasTypesToRemove[i]) is Def def)
                        gasDefGasesToRemove.AddDistinct(def);
                }
            }
        }
        else if (Mask == uint.MaxValue && !gasTypesToRemove.NullOrEmpty())
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

        if (!thingDefGasesToRemove.NullOrEmpty())
        {
            if (removeAllThingDefGases)
                yield return $"{parentDef.defName} has {nameof(CompProperties_RemoveGas)} with {nameof(removeAllThingDefGases)} set to true and non-empty {nameof(thingDefGasesToRemove)}, only one of those should be used.";

            for (var i = thingDefGasesToRemove.Count - 1; i >= 0; i--)
            {
                var gasDef = thingDefGasesToRemove[i];
                if (gasDef == null)
                {
                    thingDefGasesToRemove.RemoveAt(i);
                    yield return $"{parentDef.defName} has {nameof(CompProperties_RemoveGas)} with {nameof(thingDefGasesToRemove)} that has an empty/null entry.";
                }
                else if (gasDef.category != ThingCategory.Gas)
                {
                    thingDefGasesToRemove.RemoveAt(i);
                    yield return $"{parentDef.defName} has {nameof(CompProperties_RemoveGas)} with {nameof(thingDefGasesToRemove)} that contains a non-gas element ({nameof(ThingDef)}.{nameof(ThingDef.category)} is {nameof(gasDef.category)}, expected {ThingCategory.Gas}).";
                }
            }
        }

        if (PerformanceFishCompatibility.IsPerformanceFishActive && !gasDefGasesToRemove.NullOrEmpty())
        {
            if (removeAllGasDefGases)
                yield return $"{parentDef.defName} has {nameof(CompProperties_RemoveGas)} with {nameof(removeAllGasDefGases)} set to true and non-empty {nameof(gasDefGasesToRemove)}, only one of those should be used.";

            for (var i = gasDefGasesToRemove.Count - 1; i >= 0; i--)
            {
                var gasDef = gasDefGasesToRemove[i];
                if (gasDef == null)
                {
                    gasDefGasesToRemove.RemoveAt(i);
                    yield return $"{parentDef.defName} has {nameof(CompProperties_RemoveGas)} with {nameof(gasDefGasesToRemove)} that has an empty/null entry.";
                }
                else if (!PerformanceFishCompatibility.GasDefType.IsInstanceOfType(gasDef))
                {
                    gasDefGasesToRemove.RemoveAt(i);
                    yield return $"{parentDef.defName} has {nameof(CompProperties_RemoveGas)} with {nameof(gasDefGasesToRemove)} that contains a non-gas element (provided Def is not a subtype of Performance Fish GasDef, it is {gasDef.GetType()}).";
                }
            }
        }
    }
}