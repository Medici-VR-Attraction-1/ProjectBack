﻿using System.Collections;
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
        //무더기생성

        //_ingredient = IngredientGenerator.GetInstance().GetRandomIngredient();
        //for (int i = 0; i < 64; i++)
        //{
        //    positionCache = transform.position;
        //    boundary = 3f - Random.Range(0f, 6f);
        //    positionCache.x += boundary;
        //    boundary = 3f - Random.Range(0f, 6f);
        //    positionCache.z += boundary;
        //    boundary = 3f - Random.Range(0f, 6f);
        //    positionCache.y += boundary;
        //    Instantiate(_ingredient, positionCache, transform.rotation);
        //}

        Vector3 positionCache;
        positionCache = transform.position;
        float boundary;

        for (int i = 0; i < 4; i++)
        {
            positionCache.x = transform.position.x;
            positionCache.z = transform.position.z;
            positionCache.y += 1.5f + Random.Range(-0.5f, 0.5f);
            Instantiate(_ingredient, positionCache, transform.rotation);
            for (int j = 0; j < 3; j++)
            {
                positionCache.x = transform.position.x;
                positionCache.z += 1.5f + Random.Range(-0.5f, 0.5f);
                Instantiate(_ingredient, positionCache, transform.rotation);
                for (int k = 0; k < 3; k++)
                {
                    positionCache.x += 1.5f + Random.Range(-0.5f, 0.5f);
                    Instantiate(_ingredient, positionCache, transform.rotation);
                }
            }

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