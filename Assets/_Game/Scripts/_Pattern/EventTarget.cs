
using System;
using UnityEngine;

public class EventTarget : MonoBehaviour
{
    public void On(Enum eventType, Delegate callback)
    {
        EventBus.On(eventType, callback);
    }

    public void Off(Enum eventType, Delegate callback)
    {
        EventBus.Off(eventType, callback);
    }

    protected void Emit(Enum eventType, params object[] args)
    {
        EventBus.Emit(eventType, args);
    }
}