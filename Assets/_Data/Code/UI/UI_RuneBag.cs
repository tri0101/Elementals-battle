using UnityEngine;

public class UI_RuneBag : MonoBehaviour
{
    public Transform contentParent;
    public GameObject runeItemPrefab;

    private void Awake()
    {
        if (contentParent == null)
            contentParent = transform;
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        // clear UI
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // build lại từ inventory
        foreach (var stack in heroInventory.Instance.ownedRunes)
        {
            GameObject go = Instantiate(runeItemPrefab, contentParent);
            UIRuneItem ui = go.GetComponent<UIRuneItem>();
            ui.Setup(stack); 
            ui.gameObject.SetActive(true);
        }
    }

    
    public void AddRune(RuneData rune)
    {
        heroInventory.Instance.AddRune(rune, 1);
        Refresh(); 
    }
}
