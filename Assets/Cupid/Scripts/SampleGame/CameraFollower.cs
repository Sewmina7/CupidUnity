using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField]private Vector3 offset;
    [SerializeField]private bool autoOffset = false;
    [SerializeField]private Transform target;
    [SerializeField]private float smoothness = 0.1f;
    
    void LateUpdate()
    {
        if(target==null){return;}
        transform.position = Vector3.Lerp(transform.position, target.position + offset,smoothness);
    }

    public void SetTarget(Transform _target){
        target = _target;
        offset = transform.position - target.position;
    }
}
