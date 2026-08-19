using UnityEngine;

public class AugmentInformation : MonoBehaviour
{
    public float augmentRarity;
    public string augmentID;
    public string augmentText;
    public string augmentTitle;

    // Returns the built description when an AugmentDescriptionBuilder is present,
    // otherwise falls back to the static augmentText.
    public string GetDisplayText()
    {
        AugmentDescriptionBuilder descriptionBuilder = GetComponent<AugmentDescriptionBuilder>();
        return descriptionBuilder != null ? descriptionBuilder.Build() : augmentText;
    }
}
