using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PopUpDamage : MonoBehaviour
{
    [SerializeField] private PlayerDealsDamage playerDealsDamage;
    [SerializeField] private GameObject popUpDmgUI;
    [SerializeField] private float popUpTimer;
    [SerializeField] private float popUpFadeTimer;
    [SerializeField] private float popUpPositionOffsetYAxisMax;
    [SerializeField] private float popUpPositionOffsetYAxisMin;
    [SerializeField] private float popUpPositionOffsetXAxisPositiv;
    [SerializeField] private float popUpPositionOffsetXAxisNegativ;
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
        float randomXDir = Random.Range(popUpPositionOffsetXAxisNegativ, popUpPositionOffsetXAxisPositiv);
        float randomYDir = Random.Range(popUpPositionOffsetYAxisMin, popUpPositionOffsetYAxisMax);
        Vector3 spawnPos = enemyTransform.position + new Vector3(randomXDir, randomYDir, 0);
        GameObject newPopUpDamageUI = Instantiate(popUpDmgUI, spawnPos, Quaternion.identity);
        TextMeshPro popUpText = newPopUpDamageUI.GetComponent<TextMeshPro>();

        ModifyText(popUpText, didPlayerCrit, roundedDamage);
        yield return StartCoroutine(WobblePopUp(newPopUpDamageUI));
        yield return StartCoroutine(FadeOutPopUp(popUpText));
        Destroy(newPopUpDamageUI);
    }

    private void ModifyText(TextMeshPro popUpText, bool didPlayerCrit, float roundedDamage)
    {
        popUpText.color = didPlayerCrit ? Color.yellow : Color.white;
        popUpText.text = roundedDamage.ToString();
    }

    private IEnumerator WobblePopUp(GameObject newPopUpDamageUI)
    {
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
    }
    private IEnumerator FadeOutPopUp(TextMeshPro popUpText)
    {
        float fadeTime = 0f;
        Color startColor = popUpText.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        while (fadeTime < popUpFadeTimer)
        {
            fadeTime += Time.deltaTime;
            
            float normalizedTime = fadeTime / popUpFadeTimer;
            
            popUpText.color = Color.Lerp(startColor, targetColor, normalizedTime);

            yield return null;
        }
    }
}
