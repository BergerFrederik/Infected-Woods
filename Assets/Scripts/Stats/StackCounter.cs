using UnityEngine;

// Counts the stacks of one stackable mechanic and enforces its cap.
// Every stackable should use this instead of its own counter, so the owner's bonuses apply to it:
// playerStackCapIncreasePercent (LimitBreak) raises the cap, playerStackGainIncreasePercent (SkyKamiReach)
// raises how many stacks each gain is worth. A gain of 1.5 stacks keeps its 0.5 until it adds up to a full stack.
public class StackCounter
{
    // Guards against float drift, e.g. 3 stacks summed up as 2.9999998
    private const float WholeStackTolerance = 0.0001f;

    private readonly PlayerStats _owner;
    private readonly float _baseCap;

    private float _progress;

    // Only full stacks count for effects and the HUD, the fraction does nothing until it completes a stack
    public int Whole => Mathf.FloorToInt(_progress + WholeStackTolerance);

    // Base cap raised by the owner's percentage bonus, rounded up to the next whole stack
    public float Cap
    {
        get
        {
            if (float.IsPositiveInfinity(_baseCap)) return _baseCap;

            float raisedCap = _baseCap * (1f + _owner.playerStackCapIncreasePercent / 100f);
            return Mathf.Max(0f, Mathf.Ceil(raisedCap));
        }
    }

    public bool IsFull => Whole >= Cap;

    // Stacks per gain: 1 plus the owner's percentage bonus
    private float GainPerAdd => Mathf.Max(0f, 1f + _owner.playerStackGainIncreasePercent / 100f);

    public StackCounter(PlayerStats owner, float baseCap)
    {
        _owner = owner;
        _baseCap = baseCap;
    }

    public static StackCounter Uncapped(PlayerStats owner)
    {
        return new StackCounter(owner, float.PositiveInfinity);
    }

    // Adds one gain worth of stacks. Returns false (and changes nothing) when the cap is reached.
    // wholeStacksGained is how many full stacks this gain completed - 0 while only a fraction came in.
    public bool TryAdd(out int wholeStacksGained)
    {
        wholeStacksGained = 0;
        if (IsFull) return false;

        int wholeBefore = Whole;
        _progress = Mathf.Min(Cap, _progress + GainPerAdd);
        wholeStacksGained = Whole - wholeBefore;
        return true;
    }

    public bool TryAdd()
    {
        return TryAdd(out _);
    }

    public void Reset()
    {
        _progress = 0f;
    }
}
