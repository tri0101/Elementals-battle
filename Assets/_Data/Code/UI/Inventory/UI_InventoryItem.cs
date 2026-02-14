using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_InventoryItem : MonoBehaviour
{
    [SerializeField] private Button buttonDetail;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;

    void Awake()
    {

    }

    public void Setup(ItemData itemData, int quantity)
    {
       
        if (itemData == null)
        {
            if (icon != null) icon.sprite = null;
            if (amountText != null) amountText.text = "0";
            if (buttonDetail != null) buttonDetail.interactable = false;
            return;
        }

        if (icon != null)
        {
            icon.sprite = itemData.icon;
            ApplyIconTransform(itemData);
        }

        if (amountText != null)
            amountText.text = quantity.ToString();

        if (buttonDetail != null)
            buttonDetail.interactable = quantity > 0;
    }

    void ApplyIconTransform(ItemData item)
    {
        if (icon == null || item == null ) return;

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

        icon.transform.localScale = scale;
        icon.transform.localRotation = Quaternion.Euler(0f, 0f, rotZ);
    }
}