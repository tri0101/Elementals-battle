using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UI_FoodUpgradeItem : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amountText;
    public GameObject backEmpty;
    Button button;
    Image imagePlus;
    Coroutine glowRoutine;

    public void Setup(ItemData itemData, int amount)
    {
        icon.sprite = itemData.icon;
        amountText.text = amount.ToString();

        bool isEmpty = amount <= 0;
        backEmpty.SetActive(isEmpty);

        if (isEmpty)
        {

            StartGlow();
            button.enabled = false;
        }
        else
        {
            StopGlow();
            button.enabled = true;
        }
    }

    void Awake()
    {
        // ImagePlus là con của BackEmpty
        button = GetComponent<Button>();
        imagePlus = backEmpty.transform.GetChild(0).GetComponent<Image>();
    }

    void StartGlow()
    {
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
        // Xanh lá sáng hơn
        Color baseColor = new Color(0.2f, 1f, 0.4f);

        float t = 0f;
        float minAlpha = 0.4f;
        float maxAlpha = 1f;

        while (true)
        {
            t += Time.deltaTime * 2.5f; // tăng tốc độ nhịp chút
            float pulse = (Mathf.Sin(t) + 1f) / 2f; // 0 → 1

            float alpha = Mathf.Lerp(minAlpha, maxAlpha, pulse);

            imagePlus.color = new Color(
                baseColor.r,
                baseColor.g,
                baseColor.b,
                alpha
            );

            yield return null;
        }
    }

}
