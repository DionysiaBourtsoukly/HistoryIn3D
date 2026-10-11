using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class AssignCanvasCamera : MonoBehaviour
{
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }
    }
}