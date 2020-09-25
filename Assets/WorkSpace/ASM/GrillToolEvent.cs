using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class GrillToolEvent : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1f)]
    private float CookingSpeed = 0.1f;

    private Dictionary<int, MeshRenderer> _ingredientMeshRenderers = new Dictionary<int, MeshRenderer>();
    private readonly Color targetColor = Color.black;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GrillAction();
    }

    //충돌한 오브젝트를 검은색으로 변형
    private void GrillAction()
    {
        if (_ingredientMeshRenderers.Count > 0)
        {
            Color colorCache;
            foreach (MeshRenderer meshRenderer in _ingredientMeshRenderers.Values)
            {
                colorCache = meshRenderer.material.color;
                meshRenderer.material.color = Color.Lerp(colorCache, targetColor, 
                                                   Time.deltaTime * CookingSpeed);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ingredient")
        {
            MeshRenderer ingredientMeshRenderer = other.gameObject.GetComponent<MeshRenderer>();
            _ingredientMeshRenderers[ingredientMeshRenderer.GetInstanceID()] = ingredientMeshRenderer;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        MeshRenderer ingredientMeshRenderer = collision.gameObject.GetComponent<MeshRenderer>();
        _ingredientMeshRenderers.Remove(ingredientMeshRenderer.GetInstanceID());
    }
}
