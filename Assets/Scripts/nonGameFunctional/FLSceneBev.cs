using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FLSceneBev : MonoBehaviour
{
    void OnEnable () {
        StartCoroutine (unloader ());
    }
    private IEnumerator unloader () {
        yield return new WaitForSeconds (0.5f);
        SceneManager.LoadScene ("mainMenu");
    }
}
