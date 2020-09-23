using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientDispenser : MonoBehaviour
{
    private GameObject _ingredient = null;

    [SerializeField]
    private GameObject dispenserObject;

    [SerializeField]
    private bool IsInstantiateAll = true;

    private GameObject hand;




    private void Start()
    {
        _ingredient = IngredientGenerator.GetInstance().GetRandomIngredient();

        //Instantiate(_ingredient, transform.position, transform.rotation);

        if (IsInstantiateAll)
            GenerateTypeOne();
        else
            GenerateTypeTwo();
    }

    private void GenerateTypeOne()
    {
        //_ingredient.AddComponent<SphereCollider>();
        //_ingredient.AddComponent<Rigidbody>();
        //무더기생성
        // _ingredient = IngredientGenerator.GetInstance().GetRandomIngredient();
        
        float boundary;
        Vector3 positionCache;

        for (int i = 0; i < 64; i++) 
        {
            positionCache = transform.position;
            boundary = 3f - Random.Range(0f, 6f);
            positionCache.x += boundary;
            boundary = 3f - Random.Range(0f, 6f);
            positionCache.z += boundary;
            boundary = 3f - Random.Range(0f, 6f);
            positionCache.y += boundary;

            Instantiate(_ingredient, positionCache, transform.rotation);
        }
    }

    public void GenerateTypeTwo()
    {
        //raycast 로 한개 생성
        // dispenserObject = GameObject.FindWithTag("Dispenser");

        Collider[] col = Physics.OverlapSphere(dispenserObject.transform.position, 5f);
        if (col != null)
        {
            print(col.Length);
            for (int i = 0; i < col.Length; i++)
            {
                if (col[i].gameObject.name.Contains("hand"))
                {
                    print("hand collider chdeck");
                    // Instantiate(_ingredient, col[i].transform.position, col[i].transform.rotation);
                    Instantiate(_ingredient, transform.position, transform.rotation);
                    _ingredient.GetComponent<Rigidbody>().useGravity = enabled;
                    return;
                }
            }
            // _ingredient.AddComponent<BoxCollider>();
            return;
        }
    }
}