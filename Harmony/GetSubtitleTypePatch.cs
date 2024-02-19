
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
        [HarmonyPrefix]
        private static bool Prefix(StudentScript __instance, ref SubtitleType __result)
        {
			if (TasksLibMod.ModdedTaskExists(__instance.StudentID))
			{
				YandereTask ourTask = TasksLibMod.tasksByID[__instance.StudentID];
				if (!ourTask.IsAvailable()) // It might exist but not be available due to 80s mode or other requirements.
				{
                    return true; // Keep the original line.
				}
				__instance.Yandere.Subtitle.CustomText = ourTask.Lines[ourTask.GetTaskPhase()];
				__result = SubtitleType.Custom;
				TasksLibMod.ActivateTask(__instance.StudentID);
                return false;
			}
            return true;
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
