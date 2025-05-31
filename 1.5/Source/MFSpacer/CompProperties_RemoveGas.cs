using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MFSpacer;

public class CompProperties_RemoveGas : CompProperties
{
    public List<GasType> gasTypesToRemove = null;
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

        if (gasTypesToRemove.NullOrEmpty())
            yield return $"{gasTypesToRemove} has no elements, making this comp not do anything";
        else
        {
            var distinct = gasTypesToRemove.Distinct().ToList();
            if (distinct.Count != gasTypesToRemove.Count)
            {
                gasTypesToRemove = distinct;
                distinct.TrimExcess();
                yield return $"{gasTypesToRemove} cannot be distinct";
            }
        }
    }
}