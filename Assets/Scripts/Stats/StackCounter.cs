using UnityEngine;

// Counts the stacks of one stackable mechanic and enforces its cap.
// Every stackable with a cap should use this instead of its own "stacks < max" check,
// so the owner's playerStackCapIncreasePercent (LimitBreak and any later source) applies to it.
public class StackCounter
{
    private readonly PlayerStats _owner;
    private readonly float _baseCap;

    public float Current { get; private set; }

    // Base cap raised by the owner's percentage bonus, rounded up to the next whole stack
    public float Cap
    {
        get
        {
            float raisedCap = _baseCap * (1f + _owner.playerStackCapIncreasePercent / 100f);
            return Mathf.Max(0f, Mathf.Ceil(raisedCap));
        }
    }

    public bool IsFull => Current >= Cap;

    public StackCounter(PlayerStats owner, float baseCap)
    {
        _owner = owner;
        _baseCap = baseCap;
    }

    // Adds one stack. Returns false (and changes nothing) when the cap is reached.
    public bool TryAdd()
    {
        if (IsFull) return false;

        Current++;
        return true;
    }

    public void Reset()
    {
        Current = 0f;
    }
}
