using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Gravity : MonoBehaviour
{
    public float GRAVITY_PULL = .78f;
    public float m_GravityRadius = 1f;

    void Awake()
    {
        m_GravityRadius = GetComponent<SphereCollider>().radius;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            float gravityIntensity = m_GravityRadius / Vector3.Distance(transform.position, other.transform.position);

            other.GetComponent<Rigidbody>().AddForce((transform.position - other.transform.position) * gravityIntensity * other.GetComponent<Rigidbody>().mass * GRAVITY_PULL * Time.deltaTime);
        }
    }
}
