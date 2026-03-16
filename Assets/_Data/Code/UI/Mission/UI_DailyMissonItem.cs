using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MissonTask : MonoBehaviour, IObserver
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI amountDiamondText;
    [SerializeField] private TextMeshProUGUI amountExpText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button buttonClaim;
    [SerializeField] private Button buttonGo;

    int taskId;
    private DailyTask task;
    public DailyTask Task => task;
    DailyTaskProgress taskProgress;
    public DailyTaskProgress TaskProgress => taskProgress;
    bool shouldCheck = false;

    [SerializeField] private bool canClaimAll = false;
    public bool CanClaimAll => canClaimAll;

    public void OnNotify(object data)
    {
        if (data is int id && id == taskId) RefreshUI();
    }


    void RefreshUI()
    {
        progressText.text = $"{taskProgress.progress}/{task.target}";
        if (taskProgress.isClaimed) return;

        if (taskProgress.isCompleted)
        {
            canClaimAll = true;
            buttonClaim.gameObject.SetActive(true);
            buttonGo.gameObject.SetActive(false);
        }
        else
        {
            buttonClaim.gameObject.SetActive(false);
            buttonGo.gameObject.SetActive(true);
        }
    }

    public void SetUp(DailyTask task, DailyTaskProgress taskProgress)
    {
        DailyTaskManager.Instance.AddObserver(this);
        this.taskId = task.taskID;
        this.task = task;
        this.taskProgress = taskProgress;

        descriptionText.text = task.description;
        amountDiamondText.text = $"{task.rewards[0].amount}";
        amountExpText.text = $"{task.expReward}";
        progressText.text = $"{taskProgress.progress}/{task.target}";

        if (taskProgress.isCompleted)
        {
            buttonClaim.gameObject.SetActive(true);
            buttonGo.gameObject.SetActive(false);
        }
        else
        {
            buttonClaim.gameObject.SetActive(false);
            buttonGo.gameObject.SetActive(true);
            SetUpButtonGo(task);
        }

        buttonClaim.onClick.RemoveAllListeners();
        buttonClaim.onClick.AddListener(() => ClaimReward());
    }

    void SetUpButtonGo(DailyTask task)
    {
        if (task.action == TaskAction.None)
        {
            buttonGo.gameObject.SetActive(false);
            return;
        }
        else if (task.action == TaskAction.OpenBuyStamina)
        {
            buttonGo.onClick.AddListener(() => UI_ShowResource.Instance.UI_Exchange.ShowPanelBuyStamina());
        }
        else if (task.action == TaskAction.OpenBuyCoin)
        {
            buttonGo.onClick.AddListener(() => UI_ShowResource.Instance.UI_Exchange.ShowPanelBuyCoin());
        }
    }

    public void ClaimReward()
    {
        if (!taskProgress.isCompleted) return;

        canClaimAll = false;

        UI_CanvasReward.Instance.ClearOldItems();

       
        foreach (ItemAndAmount itemAndAmount in task.rewards)
        {
            var itemData = DatabaseManager.Instance.ItemDatabase.GetItem(itemAndAmount.itemId);
            UI_CanvasReward.Instance.SetUp(itemData, itemAndAmount.amount);
        }


        UI_CanvasReward.Instance.SetUp(DatabaseManager.Instance.ItemDatabase.GetItem(0), task.expReward);

        AccountManager.Instance.AddExp(task.expReward);
        DailyTaskManager.Instance.SetClaim(taskId);
        shouldCheck = true;
    }

    void Update()
    {
        if (shouldCheck && !UI_LevelUp.Instance.CheckActivePanelLevelUp())
        {
            shouldCheck = false;
            UI_CanvasReward.Instance.gameObject.SetActive(true);
            UI_CanvasReward.Instance.ShowReward();
            Destroy(transform.gameObject);
        }
    }
}