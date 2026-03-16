using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DropRewardItem : MonoBehaviour
{

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;
    private ItemData itemData;
    private int amount;
    void Awake()
    {

    }

    public void Setup(ItemData itemData, int amount)
    {
        
        if (itemData == null) return;
        this.itemData = itemData;
        this.amount = amount;
        GetComponent<Image>().color = itemData.colorFrame;
        icon.sprite = itemData.icon;
        
        amountText.text = amount.ToString();
        ApplyIconTransform(itemData);
        RefreshAmountText();


    }
    public void AddAmount(int add)
    {
        if (add <= 0) return;
        amount += add;
        RefreshAmountText();
    }


    private void RefreshAmountText()
    {
        if (amountText != null)
            amountText.text = amount.ToString();
    }
    public void SetUpColorText(Color color)
    {
        amountText.color = color;

    }
    void ApplyIconTransform(ItemData item)
    {
        if (icon == null || item == null) return;

        Vector3 scale = Vector3.one;
        float rotZ = 0f;

        if (item.id == 100)
        {
            scale = new Vector3(0.5f, 1.25f, 1f);
            rotZ = 45f;
        }

        else if (!string.IsNullOrEmpty(item.itemName) &&
                 item.itemName.IndexOf("Sword", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            scale = new Vector3(0.5f, 1.25f, 1f);
            rotZ = -45f;
        }
        else
        {
            scale = Vector3.one;
            rotZ = 0f;
        }
        if(item.id == 0)  icon.transform.localScale = new Vector3(1f, 0.5f, 1f);
        else icon.transform.localScale = scale;
        icon.transform.localRotation = Quaternion.Euler(0f, 0f, rotZ);
    }
}