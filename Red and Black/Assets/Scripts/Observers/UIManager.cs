using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI bulletText;

    private void OnEnable() {
        UIObserver.BulletCount += ChangeBulletCount;
    }

    private void ChangeBulletCount(int bulletCount) {
        bulletText.text = "Bullets: " + bulletCount.ToString();
    }

    private void OnDisable() {
        UIObserver.BulletCount -= ChangeBulletCount;
    }


}
