using UnityEngine;
using System.Collections.Generic;
using System;
public abstract class Subject : MonoBehaviour
{
    private List<IObserver> _observers = new List<IObserver>();

    public void AddObserver(IObserver observer)
    {
        _observers.Add(observer);
    }
    public void RemoveObbserver(IObserver observer)
    {
        _observers.Remove(observer);
    }

    protected void NotifyObservers()
    {
        _observers.ForEach((_observer) => { _observer.OnNotify(); }
        );
    }
    protected void NotifyObservers(object data)
    {
        foreach (var observer in _observers)
            observer.OnNotify(data);
    }
    protected void NotifyObservers(HeroNotifyType type, object data = null)
    {
        foreach (var observer in _observers)
            observer.OnNotify(type, data);
    }
    protected void NotifyObservers(HPNotifyType type, object data = null)
    {
        foreach (var observer in _observers)
            observer.OnNotify(type, data);
    }
}
