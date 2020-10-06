using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ObjectInteractProvider : MonoBehaviour
{
    [SerializeField]
    private ObjectInteractType InteractionType = ObjectInteractType.None;

    [SerializeField]
    private float ObjectInteractionSpeed = 30f;

    private Dictionary<int, ObjectInteractController> _interactObjects = new Dictionary<int, ObjectInteractController>();

    private void Awake()
    {
        if(InteractionType == ObjectInteractType.None)
        {
            this.enabled = false;
        }
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

            if (value.GetObjectType() == InteractionType)
                _interactObjects[componentKey] = value;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _interactObjects.Remove(collision.gameObject.GetInstanceID());
    }
}
