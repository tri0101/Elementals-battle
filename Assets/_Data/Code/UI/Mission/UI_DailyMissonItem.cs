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
    DailyTask task;
    DailyTaskProgress taskProgress;

    public void OnNotify(object data)
    {
        if (data is int id && id == taskId) RefreshUI();
    }
    void RefreshUI()
    {
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
        if(task.action == TaskAction.None)
        {
            buttonGo.gameObject.SetActive(false);
            return;
        }
        else if(task.action == TaskAction.OpenBuyStamina)
        {
            buttonGo.onClick.AddListener(() => UI_ShowResource.Instance.UI_Exchange.ShowPanelBuyStamina());
        }
        else if(task.action == TaskAction.OpenBuyCoin)
        {
            buttonGo.onClick.AddListener(() => UI_ShowResource.Instance.UI_Exchange.ShowPanelBuyCoin());
        }
        
    }

    void ClaimReward()
    {
        if (!taskProgress.isCompleted) return;
        AccountManager.Instance.AddExp(task.expReward);
        foreach(ItemAndAmount itemAndAmount in task.rewards)
        {
            PlayerInventory.Instance.AddItem(itemAndAmount.itemId, itemAndAmount.amount);

        }
        Destroy(transform.gameObject);
    }

    
}
