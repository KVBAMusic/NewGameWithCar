using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class CarBoostHandler : AbstractCarComponent
{
    [SerializeField] private float boostDecrementRate;
    [SerializeField] private float minBoostAmount;
    [SerializeField] private float maxBoostTime;
    [SerializeField] private float maxBoostAmount;
    [SerializeField] private float minFallingTimeAfterJump;
    [SerializeField] private float jumpingForce;
    [SerializeField] private float speedMultiplier;
    private NetworkVariable<float> boost = new NetworkVariable<float>(0);
    public NetworkVariable<BoostState> boostState = new NetworkVariable<BoostState>(BoostState.Idle);
    public float BoostAmount => boost.Value;
    public float SpeedMultiplier => speedMultiplier;

    public bool IsActivelyBoosting => boost.Value > boostDecrementRate;
    public bool HasBoost => boost.Value > 0;
    public bool IsDrifting => boostState.Value == BoostState.Drift;
    // temp, will remove after importing new input system
    KeyCode drift1 = KeyCode.LeftArrow;
    KeyCode drift2 = KeyCode.RightArrow;

    KeyCode driftPrimary;
    KeyCode driftSecondary;
    uint boostCount;
    float lastBoostTime;
    public float driftDirection {get; private set;}

    public void AddBoost(float boostAmount)
    {
        boost.Value = BoostAmount + boostAmount;
    }

    private void Update()
    {
        if (HasBoost)  boost.Value = BoostAmount - boostDecrementRate * Time.deltaTime;
        switch (boostState.Value)
        {
            default:
                break;
            case BoostState.Idle:
            driftDirection = 0;
                if (!car.Movement.IsOnGround) return;
                if (Input.GetKeyDown(drift1) || Input.GetKeyDown(drift2))
                {
                    boostState.Value = BoostState.Jump;
                    // jump
                    car.RB.AddForce(Vector3.up * jumpingForce * car.RB.mass);
                }
                break;
            case BoostState.Jump:
                if (!car.Movement.IsOnGround) return;
                if (!(Input.GetKey(drift1) || Input.GetKey(drift2)))
                {
                    boostState.Value = BoostState.Idle;
                }
                else
                {
                    if (Input.GetAxisRaw("Horizontal") == 0) boostState.Value = BoostState.Idle;
                    else
                    {
                        driftDirection = Input.GetAxisRaw("Horizontal");
                        boostState.Value = BoostState.Drift;
                        BeginDrift(0);
                        if (Input.GetKey(drift1))
                        {
                            driftPrimary = drift1;
                            driftSecondary = drift2;
                        }
                        else if (Input.GetKey(drift2))
                        {
                            driftPrimary = drift2;
                            driftSecondary = drift1;
                        }
                    } 
                }
                break;
            case BoostState.Drift:
                if (!(Input.GetKey(driftPrimary))) boostState.Value = BoostState.Idle;
                else
                {
                    if (Time.time - lastBoostTime > maxBoostTime) boostCount = 3;
                    if (Input.GetKeyDown(driftSecondary) && boostCount < 3u)
                    {
                        BeginDrift(boostCount + 1);
                        AddBoost(Mathf.Lerp(minBoostAmount, maxBoostAmount, (Time.time - lastBoostTime) / maxBoostTime));
                    }
                }
                break;
        }
    }

    private void BeginDrift(uint driftNumber)
    {
        boostCount = driftNumber;
        lastBoostTime = Time.time;
    }


}