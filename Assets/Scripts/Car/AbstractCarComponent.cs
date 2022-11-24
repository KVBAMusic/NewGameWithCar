using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class AbstractCarComponent : NetworkBehaviour
{
    protected CarBrain car;

    protected virtual void Awake() {
        car = GetComponent<CarBrain>();
        car.OnLapStarted += LapStarted;
    }

    protected virtual void LapStarted(object sender, EventArgs e) {}
    protected virtual void Init(object sender, EventArgs e) {}
    protected virtual void RaceEnd(object sender, EventArgs e) {}
    protected virtual void FinalLapStarted(object sender, EventArgs e) {}
}
