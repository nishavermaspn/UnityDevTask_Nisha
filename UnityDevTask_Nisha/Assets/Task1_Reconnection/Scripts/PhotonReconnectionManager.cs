using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class PhotonReconnectionManager : MonoBehaviourPunCallbacks
{
    private bool isReconnecting = false;
    private float reconnectAttemptInterval = 5f;
    private int maxReconnectAttempts = 3;
    private int reconnectAttempts = 0;
    private string lastRoomName = "";
    private string lobbyName;
    private string roomName;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void StartGame(string lobbyName)
    {
        if (!PhotonNetwork.IsConnected)
        {
            this.lobbyName = lobbyName;
            roomName = "Room" + lobbyName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(new TypedLobby(lobbyName, LobbyType.Default));
    }
    public override void OnJoinedLobby()
    {
        Debug.Log($"Joined Lobby: {lobbyName}");

        // Try to join a specific room
        JoinSpecificRoom(roomName);
    }

    // Call this method to join a specific room by name
    public void JoinSpecificRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.Log("Not connected to Photon!");
        }
    }

    // If the room does not exist, create one
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Failed to join room: {message}. Creating new room...");
        CreateRoom(roomName);
        UIManager.uIManagerInstance.ShowRoomMessage(false,roomName);

    }
    // Create a room if it doesn't exist
    private void CreateRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room creation failed: {message}");
    }
    public override void OnJoinedRoom()
    {
        lastRoomName = PhotonNetwork.CurrentRoom.Name;
        isReconnecting = false;
        reconnectAttempts = 0;
        UIManager.uIManagerInstance.ShowRoomMessage(true,roomName);
        Debug.Log("Joined Room Successfully"+lastRoomName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Disconnected from Photon: " + cause);
        if (cause != DisconnectCause.DisconnectByClientLogic && !isReconnecting)
        {
            lastRoomName = PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Name : roomName;
            StartCoroutine(AttemptReconnect());
        }
    }

    private IEnumerator AttemptReconnect()
    {
        isReconnecting = true;
        UIManager.uIManagerInstance.ShowReconnectingMessage(true);

        while (reconnectAttempts < maxReconnectAttempts)
        {

            if (PhotonNetwork.IsConnected)
            {
                StartCoroutine(AttemptRejoinRoom());
                yield break;
            }

            reconnectAttempts++;
            Debug.Log("Attempting to reconnect... Attempt: " + reconnectAttempts);
            PhotonNetwork.Reconnect();

            yield return new WaitForSeconds(reconnectAttemptInterval);
        }

        if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
        {
            StartCoroutine(AttemptRejoinRoom());
            yield break;
        }

        // If all attempts fail, return to the lobby or main menu
        Debug.Log("Reconnection failed. Returning to main menu.");
        UIManager.uIManagerInstance.ShowReconnectingMessage(false);
        UIManager.uIManagerInstance.ShowMainMenu(true);
        Debug.Log("all attempts fail, return to the lobby or main menu...");
        
    }

    private IEnumerator AttemptRejoinRoom()
    {
        yield return new WaitForSeconds(2f); // Give time for reconnection
        if (!PhotonNetwork.InRoom && !string.IsNullOrEmpty(lastRoomName))
        {
            PhotonNetwork.JoinRoom(lastRoomName);
        }
    }
}
