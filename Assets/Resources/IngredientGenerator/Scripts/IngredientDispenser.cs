using Photon.Pun;
using UnityEngine;

public class IngredientDispenser : MonoBehaviour
{
    private GameObject _ingredientPrefab = null;

    [SerializeField]
    private bool IsInstantiateAll = true;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _ingredientPrefab = IngredientGenerator.GetInstance().GetRandomIngredient();

            if (IsInstantiateAll)
                GenerateIngredientsAll();
            else
                GenerateIngredientAsync();
        }
    }

    private void GenerateIngredientsAll()
    {
        Vector3 positionCache;
        positionCache = transform.position;

        for (int i = 0; i < 4; i++)
        {
            positionCache.x = transform.position.x;
            positionCache.z = transform.position.z;
            positionCache.y += 0.2f;
            for (int j = 0; j < 4; j++)
            {
                positionCache.x = transform.position.x;
                positionCache.z += 0.2f;
                for (int k = 0; k < 4; k++)
                {
                    positionCache.x += 0.2f;
                    PhotonNetwork.InstantiateRoomObject(_ingredientPrefab.name, 
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