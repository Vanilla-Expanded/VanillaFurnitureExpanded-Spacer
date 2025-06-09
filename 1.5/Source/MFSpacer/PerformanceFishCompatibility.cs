using System;
using HarmonyLib;
using Verse;

namespace MFSpacer;

[StaticConstructorOnStartup]
public static class PerformanceFishCompatibility
{
    public static bool IsPerformanceFishActive { get; } = false;

    public static readonly Type GasDefType = null;

    public static readonly FastInvokeHandler ParallelGasGridsMethod = null;
    public static readonly FastInvokeHandler SetDirectMethod = null;
    public static readonly FastInvokeHandler OfGasTypeMethod = null;

    static PerformanceFishCompatibility()
    {
        if (!ModsConfig.IsActive("bs.performance"))
            return;

        try
        {
            IsPerformanceFishActive = true;

            GasDefType = AccessTools.TypeByName("PerformanceFish.GasDef");

            ParallelGasGridsMethod = MethodInvoker.GetHandler(AccessTools.DeclaredMethod("PerformanceFish.PrepatcherFields:ParallelGasGrids"));
            SetDirectMethod = MethodInvoker.GetHandler(AccessTools.DeclaredMethod(
                AccessTools.Inner(AccessTools.TypeByName("PerformanceFish.GasGridOptimization"), "ParallelGasGrid"),
                "SetDirect",
                [typeof(int), typeof(byte)]));
            OfGasTypeMethod = MethodInvoker.GetHandler(AccessTools.DeclaredMethod(GasDefType, "OfGasType"));
        }
        catch (Exception e)
        {
            IsPerformanceFishActive = false;

            GasDefType = null;
            ParallelGasGridsMethod = null;
            SetDirectMethod = null;
            OfGasTypeMethod = null;

            Log.Error($"[VF:Spacer] Failed initializing compatibbility with Performance Fish. Air purifier will be unable to remove vanilla and Performance Fish gases. Exception:\n{e}");
        }
    }
}