using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using TasksLib;

namespace TaskMod.Harmony
{
    [HarmonyPatch(typeof(TalkingScript), "Update")]
    static class TalkingScriptUpdatePatch
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //  IL_168c: stfld bool TaskWindowScript::TaskComplete
            return new CodeMatcher(instructions)
                .MatchEndForward(new CodeMatch(code => code.StoresField(AccessTools.Field(typeof(TaskWindowScript), nameof(TaskWindowScript.TaskComplete)))))
                .ThrowIfInvalid("Unable to find insertion site for TurnInTask() in TalkingScript.Update()!")
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    CodeInstruction.LoadField(typeof(TalkingScript), nameof(TalkingScript.S)),
                    CodeInstruction.LoadField(typeof(StudentScript), nameof(StudentScript.StudentID)),
                    CodeInstruction.Call(typeof(TasksLibMod), nameof(TasksLibMod.TurnInTask))
                )
                .InstructionEnumeration();
        }
    }
}
