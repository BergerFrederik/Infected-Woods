using System;
using UnityEngine;

public class RerollMechanic : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private event Action OnRerolled;
    private int _numRerolls;
    private int waveNumber;
    public int numRerolls
    {
        get { return _numRerolls; }
        set
        {
            _numRerolls = value;
            OnRerolled?.Invoke();
        }
    }

    private int _currentRerollPrice;

    private void OnEnable()
    {
        waveNumber = (int)gameManager.currentWaveNumber;
        int basePrice = Mathf.FloorToInt(waveNumber * 0.75f);
        int overallPrice = basePrice + GetRerollIncrease(waveNumber);
        _currentRerollPrice = overallPrice;
        
        OnRerolled += IncreaseRerollPrice;
    }

    private void OnDisable()
    {
        _numRerolls = 0;
        _currentRerollPrice = 0;
        OnRerolled -= IncreaseRerollPrice;
    }

    private void IncreaseRerollPrice()
    {
        _currentRerollPrice += GetRerollIncrease(waveNumber); 
    }

    public int GetRerollPrice()
    {
        return _currentRerollPrice;
    }

    private int GetRerollIncrease(int waveNumber)
    {
        int priceIncrease = Mathf.Max(1, Mathf.FloorToInt(0.4f * waveNumber));
        return priceIncrease;
    }
}
