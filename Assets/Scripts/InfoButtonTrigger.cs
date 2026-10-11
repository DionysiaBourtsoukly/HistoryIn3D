using UnityEngine;

public class InfoButtonTrigger : MonoBehaviour
{
    public int panelIndex = 0;

    public void OnTapped()
    {
        Debug.Log("ON TAPPED CALLED");
        if (InfoPanelManager.Instance != null)
        {
            InfoPanelManager.Instance.ShowPanel(panelIndex);
        }
        else
        {
            Debug.LogWarning("InfoPanelManager instance not found in scene!");
        }
    }
}