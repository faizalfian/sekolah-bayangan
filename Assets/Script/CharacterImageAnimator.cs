using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterImageAnimator : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    private Image characterImage;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        characterImage = GetComponent<Image>();
        characterImage.color = new Color(1, 1, 1, 0); 
    }

    public void FadeIn(Sprite newSprite)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        characterImage.sprite = newSprite;
        fadeCoroutine = StartCoroutine(Fade(0f, 1f));
    }

    public void FadeOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = characterImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            characterImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        characterImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}
