using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using TasksLib;

namespace TaskMod.Harmony
{
    [HarmonyPatch(typeof(TaskWindowScript))]
    static class TaskWindowScriptPatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch("Update")]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // IL_0031: stelem.i4
            CodeMatcher matcher = new CodeMatcher(instructions)
                .MatchEndForward(
                    new CodeMatch(OpCodes.Stelem_I4)
                ).Advance(1);
            return matcher
                .InsertAndAdvance(matcher.InstructionsWithOffsets(-4, -3)) // StudentID
                .Insert(CodeInstruction.Call(typeof(TasksLibMod), nameof(TasksLibMod.ActivateTask))
                ).InstructionEnumeration();
        }
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Postfix(TaskWindowScript __instance)
        {
            foreach (YandereTask task in TasksLibMod.allTasks)
            {
                __instance.Descriptions[task.StudentID] = task.Description;
                __instance.Icons[task.StudentID] = task.Texture;
            }
        }
    }
}
