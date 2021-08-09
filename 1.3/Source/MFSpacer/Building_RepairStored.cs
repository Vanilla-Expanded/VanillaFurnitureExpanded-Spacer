using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MFSpacer
{
    public class Building_RepairStored : Building_Storage
    {
        private int TicksCounted = 0;
        private CompPowerTrader powerComp;
        private bool shouldAutoForbid = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref shouldAutoForbid, "shouldAutoForbid", true);
        }

        public override void PostMapInit()
        {
            base.PostMapInit();
            powerComp = this.TryGetComp<CompPowerTrader>();
        }

        public override void Tick()
        {
            if (powerComp == null || powerComp.PowerOn)
            {
                ++this.TicksCounted;
                if (this.TicksCounted == 2500)
                {
                    Thing firstItem = this.Position.GetFirstItem(this.Map);
                    if (firstItem != null 
                        && firstItem.HitPoints != firstItem.MaxHitPoints 
                        && (firstItem.def.IsWithinCategory(ThingCategoryDefOf.Weapons) || firstItem.def.IsWithinCategory(ThingCategoryDefOf.Apparel)))
                    {
                        ++firstItem.HitPoints;
                    }                        

                    if (shouldAutoForbid && firstItem != null)
                    {
                        if (firstItem.HitPoints < firstItem.MaxHitPoints)
                        {
                            firstItem.SetForbidden(true);
                        }
                        else
                        {
                            firstItem.SetForbidden(false);
                        }
                    }

                    this.TicksCounted = 0;
                }
            }
            base.Tick();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            if (this.Faction == Faction.OfPlayer)
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "EnableAutoForbid".Translate(),
                    defaultDesc = "EnableAutoForbidExplanation".Translate(),
                    hotKey = KeyBindingDefOf.Misc3,
                    icon = ContentFinder<Texture2D>.Get("Icons/Forbidden", true),
                    isActive = () => this.shouldAutoForbid,
                    toggleAction = delegate ()
                    {
                        this.shouldAutoForbid = !this.shouldAutoForbid;
                    }
                };
            }
            yield break;
        }
    }
}