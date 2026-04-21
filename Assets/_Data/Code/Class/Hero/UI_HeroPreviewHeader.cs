using TMPro;
using UnityEngine;

public class UI_HeroPreviewHeader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] int currentHeroId;
    [SerializeField] private TextMeshProUGUI heroNameText;
    [SerializeField] private TextMeshProUGUI heroRoleText;
    [SerializeField] private Transform heroPreviewPanel;

    [Header("Default Rank")]
    [SerializeField] private int defaultLowestRank = 1;

    GameObject currentPreview;

    public void Setup(HeroInfo info)
    {
        if (info == null) return;

        // rank mặc định là rank thấp nhất
        HeroRankHelper.GetRankVisual(
            defaultLowestRank,
            info.Name,
            out string displayName,
            out Color color
        );

        if (info.ID != currentHeroId)
        {
            SetupPreview(info.HeroPreviewPrefabs);
        }
        currentHeroId = info.ID;

        if (heroNameText != null)
        {
            heroNameText.text = displayName;
            heroNameText.color = color;
        }

        if (heroRoleText != null)
            heroRoleText.text = info.role.ToString();
    }

    void SetupPreview(GameObject previewPrefab)
    {
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

        currentPreview = Instantiate(previewPrefab, heroPreviewPanel);
    }
}