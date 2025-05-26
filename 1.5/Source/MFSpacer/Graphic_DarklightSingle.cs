using UnityEngine;
using Verse;

namespace MFSpacer;

public class Graphic_DarklightSingle : Graphic_Single
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
        req.maskPath ??= req.path + MaskSuffix;
        base.Init(req);
        req.path += "_dark";

        darklightGraphic = new Graphic_Single();
        darklightGraphic.Init(req);
    }

    public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        => GraphicDatabase.Get<Graphic_Single>(path, newShader, drawSize, newColor, newColorTwo, data);

    public override string ToString() => $"{nameof(Graphic_DarklightSingle)}(base=({base.ToString()}), darklight=({darklightGraphic}))";
}