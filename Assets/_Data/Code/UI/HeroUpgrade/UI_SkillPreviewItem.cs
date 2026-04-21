using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillPreviewItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button buttonDetail;

    private string _skillName;
    private string _description;
    private AbilityType _abilityType;
    private string _levelText;

    public void Setup(
        Sprite Icon,
        string name,
        AbilityType abilityType,
        string levelText = "Lv.1",
        string description = ""
    )
    {
        _skillName = name;
        _abilityType = abilityType;
        _levelText = levelText;
        _description = description;

        if (icon != null) icon.sprite = Icon;

        if (buttonDetail != null)
        {
            buttonDetail.onClick.RemoveAllListeners();
            buttonDetail.onClick.AddListener(OnClickDetail);
        }
    }

    private void OnClickDetail()
    {
        var root = transform.parent.parent.parent.parent.parent;
        if (root == null) return;

        Transform panelRoot = root.Find("PanelDetailDes");
        if (panelRoot == null || panelRoot.childCount == 0) return;

        Transform panel = panelRoot.GetChild(0);
        if (panel == null) return;

        var ui = panel.GetComponent<UI_SkillDetail>();
        if (ui == null) ui = panel.GetComponentInChildren<UI_SkillDetail>(true);
        if (ui == null) return;

        ui.transform.parent.gameObject.SetActive(true);

        string type = _abilityType.ToString();
        ui.SetUp(icon, _skillName ?? string.Empty, _levelText ?? string.Empty, type, _description ?? string.Empty);
    }
}