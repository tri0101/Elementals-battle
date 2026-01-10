using UnityEngine;
using UnityEngine.UI;

public class UIRuneItem : MonoBehaviour
{
    public Image icon;

    [SerializeField] private RuneData runeData;

    public void Setup(RuneData data)
    {
        runeData = data;
        icon.sprite = data.icon; // sprite trong RuneData
    }

    public void OnClick()
    {
        Debug.Log("Click rune: " + runeData.runeName);
        // sau này: gán vào bảng ngọc
    }
}
