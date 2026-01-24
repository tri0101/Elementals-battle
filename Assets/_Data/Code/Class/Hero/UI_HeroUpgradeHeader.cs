using TMPro;
using UnityEngine;

public class UI_HeroUpgradeHeader : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI heroNameText;
    public Transform heroPreviewPanel;

    GameObject currentPreview;

    public void Setup(HeroViewData data)
    {
        // ===== NAME + COLOR =====
        HeroRankHelper.GetRankVisual(
            data.instance.rank,
            data.info.Name,
            out string displayName,
            out Color color
        );

        heroNameText.text = displayName;
        heroNameText.color = color;

        // ===== HERO PREVIEW =====
        SetupPreview(data.info.HeroPreviewPrefabs);
    }

    void SetupPreview(GameObject previewPrefab)
    {
        // Clear cũ
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }

        if (previewPrefab == null)
        {
            Debug.LogWarning("HeroPreviewPrefabs is NULL");
            return;
        }

        // Spawn làm con panel
        currentPreview = Instantiate(previewPrefab, heroPreviewPanel);

        
    }
}
