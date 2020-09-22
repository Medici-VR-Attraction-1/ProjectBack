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

        _instance = this;
    }

    public GameObject GetRandomIngredient()
    {
        int random = Random.Range(0, IngredientList.Count);
        GameObject _randomIngredient = IngredientList[random];
        IngredientList.RemoveAt(random);
        return _randomIngredient;
    }
}
