using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UI_FoodUpgradeItem : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amountText;
    public GameObject backEmpty;
    Button button;
    Image imagePlus;
    Coroutine glowRoutine;

    private ItemData itemData;
    private int currentAmount;
    private Action<ItemData> onClickCallback;

    public void Setup(ItemData itemData, int amount, Action<ItemData> onClick = null)
    {
        
        EnsureReferences();

        this.itemData = itemData;
        this.currentAmount = amount;
        this.onClickCallback = onClick;


        //Hiển thị UI
        if (icon != null)
            icon.sprite = itemData.icon;
        if (amountText != null)
            amountText.text = amount.ToString();

        bool isEmpty = amount <= 0;
        if (backEmpty != null)
            backEmpty.SetActive(isEmpty);

        if (isEmpty)
        {
            StartGlow();
            if (button != null) button.enabled = false;
        }
        else
        {
            StopGlow();
            if (button != null) button.enabled = true;
        }

        
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
       
    }

    void Awake()
    {
        
        button = GetComponent<Button>() ?? GetComponentInChildren<Button>();
        if (backEmpty != null && backEmpty.transform.childCount > 0)
            imagePlus = backEmpty.transform.GetChild(0).GetComponent<Image>();
    }

    void OnEnable()
    {
       
        EnsureReferences();
        if (backEmpty != null && backEmpty.activeSelf)
            StartGlow();
    }

    void OnDisable()
    {
        
        StopGlow();
    }

  
    void EnsureReferences()
    {
        if (button == null)
            button = GetComponent<Button>() ?? GetComponentInChildren<Button>();

        if (imagePlus == null && backEmpty != null && backEmpty.transform.childCount > 0)
            imagePlus = backEmpty.transform.GetChild(0).GetComponent<Image>();
    }

    void StartGlow()
    {
        
        if (!gameObject.activeInHierarchy) return;

        if (glowRoutine != null) return;
        glowRoutine = StartCoroutine(GlowGreen());
    }

    void StopGlow()
    {
        if (glowRoutine != null)
        {
            StopCoroutine(glowRoutine);
            glowRoutine = null;
        }

        if (imagePlus != null)
            imagePlus.color = Color.white;
    }

    IEnumerator GlowGreen()
    {
    
        Color baseColor = new Color(0.2f, 1f, 0.4f);

        float t = 0f;
        float minAlpha = 0.4f;
        float maxAlpha = 1f;

        while (true)
        {
            
            if (!gameObject.activeInHierarchy)
            {
                glowRoutine = null;
                yield break;
            }

            t += Time.deltaTime * 2.5f; 
            float pulse = (Mathf.Sin(t) + 1f) / 2f; // 0 → 1

            float alpha = Mathf.Lerp(minAlpha, maxAlpha, pulse);

            if (imagePlus != null)
            {
                imagePlus.color = new Color(
                    baseColor.r,
                    baseColor.g,
                    baseColor.b,
                    alpha
                );
            }

            yield return null;
        }
    }

    void OnClick()
    {
        if (onClickCallback != null && itemData != null && currentAmount > 0)
            onClickCallback.Invoke(itemData);
    }

}