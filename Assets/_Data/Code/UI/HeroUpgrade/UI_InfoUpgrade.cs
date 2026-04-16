using TMPro;
using UnityEngine;

public class UI_InfoUpgrade : MonoBehaviour
{
    [SerializeField] Transform infoText;
    [SerializeField] GameObject textObject;
    [SerializeField] HeroGrowthConfig growthConfig;

    private void OnEnable()
    {
        ClearLines();

        HeroViewData selected = HeroUpgradeContext.SelectedHero;
        if (selected == null || selected.info == null || selected.instance == null)
            return;

        if (infoText == null || textObject == null)
            return;

        // Lấy growthConfig giống cách các UI khác đang dùng (từ Header trong scene)
       

        HeroStat stat = HeroStatCalculator.Calculate(selected.info, selected.instance, growthConfig);

        AddLine($"Damage: {Mathf.RoundToInt(stat.damage)}");
        AddLine($"Health: {Mathf.RoundToInt(stat.health)}");
        AddLine($"Armor: {Mathf.RoundToInt(stat.armor)}");
        AddLine($"Speed: {stat.speed:0.#}");

        // percent fields
        AddLine($"Crit Rate: {stat.critRate:0.#}%");
        AddLine($"Crit Damage: {stat.critDamage:0.#}%");
        AddLine($"Life Steal: {stat.lifeSteal:0.#}%");

        
    }

    private void AddLine(string content)
    {
        GameObject go = Instantiate(textObject, infoText);
        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
            tmp = go.GetComponentInChildren<TextMeshProUGUI>();

        if (tmp != null)
            tmp.text = content;
    }

    private void ClearLines()
    {
        if (infoText == null) return;

        for (int i = infoText.childCount - 1; i >= 0; i--)
        {
            Transform ch = infoText.GetChild(i);
            if (ch == null) continue;
            Destroy(ch.gameObject);
        }
    }
}