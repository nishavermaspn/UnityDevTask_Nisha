using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class TurnBasedGame : MonoBehaviourPunCallbacks
{
    public Button[] boardButtons; // UI Buttons for the grid
    public Text statusText; // UI Text for feedback
    private int[] boardState = new int[9]; // 0 = empty, 1 = Player 1, 2 = Player 2
    private int currentPlayer = 1; // 1 for Player 1, 2 for Player 2

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("GameRoom", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room"+PhotonNetwork.CurrentRoom.Name);
        ResetBoard();
        UpdateUI();
    }

    public void OnCellClicked(int index)
    {
        Debug.Log("PhotonNetwork.LocalPlayer.ActorNumber"+ PhotonNetwork.LocalPlayer.ActorNumber);
        Debug.Log("CurrentPlayer" + currentPlayer);
        if (!PhotonNetwork.IsConnected || PhotonNetwork.LocalPlayer.ActorNumber != currentPlayer) return;
        if (boardState[index] == 0)
        {
            photonView.RPC("MakeMove", RpcTarget.All, index, currentPlayer);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient) // Only the host sends the state
        {
            photonView.RPC("SyncBoardState", newPlayer, boardState, currentPlayer);
        }
    }

    [PunRPC]
    void SyncBoardState(int[] syncedBoard, int syncedPlayer)
    {
        boardState = syncedBoard;
        currentPlayer = syncedPlayer;
        UpdateBoardUI(); // Update UI after sync
    }
    void UpdateBoardUI()
    {
        for (int i = 0; i < boardButtons.Length; i++)
        {
            if (boardState[i] == 1)
                boardButtons[i].GetComponentInChildren<Text>().text = "X";
            else if (boardState[i] == 2)
                boardButtons[i].GetComponentInChildren<Text>().text = "O";
            else
                boardButtons[i].GetComponentInChildren<Text>().text = "";
        }
        statusText.text = $"Player {currentPlayer}'s Turn";
    }

    [PunRPC]
    void MakeMove(int index, int player)
    {
        if (boardState[index] == 0)
        {
            boardState[index] = player;
            boardButtons[index].GetComponentInChildren<Text>().text = (player == 1) ? "X" : "O";

            if (CheckWin(player))
            {
                statusText.text = $"Player {player} Wins!";
                DisableBoard();
                return;
            }
            currentPlayer = (player == 1) ? 2 : 1;
            UpdateUI();
        }
    }

    bool CheckWin(int player)
    {
        int[,] winPatterns = {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Rows
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Columns
            {0, 4, 8}, {2, 4, 6} // Diagonals
        };
        for (int i = 0; i < winPatterns.GetLength(0); i++)
        {
            if (boardState[winPatterns[i, 0]] == player &&
                boardState[winPatterns[i, 1]] == player &&
                boardState[winPatterns[i, 2]] == player)
            {
                return true;
            }
        }
        return false;
    }

    void UpdateUI()
    {
        statusText.text = $"Player {currentPlayer}'s Turn";
    }

    void ResetBoard()
    {
        for (int i = 0; i < boardState.Length; i++)
        {
            boardState[i] = 0;
            boardButtons[i].GetComponentInChildren<Text>().text = "";
        }
        currentPlayer = 1;
        UpdateUI();
    }

    void DisableBoard()
    {
        foreach (Button btn in boardButtons)
        {
            btn.interactable = false;
        }
    }
}
