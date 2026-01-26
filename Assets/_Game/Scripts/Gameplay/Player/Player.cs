
using UnityEngine;

public class Player : PlayerEntity
{
    public override void Die()
    {
        Debug.Log("Player has died.");
    }
}