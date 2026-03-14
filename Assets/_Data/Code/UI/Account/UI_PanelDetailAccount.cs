using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_PanelDetailAccount : MonoBehaviour, IObserver
{
    [SerializeField] UI_PanelAccount panelAccount;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI renameCost;
    [SerializeField] private TextMeshProUGUI informText;
    [SerializeField] private TextMeshProUGUI heroOwnedText;
    [SerializeField] private TextMeshProUGUI currentStageText;

    [SerializeField] private Transform panelRename;
    [SerializeField] private TMP_InputField inputName;

    [SerializeField] private Button buttonCloseDetail;
    [SerializeField] private Button buttonRename;
    [SerializeField] private Button buttonOkRename;
    [SerializeField] private Button buttonCancelRename;

    [SerializeField] private Slider sliderExp;
    string emptyText = "New name canot be empty!";
    string duplicateText = "New name must be different from current name!";


    void Awake()
    {
        PlayerInventory.Instance.AddObserver(this);
    }
    private void OnEnable()
    {
        RefreshUI();
    
    }
    void RefreshUI()
    {
        nameText.text = $"{AccountManager.Instance.AccountI.namePlayer}";
        levelText.text = $"Lv. {AccountManager.Instance.AccountP.level}";
        int maxExp = AccountManager.Instance.GetExpToNextLevel(AccountManager.Instance.AccountP.level, out bool isMaxLevel);
        int currentExp = AccountManager.Instance.AccountP.exp;
        sliderExp.maxValue = maxExp;
        sliderExp.value = currentExp;
        expText.text = $"{currentExp}/{maxExp}";
        heroOwnedText.text = $"Heroes Owned: {PlayerInventory.Instance.GetHeroCount()}";
        currentStageText.text = $"Current Stage: {ProgressManager.Instance.GetChapter()}-{ProgressManager.Instance.GetStage()}";
        CheckButtonOk();
        buttonCloseDetail.onClick.RemoveAllListeners();
        buttonCloseDetail.onClick.AddListener(() => transform.gameObject.SetActive(false));
        buttonRename.onClick.RemoveAllListeners();
        buttonRename.onClick.AddListener(() => LoadPanelRename());
        buttonOkRename.onClick.RemoveAllListeners();
        buttonOkRename.onClick.AddListener(() => OnClickChangeName());
        buttonCancelRename.onClick.RemoveAllListeners();
        buttonCancelRename.onClick.AddListener(() => panelRename.gameObject.SetActive(false));
    }
    void LoadPanelRename()
    {
        panelRename.gameObject.SetActive(true);
    }
    void OnClickChangeName()
    {
        string newName = inputName.text;
        string currentName = AccountManager.Instance.AccountI.namePlayer;
        newName = (newName ?? string.Empty).Trim();
        currentName = (currentName ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(newName) )
        {
           informText.text = emptyText;
           informText.transform.gameObject.SetActive(true);
            return;
        }
        else if (newName == currentName)
        {
            informText.text = duplicateText;
            informText.transform.gameObject.SetActive(true);
            return;
        }
        informText.transform.gameObject.SetActive(false);

        AccountManager.Instance.SetNewName(newName);
        RefreshUI();
        PlayerInventory.Instance.ConsumeItem(2, 50);
        panelRename.gameObject.SetActive(false);
        panelAccount.RefreshUI();

    }
    void CheckButtonOk()
    {
        if(PlayerInventory.Instance.GetItemQuantity(2) >= 50)
        {
            renameCost.color = Color.white;
            buttonOkRename.interactable = true;
        }
        else
        {
            renameCost.color = Color.red;
            buttonOkRename.interactable = false;
        }
    }
    public void OnNotify(object data)
    {

        if (data is ValueTuple<int, int> tuple)
        {
            int itemId = tuple.Item1;
            int value = tuple.Item2;

            if (itemId == 2)
                CheckButtonOk();
           
        }
    }
}
