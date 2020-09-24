using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerHandAction : MonoBehaviour
{
    private delegate void ActionHandler(GameObject target);
    private ActionHandler PlayerHandActionBinder = null;

    [SerializeField]
    private GameObject HandJoint = null;

    [SerializeField]
    private int HandAnimationSpeed = 10;

    private bool _isHandUsing = false;
    private WaitForSeconds _grabTargetRate = new WaitForSeconds(0.01f);
    private Rigidbody _rigidbody = null;

    public void ActiveHandAction(GameObject target)
    {
        PlayerHandActionBinder(target);
    }

    public bool CheckHandUsing() { return _isHandUsing; }

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if(XRDevice.isPresent)
        {

        }
        else
        {
            PlayerHandActionBinder += new ActionHandler(KMPlayerHandAction);
        }
    }

    private void KMPlayerHandAction(GameObject target)
    {
        _isHandUsing = true;
        StartCoroutine(_GrabTarget(target));
    }

    private IEnumerator _GrabTarget(GameObject target)
    {
        float animationOffset = 1f / (float)HandAnimationSpeed;
        Vector3 distance;
        Vector3 originalTargetPosition = target.transform.position;

        for(int i = 0; i < HandAnimationSpeed; i++)
        {
            Debug.Log(gameObject.name);
            distance = originalTargetPosition - transform.position;
            transform.position += Vector3.Lerp(Vector3.zero, distance, animationOffset) * Mathf.Sin(i * animationOffset * 6f);

            if (i == HandAnimationSpeed / 2)
            {
                target.GetComponent<Rigidbody>().isKinematic = true;
                target.transform.SetParent(transform);
                target.transform.position = transform.position;
                target.transform.rotation = transform.rotation;
            }
            yield return _grabTargetRate;
        }
        transform.position = HandJoint.transform.position;
        _isHandUsing = false;
        yield return null;
    }
}
