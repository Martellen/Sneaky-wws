using Network;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : PhotonSingleton<NetworkManager>
{
    public class ConnectionModel
    {
        public Action onSuccess;
        public Action<short, string> onError;
    }

    public event Action<Player> OnPlayerJoined;
    public event Action<Player> OnPlayerLeft;

    /// <summary>
    /// Time to live in seconds after disconnection.
    /// </summary>
    private const int InactivePlayerTil = 180;

    private bool isConnected = false;
    private bool isConnecting = false;
    private bool? isConnectedToMaster = null;
    private bool? isConnectedToLobby = null;
    private bool? isConnectedToRoom = null;
    private ConnectionModel model = null;
    private List<Player> players = new List<Player>();


    private bool HasConnectionFailed
    {
        get
        {
            return
                isConnectedToMaster.HasValue == true && isConnectedToMaster.Value == false ||
                isConnectedToLobby.HasValue == true && isConnectedToLobby.Value == false ||
                isConnectedToRoom.HasValue == true && isConnectedToRoom.Value == false;
        }
    }
    public IReadOnlyList<Player> Players => players;


    public void Connect(string roomName, ConnectionModel model)
    {
        /// Check if already connected.
        /// Connect and return result.

        if (HasConnectionFailed == true)
        {
            ResetStates();
        }

        if (isConnected == true || isConnecting == true)
        {
            string errorMessage = $"Not yet connected to a room.";
            Debug.Log(errorMessage);
            model.onError?.Invoke(0, errorMessage);
            return;
        }

        this.model = model;
        StartCoroutine(ConnectionCoroutine(roomName));
    }

    private IEnumerator ConnectionCoroutine(string roomName)
    {
        isConnecting = true;

        yield return StartCoroutine(NameGeneratorOnline.GetRandomName(
            (name) => PhotonNetwork.NickName = name,
            () => { }
            ));

        if (isConnectedToMaster != true)
        {
            PhotonNetwork.ConnectUsingSettings();
            yield return new WaitUntil(() => isConnectedToMaster != null);
            if (isConnectedToMaster == false)
            {
                yield break;
            }
        }

        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions()
        {
            EmptyRoomTtl = 0,
            IsOpen = true,
            IsVisible = false,
            MaxPlayers = 2,
            PlayerTtl = 1000 * InactivePlayerTil,
            CleanupCacheOnLeave = true,
        }, TypedLobby.Default);
        yield return new WaitUntil(() => isConnectedToRoom != null);
        if (isConnectedToRoom == false)
        {
            yield break;
        }
        isConnected = true;
        isConnecting = false;
        model.onSuccess?.Invoke();
    }


    public void Disconnect(ConnectionModel model)
    {
        if (isConnected == false)
        {
            string errorMessage = $"Already connected to a room.";
            Debug.Log(errorMessage);
            model.onError?.Invoke(0, errorMessage);
            return;
        }
        this.model = model;
        StartCoroutine(DisconnectionCoroutine());
    }
    private IEnumerator DisconnectionCoroutine()
    {
        if (isConnectedToRoom == true)
        {
            PhotonNetwork.LeaveRoom(true);
            yield return new WaitUntil(() => isConnectedToRoom.HasValue == false || isConnectedToRoom == false);
        }
        isConnected = false;
        model.onSuccess?.Invoke();
    }




    private void ResetStates()
    {
        isConnected = false;
        isConnecting = false;
        isConnectedToMaster = null;
        isConnectedToLobby = null;
        isConnectedToRoom = null;
        model = null;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to master.");
        isConnectedToMaster = true;
    }
    public override void OnJoinedLobby()
    {
        Debug.Log($"Connected to lobby.");
        isConnectedToLobby = true;
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"Connected to room.");
        players.Clear();
        players.AddRange(PhotonNetwork.PlayerList);
        isConnectedToRoom = true;
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        string errorMessage = $"Error connecting to room (code: {returnCode}, message: {message}).";
        Debug.Log(errorMessage);
        model.onError?.Invoke(returnCode, errorMessage);
        isConnectedToRoom = false;
    }
    public override void OnLeftRoom()
    {
        players.Clear();
        isConnectedToRoom = null;
    }
    public override void OnLeftLobby()
    {
        isConnectedToLobby = null;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} joined the room.");
        players.Add(newPlayer);
        OnPlayerJoined?.Invoke(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (players.Remove(otherPlayer) == true)
        {
            Debug.Log($"Player {otherPlayer.NickName} left the room.");
            OnPlayerLeft?.Invoke(otherPlayer);
        }
        else
        {
            Debug.Log($"Player {otherPlayer.NickName} left the room, but was not in the Players list.");
        }
    }
}


//using Photon.Pun;
//using Photon.Realtime;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//public static class NetworkManager
//{
//    public interface INetworkConnection
//    {
//        event Action OnConnectedToServer;
//        event Action OnConnectedToRoom;
//        event Action OnConnectedToRoomFailed;
//        event Action OnDisconnectedFromServer;

//        event Action<Player> OnPlayerJoined;
//        event Action<Player> OnPlayerLeft;

//        IReadOnlyList<Player> Players { get; }

//        void Disconnect();
//    }
//    private class NetworkConnection : MonoBehaviourPunCallbacks, INetworkConnection
//    {
//        public event Action OnConnectedToServer;
//        public event Action OnConnectedToRoom;
//        public event Action OnConnectedToRoomFailed;
//        public event Action OnDisconnectedFromServer;

//        public event Action<Player> OnPlayerJoined;
//        public event Action<Player> OnPlayerLeft;

//        private string roomName;
//        private bool isConnectionBeingInitialized = false;
//        private List<Player> players = new List<Player>();

//        public IReadOnlyList<Player> Players => players;

//        private void Update()
//        {
//            if (isConnectionBeingInitialized == false && PhotonNetwork.IsConnectedAndReady == true)
//            {
//                isConnectionBeingInitialized = true;
//                OnConnectedToServer?.Invoke();
//            }
//        }

//        private void OnDestroy()
//        {
//            if (connection == this)
//            {
//                Debug.Log($"NetworkConnection has been destroyed without calling NetworkManager.Disconnect. Setting connection to null");
//                connection = null;
//            }
//        }

//        public void Connect(string roomName)
//        {
//            if (isConnectionBeingInitialized == false)
//            {
//                Debug.LogError($"Trying to connect to a lobby while connection is not ready. Wait for OnConnectedToServer event.");
//                return;
//            }
//            if (this.roomName != null)
//            {
//                Debug.LogError($"Already connecting.");
//                return;
//            }
//            this.roomName = roomName;
//            PhotonNetwork.JoinLobby();
//        }
//        public override void OnJoinedLobby()
//        {
//            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions()
//            {
//                IsVisible = false,
//                MaxPlayers = 2,
//            }, TypedLobby.Default);
//        }
//        public override void OnJoinRoomFailed(short returnCode, string message)
//        {
//            this.roomName = null;
//            OnConnectedToRoomFailed?.Invoke();
//        }
//        public override void OnJoinedRoom()
//        {
//            OnConnectedToRoom?.Invoke();
//        }
//        public override void OnDisconnected(DisconnectCause cause)
//        {
//            OnDisconnectedFromServer?.Invoke();
//            if (gameObject != null)
//            {
//                Destroy(gameObject);
//            }
//        }
//        public override void OnPlayerEnteredRoom(Player newPlayer)
//        {
//            players.Add(newPlayer);
//            OnPlayerJoined?.Invoke(newPlayer);
//        }
//        public override void OnPlayerLeftRoom(Player otherPlayer)
//        {
//            players.Remove(otherPlayer);
//            OnPlayerLeft?.Invoke(otherPlayer);
//        }
//        public void Disconnect()
//        {
//            if (NetworkManager.connection == this)
//            {
//                NetworkManager.connection = null;
//                this.roomName = null;
//                PhotonNetwork.Disconnect();
//            }
//        }

//    }


//    private static NetworkConnection connection;

//    public static INetworkConnection Connection => connection;


//    public static INetworkConnection Initialize()
//    {
//        PhotonNetwork.ConnectUsingSettings();
//        if (connection != null)
//        {
//            Disconnect();
//        }
//        connection = new GameObject($"NetworkConnection").AddComponent<NetworkConnection>();
//        return connection;
//    }

//    public static void Connect(string roomName)
//    {
//        if (connection == null)
//        {
//            Debug.LogError($"Trying to connect to the server with no connection. Call NetworkManager.Initialize first.");
//            return;
//        }
//        connection.Connect(roomName);
//    }

//    public static void Disconnect()
//    {
//        if (connection == null)
//        {
//            Debug.Log($"Trying to disconnect from the server with no connection.");
//            return;
//        }
//        connection.Disconnect();
//    }
//}