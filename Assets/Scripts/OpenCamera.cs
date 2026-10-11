using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public void TriggerARScene()
    {
        SceneManager.LoadScene("ARFoundation_Test");
    }
}
