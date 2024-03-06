using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Collections;
using System;

public class PlayerInLobby : NetworkBehaviour, ILobbyPlayer
{
    public uint startPlace;
    public bool IsAI => false;

    public NetworkVariable<CarCustomisationParams> carCustomisationParams = new NetworkVariable<CarCustomisationParams>(CarCustomisationParams.Default);
    public override void OnNetworkSpawn()
    {
        DontDestroyOnLoad(gameObject);
        AnnounceConnectionClientRpc();
        carCustomisationParams.OnValueChanged += UpdateCustomisationClientRpc;
    }

    public override void OnNetworkDespawn()
    {
        
    }

    [ClientRpc]
    void AnnounceConnectionClientRpc()
    {
        print($"Object of id {OwnerClientId} has connected!");
        
    }

    [ClientRpc]
    void UpdateCustomisationClientRpc(CarCustomisationParams previousValue, CarCustomisationParams newValue)
    {
        return;
    }
}