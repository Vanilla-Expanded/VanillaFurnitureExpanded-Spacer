using UnityEngine;
using Verse;

namespace MFSpacer;

public class Graphic_DarklightMulti : Graphic_Multi
{
    public Graphic darklightGraphic;

    public override Material MatAt(Rot4 rot, Thing thing = null)
    {
        if (Utils.IsDarklight(thing))
            return darklightGraphic.MatAt(rot, thing);
        return base.MatAt(rot, thing);
    }

    public override Material MatSingleFor(Thing thing)
    {
        if (Utils.IsDarklight(thing))
            return darklightGraphic.MatSingleFor(thing);
        return base.MatSingleFor(thing);
    }

    public override void TryInsertIntoAtlas(TextureAtlasGroup groupKey)
    {
        base.TryInsertIntoAtlas(groupKey);
        darklightGraphic.TryInsertIntoAtlas(groupKey);
    }

    public override void Init(GraphicRequest req)
    {
        req.maskPath ??= req.path + Graphic_Single.MaskSuffix;
        base.Init(req);
        req.path += "_dark";

        darklightGraphic = new Graphic_Multi();
        darklightGraphic.Init(req);
    }

    public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        => GraphicDatabase.Get<Graphic_DarklightMulti>(path, newShader, drawSize, newColor, newColorTwo, data, maskPath);

    public override string ToString() => $"{nameof(Graphic_DarklightMulti)}(base=({base.ToString()}), darklight=({darklightGraphic}))";
}