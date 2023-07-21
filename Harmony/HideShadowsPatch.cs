using System.Collections.Generic;
using HarmonyLib;
using System.Reflection.Emit;
using HarmonyLib.Tools;

namespace TasksLib.Harmony
{
    [HarmonyPatch(typeof(DialogueWheelScript), nameof(DialogueWheelScript.HideShadows))]
    static class HideShadowsPatch
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //	IL_0820: ldc.i4.s 81
            //  IL_0822: beq.s IL_0827
            CodeMatcher matcher = new CodeMatcher(instructions)
                .MatchEndForward(
                    new CodeMatch(code => code.LoadsConstant(81)),
                    new CodeMatch(OpCodes.Beq, name: "jump")
                ).Advance(1);
            if (matcher.IsInvalid)
            {
                UnityEngine.Debug.LogError("Unable to find match!");
                HarmonyFileLog.Writer.WriteLine("Unable to find match!");
                return instructions;
            }
            return matcher
                .InsertAndAdvance(matcher.InstructionsWithOffsets(-6, -3))
                .Insert(
                    CodeInstruction.Call(typeof(TasksLibMod), nameof(TasksLibMod.ModdedTaskExists)),
                    new CodeInstruction(OpCodes.Brtrue_S, matcher.NamedMatch("jump").operand))
                .InstructionEnumeration();
        }
    }
}
