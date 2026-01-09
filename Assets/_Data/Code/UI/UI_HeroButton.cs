using UnityEngine;

public class UI_HeroButton : MonoBehaviour
{
    public HeroData heroData;

    public void OnClick()
    {
        GameManager.Instance.selectedHero = heroData;
    }

}
