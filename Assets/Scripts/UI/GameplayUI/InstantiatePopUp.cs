using UnityEngine;

public class InstantiatePopUp : MonoBehaviour
{
    [SerializeField] private float popUpPositionOffsetYAxisMax;
    [SerializeField] private float popUpPositionOffsetYAxisMin;
    [SerializeField] private float popUpPositionOffsetXAxisPositive;
    [SerializeField] private float popUpPositionOffsetXAxisNegative;
    [SerializeField] private Color color;
    [SerializeField] private Color colorOnCrit;
    [SerializeField] private GameObject popUpPrefab;
    [SerializeField] private bool isFollowPlayer;

    private Transform _followTarget;
    
    public void Instantiate(float amount, bool isCrit, Transform target)
    {
        Color finalColor = color;
        if (isFollowPlayer)
        {
            _followTarget = this.transform;
        }

        if (isCrit)
        {
            finalColor = colorOnCrit;
        }
        
        float randomXDir = Random.Range(popUpPositionOffsetXAxisNegative, popUpPositionOffsetXAxisPositive);
        float randomYDir = Random.Range(popUpPositionOffsetYAxisMin, popUpPositionOffsetYAxisMax);
        Vector3 spawnPos = target.position + new Vector3(randomXDir, randomYDir, 0);
        GameObject popup = Instantiate(popUpPrefab, spawnPos, Quaternion.identity);
        popup.GetComponent<PopUpNumber>().Setup(amount, finalColor, _followTarget);
    }
}
