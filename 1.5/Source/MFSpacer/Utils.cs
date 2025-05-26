using Verse;

namespace MFSpacer;

public class Utils
{
    public static bool IsDarklight(Thing thing)
    {
        var comp = thing.TryGetComp<CompGlower>();
        return comp != null && DarklightUtility.IsDarklight(comp.GlowColor.ProjectToColor32);
    }
}