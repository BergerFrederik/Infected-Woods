using UnityEngine;

public class BossWave : MonoBehaviour
{
    [SerializeField] private GameObject BossPrefeab;

    private void Start()
    {
        GameObject NewEnemy = Instantiate(BossPrefeab, new Vector3(0, 10, 0), Quaternion.identity);
    }
}
