using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MFSpacer
{
    public class Building_PassiveJoyTable : Building
    {
        public static IntVec3[] AdjacentCellsAround_2x2 = new IntVec3[8];

        public override void Tick()
        {
            CompPowerTrader comp = this.TryGetComp<CompPowerTrader>();
            if (comp != null && comp.PowerOn)
            {
                List<Pawn> pawnList = this.CollectPawns();
                if (pawnList != null)
                {
                    for (int index = 0; index < pawnList.Count; ++index)
                    {
                        if (pawnList[index] != null && pawnList[index].RaceProps.Humanlike && pawnList[index].needs.joy != null)
                            pawnList[index].needs.joy.GainJoy(0.0003f, JoyKindDefOf.Gaming_Electronic);
                    }
                }
            }
            base.Tick();
        }

        public List<Pawn> CollectPawns()
        {
            List<Pawn> pawnList = new List<Pawn>();
            if (this.Map.thingGrid.CellContains(this.Position, ThingDefOf.Table_interactive_2x2c))
            {
                AdjacentCellsAround_2x2[0] = new IntVec3(0, 0, 2);
                AdjacentCellsAround_2x2[5] = new IntVec3(1, 0, 2);
                AdjacentCellsAround_2x2[1] = new IntVec3(2, 0, 0);
                AdjacentCellsAround_2x2[6] = new IntVec3(2, 0, 1);
                AdjacentCellsAround_2x2[2] = new IntVec3(0, 0, -1);
                AdjacentCellsAround_2x2[4] = new IntVec3(1, 0, -1);
                AdjacentCellsAround_2x2[3] = new IntVec3(-1, 0, 0);
                AdjacentCellsAround_2x2[7] = new IntVec3(-1, 0, 1);
                for (int index = 0; index < AdjacentCellsAround_2x2.Length; ++index)
                {
                    if ((this.Position + AdjacentCellsAround_2x2[index]).GetFirstPawn(this.Map) != null)
                        pawnList.Add((this.Position + AdjacentCellsAround_2x2[index]).GetFirstPawn(this.Map));
                }
            }
            else
            {
                for (int index = 0; index < GenAdj.CardinalDirectionsAround.Length; ++index)
                {
                    if ((this.Position + GenAdj.CardinalDirectionsAround[index]).GetFirstPawn(this.Map) != null)
                        pawnList.Add((this.Position + GenAdj.CardinalDirectionsAround[index]).GetFirstPawn(this.Map));
                }
            }
            return pawnList;
        }
    }
}