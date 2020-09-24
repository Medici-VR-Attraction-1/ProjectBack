using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoockingToolsEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        
        if (other.transform.tag == "Ingredient")
        {
          
            float h = 0f, s = 0f, v = 1f;
            Color ingredientOriginColor= other.gameObject.GetComponent<MeshRenderer>().material.color;
        //    ingredientOriginColor = Color.RGBToHSV(ingredientOriginColor, out h, out s, out v); 

            other.gameObject.GetComponent<MeshRenderer>().material.color = Color.Lerp(ingredientOriginColor, new Color(0.566f,0.355f,0.519f),1);

        }
    }

}
