using System.Text;
using UnityEngine;

// Builds an augment's description text at display time from fixed text segments
// with dynamic values spliced in between: text[0] + value[0] + text[1] + value[1] + text[2] ...
// Values are pulled live from an IAugmentDescribable on the same GameObject, so the
// text never goes stale when the underlying augment values change or scale.
public class AugmentDescriptionBuilder : MonoBehaviour
{
    [SerializeField] private string[] textSegments; // length must be placeholder count + 1
    [SerializeField] private string[] valueFormats;  // per placeholder, e.g. "0.#" or "P0"
    [SerializeField] private Color[] valueColors;    // per placeholder

    private const string DefaultFormat = "0.#";

    public string Build()
    {
        if (textSegments == null || textSegments.Length == 0)
        {
            return string.Empty;
        }

        IAugmentDescribable describable = GetComponent<IAugmentDescribable>();
        int placeholderCount = textSegments.Length - 1;

        StringBuilder result = new StringBuilder();
        for (int i = 0; i < textSegments.Length; i++)
        {
            result.Append(textSegments[i]);

            if (i >= placeholderCount)
            {
                continue;
            }

            if (describable == null)
            {
                Debug.LogWarning($"{gameObject.name}: AugmentDescriptionBuilder has placeholders but no IAugmentDescribable found.");
                continue;
            }

            float value = describable.GetPlaceholderValue(i);
            string format = (valueFormats != null && i < valueFormats.Length && !string.IsNullOrEmpty(valueFormats[i]))
                ? valueFormats[i]
                : DefaultFormat;
            string valueText = value.ToString(format);

            if (valueColors != null && i < valueColors.Length)
            {
                string hex = ColorUtility.ToHtmlStringRGB(valueColors[i]);
                result.Append($"<color=#{hex}>{valueText}</color>");
            }
            else
            {
                result.Append(valueText);
            }
        }

        return result.ToString();
    }
}
