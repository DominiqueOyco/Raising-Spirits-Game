﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    Defines the Player's Hand
 */
public class Hand : MonoBehaviour
{
    /* Hand-specific refereneces*/
    [SerializeField] private List<PlayerCard> hand;
    private int cardsInHand = 0;

    /* Player references */
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject playerSpace;
    private PlayerDeck deck;
    private Graveyard graveyard;
    private CreateGrid handGrid;

    /* Spawn point for card */
    private Vector2 cardSpot;

    void Start()
    {
        // Set references for Deck and Hand Grid
        deck = playerObj.GetComponentInChildren<PlayerDeck>();
        graveyard = playerObj.GetComponentInChildren<Graveyard>();
        handGrid = playerSpace.GetComponentInChildren<CreateGrid>();
        cardSpot = new Vector2();
    }

    /* Adds a card to the Hand from the Deck.
        If there are no cards in the Deck but there is a graveyard then
        shuffle the graveyard and put it into the deck.
        NOTE: No phantom cards are drawn using this technique
     */
    public void AddCard()
    {
        PlayerCard cardDrawn;

        if (cardsInHand > 5)
        {
            /// RESIZE CREATE GRID COORDS
            /// MOVE CARDS ALREADY IN HAND TO THEIR RESPECTIVE LOCATION
            /// THIS DOESN'T WORK CORRECTLY YET
            handGrid.ResizeGrid(handGrid.size * 0.88f, handGrid.xValUnits + 1);
            cardSpot = new Vector2();

            foreach (PlayerCard card in hand)
            {
                Debug.Log("Card is:\t" + card);
                Debug.Log("Card at:\t" + card.transform.position);

                // Change their coordinates
                Vector2 spawnPoint = handGrid.GetNearestPointOnGrid(cardSpot);
                card.SetCoord(spawnPoint);

                Debug.Log("Card now at:\t" + card.transform.position);
                cardSpot.x += handGrid.size;
            }
        }

        // Draw cards from the deck if there are cards in the Deck
        if (deck.GetDeck().Count > 0)
        {
            cardDrawn = (PlayerCard)deck.DrawCard();
        }
        // Otherwise, make the Graveyard your Deck now
        else if (graveyard.GetGraveyard().Count > 0)
        {
            graveyard.MoveToDeck(deck);    // Move cards from Graveyard to the Deck
            cardDrawn = (PlayerCard)deck.DrawCard();
        }
        // DEBUGGING PURPOSES
        else
        {
            cardDrawn = deck.testCard;
        }

        PlaceCard(cardDrawn);
    }

    /* Removes a PlayerCard if it exists in the Hand */
    public void RemoveCard(PlayerCard cardAffected)
    {
        if (cardAffected != null && hand.Contains(cardAffected))
        {
            if (cardsInHand > 6)
            {
                /// TODO: Resize to normal
                /// ???
                Debug.Log("RemoveCard, cardsInHand > 6 not finished");
            }
            else
            {
                hand.Remove(cardAffected);
                cardsInHand -= 1;
            }
        }
    }

    /* A unique draw that is only done at the beginning of the Player's turn 
        If there are no cards in the Deck but there is a graveyard then
        shuffle the graveyard and put it into the deck.
        If there are no cards in the Deck and no graveyard, then draw
        a phantom card.
    */
    public void TurnStartDraw()
    {
        PlayerCard cardDrawn;
        cardsInHand = 0;
        bool graveyardAdded = false;

        while (cardsInHand != 6)
        {
            // Draw cards from the deck if there are cards in the Deck
            if (deck.GetDeck().Count > 0)
            {
                cardDrawn = (PlayerCard)deck.DrawCard();
            }
            else
            {
                if (graveyard.GetGraveyard().Count > 0 && !graveyardAdded)
                {
                    graveyard.MoveToDeck(deck);
                    graveyardAdded = true;

                    cardDrawn = (PlayerCard)deck.DrawCard();
                }
                else
                {
                    // Draw a phantom card
                    cardDrawn = (PlayerCard)deck.phantomCard;
                }
            }

            PlaceCard(cardDrawn);
        }
    }

    private void PlaceCard(PlayerCard card)
    {
        Vector2 spawnPoint;
        spawnPoint = handGrid.GetNearestPointOnGrid(cardSpot);
        
        if (handGrid.IsPlaceable(spawnPoint))
        {
            card.SetCoord(spawnPoint);
            hand.Add(card);
            Instantiate(card, this.transform);

            cardsInHand += 1;
            cardSpot.x += handGrid.size;
        }
    }

    void UpdateHandDisplay()
    {

    }

    //-------------------//
    //----- GETTERS -----//
    //-------------------//
    public List<PlayerCard> GetHand()
    {
        return this.hand;
    }
    public int GetHandCount()
    {
        return this.cardsInHand;
    }
}
