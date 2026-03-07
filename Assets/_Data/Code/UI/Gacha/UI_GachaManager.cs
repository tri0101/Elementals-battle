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
        standardPanel.gameObject.SetActive(true);
        GachaManager.Instance.SetActiveBanner(GachaManager.Instance.StandardBanner);
        featuredPanel.gameObject.SetActive(false);
    }
    public void LoadFeatured()
    {
        featuredPanel.gameObject.SetActive (true);
        GachaManager.Instance.SetActiveBanner(GachaManager.Instance.FeaturedBanner);
        standardPanel.gameObject.SetActive(false);
    }
}
