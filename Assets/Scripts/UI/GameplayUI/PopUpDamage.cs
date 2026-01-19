using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PopUpDamage : MonoBehaviour
{
    [SerializeField] private PlayerDealsDamage playerDealsDamage;
    [SerializeField] private GameObject popUpDmgUI;
    [SerializeField] private float popUpTimer;
    [SerializeField] private float popUpFadeTimer;
    [SerializeField] private float popUpPositionOffset;
    [SerializeField] private float wobbleSpeed;
    [SerializeField] private float wobbleDamping;
    
    [Range(0f, 1f)]
    [SerializeField] private float wobbleStrength;

    private void OnEnable()
    {
        playerDealsDamage.OnInstantiatePopUpDamageUI += InstantiatePopUpDamage;
    }

    private void OnDisable()
    {
        playerDealsDamage.OnInstantiatePopUpDamageUI -= InstantiatePopUpDamage;
    }

    private void InstantiatePopUpDamage(Transform enemyTransform, float damageDealtByPlayer, bool didPlayerCrit)
    {
        StartCoroutine(PopUpDamageRuntime(enemyTransform, damageDealtByPlayer, didPlayerCrit));
    }
    
    private IEnumerator PopUpDamageRuntime(Transform enemyTransform, float damageDealtByPlayer, bool didPlayerCrit)
    {
        float roundedDamage = Mathf.RoundToInt(damageDealtByPlayer);
        
        Vector3 spawnPos = enemyTransform.position + Vector3.up * popUpPositionOffset;
        GameObject newPopUpDamageUI = Instantiate(popUpDmgUI, spawnPos, Quaternion.identity);
        TextMeshPro popUpText = newPopUpDamageUI.GetComponent<TextMeshPro>();

        popUpText.color = didPlayerCrit ? Color.yellow : Color.white;
        popUpText.text = roundedDamage.ToString();
        
        Vector3 startPos = newPopUpDamageUI.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < popUpTimer)
        {
            elapsedTime += Time.deltaTime;

            float wobble = Mathf.Sin(elapsedTime * wobbleSpeed) * Mathf.Exp(-elapsedTime * wobbleDamping) * wobbleStrength;
            
            newPopUpDamageUI.transform.position = startPos + new Vector3(0, wobble, 0);
            
            yield return null;
        }
        
        newPopUpDamageUI.transform.position = startPos;
        
        yield return new WaitForSeconds(popUpTimer);
        
        float fadeTime = 0f;
        Color startColor = popUpText.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        while (elapsedTime < popUpFadeTimer)
        {
            fadeTime += Time.deltaTime;
            
            float normalizedTime = fadeTime / popUpFadeTimer;
            
            popUpText.color = Color.Lerp(startColor, targetColor, normalizedTime);

            yield return null;
        }
        
        Destroy(newPopUpDamageUI);
    }
}
