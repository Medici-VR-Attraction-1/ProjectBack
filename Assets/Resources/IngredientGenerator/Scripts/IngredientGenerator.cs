using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientGenerator : MonoBehaviour
{
    private static IngredientGenerator _instance = null;
    public static IngredientGenerator GetInstance() { return _instance; }

    [SerializeField]
    private List<GameObject> IngredientPrefabList;

    private GameObject[] _ingredientPrefabListCache;

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            this.enabled = false;
        }
        else
        {
            // Check and Set Singleton Object
            if (_instance != null)
            {
                Debug.Log("IngredientGenerator : IngredientGenerator has Duplicated, Delete Another One");
                gameObject.SetActive(false);
            }

            _instance = this;
            _ingredientPrefabListCache = IngredientPrefabList.ToArray();
        }
    }

    // Importing a list of materials, selecting them randomly return GameObject
    // Once the written ones are deleted and list is Empty, 
    // import them from the temporary list and select them.
    public GameObject GetRandomIngredient()
    {
        if (IngredientPrefabList.Count == 0)
        {
            IngredientPrefabList = _ingredientPrefabListCache.ToList<GameObject>();
        }

        int randomNumber = Random.Range(0, IngredientPrefabList.Count);

        GameObject randomIngredient = IngredientPrefabList[randomNumber];
        IngredientPrefabList.RemoveAt(randomNumber);
        
        return randomIngredient;
    }
}
