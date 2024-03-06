using System;
using Unity.Netcode;
using UnityEngine;

public struct CarCustomisationParams : INetworkSerializable
{
    public static CarCustomisationParams Default = new CarCustomisationParams(0, 0, 0, 0, 0);

    public uint carModel;
    public uint wheels;
    public uint wings;
    public uint character;
    public uint carColour;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref carModel);
        serializer.SerializeValue(ref wheels);
        serializer.SerializeValue(ref wings);
        serializer.SerializeValue(ref character);
        serializer.SerializeValue(ref carColour);
    }

    public CarCustomisationParams(uint carModel, uint wheels, uint wings, uint character, uint carColour)
    {
        this.carModel = carModel;
        this.carColour = carColour;
        this.character = character;
        this.wheels = wheels;
        this.wings = wings;
    }

}