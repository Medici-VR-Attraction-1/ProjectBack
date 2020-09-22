using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientGenerator : MonoBehaviour
{
    private static IngredientGenerator _instance = null;
    public static IngredientGenerator GetInstance() { return _instance; }

    [SerializeField]
    private List<GameObject> IngredientList;

    [SerializeField]
    private List<GameObject> IngredientList2;

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
        bool isSame;
        //  int random = Random.Range(0, IngredientList.Count);
        //IngredientList.RemoveAt(random);
        //if (IngredientList.Count == 0)
        //{
        //    int random2 = Random.Range(0, IngredientList2.Count);
        //    _randomIngredient = IngredientList2[random2];

        //    print(IngredientList2[random2]);
        //}
        int[] random = new int[IngredientList.Count];
      
        GameObject _randomIngredient = IngredientList[random];
        for (int i = 0; i < IngredientList.Count; i++)
        {
            while (true)
            {
                isSame = false;
                for (int j = 0; j < i; j++)
                {
                    if (random[j] == random[i])
                    {
                        isSame = true;
                        break;
                    }
                }
                if (!isSame) break;
            }
        }
        return _randomIngredient;
    }
}
