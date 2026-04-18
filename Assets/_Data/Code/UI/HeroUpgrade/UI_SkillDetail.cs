using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_SkillDetail : MonoBehaviour
{
    [SerializeField] private Image imageSkill;
    [SerializeField] private TextMeshProUGUI textNameSkill;
    [SerializeField] private TextMeshProUGUI textLevelSkill;
    [SerializeField] private TextMeshProUGUI textTypeSkill;
    [SerializeField] private Button buttonClose;
    [SerializeField] private TextMeshProUGUI textDescriptionSkill;

    private void Awake()
    {
        buttonClose.onClick.RemoveAllListeners();
        buttonClose.onClick.AddListener(() => transform.parent.gameObject.SetActive(false));
    }

    public void SetUp(Image imageSkill, string name, string level, string type, string des)
    {
        Clear();
        imageSkill.sprite = imageSkill.sprite;
        textNameSkill.text = name;
        textLevelSkill.text = level;
        textTypeSkill.text = type;
        textDescriptionSkill.text = des;
    }
    public void Clear()
    {
        if (imageSkill != null)
            imageSkill.sprite = null;

        if (textNameSkill != null)
            textNameSkill.text = string.Empty;

        if (textLevelSkill != null)
            textLevelSkill.text = string.Empty;

        if (textTypeSkill != null)
            textTypeSkill.text = string.Empty;

        if (textDescriptionSkill != null)
            textDescriptionSkill.text = string.Empty;
    }
}
