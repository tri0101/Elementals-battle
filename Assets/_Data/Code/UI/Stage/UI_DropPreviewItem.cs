using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DropPreviewItem : MonoBehaviour
{
    
    public Image icon;
    

    void Awake()
    {

    }

    public void Setup(ItemData itemData, DropItemData dropData)
    {
        if (itemData == null) return;

        icon.sprite = itemData.icon;
        ApplyIconTransform(itemData);

        
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

        icon.transform.localScale = scale;
        icon.transform.localRotation = Quaternion.Euler(0f, 0f, rotZ);
    }
}