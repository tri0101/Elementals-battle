using UnityEngine;
using UnityEngine.UI;

public class UI_HeroSummonReward : MonoBehaviour
{
    [SerializeField] private Image heroIcon;


    public void SetUp(int heroId)
    {
        HeroInfo heroInfo = DatabaseManager.Instance.HeroDatabase.GetHero(heroId);
        heroIcon.sprite = heroInfo.iconFace;

    }

  
}