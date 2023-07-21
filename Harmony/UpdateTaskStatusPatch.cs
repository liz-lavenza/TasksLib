using HarmonyLib;

namespace TasksLib.Harmony
{
    [HarmonyPatch(typeof(TaskManagerScript), nameof(TaskManagerScript.UpdateTaskStatus))]
    static class UpdateTaskStatusPatch
    {
        [HarmonyPrefix]
        private static void Prefix(TaskManagerScript __instance) // We have to always run the original method since the tasks are all handled together.
        {
            foreach (YandereTask task in TasksLibMod.allTasks)
            {
                // IsAvailable -> 80s checks, other special conditions per-task.
                if (!task.IsAvailable())
                {
                    continue;
                }
                // Unlike vanilla tasks that will re-(de)activate their objects and such every time UpdateTaskStatus is called,
                // we just rely on MarkAvailable/MarkUnavailable doing that for us
                // and the mod checking everything on scene load/unload.
                // Hopefully it's a performance improvement, even if marginal.
                if (!task.IsActive())
                {
                    continue;
                }
                if (task.IsComplete())
                {
                    task.MarkComplete();
                }
            }
        }
    }
}