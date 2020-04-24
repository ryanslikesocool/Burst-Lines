using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotation = Vector3.zero;
    private float multiplier = 0;

    private Quaternion Rotation { get { return Quaternion.Euler(rotation * multiplier * Time.deltaTime); } }

    private void Update()
    {
        transform.rotation *= Rotation;

        multiplier += Time.deltaTime;
    }
}