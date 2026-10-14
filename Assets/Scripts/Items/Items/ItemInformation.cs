using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ItemInformation : MonoBehaviour
{
    public enum ItemTier
    {
        Root,
        Bud,
        Blossom,
        Special
    }
    
    [Header("Display Data")]
    public string itemName;
    public Sprite itemIcon;
    public float itemPrice;
    public string itemID;
    public ItemTier tier;

    [Header("Stats Configuration")]
    public List<ItemStat> stats = new List<ItemStat>();

    [Header("Passive Ability")]
    [TextArea(3, 5)]
    public string passiveDescription;

    
    

    private void OnEnable()
    {
        if (IsPlayerRoot())
        {
            ApplyStats(1f);
        }
    }

    private void OnDisable()
    {
        if (IsPlayerRoot())
        {
            ApplyStats(-1f);
        }
    }

    public bool IsPlayerRoot()
    {
        bool isPlayerRoot = false;
        if (this.transform.root == null) isPlayerRoot = false;
        
        string rootName = this.transform.root.name;
        if (rootName == "Player") isPlayerRoot = true;

        return isPlayerRoot;
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

    public void ApplyStats(float multiplier)
    {
        if (stats == null || stats.Count == 0) return;

        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player == null) return;

        foreach (var stat in stats)
        {
            player.AddStat(stat.statType, stat.value * multiplier);
        }
    }
    
}
