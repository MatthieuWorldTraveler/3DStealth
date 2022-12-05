using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionManager : MonoBehaviour
{
    [SerializeField] float _detectionTime;
    [SerializeField] float _raycastDistance;
    [SerializeField] LayerMask _ignoreLayers;
    GameObject _otherObj;
    bool _seen;
    float _detectionEndTimer;

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            _otherObj = collision.gameObject;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _seen = false;
            _otherObj = null;
        }
    }

    private void Update()
    {
        if (_otherObj != null)
            Detection();

        if(_seen && Time.time > _detectionEndTimer)
        {
            Debug.Log("Seen");
        }
    }

    private void Detection()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.parent.position + Vector3.up, _otherObj.transform.position - transform.parent.position);
        Debug.DrawRay(transform.parent.position + Vector3.up, _otherObj.transform.position - transform.parent.position, Color.green);
        Physics.Raycast(ray, out hit, _raycastDistance, ~_ignoreLayers);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            if (!_seen)
            {
                _detectionEndTimer = Time.time + _detectionTime;
                _seen = true; 
            }
        }
        else
            _seen = false;
    }
}
