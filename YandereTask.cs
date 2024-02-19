using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TasksLib
{
    public class YandereTask
    {
        public int StudentID;
        public string Description;
        public string[] Lines;
        /// <summary>
        /// The name of an embedded PNG resource.
        /// </summary>
        public string Icon;
        public Texture2D Texture
        {
            get
            {
                if (_texture is null && !(Icon is null))
                {
                    _texture = new Texture2D(1, 1);
                    Assembly assembly = this.GetType().Assembly;
                    Stream resourceStream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().First(name => name.EndsWith(Icon)));
                    byte[] resourceBytes = new byte[resourceStream.Length];
                    resourceStream.Read(resourceBytes, 0, (int)resourceStream.Length);
                    _texture.LoadImage(resourceBytes);
                }
                return _texture;
            }
        }
        private Texture2D _texture;
        public bool IsEighties = false;
        public bool IsCustomMode = false;
        protected TaskManagerScript taskman => TasksLibMod.taskManagerScript;
        public virtual bool IsAvailable()
        {
			if (IsEighties != taskman.Eighties)
			{
				return false;
			}
			if (IsCustomMode != taskman.StudentManager.CustomMode)
			{
				return false;
			}
            return true;
        }
        public virtual bool IsComplete()
        {
            return false;
        }
        public void MarkActive()
        {
            taskman.TaskStatus[this.StudentID] = 1;
            if (GetStudent() is null)
            {
                Debug.LogWarning($"TasksLib.YandereTask.MarkActive could not find student with ID {this.StudentID}!");
                return;
            }
            GameObject taskObject = GetTaskObject();
            if (taskObject != null) // Must use equality operator here, NOT "is" or null-coalescing operator, because only equality checks for destroyed objects.
            {
                taskObject.SetActive(true);
            }
            OnMarkActive();
        }

        /// <summary>
        /// Used for task-specific setup, like giving the player money to buy a soda with. Can be overridden.
        /// </summary>
        protected virtual void OnMarkActive()
        {
            return;
        }

        // As far as I know this is never actually done for a task that's already been discovered, but I'm including it for completeness' sake.
        public void MarkInactive()
        {
            taskman.TaskStatus[this.StudentID] = 0;
            GetTaskObject()?.SetActive(false);
        }
        public void MarkComplete()
        {
            taskman.TaskStatus[this.StudentID] = 2;
            SetTaskPhase(5);
            GameObject taskObject = GetTaskObject();
            if (taskObject != null)
            {
                GameObject.Destroy(taskObject);
            }
        }
        public void MarkTurnedIn()
        {
            taskman.TaskStatus[this.StudentID] = 3;
            OnTurnedIn();
        }
        public virtual void OnTurnedIn()
        {
            return;
        }
        public StudentScript GetStudent()
        {
            return taskman.StudentManager.Students[this.StudentID];
        }
        public GameObject GetTaskObject()
        {
            return taskman.TaskObjects[this.StudentID];
        }
        public bool IsActive()
        {
            return taskman.TaskStatus[this.StudentID] > 0;
        }
        public void SetTaskPhase(int newPhase)
        {
            StudentScript student = GetStudent();
            if (student == null)
            {
                return;
            }
            student.TaskPhase = newPhase;
        }
        public int GetTaskPhase()
        {
            return this.GetStudent()?.TaskPhase ?? 0;
        }
        public void UpdateSubtitle()
        {
            taskman.Yandere.Subtitle.CustomText = Lines[GetTaskPhase()];
            taskman.Yandere.Subtitle.UpdateLabel(SubtitleType.Custom, GetTaskPhase(), 5f); // todo: support for custom voicelines and lengths
        }
    }
}
