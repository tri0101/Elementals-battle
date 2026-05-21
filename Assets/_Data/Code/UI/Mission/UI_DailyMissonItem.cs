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
    [SerializeField] private UI_PanelMission panelMisson;

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
    private void OnDisable()
    {
        if (DailyTaskManager.Instance != null)
            DailyTaskManager.Instance.RemoveObbserver(this);
    }
    private void OnDestroy()
    {
        DailyTaskManager.Instance?.RemoveObbserver(this);
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
        panelMisson.RefreshClaimAllButton();
    }

    public void SetUp(DailyTask task, DailyTaskProgress taskProgress, UI_PanelMission panelMisson)
    {
        DailyTaskManager.Instance.AddObserver(this);
        this.panelMisson = panelMisson;
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
        else if(task.action == TaskAction.OpenMap)
        {
            buttonGo.onClick.AddListener(() => LoadMapScene());
        }
        else if(task.action == TaskAction.OpenHeroUpgrade)
        {
            buttonGo.onClick.AddListener(() => LoadHeroUpgradeScene());
        }
        else if(task.action == TaskAction.OpenHeroSummon)
        {
            buttonGo.onClick.AddListener(() => LoadHeroSummonScene());
        }
        else if(task.action == TaskAction.OpenShop)
        {
            buttonGo.onClick.AddListener(() => LoadOpenShop());
        }

    }
    void LoadOpenShop()
    {
        GameManager.Instance.LoadAdditiveScene((SceneId)8);
        GameManager.Instance.UnLoadAdditiveScene((SceneId)0);
    }
    void LoadHeroUpgradeScene()
    {
        // Pick owned hero with highest power
        if (PlayerInventory.Instance == null ||
            DatabaseManager.Instance == null ||
            DatabaseManager.Instance.HeroDatabase == null ||
            DatabaseManager.Instance.HeroGrowthConfig == null)
        {
            Debug.LogWarning("[UI_MissonTask] Missing dependencies (Inventory/Database/GrowthConfig).");
            return;
        }

        HeroViewData best = null;
        float bestPower = float.NegativeInfinity;

        var heroes = PlayerInventory.Instance.Heroes;
        for (int i = 0; i < heroes.Count; i++)
        {
            var inst = heroes[i];
            if (inst == null) continue;

            var info = DatabaseManager.Instance.HeroDatabase.GetHero(inst.heroId);
            if (info == null) continue;

            float power = HeroStatCalculator.Calculate(info, inst, DatabaseManager.Instance.HeroGrowthConfig).power;
            if (power > bestPower)
            {
                bestPower = power;
                best = new HeroViewData { info = info, instance = inst };
            }
        }

        if (best == null)
        {
            Debug.LogWarning("[UI_MissonTask] No owned hero found to open upgrade.");
            return;
        }

        HeroUpgradeContext.SelectedHero = best;
        HeroUpgradeContext.Mode = HeroUpgradeContext.OpenMode.Upgrade;

        GameManager.Instance.LoadAdditiveScene(SceneId.HeroUpgradeScene);
        GameManager.Instance.UnLoadAdditiveScene((SceneId)0);
    }
    void LoadHeroSummonScene()
    {
        GameManager.Instance.LoadAdditiveScene((SceneId)1);
        GameManager.Instance.UnLoadAdditiveScene((SceneId)0);
    }
    void LoadMapScene()
    {
        GameManager.Instance.LoadAdditiveScene((SceneId)6);
        GameManager.Instance.UnLoadAdditiveScene((SceneId)0);
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