using UnityEngine;
using UnityEngine.UI;

public class UI_GachaManager : MonoBehaviour
{
    [SerializeField] private Button buttonStandard;
    [SerializeField] private Button buttonFeatured;
    [SerializeField] private Transform standardPanel;
    [SerializeField] private Transform featuredPanel;
    private void Awake()
    {
        buttonStandard.onClick.RemoveAllListeners();
        buttonStandard.onClick.AddListener(() => LoadStandard());
        buttonFeatured.onClick.RemoveAllListeners();
        buttonFeatured.onClick.AddListener(()=>LoadFeatured());
    }

    public void LoadStandard()
    {
       
        GachaManager.Instance.SetActiveBanner(GachaManager.Instance.StandardBanner);
        UI_CostGacha costGacha = standardPanel.GetComponent<UI_CostGacha>();
        costGacha.RefreshUI();
        standardPanel.gameObject.SetActive(true);
        featuredPanel.gameObject.SetActive(false);
    }
    public void LoadFeatured()
    {
        
        GachaManager.Instance.SetActiveBanner(GachaManager.Instance.FeaturedBanner);
        UI_CostGacha costGacha = featuredPanel.GetComponent<UI_CostGacha>();
        costGacha.RefreshUI();
        featuredPanel.gameObject.SetActive(true);
        standardPanel.gameObject.SetActive(false);
    }
}
