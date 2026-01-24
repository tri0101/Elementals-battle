using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UI_RankSourceUpgradeItem : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TextMeshProUGUI amountText;

    [Header("Empty State")]
    public GameObject backEmpty;
    Image imagePlus; // con của backEmpty

    Coroutine glowRoutine;

    void Awake()
    {
        if (backEmpty != null && backEmpty.transform.childCount > 0)
            imagePlus = backEmpty.transform.GetChild(0).GetComponent<Image>();
    }

    public void Setup(ItemData itemData, int owned, int required)
    {
        // ===== ICON =====
        icon.sprite = itemData.icon;
        ApplyIconTransform(itemData);

        // ===== TEXT =====
        amountText.text = $"{owned}/{required}";

        // ===== EMPTY + GLOW =====
        bool lack = owned < required;
        backEmpty.SetActive(lack);

        if (lack) StartGlow();
        else StopGlow();
    }

    // ================= ICON RULE =================

    void ApplyIconTransform(ItemData item)
    {
        Vector3 scale = Vector3.one;
        float rotZ = 0f;

        if (item.id == 100)
        {
            scale = new Vector3(0.5f, 1.25f, 1f);
            rotZ = 45f;
        }
        else if (item.name.Contains("Sword"))
        {
            scale = new Vector3(0.5f, 1.25f, 1f);
            rotZ = -50f;
        }
        else if (item.name.Contains("Shield") || item.name.Contains("Star"))
        {
            scale = Vector3.one;
            rotZ = 0f;
        }

        icon.transform.localScale = scale;
        icon.transform.localRotation = Quaternion.Euler(0, 0, rotZ);
    }

    // ================= GLOW =================

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
        Color glowColor = new Color(0.25f, 1f, 0.25f);
        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * 2f;

            float alpha = Mathf.Lerp(
                0.5f,
                1f,
                (Mathf.Sin(t) + 1f) / 2f
            );

            imagePlus.color = new Color(
                glowColor.r,
                glowColor.g,
                glowColor.b,
                alpha
            );

            yield return null;
        }
    }
}
