using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Deck : MonoBehaviour {
    [SerializeField] private List<Card> cardsPulled = new List<Card>();
    private int deckLimit = 8;
    private int deckCurrent;
    private bool addToDeck;


    private void Awake() {
        deckCurrent = 0;
    }


    public void DrawCard(InputAction.CallbackContext ctx) {
        if (ctx.performed && deckCurrent < deckLimit) {
            GenerateCard();
        }
    }

    public void GenerateCard () {
        addToDeck = true;
        var card = new Card();
        card.Initialize();
        
        foreach (Card previousCards in cardsPulled) {
            if (previousCards.cardName == card.cardName) {
                addToDeck = false;
                GenerateCard();
            }
        }

        if (addToDeck) {
            cardsPulled.Add(card);
            UIObserver.PullCard(card.cardName);
            deckCurrent++;
        }
    }
}


[System.Serializable] 
public class Card {
    public CardSuit suit;
    public CardValue value;
    public CardEffects effectName;
    public string cardName;

    public void Initialize() {
        suit = (CardSuit)Random.Range(0, 3);
        value = (CardValue)Random.Range(0, 12);

        if (suit == CardSuit.Diamonds || suit == CardSuit.Hearts) {
            effectName = (CardEffects)Random.Range(0, 5);
        } else {
            effectName = (CardEffects)Random.Range(6, 9);
        }

        cardName = value + " of " + suit;
    }
}


public enum CardEffects {
    //if you add anything to this list, PLEASE adjust the numbers above and below in the comments
    //Red Effects (0-5)
    ricochet,
    extraAmmo,
    extraLife,
    penetrate,
    bottomless,
    speedBuff,
    //black effects (6-9)
    sturdy,
    rapidFire,
    reinforcements,
    removeBonuses
}


public enum CardSuit {
    Hearts,
    Diamonds,
    Clubs,
    Spades
}


public enum CardValue {
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}