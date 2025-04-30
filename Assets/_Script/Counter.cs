using System;
using UnityEngine;

public class Counter : MonoBehaviour{
    public Tray[] trays = new Tray[5];
    public Transform[] slots = new Transform[5];
    public SplineItemSpawner splineItemSpawner;

    public static Counter Instance{ get; private set; }

    private void Awake(){
        if (Instance == null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }


    public bool TryAddTray(Tray tray, out Vector3 slotPos){
        slotPos = Vector3.zero;
        for (int i = 0; i < trays.Length; i++){
            if (trays[i] != null) continue;
            
            trays[i] = tray;
            slotPos = slots[i].position;
            return true;
        }

        return false;
    }

    private void Update(){
        foreach (var tray in trays){
            if (tray == null) continue;
            if (splineItemSpawner.GetLastItem() == null) continue;
            
            SplineItemMover splineItem = splineItemSpawner.GetLastItem();
            if (splineItem.GetColor() != tray.GetColor()) continue;
            splineItemSpawner.RemoveLastItem();
            tray.AddMover(splineItem);
        }
    }
}