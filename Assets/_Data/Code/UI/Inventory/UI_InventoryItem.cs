using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryItem : MonoBehaviour
{
    [SerializeField] private Button buttonDetail;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;

    private ItemData _itemData;
    private int _quantity;
    private Action<GameObject,ItemData, int> _onClick;

    void Awake()
    {
        if (buttonDetail != null)
        {
            buttonDetail.onClick.RemoveAllListeners();
            buttonDetail.onClick.AddListener(HandleClick);
        }
    }

    public void Setup(ItemData itemData, int quantity, Action<GameObject,ItemData, int> onClick = null)
    {
        _itemData = itemData;
        _quantity = quantity;
        _onClick = onClick;

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

        GetComponent<Image>().color = itemData.colorFrame;
        if (buttonDetail != null)
            buttonDetail.interactable = quantity > 0;
    }

    private void HandleClick()
    {
        if (_itemData == null || _quantity <= 0) return;
        {
            GameObject itemCopy = Instantiate(transform.gameObject, null);
            itemCopy.gameObject.SetActive(false);
            var rt = itemCopy.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(150f, 150f);
            }
            Button copyButton = itemCopy.GetComponent<Button>();
            copyButton.enabled = false;
            Transform backText = itemCopy.transform.Find("BackText");
            backText.gameObject.SetActive(false);
            _onClick?.Invoke(itemCopy, _itemData, _quantity);
        }
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