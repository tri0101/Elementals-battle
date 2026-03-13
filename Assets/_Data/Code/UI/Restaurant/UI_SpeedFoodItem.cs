using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SpeedFoodItem : MonoBehaviour
{
    [SerializeField] private Button buttonUp;
    [SerializeField] private Image icon;
    [SerializeField] private Image backNo;
    [SerializeField] private TextMeshProUGUI amountText;

    [Header("Click Fly Animation (Clone)")]
    [SerializeField] private Vector2 flyTargetAnchoredPos = new Vector2(375f, 400f);
    [SerializeField] private float flyDuration = 0.25f;

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
            buttonUp.enabled = hasItem;

        if (backNo != null)
            backNo.gameObject.SetActive(!hasItem);
    }

    void OnClick()
    {
        if (onClick == null || itemData == null || currentQuantity <= 0)
            return;

        // 1) Spawn clone to fly (do not move the original)
        SpawnFlyClone();

        // 2) Apply effect immediately (UI_PanelHeroRestaurant will reload list + animation bar)
        onClick.Invoke(itemData);
    }

    void SpawnFlyClone()
    {
        // clone as sibling of current parent (="ngang hàng với cha hiện tại")
        Transform parent = transform.parent;
        Transform newParent = parent != null ? parent.parent : null;
        if (newParent == null)
            newParent = parent;

        GameObject clone = Instantiate(gameObject, newParent, true);

        // Ensure clone is above others
        clone.transform.SetAsLastSibling();

        // Remove interactivity on clone
        var cloneButton = clone.GetComponentInChildren<Button>(true);
        if (cloneButton != null) cloneButton.interactable = false;

        // Hide amount text on clone (yêu cầu: xóa text amount)
        var cloneUI = clone.GetComponent<UI_SpeedFoodItem>();
        if (cloneUI != null && cloneUI.amountText != null)
            cloneUI.amountText.gameObject.SetActive(false);

        // Ensure overlay off on clone
        if (cloneUI != null && cloneUI.backNo != null)
            cloneUI.backNo.gameObject.SetActive(false);

        // Start fly coroutine on clone
        var runner = clone.AddComponent<FlyCloneRunner>();
        runner.Play(clone.transform as RectTransform, flyTargetAnchoredPos, flyDuration);
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

    /// <summary>
    /// Helper runner living only on the clone, to animate then destroy it.
    /// </summary>
    private sealed class FlyCloneRunner : MonoBehaviour
    {
        private RectTransform rect;
        private Vector2 target;
        private float duration;

        public void Play(RectTransform rect, Vector2 targetAnchoredPos, float durationSeconds)
        {
            this.rect = rect;
            target = targetAnchoredPos;
            duration = durationSeconds;
            StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            if (rect == null)
            {
                Destroy(gameObject);
                yield break;
            }

            Vector2 from = rect.anchoredPosition;
            Vector2 to = target;

            float t = 0f;
            float dur = duration > 0f ? duration : 0.01f;

            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                float k = t / dur;
                if (k > 1f) k = 1f;

                float eased = k * k * (3f - 2f * k); // smoothstep
                rect.anchoredPosition = Vector2.Lerp(from, to, eased);
                yield return null;
            }

            rect.anchoredPosition = to;
            Destroy(gameObject);
        }
    }
}