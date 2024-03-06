using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using System.Net;
using System;
using System.Collections;
using System.Collections.Generic;

public class LobbyManager : NetworkBehaviour
{
    static readonly int maxPlayers = 8;
    public static LobbyManager Instance = null;

    public LobbySlot[] slots;

    public bool IsFull => !Array.Exists(slots, s => s == null);

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else Instance = this;
    }

    public void InitialiseLobby()
    {
        slots = new LobbySlot[maxPlayers];
    }
    
}