using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SpeedFoodItem : MonoBehaviour
{
    [SerializeField] private Button buttonUp;
    [SerializeField] private Image icon;
    [SerializeField] private Image backNo;
    [SerializeField] private TextMeshProUGUI amountText;

    private ItemData itemData;
    private int currentQuantity;
    private Action<ItemData> onClick;

    void Awake()
    {
        if (buttonUp != null)
        {
            buttonUp.onClick.RemoveAllListeners();
            buttonUp.onClick.AddListener(OnClick);
        }
    }

    public void Setup(ItemData itemData, int quantity, Action<ItemData> onClick = null)
    {
        this.itemData = itemData;
        this.currentQuantity = quantity;
        this.onClick = onClick;

        if (itemData == null)
        {
            if (icon != null) icon.sprite = null;
            if (amountText != null) amountText.text = "0";
            if (buttonUp != null) buttonUp.interactable = false;
            if (backNo != null) backNo.gameObject.SetActive(true);
            return;
        }

        if (icon != null)
        {
            icon.sprite = itemData.icon;
            ApplyIconTransform(itemData);
        }

        if (amountText != null)
            amountText.text = quantity.ToString();

        var bg = GetComponent<Image>();
        if (bg != null) bg.color = itemData.colorFrame;

        bool hasItem = quantity > 0;

        if (buttonUp != null)
            buttonUp.interactable = hasItem;

        if (backNo != null)
            backNo.gameObject.SetActive(!hasItem);
    }

    void OnClick()
    {
        if (onClick != null && itemData != null && currentQuantity > 0)
            onClick.Invoke(itemData);
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