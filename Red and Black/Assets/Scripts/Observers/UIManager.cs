using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI bulletText;
    [SerializeField] private TextMeshProUGUI cardText;

    private void OnEnable() {
        UIObserver.BulletCount += ChangeBulletCount;
        UIObserver.DisplayCardPulled += CardPulled;
    }

    private void ChangeBulletCount(int bulletCount) {
        bulletText.text = "Bullets: " + bulletCount.ToString();
    }

    private void CardPulled (string cardName) {
        cardText.text = "Card: " + cardName;
    }

    private void OnDisable() {
        UIObserver.BulletCount -= ChangeBulletCount;
        UIObserver.DisplayCardPulled -= CardPulled;
    }


}
