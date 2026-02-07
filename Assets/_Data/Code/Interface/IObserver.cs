using UnityEngine;

public interface IObserver 
{
    public void OnNotify() { }
    public void OnNotify(object data) { }
    public void OnNotify(HeroNotifyType type, object data) { }
}
