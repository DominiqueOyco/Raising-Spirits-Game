﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The container object for the Shop, which itself can contain PlayerCardContainers.
/// </summary>
public class ShopContainer : PlayerCardContainer
{
    public ShopDeck shopDeck;

    //[SerializeField]
    //public PlayerCardHolder playerCardContainer;

    /// <summary>
    /// Maximum number of cards in the shop at any given time.
    /// </summary>
    private int shopCardCount = 6;

    // Start is called before the first frame update
    void Start()
    {
        InitialCardDisplay();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Initializes the Shop's card's placements.
    /// </summary>
    protected override void InitialCardDisplay()
    {
        for (int i = 0; i < shopCardCount; i++)
        {
            if (shopDeck.cardsInDeck.Count <= 0)
            {
                Debug.Log("Shop deck is " + shopDeck.cardsInDeck.Count);
                return;
            }

            // Draw a Card
            PlayerCard cardDrawn = null;
            cardDrawn = (PlayerCard)shopDeck.cardsInDeck.Pop();

            // PlayerCardHolder
            holder.card = cardDrawn;
            PlayerCardHolder cardHolder = Instantiate(holder, containerGrid.freeLocations.Dequeue(), Quaternion.identity, this.transform);
            // Connect 
            containerGrid.cardLocationReference.Add(new Vector2(cardHolder.gameObject.transform.position.x,
                cardHolder.gameObject.transform.position.y), cardHolder);
        }
    }
}
