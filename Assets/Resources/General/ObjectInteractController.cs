using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Valve.VR.InteractionSystem;

public enum ObjectInteractType
{
    Burnable,
    Fillable,
    None
}

public enum ObjectTypeName
{
    BottomBun,
    Lettuce,
    Tomato,
    Cheese,
    Bacon,
    Beef,
    UpperBun,
    FrenchFry,
    Beverage,
    None
}

public class ObjectInteractController : MonoBehaviourPun, IPunObservable
{
    private delegate void ObjectUpdateMethod();
    private ObjectUpdateMethod InteractObjectUpdateBinder = null;

    [SerializeField]
    private ObjectTypeName ObjectMaterialType = ObjectTypeName.None;

    [SerializeField]
    private ObjectInteractType InteractionType = ObjectInteractType.None;

    [SerializeField]
    private List<MeshRenderer> TargetMeshRenderer = null;

    private float _interactAmount = 100f;
    private float _serializeInteractAmount = 100f;

    public float GetCookAmount() { return _interactAmount; }
    public ObjectInteractType GetInteractType() { return InteractionType; }
    public ObjectTypeName GetObjectTypeName() { return ObjectMaterialType; }

    public void InteractObject(float interactOffset) 
    {
        if (_interactAmount > Mathf.Epsilon && photonView.IsMine)
            _interactAmount -= Time.deltaTime * (1 / interactOffset) * 100;
    }

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        switch (InteractionType)
        {
            case ObjectInteractType.Burnable:
                InteractObjectUpdateBinder += new ObjectUpdateMethod(BurnableUpdate);
                break;

            case ObjectInteractType.Fillable:
                break;

            case ObjectInteractType.None:
                break;
        }
    }

    private void Update()
    {
        InteractObjectUpdateBinder?.Invoke();
        if (!photonView.IsMine)
        {
            _interactAmount = Mathf.Lerp(_interactAmount, _serializeInteractAmount, Time.deltaTime * 5f);
        }
    }
    #endregion

    #region Update Method for Bind
    private void BurnableUpdate()
    {
        foreach(MeshRenderer mr in TargetMeshRenderer)
        {
            mr.materials.ForEach<Material>((Material mt) =>
            {
                mt.SetFloat("Vector1_60927436", _interactAmount / 100f);
            });
        }
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_interactAmount);
        }
        else
        {
            _serializeInteractAmount = (float)stream.ReceiveNext();
        }
    }
}