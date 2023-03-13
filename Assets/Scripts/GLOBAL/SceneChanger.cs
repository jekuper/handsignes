using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator translation;
    public static SceneChanger singleton;

    private void Start () {
        if (singleton != null) {
            Debug.LogWarning ("deleting dublicating singleton on this scene");
            Destroy (gameObject);
            return;
        }
        singleton = this;
    }

    public void ChangeScene (string scene) {
        if (singleton.translation != null)
            singleton.translation.Play ("closeSceneTransition");

        StartCoroutine (Switch (scene, 0.3f));
    }
    public static void LoadScene (string scene) {
        singleton.ChangeScene (scene);
    }
    public static void OpenAnim () {
        if (singleton.translation != null)
            singleton.translation.Play ("openSceneTransition");
    }
    public static void CloseAnim () {
        if (singleton.translation != null)
            singleton.translation.Play ("closeSceneTransition");
    }
    public IEnumerator Switch (string scene, float waitTime) {
        yield return new WaitForSeconds (waitTime);
        SceneManager.LoadScene (scene);
    }
    public void QuitGame () {
        Application.Quit ();
    }
}
