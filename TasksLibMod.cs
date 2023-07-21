using System.Collections.Generic;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using HarmonyLib.Tools;

// What started off as just a simple conversion of a DLL mod tutorial to use MelonLoader became a framework for adding tasks in mods.
// Yay.

// TODO: Add support for disabling vanilla tasks and replacing them with modded ones.

namespace TasksLib
{
    public class TasksLibMod : MelonMod
    {
        public static List<YandereTask> allTasks = new List<YandereTask>();
        public static Dictionary<int, YandereTask> tasksByID = new Dictionary<int, YandereTask>();
        public override void OnEarlyInitializeMelon()
        {
            HarmonyLib.Tools.Logger.ChannelFilter = HarmonyLib.Tools.Logger.LogChannel.All;
            HarmonyFileLog.Enabled = true;
        }
        public override void OnInitializeMelon()
        {
            HarmonyFileLog.Enabled = false;
        }
        public static TaskManagerScript taskManagerScript
        {
            get
            {
                if (_taskManagerScript == null)
                {
                    _taskManagerScript = GameObject.FindObjectOfType<TaskManagerScript>();
                    if (_taskManagerScript == null) Debug.LogError($"Needed TaskManagerScript could not be found!\n{new System.Diagnostics.StackTrace()}");
                }
                return _taskManagerScript;
            }
        }
        private static TaskManagerScript _taskManagerScript;
        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            _taskManagerScript = null;
        }
        public static bool ModdedTaskExists(int ID)
        {
            Debug.Log($"Checking for modded task with StudentID {ID}");
            return tasksByID.ContainsKey(ID);
        }

        // New tasks are created by the mods that add them.
        // Prerequisites and completion conditions should be added via subtypes.
        public static void AddTask(YandereTask newTask)
        {
            allTasks.Add(newTask);
            tasksByID[newTask.StudentID] = newTask;
        }
        public static void RemoveTask(int ID)
        {
            allTasks.Remove(tasksByID[ID]);
            tasksByID.Remove(ID);
        }
        public static void RemoveTask(YandereTask task)
        {
            if(tasksByID[task.StudentID] != task)
            {
                Debug.LogError($"Somehow had two different tasks with ID {task.StudentID}?!");
                return;
            }
            allTasks.Remove(task);
            tasksByID.Remove(task.StudentID);
        }
        public static YandereTask GetTask(int ID)
        {
            return tasksByID[ID];
        }
        public static bool TryGetTask(int ID, out YandereTask task)
        {
            return tasksByID.TryGetValue(ID, out task);
        }
        public static void ActivateTask(int ID)
        {
            if (TryGetTask(ID, out YandereTask task))
            {
                task.MarkActive();
            }
        }
        public static void TurnInTask(int ID)
        {
            if (TryGetTask(ID, out YandereTask task))
            {
                task.MarkTurnedIn();
            }
        }
    }
}