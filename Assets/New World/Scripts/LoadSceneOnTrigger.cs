using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class LoadSceneOnTrigger : MonoBehaviour
{
    public int sceneToLoadNum;

    private void OnTriggerEnter(Collider other) {
        SceneManager.LoadScene(sceneToLoadNum);
    }
}
