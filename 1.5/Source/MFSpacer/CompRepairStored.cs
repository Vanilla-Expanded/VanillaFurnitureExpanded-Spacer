using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace MFSpacer
{
    [StaticConstructorOnStartup]
    public class CompRepairStored : ThingComp
    {
        private Building_Storage storage;

        private CompPowerTrader powerComp;
        private bool shouldAutoForbid = true;

        private bool hasAnyThing = false;
        private Effecter effecter = null;

        private static Texture2D CommandTex { get; } = ContentFinder<Texture2D>.Get("Icons/Forbidden");

        public CompProperties_RepairStored Props => (CompProperties_RepairStored)props;

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref shouldAutoForbid, "shouldAutoForbid", true);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                LongEventHandler.ExecuteWhenFinished(() =>
                {
                    hasAnyThing = storage.GetSlotGroup().HeldThings.Any(x => storage.GetParentStoreSettings().AllowedToAccept(x));
                    if (hasAnyThing)
                        MakeEffecter();
                });
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            powerComp = parent.GetComp<CompPowerTrader>();

            storage = parent as Building_Storage;
            if (storage == null)
                Log.Error($"{nameof(CompRepairStored)} has a parent that is not {nameof(Building_Storage)}");
        }

        public override void CompTick()
        {
            if (parent.Spawned)
            {
                if ((powerComp == null || powerComp.PowerOn) && storage != null && parent.IsHashIntervalTick(Props.repairInterval))
                {
                    hasAnyThing = false;

                    foreach (var thing in storage.GetSlotGroup().HeldThings)
                    {
                        if (storage.GetParentStoreSettings().AllowedToAccept(thing))
                        {
                            if (thing.HitPoints < thing.MaxHitPoints)
                            {
                                // Don't allow the HP to go over maximum if repairAmount is more than 1
                                thing.HitPoints = Mathf.Min(thing.HitPoints + Props.repairAmount, thing.MaxHitPoints);

                                if (thing.HitPoints < thing.MaxHitPoints)
                                    hasAnyThing = true;
                            }

                            if (shouldAutoForbid)
                                thing.SetForbidden(thing.HitPoints < thing.MaxHitPoints);
                        }
                    }
                }
            }
            else
            {
                hasAnyThing = false;
            }

            if (hasAnyThing && parent.MapHeld == Find.CurrentMap)
            {
                MakeEffecter();
                effecter?.EffectTick(parent, parent);
            }
            else
            {
                DestroyEffecter();
            }
        }

        public virtual void Notify_ReceivedThing(Thing newItem)
        {
            if (shouldAutoForbid)
                newItem.SetForbidden(newItem.HitPoints < newItem.MaxHitPoints);

            if (!hasAnyThing)
                hasAnyThing = storage.GetSlotGroup().HeldThings.Any(x => storage.GetParentStoreSettings().AllowedToAccept(x));
            MakeEffecter();
        }

        public virtual void Notify_LostThing(Thing newItem)
        {
            if (effecter != null && !storage.GetSlotGroup().HeldThings.Any())
                DestroyEffecter();
        }

        protected void MakeEffecter()
        {
            if (effecter == null && Props.effecterDef != null)
                effecter ??= Props.effecterDef.SpawnAttached(parent, parent.MapHeld);
        }

        protected void DestroyEffecter()
        {
            effecter?.Cleanup();
            effecter = null;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
                yield return gizmo;

            if (parent.Faction == Faction.OfPlayer)
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