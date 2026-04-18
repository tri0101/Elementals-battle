using TMPro;
using Unity.Properties;
using UnityEngine;

public class UI_InfoUpgrade : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] Transform infoTextPanel;
    [SerializeField] Transform tagPanel;
    [SerializeField] HeroGrowthConfig growthConfig;
    [Header("GameObjects")]
    [SerializeField] GameObject tagObject;
    [SerializeField] GameObject textObject;

    private void OnEnable()
    {
        ClearLines();
        RefreshUI(HeroUpgradeContext.SelectedHero);
    }

    public void RefreshUI(HeroViewData selected)
    {
        ClearLines();
        if (selected == null || selected.info == null || selected.instance == null)
            return;

        if (infoTextPanel == null || textObject == null)
            return;

        HeroStat stat = HeroStatCalculator.Calculate(selected.info, selected.instance, growthConfig);

        AddLine($"Damage: {Mathf.RoundToInt(stat.damage)}");
        AddLine($"Health: {Mathf.RoundToInt(stat.health)}");
        AddLine($"Armor: {Mathf.RoundToInt(stat.armor)}");
        AddLine($"Speed: {stat.speed:0.#}");

        
        AddLine($"Crit Rate: {stat.critRate:0.#}%");
        AddLine($"Crit Damage: {stat.critDamage:0.#}%");
        AddLine($"Life Steal: {stat.lifeSteal:0.#}%");
        AddLine($"Control-Free: {stat.controlFree:0.#}%");

        
        PopulateTags(selected.info);
    }

    private void AddLine(string content)
    {
        GameObject go = Instantiate(textObject, infoTextPanel);
        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
            tmp = go.GetComponentInChildren<TextMeshProUGUI>();

        if (tmp != null)
            tmp.text = content;
    }

    private void PopulateTags(HeroInfo info)
    {
        if (tagPanel == null || tagObject == null || info == null || info.tags == null || info.tags.Count == 0)
            return;

        
        foreach (var tag in info.tags)
        {
            GameObject go = Instantiate(tagObject, tagPanel);
            TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
            if (tmp == null)
                tmp = go.GetComponentInChildren<TextMeshProUGUI>();

            if (tmp != null)
                tmp.text = tag.ToString();
        }
    }

    private void ClearLines()
    {
        if (infoTextPanel != null)
        {
            for (int i = infoTextPanel.childCount - 1; i >= 0; i--)
            {
                Transform ch = infoTextPanel.GetChild(i);
                if (ch == null) continue;
                Destroy(ch.gameObject);
            }
        }

        if (tagPanel != null)
        {
            for (int i = tagPanel.childCount - 1; i >= 0; i--)
            {
                Transform ch = tagPanel.GetChild(i);
                if (ch == null) continue;
                Destroy(ch.gameObject);
            }
        }
    }
}