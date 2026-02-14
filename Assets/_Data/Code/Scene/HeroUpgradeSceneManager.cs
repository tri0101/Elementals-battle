using UnityEngine;

public class HeroUpgradeSceneManager : MonoBehaviour
{
    [SerializeField] Transform panelRank;
    [SerializeField] Transform panelLevel;
    [SerializeField] Transform panelSkill;

    public void LoadPanelRank()
    {
        panelRank.gameObject.SetActive(true);
        panelLevel.gameObject.SetActive(false);
        panelSkill.gameObject.SetActive(false);
    }
    public void LoadPanelLevel()
    {
        panelLevel.gameObject.SetActive(true);
        panelRank.gameObject.SetActive(false);
        panelSkill.gameObject.SetActive(false);
    }
    public void LoadPanelSKill()
    {
        panelSkill.gameObject.SetActive(true);
        panelRank.gameObject.SetActive(false);
        panelLevel.gameObject.SetActive(false);
    }
}
