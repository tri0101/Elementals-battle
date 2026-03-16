using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DailyTaskProgress
{
    public int taskID;
    public int progress;
    public bool isCompleted;
    public bool isClaimed;
}

public class DailyTaskManager : Subject
{
    public static DailyTaskManager Instance;

    [SerializeField] private DailyTaskSO DailyTaskSO;

    [SerializeField] private  List<DailyTaskProgress> tasks = new List<DailyTaskProgress>();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeTasksIfNeeded();
    }

    private void InitializeTasksIfNeeded()
    {
        if (DailyTaskSO == null || DailyTaskSO.dailyTasks == null)
        {
            Debug.LogError("[DailyTaskManager] DailyTaskSO is not assigned or empty.");
            return;
        }

        // Không init lại nếu đã có data (sau này bạn load save vào tasks thì không bị overwrite)
        if (tasks.Count > 0)
            return;

        foreach (var t in DailyTaskSO.dailyTasks)
        {
            if (t == null) continue;

            // tránh duplicate id
            if (tasks.Exists(x => x.taskID == t.taskID))
                continue;

            tasks.Add(new DailyTaskProgress
            {
                taskID = t.taskID,
                progress = 0,
                isCompleted = false,
                isClaimed = false
            });
        }
    }

    public DailyTask GetDailyTask(int taskId)
    {
        return DailyTaskSO != null ? DailyTaskSO.GetDailyTask(taskId) : null;
    }

    public List<DailyTaskProgress> GetDailyTaskProgress()
    {
        return tasks;
    }
    public void SetClaim(int takskID)
    {
        DailyTaskProgress taskProgress = tasks.Find(x => x.taskID == takskID);
        if (taskProgress != null)
        {
            taskProgress.isClaimed = true;
            NotifyObservers(takskID);
        }
    }
    public void AddProgress(int taskID, int amount)
    {
        if (amount <= 0) return;

        DailyTask task = DailyTaskSO != null ? DailyTaskSO.GetDailyTask(taskID) : null;
        if (task == null) return;

        DailyTaskProgress taskProgress = tasks.Find(x => x.taskID == taskID);
        if (taskProgress == null)
        {
            
            taskProgress = new DailyTaskProgress
            {
                taskID = taskID,
                progress = 0,
                isCompleted = false,
                isClaimed = false
            };
            tasks.Add(taskProgress);
        }

        //if (taskProgress.isClaimed) return;

        taskProgress.progress += amount;

        if (taskProgress.progress >= task.target)
        {
            //taskProgress.progress = task.target;
            taskProgress.isCompleted = true;
        }
        NotifyObservers(taskID);
    }

    void ResetDailyTasks()
    {
        foreach (var task in tasks)
        {
            task.progress = 0;
            task.isCompleted = false;
            task.isClaimed = false;
        }
    }
}