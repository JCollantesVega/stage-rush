using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectFirstButton : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

}
