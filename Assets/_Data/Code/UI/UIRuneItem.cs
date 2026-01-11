using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRuneItem : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public Button buttonParent;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI statTextPrefab;

    [Header("Refs")]
    public UI_RunePage runePage;
    public GameObject UIStat;

    private RuneStack stack;
    private int usedCount = 0; // số rune đang dùng trong page

    // ================= SETUP =================
    public void Setup(RuneStack runeStack)
    {
        stack = runeStack;
        usedCount = 0;

        icon.sprite = stack.rune.icon;
        UpdateCountText();

        ClearStats();
        BuildStats();
    }

    void UpdateCountText()
    {
        int available = stack.count - usedCount;
        countText.text = "x" + available;

        buttonParent.interactable = available > 0;
    }

    void ClearStats()
    {
        for (int i = buttonParent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = buttonParent.transform.GetChild(i);
            if (child.GetComponent<TextMeshProUGUI>() != null)
                Destroy(child.gameObject);
        }
    }

    void BuildStats()
    {
        foreach (var statValue in stack.rune.stats)
        {
            TextMeshProUGUI statText =
                Instantiate(statTextPrefab, buttonParent.transform);

            string valueText = statValue.isPercent
                ? $"+{statValue.value}%"
                : $"+{statValue.value}";

            statText.text = $"{valueText} {statValue.stat}";
            statText.enabled = true;
        }
    }
    private void Start()
    {
        buttonParent.onClick.AddListener(OnClick);
    }
    // ================= CLICK =================
    void OnClick()
    {
        if (runePage.TryAddRune(stack.rune, this))
        {
            usedCount++;
            UpdateCountText();
        }
    }
    // ================= SLOT CALLBACK =================
    public void OnRuneRemovedFromPage()
    {
        usedCount--;
        UpdateCountText();
    }
}
