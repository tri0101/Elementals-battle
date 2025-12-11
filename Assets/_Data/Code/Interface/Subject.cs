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
}
