using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class CarBoostHandler : AbstractCarComponent
{
    [SerializeField] private float boostDecrementRate;
    [SerializeField] private float minBoostTime;
    [SerializeField] private float minBoostAmount;
    [SerializeField] private float maxBoostTime;
    [SerializeField] private float maxBoostAmount;
    private NetworkVariable<float> boost = new NetworkVariable<float>(0);
    public NetworkVariable<BoostState> boostState = new NetworkVariable<BoostState>(BoostState.Idle);
    public float BoostAmount => boost.Value;

    public bool IsActivelyBoosting => boost.Value > boostDecrementRate;
    public bool HasBoost => boost.Value > 0;
    public bool IsDrifting => boostState.Value == BoostState.Drift;
    // temp, will remove after importinh new input system
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
                // check if on the ground
                if (Input.GetKeyDown(drift1) || Input.GetKeyDown(drift2))
                {
                    boostState.Value = BoostState.Jump;
                    // jump
                }
                break;
            case BoostState.Jump:
                // check if touched the ground
                if (!(Input.GetKey(drift1) || Input.GetKey(drift2)))
                {
                    boostState.Value = BoostState.Idle;
                }
                else
                {
                    if (Input.GetAxisRaw("Howizontal") == 0) boostState.Value = BoostState.Idle;
                    else
                    {
                        driftDirection = Input.GetAxisRaw("Horizontal");
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
                        // begin drift here
                    } 
                }
                break;
            case BoostState.Drift:
                if (!(Input.GetKey(driftPrimary))) boostState.Value = BoostState.Idle;
                else
                {

                }
                break;
        }
    }
}