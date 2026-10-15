// An augment whose description shows values of the player it is shown to (crit chance, heal power, ...).
// The panel hands that player in: the shop builds the text from the prefab, which belongs to no player yet
// and is shared by all of them, so it can neither look the player up nor remember it.
public interface IPlayerDependentDescribable : IAugmentDescribable
{
    float GetPlaceholderValue(int index, PlayerStats playerStats);
}
