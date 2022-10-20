using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MFSpacer
{
    [StaticConstructorOnStartup]
    public class Building_RepairStored : Building_Storage
    {
        private int TicksCounted = 0;
        private CompPowerTrader powerComp;
        private bool shouldAutoForbid = true;
        private Texture2D cachedCommandTex;

        private Texture2D CommandTex
        {
            get
            {
                if (cachedCommandTex == null)
                    cachedCommandTex = ContentFinder<Texture2D>.Get("Icons/Forbidden");
                return cachedCommandTex;
            }
        }

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
                ++TicksCounted;
                if (TicksCounted == 2500)
                {
                    Thing firstItem = Position.GetFirstItem(Map);
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

                    TicksCounted = 0;
                }
            }
            base.Tick();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
                yield return gizmo;

            if (Faction == Faction.OfPlayer)
            {
                Command_Toggle commandToggle = new Command_Toggle();
                commandToggle.hotKey = KeyBindingDefOf.Command_TogglePower;
                commandToggle.icon = CommandTex;
                commandToggle.defaultLabel = "EnableAutoForbid".Translate();
                commandToggle.defaultDesc = "EnableAutoForbidExplanation".Translate();
                commandToggle.isActive = () => shouldAutoForbid;
                commandToggle.toggleAction = () => shouldAutoForbid = !shouldAutoForbid;
                yield return commandToggle;
            }
        }
    }
}