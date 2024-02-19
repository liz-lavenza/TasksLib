using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

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

        // Here, we modify the base method to have null checks every time we try to activate a base-game task's TaskObject...
        // I really hope the vanilla game uses a better system for tasks someday.
        private static void AddNullcheck(CodeMatcher matcher, ILGenerator generator)
        {
            // IL_0060: callvirt instance void [UnityEngine.CoreModule]UnityEngine.GameObject::SetActive(bool)
            // Branch before, label after
            // dup can be used to get another copy of the object to do a truthiness check on.
            // In essence:
            // 0: dup
            // 1: call op_True
            // 2: brfalse 5 // if object
            // 3: original code
            // 4: branch 6 // skip the pop since we already used the value
            // 5: pop // remove the original value since we aren't using it
            // We also have to do it in two parts, first to make and add the labels and then to insert the code.
            Label skipLabel = generator.DefineLabel();
            Label endLabel = generator.DefineLabel();
            matcher
                .MatchStartForward(
                    new CodeMatch(code => code.LoadsField(AccessTools.Field(typeof(TaskManagerScript), nameof(TaskManagerScript.TaskObjects)))),
                    new CodeMatch(code => code.LoadsConstant()),
                    new CodeMatch(OpCodes.Ldelem_Ref),
                    new CodeMatch(OpCodes.Ldc_I4_1), // true
                    new CodeMatch(code => code.Calls(AccessTools.Method(typeof(UnityEngine.GameObject), nameof(UnityEngine.GameObject.SetActive))))
                )
                .Advance(2) // We specifically want to insert stuff after Ldelem_Ref, so that the ref is on the top of the stack.
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Dup),
                    CodeInstruction.Call(typeof(UnityEngine.Object), "op_True")
                )
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Brfalse_S, skipLabel)
                )
                .Advance(2) // Move past the SetActive() call.
                .Insert(
                    new CodeInstruction(OpCodes.Br, endLabel),
                    new CodeInstruction(OpCodes.Pop)
                )
                .Advance(1) // Go to the Pop instruction
                .AddLabels(new Label[1] { skipLabel })
                .Advance(1) // Go to the instruction after our modified code
                .AddLabels(new Label[1] { endLabel });
        }
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new CodeMatcher(instructions);
            while (matcher.IsValid)
            {
                AddNullcheck(matcher, generator);
            }
            return matcher.InstructionEnumeration();
        }
    }
}