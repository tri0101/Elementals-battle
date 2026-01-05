using UnityEngine;

public interface IObserver 
{
    public void OnNotify();
    public void OnNotify(object data) { }
    public void OnNotify(object data1,object data2) { }
}
