using UnityEngine;

public class HeroTransform : MonoBehaviour
{
    HeroControl heroControl;
    public HeroControl heroControlhero => heroControl;

    private void Awake()
    {
        heroControl = GetComponent<HeroControl>();
    }
}
