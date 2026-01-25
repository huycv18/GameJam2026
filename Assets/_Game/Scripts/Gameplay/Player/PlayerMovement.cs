
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerInput _input;
    private Rigidbody2D _rb;
    [SerializeField] private float speed;

    void Awake()
    {
        _input = this.GetComponent<PlayerInput>();
        _rb = this.GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        _input.On<Vector2>(PlayerInputType.Move, Move);
    }

    void OnDisable()
    {
        _input.Off<Vector2>(PlayerInputType.Move, Move);
    }

    private void Move(Vector2 direction)
    {
        _rb.velocity = new Vector2(direction.x * speed, _rb.velocity.y);
    }

    
}