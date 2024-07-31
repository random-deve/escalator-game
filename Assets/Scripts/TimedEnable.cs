using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEnable : MonoBehaviour
{
    [System.Serializable]
    public class ObjectToEnable
    {
        public GameObject theObject;
        public float time;
        public bool enable;
    }

    public List<ObjectToEnable> objects = new List<ObjectToEnable>();

    private void Awake()
    {
        objects.Sort((a, b) => a.time.CompareTo(b.time));
        StartCoroutine(EnableObjectsAtTime());
    }

    private IEnumerator EnableObjectsAtTime()
    {
        foreach (ObjectToEnable _object in objects)
        {
            yield return new WaitForSeconds(_object.time);
            _object.theObject.SetActive(_object.enable);
        }
    }
}
