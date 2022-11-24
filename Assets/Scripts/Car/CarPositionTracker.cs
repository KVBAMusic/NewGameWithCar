using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPositionTracker : AbstractCarComponent
{
    public int Position;

    public int lap {get; private set;}
    public int nextPointOnPath {get; private set;}
    public float distanceToPoint {get; private set;}

    protected override void LapStarted(object sender, EventArgs e)
    {
        lap++;
        nextPointOnPath = 0;
    }

    protected override void Awake()
    {
        base.Awake();
        lap = 1;
        nextPointOnPath = 0;
    }

    void Update()
    {
        var nextPoint = car.path.GetPoint(nextPointOnPath);
        distanceToPoint = Vector3.Distance(nextPoint, transform.position);
        if (distanceToPoint < 30)
        {
            nextPointOnPath = Math.Clamp(nextPointOnPath + 1, 0, car.path.NumPoints - 1);
        }
    }
}