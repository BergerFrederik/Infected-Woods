using System.Collections;
using TMPro;
using UnityEngine;

public class PopUpNumber : MonoBehaviour
{
    [SerializeField] private float popUpTimer;
    [SerializeField] private float popUpFadeTimer;
    [SerializeField] private float wobbleSpeed;
    [SerializeField] private float wobbleDamping;
    
    [Range(0f, 1f)]
    [SerializeField] private float wobbleStrength;
    [SerializeField] private TextMeshPro popUpText;
    
    private Vector3 _startLocalPos;
    
    public void Setup(float amount, Color color, Transform followTarget = null)
    {
        popUpText.text = $"{Mathf.RoundToInt(amount)}";
        popUpText.color = color;

        if (followTarget != null)
        {
            this.transform.SetParent(followTarget);
        }
        
        _startLocalPos = this.transform.localPosition;
        
        StartCoroutine(PopUpSequence());
    }

    private IEnumerator PopUpSequence()
    {
        yield return StartCoroutine(WobblePopUp());
        yield return StartCoroutine(FadeOutPopUp());
        Destroy(this.gameObject);
    }
    
    private IEnumerator WobblePopUp()
    {
        float elapsedTime = 0f;

        while (elapsedTime < popUpTimer)
        {
            elapsedTime += Time.deltaTime;
            
            float wobble = Mathf.Sin(elapsedTime * wobbleSpeed) * Mathf.Exp(-elapsedTime * wobbleDamping) * wobbleStrength;
            
            this.transform.localPosition = _startLocalPos + new Vector3(0, wobble, 0);
            
            yield return null;
        }
        
        this.transform.localPosition = _startLocalPos;
    }

    private IEnumerator FadeOutPopUp()
    {
        float fadeTime = 0f;
        Color startColor = popUpText.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        
        Vector3 posAtFadeStart = this.transform.localPosition;

        while (fadeTime < popUpFadeTimer)
        {
            fadeTime += Time.deltaTime;
            float normalizedTime = fadeTime / popUpFadeTimer;
            
            popUpText.color = Color.Lerp(startColor, targetColor, normalizedTime);
            
            this.transform.localPosition = posAtFadeStart + new Vector3(0, normalizedTime * 0.3f, 0);

            yield return null;
        }
    }
}
