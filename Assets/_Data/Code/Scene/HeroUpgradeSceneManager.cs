using UnityEngine;

public class HeroUpgradeSceneManager : MonoBehaviour
{
    [SerializeField] Transform panelRank;
    [SerializeField] Transform panelLevel;
    [SerializeField] Transform panelSkill;
    [SerializeField] Transform panelStar;
    [SerializeField] Transform panelFightSoul;
    [SerializeField] Transform barLevel;
    [SerializeField] Transform barStar;

    public void LoadPanelRank()
    {
        barLevel.gameObject.SetActive(true);
        barStar.gameObject.SetActive(false);
        panelRank.gameObject.SetActive(true);
        panelLevel.gameObject.SetActive(false);
        panelSkill.gameObject.SetActive(false);
        panelStar.gameObject.SetActive(false);
        panelFightSoul.gameObject.SetActive(false);
    }
    public void LoadPanelLevel()
    {
        barLevel.gameObject.SetActive(true);
        barStar.gameObject.SetActive(false);
        panelLevel.gameObject.SetActive(true);
        panelRank.gameObject.SetActive(false);
        panelSkill.gameObject.SetActive(false);
        panelStar.gameObject.SetActive(false);
        panelFightSoul.gameObject.SetActive(false);
    }
    public void LoadPanelSKill()
    {
     
        panelSkill.gameObject.SetActive(true);
        panelRank.gameObject.SetActive(false);
        panelLevel.gameObject.SetActive(false);
        panelStar.gameObject.SetActive(false);
        panelFightSoul.gameObject.SetActive(false);
    }
    public void LoadPanelStar()
    {
        barLevel.gameObject.SetActive(false);
        barStar.gameObject.SetActive(true);
        panelSkill.gameObject.SetActive(false);
        panelRank.gameObject.SetActive(false);
        panelLevel.gameObject.SetActive(false);
        panelStar.gameObject.SetActive(true);
        panelFightSoul.gameObject.SetActive(false);
    }

    public void LoadPanelFightSoul()
    {
        panelFightSoul.gameObject.SetActive(true);
        panelSkill.gameObject.SetActive(false);
        panelRank.gameObject.SetActive(false);
        panelLevel.gameObject.SetActive(false);
        panelStar.gameObject.SetActive(false);

    }
}
