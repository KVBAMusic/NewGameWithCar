using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats Object", menuName = "Car Stats Object")]
public class CarStatObject : ScriptableObject
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float steerAngle;

    public CarStats stats{
        get
        {
            return new CarStats(maxSpeed, acceleration, steerAngle);
        }
    }
}
