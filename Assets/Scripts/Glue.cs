using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glue : MonoBehaviour
{
    public LayerMask attachLayers;
    public float maxDistance = 5f;
    public float attachmentRadius = 1f;
    public bool isAttached = false;
    public Transform attachTarget;

    private Vector3 offset;

    private void Update()
    {
        if (!isAttached)
        {
            TryAttach();
        } else
        {
            if (attachTarget != null)
                transform.position = attachTarget.position - offset;
        }
    }

    void TryAttach()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attachmentRadius, attachLayers);

        foreach (Collider collider in colliders)
        {
            Vector3 direction = collider.transform.position - transform.position;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, direction.normalized, out hit, maxDistance, attachLayers))
            {
                attachTarget = hit.transform;
                isAttached = true;
                offset = attachTarget.position - transform.position;
                break;
            }
        }
    }
}
