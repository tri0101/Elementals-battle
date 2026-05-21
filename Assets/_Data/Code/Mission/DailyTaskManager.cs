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

    [SerializeField] private List<DailyTaskProgress> tasks = new List<DailyTaskProgress>();

    private bool needReset = false;
    public bool NeedReset => needReset;
    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
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

        // luôn merge để bù task thiếu (kể cả khi đã có save)
        MergeMissingTasksFromSO();
    }

    private void MergeMissingTasksFromSO()
    {
        if (DailyTaskSO == null || DailyTaskSO.dailyTasks == null)
            return;

        bool changed = false;

        foreach (var t in DailyTaskSO.dailyTasks)
        {
            if (t == null) continue;
            if (tasks.Exists(x => x.taskID == t.taskID)) continue;

            tasks.Add(new DailyTaskProgress
            {
                taskID = t.taskID,
                progress = 0,
                isCompleted = false,
                isClaimed = false
            });

            changed = true;
        }

        if (changed)
            NotifyObservers();
    }

    public void SetTasks(List<DailyTaskProgress> savedTasks)
    {
        tasks.Clear();

        if (savedTasks != null)
            tasks.AddRange(savedTasks);

        //bù task mới từ SO
        MergeMissingTasksFromSO();

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
            NotifyObservers();
        }
    }

    public void AddProgress(int taskID, int amount)
    {
        if (amount <= 0) return;

        DailyTask task = DailyTaskSO != null ? DailyTaskSO.GetDailyTask(taskID) : null;
        if (task == null) return;

        DailyTaskProgress taskProgress = tasks.Find(x => x.taskID == taskID);
        if (taskProgress != null && taskProgress.isCompleted) return;

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

        taskProgress.progress += amount;

        if (taskProgress.progress >= task.target)
            taskProgress.isCompleted = true;

        NotifyObservers(taskID);
        NotifyObservers();
    }

    public void ResetDailyTasks()
    {
        foreach (var task in tasks)
        {
            task.progress = 0;
            task.isCompleted = false;
            task.isClaimed = false;
        }

        NotifyObservers();
    }
    private void Update()
    {
        if(TimeManager.Instance.ShouldResetDailyMission())
        {
            ResetDailyTasks();
            needReset = true;
        }
    }
    public void SetReset(bool value)
    {
        needReset = value;
    }
}