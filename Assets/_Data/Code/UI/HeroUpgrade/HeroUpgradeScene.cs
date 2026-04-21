using UnityEngine;

public class HeroUpgradeScene : MonoBehaviour
{
    [SerializeField] Transform backUpTransform;
    [SerializeField] Transform backPreviewTransform;
    public Transform BackPreviewTransform => backPreviewTransform;

    void OnEnable()
    {
        ApplyMode();
    }

    public void ApplyMode()
    {
        bool isPreview = HeroUpgradeContext.Mode == HeroUpgradeContext.OpenMode.Preview;

        if (backUpTransform != null)
            backUpTransform.gameObject.SetActive(!isPreview);

        if (backPreviewTransform != null)
            backPreviewTransform.gameObject.SetActive(isPreview);
    }
}