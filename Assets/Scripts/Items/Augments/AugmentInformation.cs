using UnityEngine;

public class AugmentInformation : MonoBehaviour
{
    public AugmentRarity augmentRarity;
    public string augmentID;
    public string augmentText;
    public string augmentTitle;

    // Returns the built description when an AugmentDescriptionBuilder is present,
    // otherwise falls back to the static augmentText. The player is the one the text is shown to.
    public string GetDisplayText(PlayerStats playerStats)
    {
        AugmentDescriptionBuilder descriptionBuilder = GetComponent<AugmentDescriptionBuilder>();
        return descriptionBuilder != null ? descriptionBuilder.Build(playerStats) : augmentText;
    }
}
