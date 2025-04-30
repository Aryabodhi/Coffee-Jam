using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class SplineItemSpawner : MonoBehaviour
{
    public SplineContainer spline;
    public List<SplineItemMover> activeItems = new List<SplineItemMover>();
    
    public List<SplineItemMover> items = new List<SplineItemMover>();

    public float spawnCooldown = 1f;
    private float timer;

    private void Update()
    {
        if(items.Count <= 0) return;
        
        timer += Time.deltaTime;

        if (timer >= spawnCooldown && CanSpawn())
        {
            timer = 0f;
            SplineItemMover mover = items.First();
            mover.splineContainer = spline;
            mover.t = 0;
            items.Remove(mover);
            
            if (activeItems.Count > 0)
                mover.itemInFront = activeItems[activeItems.Count - 1];

            activeItems.Add(mover);
        }

    }

    public SplineItemMover GetLastItem(){
        return activeItems.Find(item=>item.t >= 1f);
    }

    public void RemoveLastItem(){
        activeItems.RemoveAt(0);
        if (activeItems.Count > 0){
            activeItems.First().itemInFront = null;
        }
    }

    private bool CanSpawn()
    {
        if (activeItems.Count == 0) return true;

        var firstItem = activeItems[activeItems.Count - 1];
        Vector3 spawnPos = spline.EvaluatePosition(0);
        Vector3 frontPos = spline.EvaluatePosition(firstItem.t);

        return Vector3.Distance(spawnPos, frontPos) > 1.5f; 
    }
}