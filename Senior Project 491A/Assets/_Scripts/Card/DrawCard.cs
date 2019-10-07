﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class DrawCard : MonoBehaviourPunCallbacks
{
    /* References the grid space for cards played during the turn */
    [SerializeField]
    private Grid handGrid;
    
    /* References the player's Deck */
    [SerializeField]
    private List<GameObject> deck = new List<GameObject>(10);

    /* Spawn point for car */
    private Vector2 spot = new Vector2();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            // Check if the player has pressed the "draw" button
            if (Input.GetKeyDown("space"))
            {
                // Get card from deck
                GameObject cardDrawn = Instantiate(deck[0]);

                print(cardDrawn);   // DEBUGGING PURPOSES

                // Show card on the field and shift x-pos
                Vector2 spawnPoint = handGrid.GetComponent<Grid>().GetNearestPointOnGrid(spot);
                cardDrawn.transform.position = spawnPoint;
                spot.x += 2.0f;

                // Remove card from deck
                deck.RemoveAt(0);
                photonView.RPC("RPCDrawCard", RpcTarget.Others, spawnPoint);
            }
        }

    }

    [PunRPC]
    private void RPCDrawCard(Vector2 spawnPoint)
    {
        GameObject cardDrawn = Instantiate(deck[0]);
        cardDrawn.transform.position = spawnPoint;
        spot.x += 2.0f;
        deck.RemoveAt(0);

    }

}
