using RimWorld;
using Verse;

namespace MFSpacer
{
    [DefOf]
    public static class HediffDefOf
    {
        public static HediffDef Bed_RefreshingSleep;

        static HediffDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf));
    }
}