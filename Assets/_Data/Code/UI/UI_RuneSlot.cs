using UnityEngine;
using UnityEngine.UI;

public class UI_RuneSlot : MonoBehaviour
{
    public Button button;
    public RuneData currentRune;

    public bool IsEmpty => currentRune == null;

    private void Awake()
    {
        button = transform.GetChild(0).GetComponent<Button>();
    }
    public void SetRune(RuneData rune)
    {
        currentRune = rune;
        button.image.sprite = rune.icon;
        button.image.enabled = true;
        button.gameObject.SetActive(true);  
    }

    public void Clear()
    {
        currentRune = null;
        button.image.sprite = null;
        button.image.enabled = false;
        button.gameObject.SetActive(false);
    }
}
