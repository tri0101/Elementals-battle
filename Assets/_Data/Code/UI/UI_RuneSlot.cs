using UnityEngine;
using UnityEngine.UI;

public class UI_RuneSlot : MonoBehaviour
{
    public Button button;
    public RuneData currentRune;

    private UIRuneItem sourceItem; // 👈 rune này đến từ đâu

    public bool IsEmpty => currentRune == null;
    public Transform imageFull;

    private void Awake()
    {
        button = transform.GetChild(1).GetComponent<Button>();
        button.onClick.AddListener(OnClickSlot);
        imageFull = transform.GetChild(0);
    }

    public void SetRune(RuneData rune, UIRuneItem source)
    {
        currentRune = rune;
        sourceItem = source;

        button.image.sprite = rune.icon;
        button.image.enabled = true;
        button.gameObject.SetActive(true);
        imageFull.gameObject.SetActive(true);
    }

    void OnClickSlot()
    {
        if (currentRune == null) return;

        // trả rune về đúng bag item
        sourceItem.OnRuneRemovedFromPage();
        imageFull.gameObject.SetActive(false);
        Clear();
    }

    public void Clear()
    {
        currentRune = null;
        sourceItem = null;

        button.image.sprite = null;
        button.image.enabled = false;
        button.gameObject.SetActive(false);
    }
}
