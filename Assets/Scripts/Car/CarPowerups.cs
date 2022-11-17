using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CarPowerups : CarComponent
{
    public enum Powerup
    {
        Boost,
        Rocket,
        Mine
    }

    protected override void Awake() 
    {
        base.Awake();
    }

    public Powerup powerup;

    public void PowerupCollected(object sender, EventArgs e)
    {
        
    }
}