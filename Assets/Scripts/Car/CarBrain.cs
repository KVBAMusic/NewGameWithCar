using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using PathCreation;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(CarAIController))]
[RequireComponent(typeof(CarPositionTracker))]
public class CarBrain : NetworkBehaviour
{
    public event EventHandler OnLapStarted;
    public event EventHandler OnPowerupCollected;
    public event EventHandler OnInit;
    public Guid guid {get; private set;}

    [SerializeField] private Rigidbody rb;
    [SerializeField] private CarMovement movement;
    [SerializeField] private CarAIController aIController;
    [SerializeField] private CarPositionTracker position;
    [SerializeField] private NetworkObject networkObject;
    private NetworkManager networkManager;
    private ulong clientId;

    public Rigidbody RB => rb;
    public CarMovement Movement => movement;
    public CarAIController AIController => aIController;
    public CarPositionTracker Position => position;
    public bool isAI = true;


    public VertexPath path {get; private set;}

    // przykładowy NetworkVariable:
    //
    // NetworkVariable<T> netVar = new NetowrkVariable<T>();
    // NetworkVariable<T> netVar = new NetowrkVariable<T>(wartość);
    // NetworkVariable<T> netVar = new NetowrkVariable<T>(wartość, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.[tu coś wstaw]);
    // ---------------------------------
    // T NIE MOŻE BYĆ NULLABLE!!!
    // jeśli chcesz dać klasę, zrób dedykowanego structa z tą klasą (musi implementować INetworkSerializable)
    // tam będzie parametr serializer, po prostu daj
    //
    // serializer.SerializeValue(ref [pole structa]);
    // DLA KAŻDEGO POLA
    // ---------------------------------
    // jeśli chcesz użyć stringa, najpierw daj na górze
    //
    // using Unity.Collections;
    //
    // a potem użyj któregoś structa FixedString, np FixedString128Bytes
    // NIE MOŻNA ZMIENIAĆ DŁUGOŚCI FixedString, GDYŻ NIE JEST ON REALOKOWANY!!!!
    // ---------------------------------
    // jak chcesz zmienić wartość NetworkVariable:
    //
    // netVar.Value = [nowa wartość];
    // ---------------------------------
    // jak chcesz, żeby coś się działo po zmianie wartości:
    //
    // netVar.OnValueChanged += (T previousValue, T newValue) => {// tu daj kod};

    // w przypadku, gdy musisz zrobić jakiś init z sieci daj to w 
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    private void Awake() 
    {
        networkManager = FindObjectOfType<NetworkManager>();
        clientId = networkManager.LocalClientId;
        Init(CarSettings.NewPlayer(new CarStats()));
    }

    private void OnEnable()
    {
        transform.position = new Vector3(135, 36, -14); //temporary
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
