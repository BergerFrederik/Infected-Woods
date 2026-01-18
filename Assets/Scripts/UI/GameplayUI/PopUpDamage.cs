using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PopUpDamage : MonoBehaviour
{
    [SerializeField] private PlayerDealsDamage playerDealsDamage;
    [SerializeField] private GameObject popUpDmgUI;

    private void OnEnable()
    {
        playerDealsDamage.OnInstantiatePopUpDamageUI += InstantiatePopUpDamage;
    }

    private void OnDisable()
    {
        playerDealsDamage.OnInstantiatePopUpDamageUI -= InstantiatePopUpDamage;
    }

    private void InstantiatePopUpDamage(Transform enemyTransform, float damageDealtByPlayer)
    {
        StartCoroutine(PopUpDamageRuntime(enemyTransform, damageDealtByPlayer));
    }
    
    private IEnumerator PopUpDamageRuntime(Transform enemyTransform, float damageDealtByPlayer)
    {
        float roundedDamage = Mathf.RoundToInt(damageDealtByPlayer);
        GameObject newPopUpDamageUI = Instantiate(popUpDmgUI, enemyTransform.position, Quaternion.identity);
        newPopUpDamageUI.GetComponent<TextMeshPro>().text = damageDealtByPlayer.ToString();
        yield return new WaitForSeconds(1f);
        Destroy(newPopUpDamageUI);
    }
}
