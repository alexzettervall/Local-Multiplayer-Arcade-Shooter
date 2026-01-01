using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public bool destroyIfNoTarget = false;

    public void Update()
    {
        if (target == null)
        {
            if (destroyIfNoTarget)
            {
                Destroy(gameObject);
            }
            return;
        }
        
        transform.position = target.position;
    }
}
