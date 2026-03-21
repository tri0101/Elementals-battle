using UnityEngine;

public interface IObserver 
{
    public void OnNotify() { }
    public void OnNotify(object data) { }
    public void OnNotify(ModifyStatType type) { }
    public void OnNotify(HeroNotifyType type, object data) { }
    public void OnNotify(HPNotifyType type, object data) { }
    public void OnNotify(HPNotifyType type, object data, DamageType damageType) { }
}
