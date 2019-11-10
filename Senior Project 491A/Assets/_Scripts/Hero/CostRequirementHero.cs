﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CostRequirementHero : Hero
{
    [SerializeField] private int cardEffectRequirementCount = 3;

    [SerializeField] protected List<PlayerCard> cardsPlayedForEffect;


    protected virtual void OnEnable()
    {
        PlayZone.CardPlayed += OtherValidateEffectRequirement;
        TurnManager.PlayerSwitched += ClearEffectBuffer;
    }

    protected void OnDisable()
    {
        PlayZone.CardPlayed -= OtherValidateEffectRequirement;
        TurnManager.PlayerSwitched -= ClearEffectBuffer;
    }

    private void ClearEffectBuffer()
    {
        cardsPlayedForEffect.Clear();
    }

    protected virtual void OtherValidateEffectRequirement(PlayerCard cardPlayed)
    {
        if (TurnPlayerHeroManager.Instance.ActiveTurnHero != this)
            return;

        if (cardPlayed.CardEffect is OnPlayEffects)
        {
            if (cardPlayed.CardType == CardType.CardTypes.None)
            {
                //Debug.Log("Card has no type nothing to evaluate");
                return;
            }

            cardsPlayedForEffect.Add(cardPlayed);

            //Debug.Log("Card added to hero effect check");

            if (cardsPlayedForEffect.Count < cardEffectRequirementCount)
            {
                //Debug.Log("Card history too small to evaluate");
                return;
            }

            int lastCardPlayedReference = cardsPlayedForEffect.Count - 1;


            for (int i = 1; i < cardEffectRequirementCount; i++)
            {
                if (cardsPlayedForEffect[lastCardPlayedReference - i].CardType != cardPlayed.CardType)
                {
                    //Debug.Log("last 3 cards were not the same type");
                    return;
                }
            }

            TriggerHeroPowerEffect();
            cardsPlayedForEffect.Clear();
        }
    }
}