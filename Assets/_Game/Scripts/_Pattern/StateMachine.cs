using System;
using System.Collections.Generic;

public class StateMachine<T>
{
    public T CurrentState { get; private set; }

    private readonly Dictionary<T, Action> onEnter = new();
    private readonly Dictionary<T, Action> onUpdate = new();
    private readonly Dictionary<T, Action> onExit = new();

    private bool hasState = false;

    public StateMachine(T initialState)
    {
        if (!typeof(T).IsEnum) {
            throw new Exception("StateMachine<T> requires T to be an Enum");
        }

        CurrentState = initialState;
        hasState = true;
    }

    public void AddState(
        T state,
        Action enter = null,
        Action update = null,
        Action exit = null)
    {
        if (enter != null) onEnter[state] = enter;
        if (update != null) onUpdate[state] = update;
        if (exit != null) onExit[state] = exit;
    }

    public void ChangeState(T newState)
    {
        if (hasState && EqualityComparer<T>.Default.Equals(CurrentState, newState))
            return;

        if (hasState && onExit.TryGetValue(CurrentState, out var exit))
            exit?.Invoke();

        CurrentState = newState;
        hasState = true;

        if (onEnter.TryGetValue(CurrentState, out var enter))
            enter?.Invoke();
    }

    public void Update()
    {
        if (!hasState) return;

        if (onUpdate.TryGetValue(CurrentState, out var update))
            update?.Invoke();
    }
}
