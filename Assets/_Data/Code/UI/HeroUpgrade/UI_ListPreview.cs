using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ListPreview : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] Transform infoTextPanel;
    [SerializeField] Transform tagPanel;
    [SerializeField] Transform skillPanel;
    [SerializeField] Transform fightSoulPanel;
    [SerializeField] Transform contentPanel;

    [Header("GameObjects")]
    [SerializeField] GameObject tagObject;
    [SerializeField] GameObject textObject;
    [SerializeField] GameObject skillObject;
    [SerializeField] GameObject fightSoul;

    void OnEnable()
    {
        if (HeroUpgradeContext.Mode != HeroUpgradeContext.OpenMode.Preview)
            return;

        RefreshUI(HeroUpgradeContext.SelectedHero);
    }

    public void RefreshUI(HeroViewData selected)
    {
        ClearAll();

        if (selected == null || selected.info == null)
            return;

        ResetContentPanelY();
        PopulateInfoText(selected.info);
        PopulateTags(selected.info);
        PopulateSkills(selected.info);
        PopulateSouls(selected.info);

    }

    private void ResetContentPanelY()
    {
        if (contentPanel == null) return;

        var rt = contentPanel.GetComponent<RectTransform>();
        if (rt == null) return;

        // đưa content về đầu (y = 0) sau mỗi lần refresh (cho ScrollRect)
        Vector2 pos = rt.anchoredPosition;
        pos.y = 0f;
        rt.anchoredPosition = pos;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    private void PopulateInfoText(HeroInfo info)
    {
        if (infoTextPanel == null || textObject == null || info == null) return;

        AddLine($"Damage: {Mathf.RoundToInt(info.damage)}");
        AddLine($"Health: {Mathf.RoundToInt(info.health)}");
        AddLine($"Armor: {Mathf.RoundToInt(info.armor)}");
        AddLine($"Speed: {info.speed:0.#}");

        AddLine($"Crit Rate: {info.criticalRate * 1f:0.#}%");
        AddLine($"Crit Damage: {info.criticalDamageRate:0.#}%");
        AddLine($"Life Steal: {info.lifeSteal * 1f:0.#}%");
        AddLine($"Control-Free: {info.controlFree * 1f:0.#}%");
    }

    private void AddLine(string content)
    {
        if (infoTextPanel == null || textObject == null) return;

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

    private void PopulateSkills(HeroInfo info)
    {
        if (skillPanel == null || skillObject == null || info == null) return;

        AddSkillIcon(info.skill);
        AddSkillIcon(info.ultimate);
        AddSkillIcon(info.empower);
        AddSkillIcon(info.passive);
      
    }

    private void AddSkillIcon(AbilityInfo ability)
    {
        if (ability == null ) return;
        if (skillPanel == null || skillObject == null) return;

        GameObject go = Instantiate(skillObject, skillPanel);

        
        UI_SkillPreviewItem uI_SkillPreviewItem = go.GetComponent<UI_SkillPreviewItem>();
        uI_SkillPreviewItem.Setup(ability.icon, ability.abilityName, ability.type, "Lv.1",ability.description);


    }

    private void PopulateSouls(HeroInfo info)
    {
        if (fightSoulPanel == null || fightSoul == null || info == null || info.soulID == null || info.soulID.Count == 0)
            return;

        int soulId = info.soulID[0];
        if (soulId <= 0) return;

        Instantiate(fightSoul, fightSoulPanel);
    }

    private void ClearAll()
    {
        ClearChildren(infoTextPanel);
        ClearChildren(tagPanel);
        ClearChildren(skillPanel);
        ClearChildren(fightSoulPanel);
    }

    private void ClearChildren(Transform parent)
    {
        if (parent == null) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var ch = parent.GetChild(i);
            if (ch == null) continue;
            Destroy(ch.gameObject);
        }
    }
}