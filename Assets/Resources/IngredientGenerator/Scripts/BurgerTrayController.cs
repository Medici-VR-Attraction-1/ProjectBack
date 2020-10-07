using Photon.Pun;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class BurgerTrayController : MonoBehaviourPun
{
    [SerializeField]
    private float StackOffset = 0.065f;
    private Vector3 _offsetVector;

    private Vector3 _currentPosition;
    private List<Transform> _ingredientList = new List<Transform>();

    private StringBuilder _recipeDataCache = new StringBuilder();

    private string _targetRecipeCode = null;
    private bool _isAvailable = false;

    public bool IsAvailableRecipeCode()
    {
        if (_isAvailable)
        {
            _isAvailable = false;
            return true;
        }
        return false; 
    }
    public void SetTargetRecipeCode(string recipe) { _targetRecipeCode = recipe; }

    private void Awake()
    {
        _currentPosition = transform.position;
        _offsetVector = new Vector3(0, StackOffset, 0);

        _ingredientList.Add(transform);
    }

    private void InteractObjectListsAdd(Transform tr)
    {
        _ingredientList.Add(tr);

        ObjectTypeName otn = tr.GetComponent<ObjectInteractController>().GetObjectTypeName();
        _recipeDataCache.Append((int)otn);

        //
        if (RecipeManager.GetInstance().IsAvailableRecipe(_recipeDataCache.ToString()))
        {
            if(_recipeDataCache.ToString() == _targetRecipeCode && photonView.IsMine)
            {
                _isAvailable = true;

                photonView.RPC("BroadcastClearBuffer", RpcTarget.All, null);
            }
        }
        //
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

    [PunRPC]
    private void BroadcastClearBuffer()
    {
        _ingredientList.Remove(transform);
        _ingredientList.ForEach<Transform>((Transform trn) =>
        {
            PhotonView pv = trn.GetComponent<PhotonView>();
            if (pv.IsMine) PhotonNetwork.Destroy(trn.gameObject);
        });

        _recipeDataCache.Clear();
        _ingredientList.Clear();
        _ingredientList.Add(transform);
    }
}