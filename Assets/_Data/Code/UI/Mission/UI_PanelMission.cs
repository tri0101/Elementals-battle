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
        foreach (var task in tasks)
        {
            var taskData = DailyTaskManager.Instance.GetDailyTask(task.taskID);
            if (task.isClaimed) continue;
            var item = Instantiate(taskItem, content);
            item.GetComponent<UI_MissonTask>().SetUp(taskData, task);
        }
    }
    void ClaimAll()
    {
        int totalExp = 0;
        foreach (Transform child in content)
        {
            var taskItem = child.GetComponent<UI_MissonTask>();
            if (!taskItem.TaskProgress.isCompleted) continue;
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
        if (shouldCheck && !UI_LevelUp.Instance.CheckActivePanelLevelUp())
        {
            shouldCheck = false;
            UI_CanvasReward.Instance.gameObject.SetActive(true);
            UI_CanvasReward.Instance.ShowReward();
            DestroyAllChild();
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
