using UnityEngine;

public interface IObserver 
{
    public void OnNotify();
    public void OnNotify(object data) { }
}
