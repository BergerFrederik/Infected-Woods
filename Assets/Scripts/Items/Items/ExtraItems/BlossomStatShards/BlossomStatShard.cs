using TMPro;
using UnityEngine;

public class BlossomStatShard : MonoBehaviour
{
    [SerializeField] private string statName;
    public string StatName => statName;

    [SerializeField]
    [TextArea]
    private string shardDescription;
    public string ShardDescription => shardDescription;

    [SerializeField] private Sprite shardIcon;
    public Sprite ShardIcon => shardIcon;
}
