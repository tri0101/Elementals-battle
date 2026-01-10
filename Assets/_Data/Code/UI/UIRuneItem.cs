using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRuneItem : MonoBehaviour
{
    public Image icon;
    public Button buttonParent;
    
    public UI_RunePage runePage;
    public TextMeshProUGUI statTextPrefab; // prefab TMP

    [SerializeField] private RuneData runeData;

    public void Setup(RuneData data)
    {
        runeData = data;

        // set icon
        icon.sprite = data.icon;

        // clear stat cũ (nếu reuse prefab)
        for (int i = buttonParent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = buttonParent.transform.GetChild(i);
            if (child.GetComponent<TextMeshProUGUI>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        // tạo text cho từng stat
        foreach (var statValue in runeData.stats)
        {
            TextMeshProUGUI statText =
                Instantiate(statTextPrefab, buttonParent.transform);

            string valueText = statValue.isPercent
                ? $"+{statValue.value}%"
                : $"+{statValue.value}";

            string statName = statValue.stat.ToString();

            statText.text = $"{valueText} {statName}";
            statText.enabled = true;
        }
    }


    private void Start()
    {
        buttonParent.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        bool added = runePage.TryAddRune(runeData);
        if (added)
        {
            Destroy(gameObject); // ❗ XÓA RUNE KHỎI BAG
        }
    }
}
