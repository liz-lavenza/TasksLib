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
            return new CodeMatcher(instructions)
                // 	IL_16b1: ldc.i4.3
                //  IL_16b2: stelem.i4
                .MatchEndForward(
                    new CodeMatch(code => code.LoadsField(AccessTools.Field(typeof(StudentScript), nameof(StudentScript.StudentID)))),
                    new CodeMatch(OpCodes.Ldc_I4_3),
                    new CodeMatch(OpCodes.Stelem_I4)
                )
                .Advance(-5)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    CodeInstruction.LoadField(typeof(TalkingScript), nameof(TalkingScript.S)),
                    CodeInstruction.LoadField(typeof(StudentScript), nameof(StudentScript.StudentID)),
                    CodeInstruction.Call(typeof(TasksLibMod), nameof(TasksLibMod.ActivateTask))
                )
                //  IL_168c: stfld bool TaskWindowScript::TaskComplete
                .MatchEndForward(new CodeMatch(code => code.LoadsField(AccessTools.Field(typeof(TaskWindowScript), nameof(TaskWindowScript.TaskComplete)))))
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
