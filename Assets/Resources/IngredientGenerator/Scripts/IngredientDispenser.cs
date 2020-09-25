using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientDispenser : MonoBehaviour
{
    private GameObject _ingredientPrefab = null;

    [SerializeField]
    private bool IsInstantiateAll = true;

    private void Start()
    {
        _ingredientPrefab = IngredientGenerator.GetInstance().GetRandomIngredient();

        if (IsInstantiateAll)
            GenerateIngredientsAll();
        else
            GenerateIngredientAsync();
    }

    //4*4*4 형태로 재료 무더기 생성
    private void GenerateIngredientsAll()
    {
        Vector3 positionCache;
        positionCache = transform.position;

        for (int i = 0; i < 4; i++)
        {
            positionCache.x = transform.position.x;
            positionCache.z = transform.position.z;
            positionCache.y += 1.5f;
            for (int j = 0; j < 4; j++)
            {
                positionCache.x = transform.position.x;
                positionCache.z += 1.5f;
                for (int k = 0; k < 4; k++)
                {
                    positionCache.x += 1.5f;
                    Instantiate(_ingredientPrefab, 
                             positionCache * Random.Range(1.0f, 1.02f), 
                             transform.rotation);
                }
            }
        }
    }

    public void GenerateIngredientAsync()
    {

    }
}