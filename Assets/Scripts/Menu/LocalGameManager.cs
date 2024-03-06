using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;
using UnityEngine.Events;

public sealed class LocalGameManager : MonoBehaviour
{
    
    private static LocalGameManager singleton = null;
    public LocalGameManager Instance => singleton;

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    private static bool IsValidIp(string ip) => IPAddress.TryParse(ip, out _);

    public UnityEvent OnConnectionSuccess;
    public UnityEvent OnConnectionFail;
    public UnityEvent OnHostDisconnected;

    [SerializeField] private NetworkManager netManager;
    [SerializeField] private UnityTransport netTransport;

    [SerializeField] private TMP_InputField ipInputField;

    public List<ILobbyPlayer> players;
    private void Awake() {
        if (singleton != null && singleton != this)
            Destroy(this.gameObject);
        else singleton = this;
    }

    public void SetIp(string ip)
    {
        netTransport.SetConnectionData(ip, 7777);
    }

    public void SetOwnLocalIp()
    {
        netTransport.SetConnectionData(GetLocalIPAddress(), 7777);
    }

    public void HostStartLobby()
    {
        // get local ip address
        // set it to the NetworkTransport class
        SetIp("127.0.0.1");
        // start host
        if (netManager.StartHost())
        {
            OnConnectionSuccess.Invoke();
        }
        else OnConnectionFail.Invoke();
    }

    public void StartSinglePlayer()
    {
        // set ip to localhost
        SetIp("127.0.0.1");
        // spawn 
    }

    public void ClientConnect()
    {
        // check if the IP from the input field is valid
        if (!IsValidIp(ipInputField.text)) 
        {
            OnConnectionFail.Invoke();
            Debug.LogWarning("Invalid host IP");
        }
        else
        {
            SetIp(ipInputField.text);
            if (netManager.StartClient())
            {
                OnConnectionSuccess.Invoke();
            }
            else OnConnectionFail.Invoke();
        }
    }
}