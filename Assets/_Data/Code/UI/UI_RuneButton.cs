using UnityEngine;

public class UI_RuneButton : MonoBehaviour
{
    public RuneData runeData;

    public void OnClick()
    {
        GameManager.Instance.selectedRunes.Add(runeData);
        Debug.Log("Chọn ngọc: " + runeData.runeName);
    }
}
