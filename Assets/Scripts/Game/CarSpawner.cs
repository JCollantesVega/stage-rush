using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSpawner : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawnPoint = GameObject.FindWithTag("PlayerSpawn");

        if (spawnPoint != null && GameManager.Instance.selectedCar != null)
        {
            Instantiate(
                GameManager.Instance.selectedCar,
                spawnPoint.transform.position,
                spawnPoint.transform.rotation
            );
        }
    }
}
