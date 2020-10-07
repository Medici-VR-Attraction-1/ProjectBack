using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class BurgerTrayController : MonoBehaviourPun
{
    [SerializeField]
    private float StackOffset = 0.065f;
    private Vector3 _offsetVector;

    private Vector3 _currentPosition;
    private List<Transform> _ingredientList = new List<Transform>();
    private List<ObjectTypeName> _burgerProperty = new List<ObjectTypeName>();

    private void Awake()
    {
        _currentPosition = transform.position;
        _offsetVector = new Vector3(0, StackOffset, 0);

        _ingredientList.Add(transform);
    }

    private void InteractObjectListsAdd(Transform tr)
    {
        _ingredientList.Add(tr);
        _burgerProperty.Add(tr.GetComponent<ObjectInteractController>().GetObjectTypeName());
    }

    private void OnTriggerStay(Collider other)
    {
        Transform colObject = other.transform;

        if (colObject.tag == "Ingredient" && !_ingredientList.Contains(colObject))
        {
            HoldableObjectContoller hoc = colObject.GetComponent<HoldableObjectContoller>();
            if (hoc.photonView.IsMine && !hoc.CheckHoldByPlayer())
            {
                other.attachedRigidbody.isKinematic = true;
                other.enabled = false;

                colObject.position = _currentPosition + _offsetVector * _ingredientList.Count;
                colObject.rotation = Quaternion.identity;
                colObject.Rotate(new Vector3(0, Random.Range(0f, 360f), 0));

                InteractObjectListsAdd(colObject);
                photonView.RPC("BroadcastInteractID", RpcTarget.Others, hoc.componentID);
            }
        }
    }

    [PunRPC]
    private void BroadcastInteractID(int id)
    {
        InteractObjectListsAdd(HoldableObjectContoller.hash[id]);
    }
}