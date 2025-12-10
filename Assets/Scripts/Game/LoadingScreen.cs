using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI loadingText;
    void Start()
    {
        StartCoroutine(LoadASync());
        StartCoroutine(AnimateText());
    }

    IEnumerator LoadASync()
    {
        string sceneToLoad = PlayerPrefs.GetString("SceneToLoad");
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if(progressBar != null)
            {
                progressBar.value = progress;
            }


            if(operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator AnimateText()
    {
        string text = "Loading";
        int dots = 0;

        while (true)
        {
            string dotText = new string('.', dots);

            loadingText.text = text + dotText;

            dots = (dots + 1) % 4;


            yield return new WaitForSeconds(0.75f);
        }
    }

}
