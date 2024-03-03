using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTaskController : LocalSingletonBehaviour<LevelTaskController> {
	private Level level;
	private List<Task> targetTasks = new List<Task>();
	private List<Task> tasks = new List<Task>();
	private List<string> completedTasks = new List<string>();

	public List<Task> Tasks {
		get {
			return tasks;
		}
	}
	public List<Task> TargetTasks {
		get {
			return targetTasks;
		}
	}

	public event Action<Task> OnTaskUpdated = delegate { };
	public event Action<Task> OnTaskComplete = delegate { };

    public bool TasksInited = false;

	public void DoInit (Level level) {
		this.level = level;

		if (level.Task1 != null) targetTasks.Add(level.Task1);
		if (level.Task2 != null) targetTasks.Add(level.Task2);
		if (level.Task3 != null) targetTasks.Add(level.Task3);

		foreach (Task task in targetTasks) {
			if (!string.IsNullOrEmpty(task.Key))
				tasks.Add(new Task(task.Key , 0));
		}

        TasksInited = true;
	}

	public void UpdateTask (string key , int increaseValue) {
		Task task = tasks.Find(t => t.Key.Equals(key));

		if (task == null) return;

		if (!completedTasks.Contains(key)) {
			task.Value += increaseValue;

			if (task.Value >= targetTasks.Find(t => t.Key.Equals(key)).Value) {
				completedTasks.Add(key);

				OnTaskUpdated.Invoke(task);
				OnTaskComplete.Invoke(task);
			} else {
				OnTaskUpdated.Invoke(task);
			}
		}
	}

    public void UpdateTaskSet (string key, int currentValue) {
        Task task = tasks.Find(t => t.Key.Equals(key));

        if (task == null) return;

        if (!completedTasks.Contains(key)) {
            task.Value = currentValue;

            if (task.Value >= targetTasks.Find(t => t.Key.Equals(key)).Value) {
                completedTasks.Add(key);

                OnTaskUpdated.Invoke(task);
                OnTaskComplete.Invoke(task);
            } else {
                OnTaskUpdated.Invoke(task);
            }
        }
    }

	public bool AllTaskDone () {
		int countDone = 0;
		Task tt = null;
		foreach (Task t in tasks) {
			tt = targetTasks.Find(ttt => t.Key.Equals(ttt.Key));
			if (t.Value >= tt.Value) countDone++;
		}
		return countDone.Equals(tasks.Count);
	}

    public void DoFirstTask() {
        foreach (Task t in tasks) {
            if (!completedTasks.Contains(t.Key)) {
                t.Value = targetTasks.Find(tt => tt.Key.Equals(t.Key)).Value;

                completedTasks.Add(t.Key);

                OnTaskUpdated.Invoke(t);
                OnTaskComplete.Invoke(t);
                break;
            }
        }
    }

    public void DoAllTasks() {
        foreach (Task t in tasks) {
            if (!completedTasks.Contains(t.Key)) {
                t.Value = targetTasks.Find(tt => tt.Key.Equals(t.Key)).Value;

                completedTasks.Add(t.Key);

                OnTaskUpdated.Invoke(t);
                OnTaskComplete.Invoke(t);
            }
        }
    }

}
