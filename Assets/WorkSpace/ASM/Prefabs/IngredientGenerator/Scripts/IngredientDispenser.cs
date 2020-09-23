using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientDispenser : MonoBehaviour
{
    private GameObject _ingredient = null;

    [SerializeField]
    private GameObject dispenserObject;


    private void Start()
    {
        _ingredient = IngredientGenerator.GetInstance().GetRandomIngredient();

        //Instantiate(_ingredient, transform.position, transform.rotation);

        StartCoroutine("GenerateTypeOne");
        //  GenerateTypeTwo();
    }

    IEnumerator GenerateTypeOne()
    {
        //_ingredient.AddComponent<SphereCollider>();
        //_ingredient.AddComponent<Rigidbody>();
        //무더기생성
        // _ingredient = IngredientGenerator.GetInstance().GetRandomIngredient();
        int i = 0;
        while (i < 10)
        {
            Instantiate(_ingredient, transform.position, transform.rotation);
            yield return new WaitForSeconds(0.2f);
            i++;
        }
    }
    public void GenerateTypeTwo()
    {
        //한개씩 생성
        // dispenserObject = GameObject.FindWithTag("Dispenser");
        Collider[] col = Physics.OverlapSphere(dispenserObject.transform.position, 1f);
        if (col != null && col[0].gameObject.name.Contains("hand"))
        {
            print("col");
            Instantiate(_ingredient, col[0].transform.position, col[0].transform.rotation);
            _ingredient.AddComponent<BoxCollider>();
            return;
        }
    }
}