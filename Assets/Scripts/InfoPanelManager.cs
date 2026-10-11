using UnityEngine;

public class InfoPanelManager : MonoBehaviour
{
    public static InfoPanelManager Instance;

    public GameObject[] infoPanels;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPanel(int panelIndex)
    {
        CloseAllPanels();

        if (panelIndex >= 0 && panelIndex < infoPanels.Length)
        {
            if (infoPanels[panelIndex] != null)
            {
                infoPanels[panelIndex].SetActive(true);
            }
        }
    }

    public void CloseAllPanels()
    {
        foreach (GameObject panel in infoPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }
}