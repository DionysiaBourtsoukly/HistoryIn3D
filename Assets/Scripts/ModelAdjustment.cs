using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ModelAdjustmentUI : MonoBehaviour
{
    [Header("Script & Panel References")]
    public ARTapToPlace placementScript; 
    public GameObject controlPanel;       

    [Header("Performance Management")]
    [Tooltip("Drag the MediaPipeHandTracker object here")]
    public GameObject mediaPipeTracker;

    [Header("Adjustment Increments")]
    public float nudgeAmount = 0.01f;   
    public float scaleAmount = 0.02f; 
    public float rotationAmount = 5.0f; 

    private GameObject GetModel()
    {
        return placementScript != null ? placementScript.spawnedStadium : null;
    }

    public void ToggleMenu()
    {
        if (controlPanel != null)
        {
            bool isOpening = !controlPanel.activeSelf;
            
            controlPanel.SetActive(isOpening);
        }
    }

    // --- Move / Nudge Controls ---
    public void NudgeForward()  { MoveModel(GetFlatForward() * nudgeAmount); }
    public void NudgeBackward() { MoveModel(-GetFlatForward() * nudgeAmount); }
    public void NudgeRight()    { MoveModel(-GetFlatRight() * nudgeAmount); }
    public void NudgeLeft()     { MoveModel(GetFlatRight() * nudgeAmount); }
    public void NudgeUp()       { MoveModel(Vector3.up * nudgeAmount); }
    public void NudgeDown()     { MoveModel(Vector3.down * nudgeAmount); }

    private void MoveModel(Vector3 translation)
    {
        GameObject model = GetModel();
        if (model == null) return;

        RemoveAnchor(model);
        model.transform.position += translation;
        ApplyAnchor(model);
    }

    private Vector3 GetFlatForward()
    {
        GameObject model = GetModel();
        if (model == null) return Vector3.forward;
        Quaternion flatRotation = Quaternion.Euler(0, model.transform.eulerAngles.y, 0);
        return flatRotation * Vector3.forward;
    }

    private Vector3 GetFlatRight()
    {
        GameObject model = GetModel();
        if (model == null) return Vector3.right;
        Quaternion flatRotation = Quaternion.Euler(0, model.transform.eulerAngles.y, 0);
        return flatRotation * Vector3.right;
    }

    // --- Rotation Controls ---
    public void RotateLeft()
    {
        GameObject model = GetModel();
        if (model == null) return;

        RemoveAnchor(model);
        model.transform.Rotate(Vector3.up, rotationAmount, Space.World);
        ApplyAnchor(model);
    }

    public void RotateRight()
    {
        GameObject model = GetModel();
        if (model == null) return;

        RemoveAnchor(model);
        model.transform.Rotate(Vector3.up, -rotationAmount, Space.World);
        ApplyAnchor(model);
    }

    // --- Scale Controls ---
    public void ScaleUp()
    {
        GameObject model = GetModel();
        if (model == null) return;

        RemoveAnchor(model);
        model.transform.localScale *= 1.1f;
        ApplyAnchor(model);
    }

    public void ScaleDown()
    {
        GameObject model = GetModel();
        if (model == null) return;

        RemoveAnchor(model);
        model.transform.localScale *= 0.9f;
        ApplyAnchor(model);
    }

    private void RemoveAnchor(GameObject model)
    {
        // unparent before remove
        Transform parent = model.transform.parent;
        if (parent != null && parent.GetComponent<ARAnchor>() != null)
        {
            model.transform.SetParent(null, true);
            Destroy(parent.gameObject);
        }

        ARAnchor anchorComponent = model.GetComponent<ARAnchor>();
        if (anchorComponent != null)
        {
            DestroyImmediate(anchorComponent);
        }
    }

    private void ApplyAnchor(GameObject model)
    {
        if (model.GetComponent<ARAnchor>() == null)
        {
            model.AddComponent<ARAnchor>();
        }
    }

    // --- Layer Controls ---
    public string layerAName = "a";
    public string layerBName = "b";

    public void HideNextLayer()
    {
        GameObject model = GetModel();
        if (model == null) return;

        Transform partA = FindChildRecursive(model.transform, layerAName);
        Transform partB = FindChildRecursive(model.transform, layerBName);

        if (partA != null && partA.gameObject.activeSelf)
        {
            partA.gameObject.SetActive(false);
        }
        else if (partB != null && partB.gameObject.activeSelf)
        {
            partB.gameObject.SetActive(false);
        }
    }

    public void RevealNextLayer()
    {
        GameObject model = GetModel();
        if (model == null) return;

        Transform partA = FindChildRecursive(model.transform, layerAName);
        Transform partB = FindChildRecursive(model.transform, layerBName);

        if (partB != null && !partB.gameObject.activeSelf)
        {
            partB.gameObject.SetActive(true);
        }
        else if (partA != null && !partA.gameObject.activeSelf)
        {
            partA.gameObject.SetActive(true);
        }
    }

    private Transform FindChildRecursive(Transform parent, string nameToFind)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Equals(nameToFind, System.StringComparison.OrdinalIgnoreCase))
                return child;
        }
        return null;
    }
}