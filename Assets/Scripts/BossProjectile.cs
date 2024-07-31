using UnityEngine;
using System.Collections;

public class BossProjectile : MonoBehaviour
{
    public float speed = 25f;
    public float damage = 5f;
    private Vector3 targetPosition;
    private Vector3 direction;
    private bool stop;

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    private void Update()
    {
        if (targetPosition != Vector3.zero)
        {

            if (!stop)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    stop = true;
                    StartCoroutine(LowTierGod(3f));
                }
            }

        }
    }

    private IEnumerator LowTierGod(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponentInParent<PlayerController>())
            {
                other.GetComponentInParent<PlayerController>().TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            if (other.GetComponent<AcceptanceBoss>())
            {
                // heal boss for now but increase range later? idk
                other.GetComponent<AcceptanceBoss>().health += damage * other.GetComponent<AcceptanceBoss>().phase;
            }
            Destroy(gameObject);
        }
    }
}
