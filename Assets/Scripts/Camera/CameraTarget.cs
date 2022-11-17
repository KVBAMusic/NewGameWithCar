using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform target;

    CarBrain car;

    private void Start() {
        target.TryGetComponent<CarBrain>(out car);
    }

    // Update is called once per frame
    void Update()
    {
        var pos = target.position;
        var rot = target.rotation;
        if (car.Movement.isReversing)
        {
            rot *= Quaternion.Euler(0, 180, 0);
        }
        transform.SetPositionAndRotation(pos, rot);
    }
}
