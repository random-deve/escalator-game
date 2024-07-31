using UnityEngine;

public class EnableOnNoChildren : MonoBehaviour
{
    [Header("Parent and Target Objects")]
    public GameObject parentObject;
    public GameObject targetObject;
    public GameObject particles;

    [Header("Settings")]
    public bool disableTargetInitially = true;
    public bool enable = true;

    private bool particlesCreated;

    private void Start()
    {
        if (disableTargetInitially && targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (parentObject != null && parentObject.transform.childCount == 0 && parentObject.activeInHierarchy)
        {
            if (targetObject != null)
            {
                targetObject.SetActive(enable);
                if (particles != null && !particlesCreated)
                    Instantiate(particles);
                    particlesCreated = true;
            }
        }
    }
}
