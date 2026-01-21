using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_HeroItem : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nameHero;
    public Transform starRoot;
    public Transform rankRoot;
    public Image frameRank;

    private int blackRank = 1;
    private int greenRank = 5;

    private Color blackColor = Color.black;
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);

    public void Setup(HeroViewData data)
    {
        icon.sprite = data.info.iconFace;
        levelText.text = $"{data.instance.level}";

        UpdateStar(data.instance.star);
        UpdateClass(data.instance.rank, data.info.Name);
    }

    void UpdateStar(int star)
    {
        for (int i = 0; i < starRoot.childCount; i++)
            starRoot.GetChild(i).gameObject.SetActive(i < star);
    }

    void UpdateClass(int rank, string heroName)
    {
        // reset icon rank con
        for (int i = 0; i < rankRoot.childCount; i++)
            rankRoot.GetChild(i).gameObject.SetActive(false);

        Color rankColor;
        int plusValue = 0;

        // ===== BLACK RANK =====
        if (rank < greenRank)
        {
            rankColor = blackColor;
            frameRank.color = blackColor;

            if (rank > blackRank)
            {
                plusValue = rank - blackRank;

                for (int i = 0; i < plusValue && i < rankRoot.childCount; i++)
                    rankRoot.GetChild(i).gameObject.SetActive(true);
            }
        }
        // ===== GREEN RANK =====
        else
        {
            rankColor = greenColor;
            frameRank.color = greenColor;

            if (rank > greenRank)
            {
                plusValue = rank - greenRank;

                for (int i = 0; i < plusValue && i < rankRoot.childCount; i++)
                    rankRoot.GetChild(i).gameObject.SetActive(true);
            }
        }

        // ===== SET NAME + COLOR =====
        nameHero.color = rankColor;

        if (plusValue > 0)
            nameHero.text = $"{heroName} +{plusValue}";
        else
            nameHero.text = heroName;
    }
}
