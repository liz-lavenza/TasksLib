
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace TasksLib.Harmony
{
    // We use a transpiler patch here to circumvent some of the subtitle code
    // and avoid the horrible subtitle enum system.
    [HarmonyPatch(typeof(StudentScript), nameof(StudentScript.TaskLineResponseType), MethodType.Getter)]
    static class GetSubtitleTypePatch
    {
        [HarmonyPostfix]
        private static void Postfix(StudentScript __instance, ref SubtitleType __result)
        {
            if (__result == SubtitleType.TaskGenericLine)
            {
                if (TasksLibMod.ModdedTaskExists(__instance.StudentID))
                {
                    YandereTask ourTask = TasksLibMod.tasksByID[__instance.StudentID];
                    __instance.Yandere.Subtitle.CustomText = ourTask.Lines[ourTask.GetTaskPhase()];
                    __result = SubtitleType.Custom;
                }
            }
        }
        [HarmonyTranspiler]
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
    }
/*
    [HarmonyPatch(typeof(TalkingScript), "Update")]
    static class TalkingScriptUpdatePatch
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new CodeMatcher(instructions)
                .MatchEndForward(
                    new CodeMatch(code => code.Calls(AccessTools.Method(typeof(SubtitleScript), nameof(UpdateLabel)), name = "")
                )
                .Advance(-5);
            return matcher
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    CodeInstruction.LoadField(typeof(TalkingScript), nameof(TalkingScript.S)),
                    CodeInstruction.LoadField(typeof(StudentScript), nameof(StudentScript.StudentID)),
                    CodeInstruction.Call(typeof(TasksLibMod), nameof(TasksLibMod.ModdedTaskExists)),
                    new CodeInstruction(OpCodes.Brtrue_S, matcher.NamedMatch("jump").opcode))
                .InstructionEnumeration();
            // ldfld bool TaskManagerScript::Proceed

            // Quick walk-through of what we need the IL to do:
            // If ModdedTaskExists(S.StudentID):
            //   TasksLib.tasksByID[S.StudentID].UpdateSubtitle();
            // Else:
            //   S.Subtitle.UpdateLabel(S.TaskLineResponseType, S.TaskPhase, S.Subtitle.GetClipLength(S.StudentID, S.TaskPhase));
        }
    }
*/
}
