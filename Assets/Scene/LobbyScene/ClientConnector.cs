using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ClientConnector : MonoBehaviourPunCallbacks
{
    private readonly byte maxPlayerPerRoom = 2;

    #region Private Field
    private bool isConnecting = false;
    private string gameVersion = "0.2";
    #endregion

    #region Public Method
    public void TryConnect()
    {
        Debug.LogWarning("Launcher Try Connect Please Wait...");

        if (!isConnecting)
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Photon Server Already Connected, Join Random Room...");
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                Debug.LogWarning("Connecting to PhotonNetwork...");
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
            }
        }

        isConnecting = true;
    }
    #endregion

    #region MonoBehaviour Callback
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        TryConnect();
    }
    #endregion

    #region Pun Callback
    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            Debug.LogWarning("Master Server Connection Success, Try to Join Another Room...");
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);
        Debug.Log("Can't Find Available Room, Create New Room");

        PhotonNetwork.CreateRoom(PhotonNetwork.CountOfRooms.ToString(),
                               new RoomOptions { MaxPlayers = maxPlayerPerRoom, CleanupCacheOnLeave = true });
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning(cause.ToString());
        Debug.LogWarning("Connection Lost, Please Press Connect Button");

        isConnecting = false;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Connection Success, Load Game Scene...");
        
        PhotonNetwork.LoadLevel("MultiPlayerScene");
    }
    #endregion
}
