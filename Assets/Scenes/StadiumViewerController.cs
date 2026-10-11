using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class StadiumViewerController : MonoBehaviour
{
    [Header("Touch Settings")]
    public float rotateSpeed = 0.2f;
    public float zoomSpeed = 0.002f;
    public float minScale = 0.001f;
    public float maxScale = 10.0f;

    void OnEnable()
    {
        // Enables multi-touch tracking under New Input System
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        var touches = Touch.activeTouches;

        // 1-Finger Touch: Rotate
        if (touches.Count == 1)
        {
            var touch = touches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                transform.Rotate(Vector3.up, -touch.delta.x * rotateSpeed, Space.World);
                transform.Rotate(Vector3.right, touch.delta.y * rotateSpeed, Space.World);
            }
        }
        // 2-Finger Pinch: Zoom
        else if (touches.Count == 2)
        {
            var touch0 = touches[0];
            var touch1 = touches[1];

            Vector2 touch0Prev = touch0.screenPosition - touch0.delta;
            Vector2 touch1Prev = touch1.screenPosition - touch1.delta;

            float prevDistance = (touch0Prev - touch1Prev).magnitude;
            float currentDistance = (touch0.screenPosition - touch1.screenPosition).magnitude;
            float delta = currentDistance - prevDistance;

            Vector3 targetScale = transform.localScale + Vector3.one * (delta * zoomSpeed);
            targetScale.x = Mathf.Clamp(targetScale.x, minScale, maxScale);
            targetScale.y = Mathf.Clamp(targetScale.y, minScale, maxScale);
            targetScale.z = Mathf.Clamp(targetScale.z, minScale, maxScale);

            transform.localScale = targetScale;
        }
    }
}