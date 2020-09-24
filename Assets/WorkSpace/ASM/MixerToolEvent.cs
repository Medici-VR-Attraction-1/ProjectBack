using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixerToolEvent : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1f)]
    private float CookingSpeed = 0.1f;

    private Dictionary<int, Transform> _ingredientTransform = new Dictionary<int, Transform>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MixerAction();
    }

    private void MixerAction()
    {
        if (_ingredientTransform.Count != 0)
        {
            foreach (Transform targetTranfor in _ingredientTransform.Values)
            {
                //크기가 줄어든다
                //특정 크기 밑이 되면 삭제

                //가능하면 물색도 같이 바꿔주기 해당 재료색으로
            }
            // 액체를 채워줍시다
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Ingredient")
        {
            _ingredientTransform[other.transform.GetInstanceID()] = other.transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _ingredientTransform.Remove(collision.transform.GetInstanceID());
    }
}
