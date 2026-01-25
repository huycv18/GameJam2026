
using UnityEngine;


public abstract class Hazard : EventTarget
{

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IAffectable affectable))
        {
            return;
        }
        this.Apply(affectable);
    }

    public virtual void Apply(IAffectable affectable)
    {

    }
}