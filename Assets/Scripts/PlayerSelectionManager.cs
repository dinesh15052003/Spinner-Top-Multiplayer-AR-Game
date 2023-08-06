using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{
    public Transform playerSwitcherTransform;
    public GameObject[] spinnerTopModels;

    public Button next_button;
    public Button prev_button;

    public int playerSelectionNumber;

    [Header("UI")]
    public TextMeshProUGUI playerModelType_Text;

    public GameObject uI_Selection;
    public GameObject uI_AfterSelection;

    #region UNITY Methods
    // Start is called before the first frame update
    void Start()
    {
        uI_Selection.SetActive(true);
        uI_AfterSelection.SetActive(false);

        playerSelectionNumber = 0;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI Callback Methods
    public void NextPlayer()
    {
        playerSelectionNumber += 1; 
        if (playerSelectionNumber >= spinnerTopModels.Length)
            playerSelectionNumber = 0;
        next_button.enabled = false;
        prev_button.enabled = false;
        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, 90, 1.0f));
        if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            // This means the player models type is ATTACK
            playerModelType_Text.text = "Attack";
        }
        else
        {
            // This means the player models type is Defend
            playerModelType_Text.text = "Defend";
        }
    }
    public void PreviousPlayer()
    {
        playerSelectionNumber -= 1;
        if (playerSelectionNumber <= 0)
            playerSelectionNumber = spinnerTopModels.Length-1;
        next_button.enabled = false;
        prev_button.enabled = false;
        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, -90, 1.0f));
        if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            // This means the player models type is ATTACK
            playerModelType_Text.text = "Attack";
        }
        else
        {
            // This means the player models type is Defend
            playerModelType_Text.text = "Defend";
        }
    }

    public void OnSelectButtonClicked()
    {
        uI_Selection.SetActive(false);
        uI_AfterSelection.SetActive(true);

        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { { MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }
    public void OnReselectButtonClicked()
    {
        uI_Selection.SetActive(true);
        uI_AfterSelection.SetActive(false);
    }
    public void OnBattleButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Gameplay");
    }
    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }
    #endregion

    #region Private Methods
    IEnumerator Rotate(Vector3 axis, Transform transformToRotate, float angle, float duration)
    {
        Quaternion originalRotation = transformToRotate.rotation;
        Quaternion finalRotation = transformToRotate.rotation * Quaternion.Euler(axis*angle);

        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            transformToRotate.rotation = Quaternion.Slerp(originalRotation, finalRotation, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        transformToRotate.rotation = finalRotation;

        next_button.enabled = true;
        prev_button.enabled = true;
    }
    #endregion
}
