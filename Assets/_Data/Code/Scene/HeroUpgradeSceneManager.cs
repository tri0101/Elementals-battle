using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeroUpgradeSceneManager : MonoBehaviour
{
    public enum PanelType
    {
        Rank,
        Level,
        Skill,
        Star,
        FightSoul,
        InfoUpgrade
    }

    [System.Serializable]
    public class PanelData
    {
        public PanelType type;
        public GameObject panel;

        [Header("Optional")]
        public Image buttonImage;
    }

    [SerializeField] private List<PanelData> panels;

    [Header("Bars")]
    [SerializeField] private GameObject barLevel;
    [SerializeField] private GameObject barStar;

    [Header("Sprites")]
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite normalSprite;

    private PanelType currentPanel;

    private void Start()
    {
        LoadPanel(PanelType.Rank);
    }

    public void LoadPanel(PanelType type)
    {
        currentPanel = type;

        foreach (var data in panels)
        {
            bool isSelected = data.type == type;

            if (data.panel != null)
                data.panel.SetActive(isSelected);

            if (data.buttonImage != null)
            {
                data.buttonImage.sprite =
                    isSelected ? selectedSprite : normalSprite;
            }
        }

        UpdateBars(type);
    }

    void UpdateBars(PanelType type)
    {
        bool showLevelBar =
            type == PanelType.Rank ||
            type == PanelType.Level;

        bool showStarBar =
            type == PanelType.Star;

        if (barLevel != null)
            barLevel.SetActive(showLevelBar);

        if (barStar != null)
            barStar.SetActive(showStarBar);
    }

    public void LoadPanelRank()
    {
        LoadPanel(PanelType.Rank);
    }

    public void LoadPanelLevel()
    {
        LoadPanel(PanelType.Level);
    }

    public void LoadPanelSkill()
    {
        LoadPanel(PanelType.Skill);
    }

    public void LoadPanelStar()
    {
        LoadPanel(PanelType.Star);
    }

    public void LoadPanelFightSoul()
    {
        LoadPanel(PanelType.FightSoul);
    }

    public void LoadPanelInfoUpgrade()
    {
        LoadPanel(PanelType.InfoUpgrade);
    }
}