using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientDispenser : MonoBehaviour
{
    private GameObject _ingredient = null;

    private void Start()
    {
        _ingredient = IngredientGenerator.GetInstance().GetRandomIngredient();

        Instantiate(_ingredient, transform.position, transform.rotation);
    }
}
