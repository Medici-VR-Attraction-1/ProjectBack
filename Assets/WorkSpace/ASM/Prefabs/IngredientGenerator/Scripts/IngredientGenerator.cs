using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientGenerator : MonoBehaviour
{
    private static IngredientGenerator _instance = null;
    public static IngredientGenerator GetInstance() { return _instance; }

    [SerializeField]
    private List<GameObject> IngredientList;

    private void Awake()
    {
        // Check and Set Singleton Object
        if (_instance != null)
        {
            Debug.Log("IngredientGenerator : IngredientGenerator has Duplicated, Delete Another One");
            gameObject.SetActive(false);
        }
    }

    public GameObject GetRandomIngredient() 
    { 
        return null; 
    }
}
