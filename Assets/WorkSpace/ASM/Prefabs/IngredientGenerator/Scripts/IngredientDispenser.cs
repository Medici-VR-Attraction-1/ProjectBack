using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientDispenser : MonoBehaviour
{
    private GameObject _ingredient = null;

    [SerializeField]
    private GameObject dispenserObject;

    private GameObject hand;




    private void Start()
    {
        _ingredient = IngredientGenerator.GetInstance().GetRandomIngredient();

        //Instantiate(_ingredient, transform.position, transform.rotation);

        // StartCoroutine("GenerateTypeOne");
        GenerateTypeTwo();
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