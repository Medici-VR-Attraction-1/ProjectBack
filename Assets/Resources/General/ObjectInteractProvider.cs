using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;

public class ObjectInteractProvider : MonoBehaviourPun
{
    [SerializeField]
    private ObjectInteractType InteractionType = ObjectInteractType.None;

    [SerializeField]
    private float ObjectInteractionSpeed = 30f;

    [SerializeField]
    private GameObject BurnVFXObject = null;

    private GameObject _vfxObject;

    private Dictionary<int, ObjectInteractController> _interactObjects = new Dictionary<int, ObjectInteractController>();
    
    private Dictionary<int, VisualEffect> _vfxObjectsHash = new Dictionary<int, VisualEffect>();
    private Queue<VisualEffect> _vfxObjectPool = new Queue<VisualEffect>();

    private void Awake()
    {
        if(InteractionType == ObjectInteractType.None)
        {
            this.enabled = false;
        }

        _vfxObject = BurnVFXObject;

        GameObject cache;
        for(int i = 0; i < 20; i++)
        {
            cache = Instantiate(_vfxObject);
            _vfxObjectPool.Enqueue(cache.GetComponent<VisualEffect>());
            cache.SetActive(false);
        }
    }

    private VisualEffect SetUpAndStartVFX(Vector3 position)
    {
        VisualEffect ve = _vfxObjectPool.Dequeue();
        ve.gameObject.SetActive(true);
        ve.Play();
        ve.transform.position = position;
        ve.transform.rotation = Quaternion.identity;

        return ve;
    }

    private void ReturnVFXToObjectPool(VisualEffect ve)
    {
        ve.Stop();
        _vfxObjectPool.Enqueue(ve);
    }

    private void Update()
    {
        if(_interactObjects.Count > 0)
        {
            foreach(ObjectInteractController controller in _interactObjects.Values)
            {
                controller.InteractObject(ObjectInteractionSpeed);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ingredient")
        {
            int componentKey = collision.gameObject.GetInstanceID();
            ObjectInteractController value = collision.gameObject.GetComponent<ObjectInteractController>();

            if (value.GetInteractType() == InteractionType)
                _interactObjects[componentKey] = value;

            photonView.RPC("_StartVFX", RpcTarget.All, componentKey, collision.transform.position);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _interactObjects.Remove(collision.gameObject.GetInstanceID());

        photonView.RPC("_StopVFX", RpcTarget.All, collision.gameObject.GetInstanceID());
    }

    [PunRPC]
    private void _StartVFX(int componentKey, Vector3 position)
    {
        _vfxObjectsHash[componentKey] = SetUpAndStartVFX(position);
    }

    [PunRPC]
    private void _StopVFX(int componentKey)
    {
        VisualEffect ve = _vfxObjectsHash[componentKey];
        ReturnVFXToObjectPool(ve);
    }
}
