
using UnityEngine;


public abstract class Hazard : EventTarget
{

    void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent<Entity>(out Entity entity);
        if (entity == null)
        {
            return;
        }
        this.OnActivate(entity);
    }

    protected virtual void OnActivate(Entity entity)
    {

    }
}