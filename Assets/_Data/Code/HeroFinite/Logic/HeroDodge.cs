using Unity.Jobs;
using UnityEngine;

public class HeroDodge : MonoBehaviour
{
    [SerializeField] HeroControl heroControl;
    public HeroControl heroControlhero => heroControl;
    [SerializeField] private int countDodge = 0;
    public int CountDodge => countDodge;
    private void Awake()
    {
        heroControl = GetComponent<HeroControl>();
    }
    public void IncreaseDodgeCount()
    {
        countDodge++;
        if(countDodge > 0)
        {
            heroControl.CanDodge = false;
        }
    }
}
