
using UnityEngine;

public enum PlayerInputType
{
    Move,
    Jump,
}

public class PlayerInput : EventTarget
{
    public void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        Vector2 moveDirection = new(moveX, 0);

        if (moveDirection != Vector2.zero)
        {
            this.Emit<Vector2>(PlayerInputType.Move, moveDirection);
        }
    }
}