using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsRot : MonoBehaviour
{
    void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, 0, transform.eulerAngles.z);
    }
}
