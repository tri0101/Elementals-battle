using TMPro;
using UnityEngine;

public class UI_HeroUpgradeHeader : MonoBehaviour
{
    public TextMeshProUGUI heroNameText;

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
    }
}
