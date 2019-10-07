﻿using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RandomNumberNetworkGenerator : MonoBehaviourPunCallbacks
{
    public int randomNumber;

    private static RandomNumberNetworkGenerator _instance;

    public static RandomNumberNetworkGenerator Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != this && _instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        randomNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties["deckRandomValue"];
        Debug.Log("RandomSyncedValue: " + randomNumber);

    }
}
