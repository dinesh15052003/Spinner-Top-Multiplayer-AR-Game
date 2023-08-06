using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField playerNameInputField;
    public GameObject ui_LoginGameObject;

    [Header("Lobby UI")]
    public GameObject ui_LobbyGameobject;
    public GameObject ui_3DGameobject;

    [Header("Connection Status UI")]
    public GameObject ui_ConnectionStatusGameobject;
    public Text connectionStatusText;
    public bool showConnectionStatus = false;

    #region UNITY Methods
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            // Activating only Lobby UI
            ui_LobbyGameobject.SetActive(true);
            ui_3DGameobject.SetActive(true);

            ui_ConnectionStatusGameobject.SetActive(false);
            ui_LoginGameObject.SetActive(false);
        }
        else
        {
            // Activating only Login UI since we did not connect to photon yet
            ui_LobbyGameobject.SetActive(false);
            ui_3DGameobject.SetActive(false);
            ui_ConnectionStatusGameobject.SetActive(false);

            ui_LoginGameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (showConnectionStatus)
        {
            connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
        }
    }
    #endregion

    #region UI Callback Methods
    public void OnEnterGameButtonClicked()
    {
        string playerName = playerNameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            ui_LobbyGameobject.SetActive(false);
            ui_3DGameobject.SetActive(false);
            ui_LoginGameObject.SetActive(false);

            showConnectionStatus = true;
            ui_ConnectionStatusGameobject.SetActive(true);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;

                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("Player name is invalid or empyty!");
        }
    }

    public void OnQuickMatchButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
    }
    #endregion

    #region Photon Callback Methods
    public override void OnConnected()
    {
        Debug.Log("We connected to Internet");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon Server.");

        ui_LobbyGameobject.SetActive(true);
        ui_3DGameobject.SetActive(true);

        ui_LoginGameObject.SetActive(false);
        ui_ConnectionStatusGameobject.SetActive(false);

    }
    #endregion
}
