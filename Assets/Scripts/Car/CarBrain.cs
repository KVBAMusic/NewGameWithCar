using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(CarAIController))]
[RequireComponent(typeof(CarPositionTracker))]
public class CarBrain : MonoBehaviour
{
    public event EventHandler OnLapStarted;
    public event EventHandler OnPowerupCollected;
    public event EventHandler OnInit;
    public Guid guid {get; private set;}

    [SerializeField] private Rigidbody rb;
    [SerializeField] private CarMovement movement;
    [SerializeField] private CarAIController aIController;
    [SerializeField] private CarPositionTracker position;

    public Rigidbody RB => rb;
    public CarMovement Movement => movement;
    public CarAIController AIController => aIController;
    public CarPositionTracker Position => position;
    public bool isAI = true;

    public VertexPath path {get; private set;}

    private void Awake() 
    {
        Init(CarSettings.NewPlayer(new CarStats()));
    }

    private void Init(CarSettings settings)
    {
        guid = new Guid();
        rb.centerOfMass = new Vector3(0, -1, 0);
        path = GameObject.FindGameObjectWithTag("first path").GetComponent<PathCreator>().path;

        isAI = settings.isAI;

        OnInit?.Invoke(this, EventArgs.Empty);
    }

    private void OnTriggerEnter(Collider other) 
    {
        switch(other.gameObject.tag)
        {
            case "start line":
                if (position.nextPointOnPath >= path.NumPoints * .9f) OnLapStarted?.Invoke(this, EventArgs.Empty);
                break;
            case "powerup":
                OnPowerupCollected?.Invoke(this, EventArgs.Empty);
                break;
        }
    }
}

public struct CarSettings
{
    public readonly bool isAI;
    public readonly CarStats stats;

    private CarSettings(bool isAI, CarStats stats)
    {
        this.isAI = isAI;
        this.stats = stats;
    }

    public static CarSettings NewPlayer(CarStats stats)
    {
        return new CarSettings(false, stats);
    }

    public static CarSettings NewBot(CarStats stats)
    {
        return new CarSettings(true, stats);
    }
}
