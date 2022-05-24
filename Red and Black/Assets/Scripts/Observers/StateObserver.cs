using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateObserver {
    public delegate void ObserveState(string state);
    public static event ObserveState NotifyState;

    public static void StateChanged(string state) {
        if (NotifyState != null) {
            NotifyState(state);
        }
    }
}
