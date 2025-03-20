using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
    public GameObject ReconnectPnl;
    public GameObject JoinedRoomPnl;
    public GameObject MainMenuPnl;
    public Text RoomInfoText;
    public static UIManager uIManagerInstance { get; private set; }

    private void Awake()
    {
        if (uIManagerInstance != null && uIManagerInstance != this)
        {
            Destroy(gameObject);
        }
        uIManagerInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowReconnectingMessage(bool isShowReconnect)
    {
        ReconnectPnl.SetActive(isShowReconnect);
    }

    public void ShowRoomMessage(bool isShowMsg,string roomName)
    {
        JoinedRoomPnl.SetActive(isShowMsg);
        RoomInfoText.text = "Player has joined the room " + roomName;
        ReconnectPnl.SetActive(!isShowMsg);
        MainMenuPnl.SetActive(!isShowMsg);
    }
    public void ShowMainMenu(bool isShow)
    {
        MainMenuPnl.SetActive(isShow);
        if (JoinedRoomPnl.activeInHierarchy)
        {
            JoinedRoomPnl.SetActive(!isShow);
        }
    }

}
