using RimWorld;
using Verse;

namespace MFSpacer
{
    public class Building_RefreshingBed : Building_Bed
    {
        private int[] TicksCounted = new int[2];

        public override void Tick()
        {
            for (int slotIndex = 0; slotIndex < this.SleepingSlotsCount; ++slotIndex)
            {
                if (this.GetCurOccupant(slotIndex) != null)
                {
                    Pawn curOccupant = this.GetCurOccupant(slotIndex);
                    ++this.TicksCounted[slotIndex];
                    if (this.TicksCounted[slotIndex] == 1250 && curOccupant.GetPosture() == PawnPosture.LayingInBed)
                    {
                        Hediff firstHediffOfDef = curOccupant.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Bed_RefreshingSleep);
                        if (firstHediffOfDef == null)
                            curOccupant.health.AddHediff(HediffDefOf.Bed_RefreshingSleep);
                        else
                            firstHediffOfDef.Severity += 0.05f;
                        this.TicksCounted[slotIndex] = 0;
                    }
                }
            }
            base.Tick();
        }
    }
}