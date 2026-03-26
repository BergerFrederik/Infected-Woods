using System;
using UnityEngine;

public class RerollMechanic : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private int _numRerolls;
    public int numRerolls
    {
        get { return _numRerolls; }
        set
        {
            _numRerolls = value;
        }
    }

    private int _currentRerollPrice;

    private void OnDisable()
    {
        _numRerolls = 0;
        _currentRerollPrice = 0;
    }

    public int GetRerollPrice()
    {
        int waveNumber = (int)gameManager.currentWaveNumber;
        if (_numRerolls == 0)
        {
            int basePrice = Mathf.FloorToInt(waveNumber * 0.75f);
            int overallPrice = basePrice + GetRerollIncrease(waveNumber);
            _currentRerollPrice = overallPrice;
        }
        else
        {
            _currentRerollPrice += GetRerollIncrease(waveNumber); 
        }
        
        return _currentRerollPrice;
    }

    private int GetRerollIncrease(int waveNumber)
    {
        int priceIncrease = Mathf.Max(1, Mathf.FloorToInt(0.4f * waveNumber));
        return priceIncrease;
    }
}
