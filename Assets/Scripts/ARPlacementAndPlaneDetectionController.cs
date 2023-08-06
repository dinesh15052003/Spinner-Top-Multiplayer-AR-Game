using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems; 

public class ARPlacementAndPlaneDetectionController : MonoBehaviour
{
    ARPlaneManager m_ARPlaneManager;
    ARPlacementManager m_ARPlacementManager;

    public GameObject placeButton;
    public GameObject adjustButton;
    public GameObject searchForGamesButton;
    public TextMeshProUGUI informUIPanel_Text;
    public GameObject scaleSlider;

    private void Awake()
    {
        m_ARPlaneManager= GetComponent<ARPlaneManager>();
        m_ARPlacementManager = GetComponent<ARPlacementManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        placeButton.SetActive(true);
        scaleSlider.SetActive(true);

        adjustButton.SetActive(false);
        searchForGamesButton.SetActive(false);

        informUIPanel_Text.text = "Move Phone to detect planes and place the Battle Arena!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled= false;
        m_ARPlacementManager.enabled= false;
        SetAllPlaneActiveOrDeactive(false);
        scaleSlider.SetActive(false);

        placeButton.SetActive(false);
        adjustButton.SetActive(true);
        searchForGamesButton.SetActive(true);

        informUIPanel_Text.text = "Great! You placed the ARENA ... Now, search for games to BATTLE!";
    }
    public void EnableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = true;
        m_ARPlacementManager.enabled = true;
        SetAllPlaneActiveOrDeactive(true);
        scaleSlider.SetActive(true);

        placeButton.SetActive(true);
        adjustButton.SetActive(false);
        searchForGamesButton.SetActive(false);

        informUIPanel_Text.text = "Move Phone to detect planes and place the Battle Arena!";
    }

    private void SetAllPlaneActiveOrDeactive(bool value)
    {
        foreach (var plane in m_ARPlaneManager.trackables)
            plane.gameObject.SetActive(value);
    }
}
