using UnityEngine;

public class HeroIdle : MonoBehaviour
{
    HeroControl heroControl;
    public HeroControl HeroControl => heroControl;

    private void Awake()
    {
        heroControl = GetComponent<HeroControl>();
    }


    
}
