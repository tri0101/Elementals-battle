using System.Collections.Generic;
using NUnit.Framework.Internal.Commands;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_PanelMission : MonoBehaviour
{
    [SerializeField] private Transform panelDailyTask;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject taskItem;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button claimAllButton;
    private bool shouldCheck = false;

    public void OnClickPanel()
    {
        ClearItem();
        SetUp();
        RefreshClaimAllButton();
        panelDailyTask.gameObject.SetActive(true);
        claimAllButton.onClick.RemoveAllListeners();
        claimAllButton.onClick.AddListener(() => ClaimAll());
    }

    void Start()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => panelDailyTask.gameObject.SetActive(false));
    }

    void SetUp()
    {
        var tasks = DailyTaskManager.Instance.GetDailyTaskProgress();
        if (tasks == null || tasks.Count == 0)
            return;

        // Filter not-claimed
        var list = new List<DailyTaskProgress>(tasks.Count);
        for (int i = 0; i < tasks.Count; i++)
        {
            var t = tasks[i];
            if (t == null) continue;
            if (t.isClaimed) continue;
            list.Add(t);
        }

        // Sort: completed first, then by taskID asc
        list.Sort((a, b) =>
        {
            int aCompleted = a.isCompleted ? 0 : 1;
            int bCompleted = b.isCompleted ? 0 : 1;

            int byCompleted = aCompleted.CompareTo(bCompleted);
            if (byCompleted != 0) return byCompleted;

            return a.taskID.CompareTo(b.taskID);
        });

        for (int i = 0; i < list.Count; i++)
        {
            var taskProgress = list[i];
            var taskData = DailyTaskManager.Instance.GetDailyTask(taskProgress.taskID);
            if (taskData == null) continue;

            var item = Instantiate(taskItem, content);
            item.GetComponent<UI_MissonTask>().SetUp(taskData, taskProgress, this);
        }
    }

    public void RefreshClaimAllButton()
    {
        if (claimAllButton == null || content == null)
            return;

        bool hasAnyClaimable = false;

        foreach (Transform child in content)
        {
            var missionItem = child.GetComponent<UI_MissonTask>();
            if (missionItem == null) continue;

            var p = missionItem.TaskProgress;
            if (p != null && p.isCompleted && !p.isClaimed)
            {
                hasAnyClaimable = true;
                break;
            }
        }

        claimAllButton.interactable = hasAnyClaimable;
    }

    void ClaimAll()
    {
        int totalExp = 0;

        UI_CanvasReward.Instance.ClearOldItems();
        foreach (Transform child in content)
        {
            var taskItem = child.GetComponent<UI_MissonTask>();
            if (taskItem == null) continue;
            if (!taskItem.TaskProgress.isCompleted) continue;
            if (taskItem.TaskProgress.isClaimed) continue;

            foreach (ItemAndAmount itemAndAmount in taskItem.Task.rewards)
            {
                var itemData = DatabaseManager.Instance.ItemDatabase.GetItem(itemAndAmount.itemId);
                UI_CanvasReward.Instance.SetUp(itemData, itemAndAmount.amount);

                totalExp += taskItem.Task.expReward;
                DailyTaskManager.Instance.SetClaim(taskItem.Task.taskID);
            }
        }

        shouldCheck = true;
        UI_CanvasReward.Instance.SetUp(DatabaseManager.Instance.ItemDatabase.GetItem(0), totalExp);
        AccountManager.Instance.AddExp(totalExp);

        RefreshClaimAllButton();
    }

    void ClearItem()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    void Update()
    {
        if(DailyTaskManager.Instance.NeedReset)
        {
            ClearItem();
            SetUp();
            RefreshClaimAllButton();
            DailyTaskManager.Instance.SetReset(false);
        }
        if (shouldCheck && !UI_LevelUp.Instance.CheckActivePanelLevelUp())
        {
            shouldCheck = false;
            UI_CanvasReward.Instance.gameObject.SetActive(true);
            UI_CanvasReward.Instance.ShowReward();
            DestroyAllChild();
            RefreshClaimAllButton();
        }
    }

    void DestroyAllChild()
    {
        foreach (Transform child in content)
        {
            UI_MissonTask taskItem = child.GetComponent<UI_MissonTask>();
            if (!taskItem.TaskProgress.isClaimed) continue;
            Destroy(child.gameObject);
        }
    }
    
}