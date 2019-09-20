﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PurchaseHandler : MonoBehaviour
{
    private static PurchaseHandler _instance;

    public static PurchaseHandler Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnEnable()
    {
        DragCard.ShopCardClicked += HandlePurchase;
    }

    private void OnDisable()
    {
        DragCard.ShopCardClicked -= HandlePurchase;
    }


    private void HandlePurchase(PlayerCardHolder cardSelected)
    {
        if (TurnManager.Instance.turnPlayer.Currency >= cardSelected.card.CardCost)
        {
            TurnManager.Instance.turnPlayer.graveyard.graveyard.Add(cardSelected.card);

            TurnManager.Instance.turnPlayer.Currency -= cardSelected.card.CardCost;
            
            Destroy(cardSelected.gameObject);
        }
        else
        {
            Debug.Log("Cannot purchase. Not enough currecny");
        }
    }
}




//public class PurchaseHandler : MonoBehaviour
//{
//    public delegate void _CardBought(Card cardBuying);
//    public static event _CardBought CardBought;
//
//    private Player turnPlayer;
//
//    public void Start()
//    {
//        turnPlayer = TurnManager.Instance.turnPlayer;
//    }
//
//    public bool isPurchasable(Card cardClicked)
//    {
//        bool canBePurchased;
//        // TODO
//
//        canBePurchased = true;
//
//        return canBePurchased;
//    }
//
//    public void PurchaseCard(Card cardBuying)
//    {
//        CardBought?.Invoke(cardBuying);
//    }
//
//}
