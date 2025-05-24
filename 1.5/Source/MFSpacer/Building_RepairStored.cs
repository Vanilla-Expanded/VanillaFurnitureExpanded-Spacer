using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MFSpacer
{
    [StaticConstructorOnStartup]
    public class Building_RepairStored : Building_Storage
    {
        private CompPowerTrader powerComp;
        private bool shouldAutoForbid = true;
        private Texture2D cachedCommandTex;

        private Texture2D CommandTex => cachedCommandTex ??= ContentFinder<Texture2D>.Get("Icons/Forbidden");

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref shouldAutoForbid, "shouldAutoForbid", true);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            powerComp = GetComp<CompPowerTrader>();
        }

        public override void Tick()
        {
            if (powerComp == null || powerComp.PowerOn)
            {
                if (this.IsHashIntervalTick(2500))
                {
                    foreach (var thing in GetSlotGroup().HeldThings)
                    {
                        if (thing.HitPoints < thing.MaxHitPoints && GetParentStoreSettings().AllowedToAccept(thing))
                            thing.HitPoints++;

                        if (shouldAutoForbid)
                            thing.SetForbidden(thing.HitPoints < thing.MaxHitPoints);
                    }
                }
            }

            base.Tick();
        }

        public override void Notify_ReceivedThing(Thing newItem)
        {
            base.Notify_ReceivedThing(newItem);

            if (shouldAutoForbid)
                newItem.SetForbidden(newItem.HitPoints < newItem.MaxHitPoints);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
                yield return gizmo;

            if (Faction == Faction.OfPlayer)
            {
                yield return new Command_Toggle
                {
                    hotKey = KeyBindingDefOf.Command_TogglePower,
                    icon = CommandTex,
                    defaultLabel = "EnableAutoForbid".Translate(),
                    defaultDesc = "EnableAutoForbidExplanation".Translate(),
                    isActive = () => shouldAutoForbid,
                    toggleAction = () => shouldAutoForbid = !shouldAutoForbid
                };
            }
        }
    }
}