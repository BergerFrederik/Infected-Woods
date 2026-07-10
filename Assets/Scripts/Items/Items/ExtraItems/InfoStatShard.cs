using UnityEngine;

public class InfoStatShard : MonoBehaviour
{
    private string _statName;
    public string StatName
    {
        get => _statName;
        set => _statName = value;
    }
    
    private float _statValue;
    public float StatValue
    {
        get => _statValue;
        set => _statValue = value;
    }
}
