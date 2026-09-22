using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [Header("UI References")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("Target Scene")]
    public string sceneToLoad = "SampleScene";

    private void Start()
    {
        //Fadein
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    //saat tombol play di klik
    public void PlayGame()
    {
        StartCoroutine(FadeOutAndLoadScene());
    }

    //FadeOut
    private IEnumerator FadeOutAndLoadScene()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    //Fungsi FadeIn
    private IEnumerator FadeIn()
    {
        float timer = fadeDuration;
        Color color = fadeImage.color;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            color.a = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }
}