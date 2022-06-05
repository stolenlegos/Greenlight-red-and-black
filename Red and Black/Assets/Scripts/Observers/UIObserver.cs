using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObserver {
    public delegate void MagChange(int bulletCount);
    public static event MagChange BulletCount;

    public delegate void CardPull(string cardName);
    public static event CardPull DisplayCardPulled;

    public static void ObserveMag(int bulletCount) {
        if (BulletCount != null) {
            BulletCount(bulletCount);
        }
    }

    public static void PullCard(string cardName) {
        if (DisplayCardPulled != null) {
            DisplayCardPulled(cardName);
        }
    }
}
