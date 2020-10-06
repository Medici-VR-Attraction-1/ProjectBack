using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Valve.VR.InteractionSystem;

public struct CounterData
{
    public Vector3 CounterPosition;
    public CounterTriggerHandler CounterComponent;
    public string CounterName;

    public CounterData(Vector3 position, CounterTriggerHandler component, string counterName)
    {
        CounterPosition = position;
        CounterComponent = component;
        CounterName = counterName;
    }
}

public class GuestManager : MonoBehaviourPunCallbacks
{
    private static GuestManager _instance = null;
    public static GuestManager GetInstance() { return _instance; }

    [SerializeField]
    private GameObject GuestPrefab = null;

    [SerializeField, Range(1f, 50f)]
    private float GuestSpawnRate = 3.0f;

    private Queue<CounterData> _counterQueue = new Queue<CounterData>();
    private WaitForSeconds _guestSpawnWait = null;
    private string[] _serializeTargetNamesCache;

    #region Public Method
    public CounterData GetCounterFromQueue() 
    {
        CounterData counterData = _counterQueue.Dequeue();

        List<string> data = new List<string>();
        _counterQueue.ForEach<CounterData>((CounterData temp) => data.Add(temp.CounterName));
        photonView.RPC("_BroadcastQueueData", RpcTarget.Others, (object)data.ToArray());

        return counterData; 
    }

    public void ReturnCounter(CounterData data) { _counterQueue.Enqueue(data); }
    #endregion

    #region MonoBehaviour Callbacks
    // Make Random Position Queue
    private void Awake()
    {
        if(_instance != null || GuestPrefab == null)
        {
            Debug.LogError("Guest Manager Has Duplicated or Prefab Missing, Please Check Object");
            gameObject.SetActive(false);
        }
        _instance = this;

        _guestSpawnWait = new WaitForSeconds(GuestSpawnRate);

        if (photonView.IsMine)
        {
            List<Transform> guestSeats = transform.GetComponentsInChildren<Transform>().ToList<Transform>();
            guestSeats.RemoveAt(0);

            int randomIndex;
            while (guestSeats.Count != 0)
            {
                randomIndex = Random.Range(0, guestSeats.Count - 1);
                Transform tr = guestSeats[randomIndex];
                _counterQueue.Enqueue(new CounterData(tr.position,
                                                  tr.GetComponent<CounterTriggerHandler>(),
                                                  tr.name));
                guestSeats.RemoveAt(randomIndex);
            }
        }
    }

    // Start Spawn Action
    private void Start()
    {
        if (photonView.IsMine) StartCoroutine(_SpawnGuestToDefaultPosition());
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) PhotonNetwork.LeaveRoom();
    }
    // Instantiate Guest at Available Time with Available Counter
    private IEnumerator _SpawnGuestToDefaultPosition()
    {
        GameObject GuestCache;
        while (this.enabled)
        {
            if (_counterQueue.Count != 0)
            {
                // Instantiate Guest
                GuestCache = PhotonNetwork.InstantiateRoomObject(GuestPrefab.name, transform.position, Quaternion.identity);
            }

            yield return _guestSpawnWait;
        }

        yield return null;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        List<string> data = new List<string>();
        _counterQueue.ForEach<CounterData>((CounterData temp) => data.Add(temp.CounterName));
        photonView.RPC("_BroadcastQueueData", newPlayer, (object)data.ToArray());
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        if(PhotonNetwork.LocalPlayer == newMasterClient)
        {
            Debug.Log("This Client is Master Client");

            _counterQueue.Clear();

            _serializeTargetNamesCache.ForEach<string>((string temp) => {
                Transform tr = transform.Find(temp);
                _counterQueue.Enqueue(new CounterData(tr.position,
                                    tr.GetComponent<CounterTriggerHandler>(),
                                    tr.name));
            });

            photonView.TransferOwnership(newMasterClient);
            StartCoroutine(_SpawnGuestToDefaultPosition());
        }
    }

    [PunRPC]
    // Recieve Counter Name on Queue and Make Queue Data
    private void _BroadcastQueueData(string[] data)
    {
        Debug.Log("Queue Data Broadcast");
        _serializeTargetNamesCache = data.ToArray<string>();
    }
}
