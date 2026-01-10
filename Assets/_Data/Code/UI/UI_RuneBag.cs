using UnityEngine;

public class UI_RuneBag : MonoBehaviour
{
    public Transform contentParent;      // Grid / Vertical Layout Group
    public GameObject runeItemPrefab;    // UI_RuneItem


    private void Awake()
    {
        contentParent = transform;
    }
    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        // Xóa UI cũ
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Spawn UI theo inventory
        foreach (var rune in PlayerInventory.Instance.ownedRunes)
        {
            GameObject go = Instantiate(runeItemPrefab, contentParent);
            UIRuneItem ui = go.GetComponent<UIRuneItem>();
            ui.gameObject.SetActive(true);
            ui.Setup(rune);
        }
    }
}
