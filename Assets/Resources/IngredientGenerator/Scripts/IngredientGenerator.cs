using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientGenerator : MonoBehaviour
{
    private static IngredientGenerator _instance = null;
    public static IngredientGenerator GetInstance() { return _instance; }

    [SerializeField]
    private List<GameObject> IngredientList;

    private GameObject[] _ingredientListCache;

    private void Awake()
    {
        // Check and Set Singleton Object
        if (_instance != null)
        {
            Debug.Log("IngredientGenerator : IngredientGenerator has Duplicated, Delete Another One");
            gameObject.SetActive(false);
        }

        _instance = this;
        _ingredientListCache = IngredientList.ToArray();
    }
    //재료 리스트를 가져와서 랜덤으로 선택후 한번 쓰인것은 삭제되고 모두 삭제되면 임시 리스트에서 가져와서 선택
    public GameObject GetRandomIngredient()
    {
        if (IngredientList.Count == 0)
        {
            IngredientList = _ingredientListCache.ToList<GameObject>();
        }

        int randomNumber = Random.Range(0, IngredientList.Count);
        GameObject randomIngredient = IngredientList[randomNumber];

        IngredientList.RemoveAt(randomNumber);

        return randomIngredient;
    }


}
