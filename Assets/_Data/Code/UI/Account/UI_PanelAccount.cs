using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UI_PanelAccount : MonoBehaviour, IObserver
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;


    [SerializeField] private Transform panelDetail;
    [SerializeField] private Button buttonDetail;



    private void OnEnable()
    {
        RefreshUI();
        AccountManager.Instance.AddObserver(this);
    }
    public void RefreshUI()
    {
        var accountI = AccountManager.Instance.AccountI;
        var accountP = AccountManager.Instance.AccountP;
        nameText.text = $"{accountI.namePlayer}";
        levelText.text = $"Lv. {accountP.level}";
        buttonDetail.onClick.RemoveAllListeners();
        buttonDetail.onClick.AddListener(() => LoadPanelDetail());

    }

    void LoadPanelDetail()
    {
        panelDetail.gameObject.SetActive(true);
       
    }
    public void OnNotify()
    {
        RefreshUI();
    }
}
