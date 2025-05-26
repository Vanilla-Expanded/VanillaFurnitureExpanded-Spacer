using Verse;

namespace MFSpacer;

public class CompDarklightGraphicGlower : CompGlower
{
    protected override void SetGlowColorInternal(ColorInt? color)
    {
        base.SetGlowColorInternal(color);

        if (parent.Spawned)
            parent.DirtyMapMesh(parent.Map);
    }
}