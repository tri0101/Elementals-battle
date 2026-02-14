using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UI_SkillUpgradeItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costUp;
    
    Button button;
    
    private Action<ItemData> onClickCallback;

    public void Setup(Sprite Icon, string name, int currentLevel, int cost, Action<ItemData> onClick = null)
    {






        //Hiển thị UI
        if (icon != null)
            icon.sprite = Icon;
        if (levelText != null)
            levelText.text = "Lv." + currentLevel.ToString();
        if (costUp != null)
            costUp.text = cost.ToString();
        if (nameText != null)
            nameText.text = name;

       

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

    }

    


  

   
    
    void OnClick()
    {
        
    }

}