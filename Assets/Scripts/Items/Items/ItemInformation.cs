using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ItemInformation : MonoBehaviour
{
    [Header("Display Data")]
    public string itemName;
    public Sprite itemIcon;
    public float itemPrice;
    public string itemID;

    [Header("Stats Configuration")]
    public List<ItemStat> stats = new List<ItemStat>();

    [Header("Passive Ability")]
    [TextArea(3, 5)]
    public string passiveDescription;
    // Hier könnte später ein Verweis auf ein Passive-Skript hin (z.B. ScriptableObject)
    // public PassiveAbility passiveLogic; 

    public string GetFormattedTooltipText()
    {
        string fullDescription = "";
        
        foreach (var stat in stats)
        {
            fullDescription += $"{SplitCamelCase(stat.statType.ToString())}: +{stat.value}\n";
        }
        
        if (!string.IsNullOrEmpty(passiveDescription))
        {
            fullDescription += $"\nPassive: {passiveDescription}";
        }

        return fullDescription;
    }

    private string SplitCamelCase(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1").Trim();
    }
    
    public string GetStatsAsText()
    {
        if (stats == null || stats.Count == 0) return "";

        string formattedStats = "";

        foreach (var stat in stats)
        {
            string readableName = Regex.Replace(stat.statType.ToString(), "([a-z])([A-Z])", "$1 $2");
            
            formattedStats += $"{readableName}: +{stat.value}\n";
        }

        return formattedStats;
    }
    
}
