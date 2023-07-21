using HarmonyLib;

namespace TasksLib.Harmony
{
    [HarmonyPatch(typeof(TaskWindowScript), nameof(TaskWindowScript.AltGenericCheck))]
    static class AltGenericCheckPatch
    {
        private static bool Prefix(TaskWindowScript __instance, int TempID)
        {
            if (TasksLibMod.tasksByID.ContainsKey(TempID))
            {
                __instance.Generic = false;
                return false; // don't call parent
            }
            return true;
        }
    }
}