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

    private void Update()
    {
        if (_ingredientList.Count > 1 && PhotonNetwork.IsMasterClient)
        {
            for (int i = 1; i < _ingredientList.Count; i++)
            {
                if (Vector3.Distance(_ingredientList[i].position, _ingredientList[i - 1].position) > 1f) 
                {
                    InteractObjectListsRemove(i);
                    SortIngredientStacks(i);
                }
            }
        }
    }

    private void SortIngredientStacks(int index)
    {
        for (int i = index; i < _ingredientList.Count; i++)
        {
            _ingredientList[i].position -= _offsetVector;
        }
    }

    private void InteractObjectListsRemove(int index)
    {
        _ingredientList.RemoveAt(index);
        _burgerProperty.RemoveAt(index);
    }

    private void InteractObjectListsAdd(Transform tr)
    {
        _ingredientList.Add(tr);

        ObjectInteractController oic = tr.GetComponent<ObjectInteractController>();
        _burgerProperty.Add(oic.GetObjectTypeName());
    }

    private void OnCollisionEnter(Collision collision)
    {
        Transform colObject = collision.transform;

        if (colObject.tag == "Ingredient" && !_ingredientList.Contains(colObject))
        {
            Rigidbody _ingredientBody = colObject.GetComponent<Rigidbody>();
            _ingredientBody.isKinematic = true;

            colObject.position = _currentPosition + _offsetVector * _ingredientList.Count;
            colObject.rotation = Quaternion.identity;
            colObject.Rotate(new Vector3(0, Random.Range(0f, 360f), 0));

            InteractObjectListsAdd(colObject);
        }
    }
}