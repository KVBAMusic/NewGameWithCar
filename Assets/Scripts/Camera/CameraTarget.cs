using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    CarBrain target;

    private void Start() {
        CarBrain target = null;
        try
        {
            target = FindObjectOfType<CarBrain>();
        }
        catch
        {
            target = null;
        }
        finally
        {
            if (target != null)
            {
                if (target.IsOwner)
                {
                    this.target = target;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target is null) return;
        var pos = target.transform.position;
        var rot = target.transform.rotation;
        if (target.Movement.isReversing)
        {
            rot *= Quaternion.Euler(0, 180, 0);
        }
        transform.SetPositionAndRotation(pos, rot);
    }

    public void SetTarget(CarBrain target)
    {
        this.target = target;
    }
}
