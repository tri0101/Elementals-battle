using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
public enum TaskAction
{
    None,
    OpenBuyStamina,
    OpenBuyCoin,
    OpenShop,
    OpenStage,
    OpenHeroSummon
}


[Serializable]
public class DailyTask
{
    public int taskID;
    public string description;
    public int target;
    public List<ItemAndAmount> rewards = new List<ItemAndAmount>();
    public int expReward;

    public TaskAction action;
}

[CreateAssetMenu(fileName = "DailyTask", menuName = "Task/Daily Task")]
public class DailyTaskSO : ScriptableObject
{
    public List<DailyTask> dailyTasks = new List<DailyTask>();

    public DailyTask GetDailyTask(int taskID)
    {
        DailyTask task = dailyTasks.Find(x => x.taskID == taskID);
        if (task != null) return task;
        else return null;
    }
}