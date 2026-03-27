using UnityEngine;

public class Effect_Item : MonoBehaviour, IObserver
{
    [SerializeField] private AbilityEffectType type;
    [SerializeField] private Transform target;
    [Header("Other script")]
    [SerializeField] private Animator animator;
    [SerializeField] private Effect_ItemEvent effectEvent;
    void Awake()
    {
        effectEvent = transform.GetChild(0).GetComponent<Effect_ItemEvent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }
    public void SetType(AbilityEffectType newType)
    {
        type = newType;
    }
    public AbilityEffectType GetAbilityType()
    {
        return type;
    }
    //private void OnDestroy()
    //{
    //    HeroControl heroC = target.GetComponent<HeroControl>();
    //    heroC.RemoveObbserver(this);
    //}
    public void SetTarget(Transform target)// target mà hiệu ứng này đang áp dụng lên, có thể là hero hoặc enemy
    {
        HeroControl heroC = target.GetComponent<HeroControl>();
        heroC.AddObserver(this);
        this.target= target;
    }
    public void OnNotify(string name, bool value)
    {
        if (animator == null) return;
        SetBool(name, value);
    }
    public void SetBool(string name, bool value)
    {
        if (animator == null) return;
        animator.SetBool(name, value);
    }
}
