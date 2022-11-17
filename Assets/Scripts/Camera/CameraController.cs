using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CameraTarget target;

    [SerializeField] private float positionSmoothingAmount;
    [SerializeField] private float rotationmoothingAmount;
    public bool noRoll = true;

    // Update is called once per frame
    void Update()
    {
        var targetPos = target.transform.position;
        var targetRot = target.transform.rotation;

        if (noRoll)
        {
            var eul = targetRot.eulerAngles;
            targetRot.eulerAngles = new Vector3(eul.x, eul.y, 0);
        }

        transform.position = targetPos;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationmoothingAmount);
    }
}
