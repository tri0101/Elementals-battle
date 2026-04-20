using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_SoulItem : MonoBehaviour
{
    [SerializeField] private Image soulIcon;
    [SerializeField] private Button button;
    SoulViewData currentSoulViewData;
    int currentIndex; // hồn lực thứ mấy của hero
    public Action<SoulViewData,int> OnButtonClicked;


    public void SetUp(SoulViewData soulData, int index,  Action <SoulViewData, int> onClick)
    {
        currentIndex = index;
        soulIcon.sprite = soulData.info.spriteSoul;
        button.onClick.RemoveAllListeners();
        currentSoulViewData = soulData;
        OnButtonClicked = onClick;
        
        button.onClick.AddListener(OnClick);
    }
    private void OnClick()
    {
        OnButtonClicked?.Invoke(currentSoulViewData, currentIndex);
    }
}
