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
    //      NetworkVariable<T> netVar = new NetowrkVariable<T>();
    //      NetworkVariable<T> netVar = new NetowrkVariable<T>(wartość);
    //      NetworkVariable<T> netVar = new NetowrkVariable<T>(wartość, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.[tu coś wstaw]);
    //
    // T NIE MOŻE BYĆ NULLABLE!!!
    // ---------------------------------
    // jeśli chcesz dać klasę, zrób dedykowanego structa z tą klasą (musi implementować INetworkSerializable)
    // tam będzie parametr serializer, po prostu daj
    //
    //      serializer.SerializeValue(ref [pole structa]);
    //
    // DLA KAŻDEGO POLA
    // ---------------------------------
    // jeśli chcesz użyć stringa, najpierw daj na górze
    //
    //      using Unity.Collections;
    //
    // a potem użyj któregoś structa FixedString, np FixedString128Bytes
    // NIE MOŻNA ZMIENIAĆ DŁUGOŚCI FixedString, GDYŻ NIE JEST ON REALOKOWANY!!!!
    // ---------------------------------
    // jak chcesz zmienić wartość NetworkVariable:
    //
    //      netVar.Value = [nowa wartość];
    // ---------------------------------
    // jak chcesz, żeby coś się działo po zmianie wartości:
    //
    //      netVar.OnValueChanged += (T previousValue, T newValue) => {// tu daj kod};


    // RPC - Alternatywny sposób komunikacji klienta z serwerem
    // ServerRpc działa TYLKO NA SERWERZE!!!
    // KOD WYKONYWANY JAKO ServerRpc NIE JEST WYKONYWANY NA KLIENTACH!!!
    // ------------------------------
    // Wszystkie metody serwera muszą w nazwie zawrzeć na końcu
    //
    //      ServerRpc
    //
    // i być opatrzone atrybutem
    //
    //      [ServerRpc]
    //
    // PARAMETRY TAKICH METOD NIE MOGĄ BYĆ NULLABLE!!!
    // -------------------------------
    // do takich metod możemy dać parametr typu
    //      ServerRpcParams
    // np.
    //
    //      [ServerRpc]
    //      private void MethodServerRpc(ServerRpcParams serverRpcParams)
    //      {
    //          ...
    //      }
    //
    // ServerRpcParams posiada 2 structy:
    //
    //      public ServerRpcSendParams Send;
    //      public ServerRpcReceiveParams Receive;
    //
    // które można używać żeby sprecyzować, czy chcesz np. wysłać dane z serwera do klientów, lub
    // nasłuchiwać danych od klientów
    // ----------------------------------
    
    // ClientRpc działa TYLKO NA SERWERZE!!!
    // KOD WYKONYWANY JAKO ClientRpc JEST WYKONYWANY NA WSZYSTKICH KLIENTACH!!!
    // ------------------------------
    // Wszystkie metody serwera muszą w nazwie zawrzeć na końcu
    //
    //      ClientRpc
    //
    // i być opatrzone atrybutem
    //
    //      [ClientRpc]
    //
    // PARAMETRY TAKICH METOD NIE MOGĄ BYĆ NULLABLE!!!
    // -------------------------------
    // do takich metod możemy dać parametr typu
    //      ClientRpcParams
    // np.
    //
    //      [ClientRpc]
    //      private void MethodClientRpc(ClientRpcParams serverRpcParams)
    //      {
    //          ...
    //      }
    //
    // ServerRpcParams posiada 2 structy:
    //
    //      public ClientRpcSendParams Send;
    //      public ClientRpcReceiveParams Receive;
    //
    // które można używać żeby np. sprecyzować,
    // do których klientów chcesz wysłać dany ClientRpc
    // -----------------------------------------
    // ten kod wyśle słowo "test" do klienta o ID 1
    //      [ClientRpc]
    //      private void TestClientRpc(ClientRpcParams clientRpcParams)
    //      {
    //          Debug.Log("test");
    //      }
    //
    //      private void Update()
    //      {
    //          if (Input.GetKeyDown(KeyCode.K))
    //          {
    //              TestClientRpc(new ClientRpcParams{ Send = new ClientRpcSendParams {TargetClientIds = new List<uint>{1} } } );
    //          }
    //      }
    //
    // ----------------------------------


    // w przypadku, gdy musisz zrobić jakiś init z sieci daj to w 
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        FindObjectOfType<CameraTarget>().SetTarget(this);
    }

    private void Awake() 
    {
        networkManager = FindObjectOfType<NetworkManager>();
        clientId = networkManager.LocalClientId;
        Init(CarSettings.NewPlayer(new CarStats()));
    }

    private void OnEnable()
    {
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
