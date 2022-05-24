using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObserver {
    public delegate void MagChange(int bulletCount);
    public static event MagChange BulletCount;

    public static void ObserveMag(int bulletCount) {
        if (BulletCount != null) {
            BulletCount(bulletCount);
        }
    }
}
