﻿using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// This class defines all characteristics and functions of the lobby menu.
/// </summary>
public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private InputField nameInp, roomInput;

    [SerializeField]
    private Text photonStatus, welcomeUser, roomName;

    [SerializeField]
    private GameObject mainLobbyCanvas, roomLobbyCanvas;

    [SerializeField]
    private GameObject createRoomPanel, backButton, createRoomButton;

    [SerializeField]
    private Button photonGenerateRoomButton;

    private readonly int minRoomNameLen = 4;

    private void Awake()
    {
        PhotonNetwork.NickName = nameInp.text;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NetworkingClient.EnableLobbyStatistics = true;
        // Settings defined via PhotonServerSettings
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {

    }

    public override void OnConnectedToMaster()
    {
        photonStatus.text = "Connected to master.";

        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Destroy(photonStatus);
    }

    public override void OnJoinedLobby()
    {
        welcomeUser.text = "Welcome, " + PhotonNetwork.LocalPlayer.NickName;
        mainLobbyCanvas.SetActive(true);
        //Debug.Log("Joined lobby: " + PhotonNetwork.CurrentLobby.ToString());
    }

    public override void OnJoinedRoom()
    {
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        mainLobbyCanvas.SetActive(false);
        roomLobbyCanvas.SetActive(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("<Color=Red><a>Join Room Failed</a></Color>", this);
    }

    public void OnClick_GeneratePhotonRoom()
    {
        System.Random randomNumber = new System.Random();
        int randomInt = randomNumber.Next();
        RoomOptions options = new RoomOptions
        {
            IsOpen = true,
            IsVisible = true,
            MaxPlayers = 2,
            CustomRoomProperties = new Hashtable() { { "deckRandomValue", randomInt } }
        };
        PhotonNetwork.JoinOrCreateRoom(roomInput.text, options, null);
    }

    public void OnRoomNameField_Changed()
    {
        if (roomInput.text.Length >= minRoomNameLen)
        {
            photonGenerateRoomButton.interactable = true;
        }
        else
        {
            photonGenerateRoomButton.interactable = false;
        }
    }

    public void OnClick_CreateRoom()
    {
        createRoomButton.SetActive(false);
        createRoomPanel.SetActive(true);
    }

    public void OnClick_CreateRoomBack()
    {
        createRoomButton.SetActive(true);
        createRoomPanel.SetActive(false);
    }
}
