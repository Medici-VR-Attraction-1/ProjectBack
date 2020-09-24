using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerHandAction : MonoBehaviour
{
    [SerializeField]
    private GameObject HandJoint = null;

    [SerializeField]
    private int HandAnimationSpeed = 10;

    private bool _isHandUsing = false;
    private GameObject _grabTarget = null;
    private WaitForSeconds _grabTargetRate = new WaitForSeconds(0.01f);

    public bool CheckHandUsing() { return _isHandUsing; }

    public void KMPlayerGrabHandAction(GameObject target)
    {
        _isHandUsing = true;
        StartCoroutine(_GrabTarget(target));
    }

    public void KMPlayerPutHandAction(Vector3 point)
    {
        _isHandUsing = false;
        StartCoroutine(_PutTarget(point));
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
        _isHandUsing = true;
        _grabTarget = target;
        yield return null;
    }

    private IEnumerator _PutTarget(Vector3 point)
    {
        float animationOffset = 1f / (float)HandAnimationSpeed;
        Vector3 distance;

        for (int i = 0; i < HandAnimationSpeed; i++)
        {
            Debug.Log(gameObject.name);
            distance = point - transform.position;
            transform.position += Vector3.Lerp(Vector3.zero, distance, animationOffset) * Mathf.Sin(i * animationOffset * 6f);

            if (i == HandAnimationSpeed / 2)
            {
                _grabTarget.transform.SetParent(null);
                _grabTarget.GetComponent<Rigidbody>().isKinematic = false;
            }
            yield return _grabTargetRate;
        }

        transform.position = HandJoint.transform.position;
        _isHandUsing = false;
        _grabTarget = null;
        yield return null;
    }
}
