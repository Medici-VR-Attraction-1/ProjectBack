using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

public class MultiPlayGameManager : MonoBehaviour
{
    private static MultiPlayGameManager _instance = null;

    #region Public Static Method
    public static MultiPlayGameManager GetInstance() { return _instance; }
    #endregion

    #region Public Method
    public void AddPlayerScore(int value) { _playerScore += value; }
    #endregion

    [SerializeField]
    private GameObject VRPlayerInstance = null;

    [SerializeField]
    private GameObject KMPlayerInstance = null;

    [SerializeField]
    private Transform PlayerStartPoint = null;

    [SerializeField]
    private Transform SubPlayerStartPoint = null;

    private GameObject _playerInstance = null;
    private int _playerScore = 0;

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        Application.targetFrameRate = 90;
        // Check and Set Singleton Object
        if (_instance != null)
        {
            Debug.Log("MainSceneGameManager : Game Manager has Duplicated, Delete Another One");
            gameObject.SetActive(false);
        }
        else
        {
            _instance = this;
        }

        // Check Property Set Collectly and Instatiate by Player
        if (!CheckIsPropertySet())
        {
            Debug.Log("MainSceneGameManager : Gama Manager Property has Not Set, Check Properties");
            gameObject.SetActive(false);
            Application.Quit();
        }

        GameObject targetPrefab = XRDevice.isPresent ? VRPlayerInstance : KMPlayerInstance;

        if (!PhotonNetwork.IsConnected)
        {
            _playerInstance = Instantiate(targetPrefab,
                                    PlayerStartPoint.position,
                                    PlayerStartPoint.rotation);
        }
        else
        {
            PlayerStartPoint = PhotonNetwork.IsMasterClient ? PlayerStartPoint : SubPlayerStartPoint;
            _playerInstance = PhotonNetwork.Instantiate(targetPrefab.name,
                                                 PlayerStartPoint.position,
                                                 PlayerStartPoint.rotation);
        }
    }
    #endregion

    #region Private Method
    // Check Serialize Field is Set On Inspector. If Properties all Set, return true.
    private bool CheckIsPropertySet()
    {
        bool checkResult = VRPlayerInstance != null
                         && KMPlayerInstance != null
                         && PlayerStartPoint != null;

        return checkResult;
    }
    #endregion
}
