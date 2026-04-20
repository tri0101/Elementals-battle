using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryInfo : MonoBehaviour
{
    [SerializeField] private Transform holder;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI amountText;

    public void Setup(GameObject itemCopy, ItemData itemData,int quantity)
    {

        ClearOldItem();
        itemCopy.transform.SetParent(holder, false);
        itemCopy.transform.localPosition = Vector3.zero;
        descText.text = itemData.description;
        nameText.text = itemData.itemName;
        if (amountText != null) amountText.text = "Quantity: "+ quantity.ToString();
        itemCopy.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }
    private void ClearOldItem()
    {
        if (holder == null) return;
        if (holder.childCount > 0)
        {
           Destroy(holder.GetChild(0).gameObject);
        }
    }

}