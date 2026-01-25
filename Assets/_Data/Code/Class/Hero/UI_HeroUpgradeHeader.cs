using TMPro;
using UnityEngine;

public class UI_HeroUpgradeHeader : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI heroNameText;
    public TextMeshProUGUI heroRoleText;
    public Transform heroPreviewPanel;

    GameObject currentPreview;

    public void Setup(HeroViewData data)
    {
        
        HeroRankHelper.GetRankVisual(
            data.instance.rank,
            data.info.Name,
            out string displayName,
            out Color color
        );

        heroNameText.text = displayName;
        heroNameText.color = color;
        heroRoleText.text = data.info.role.ToString();


        SetupPreview(data.info.HeroPreviewPrefabs);
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
