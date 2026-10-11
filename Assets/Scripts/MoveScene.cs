using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveToNext());  
    }

    IEnumerator MoveToNext() 
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }
}
