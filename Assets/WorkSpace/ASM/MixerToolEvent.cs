using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixerToolEvent : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1f)]
    private float CookingSpeed = 0.1f;

    [SerializeField]
    private GameObject Juice = null;

    private Dictionary<int, Transform> _ingredientTransform = new Dictionary<int, Transform>();
    private MeshRenderer _juiceMeshRenderer = null;
    private Color _originalColor;
    private readonly Vector3 _ingredientScaleOffset = new Vector3(1f, 1f, 1f);
    private readonly Vector3 _juiceScaleOffset = new Vector3(0f, 1f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        if (Juice == null) Debug.Log("MixerToolEvent: No Inspector Value");

        _juiceMeshRenderer = Juice.GetComponentInChildren<MeshRenderer>();
        _originalColor = _juiceMeshRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        MixerAction();
    }

    // Material: Delete when smaller 
    // Water: Transforming into material color as it rises
    private void MixerAction()
    {
        if (_ingredientTransform.Count != 0)
        {
            foreach (Transform targetTranform in _ingredientTransform.Values)
            {
                targetTranform.localScale -= _ingredientScaleOffset * Time.deltaTime * CookingSpeed;

                if (targetTranform.localScale.y < 0.01f)
                {
                    // Issue: Collection Modified in Foreach Enurmarator
                    _ingredientTransform.Remove(targetTranform.GetInstanceID());
                    Destroy(targetTranform.gameObject);
                }

                _juiceMeshRenderer.material.color = Color.Lerp(_juiceMeshRenderer.material.color, 
                                                       targetTranform.GetComponent<MeshRenderer>().material.color, 
                                                       Time.deltaTime * CookingSpeed);

                if(Juice.transform.localScale.y < 3f)
                {
                    Juice.transform.localScale += _juiceScaleOffset * Time.deltaTime * CookingSpeed;
                }
                else
                {
                    // Add: Juice Full Event Handler
                } 
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Ingredient")
        {
            _ingredientTransform[other.transform.GetInstanceID()] = other.transform;
        }
    }
}
