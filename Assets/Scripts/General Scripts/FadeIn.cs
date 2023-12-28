using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private bool OnAwake = true;
    [SerializeField] private float FadeTime = 1;

    public bool doFade = false;

    Renderer selfRender;
    Color selfColor;
    float startingAlpha;

    private void Start()
    {
        if (!OnAwake)
        {
            Fade();
        }
    }

    private void Awake()
    {
        if (OnAwake)
        {
            Fade();
        }
    }

    private void Fade()
    {
        selfRender = GetComponent<Renderer>();
        selfColor = selfRender.material.color;

        startingAlpha = selfColor.a;

        Color clearColor = new(selfColor.r, selfColor.g, selfColor.b, 0);

        selfRender.material.color = clearColor;



        StartCoroutine(nameof(FadeCoroutine));
    }
    private IEnumerator FadeCoroutine()
    {
        int i = 0;
        while (selfRender.material.color.a < startingAlpha)
        {
            i++;
            Color newColor = new(selfColor.r, selfColor.g, selfColor.b, Mathf.Lerp(selfRender.material.color.a, startingAlpha, FadeTime));

            selfRender.material.color = newColor;
            
            

            if (i >= 2000)
            {
                Debug.Log("Wuh oh");
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
