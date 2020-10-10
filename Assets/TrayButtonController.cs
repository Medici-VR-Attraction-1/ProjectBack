using UnityEngine;

public class TrayButtonController : MonoBehaviour
{
    [SerializeField]
    private BurgerTrayController burgetTrayController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Hand")
        {
            burgetTrayController.CleanUpTray();
        }
    }
}
