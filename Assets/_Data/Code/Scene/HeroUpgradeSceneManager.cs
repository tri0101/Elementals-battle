using UnityEngine;

public class HeroUpgradeSceneManager : MonoBehaviour
{
    [SerializeField] Transform panelRank;
    [SerializeField] Transform panelLevel;

    public void LoadPanelRank()
    {
        panelRank.gameObject.SetActive(true);
        panelLevel.gameObject.SetActive(false);
    }
    public void LoadPanelLevel()
    {
        panelLevel.gameObject.SetActive(true);
        panelRank.gameObject.SetActive(false);
    }
}
