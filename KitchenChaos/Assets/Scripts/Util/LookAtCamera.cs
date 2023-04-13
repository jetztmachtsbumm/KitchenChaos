using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    [SerializeField] private bool invert;

    private void LateUpdate()
    {
        if (invert)
        {
            Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
            transform.LookAt(transform.position + dirFromCamera);
        }
        else
        {
            transform.LookAt(Camera.main.transform);
        }
    }

}
