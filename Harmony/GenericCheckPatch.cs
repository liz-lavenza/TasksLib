using HarmonyLib;

namespace TasksLib.Harmony
{
    [HarmonyPatch(typeof(TaskWindowScript), nameof(TaskWindowScript.GenericCheck))]
    static class GenericCheckPatch
    {
        private static bool Prefix(TaskWindowScript __instance)
        {
            if (TasksLibMod.ModdedTaskExists(__instance.Yandere.TargetStudent.StudentID))
            {
                __instance.Generic = false;
                return false; // don't call parent
            }
            return true;
        }
    }
}