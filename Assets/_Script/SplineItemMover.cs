using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class SplineItemMover : MonoBehaviour{
    public SplineContainer splineContainer;
    public float speed = 2f;
    public float minDistance; 

    public float t; // Current position on spline (0 to 1)
    public SplineItemMover itemInFront;
    public ColorIdentifier colorIdentifier;

    void Update(){
        if(!splineContainer) return;
        if (CanMoveForward(Time.deltaTime)){
            t += (speed / splineContainer.CalculateLength()) * Time.deltaTime;
            t = Mathf.Clamp01(t);

            Vector3 pos;
            pos = splineContainer.EvaluatePosition(t);
            transform.position = pos;
        }
    }

    bool CanMoveForward(float deltaTime){
        if (t >= 1f) return false;
        if (itemInFront == null) return true;

        float predictedT = t + (speed / splineContainer.CalculateLength()) * deltaTime;
        predictedT = Mathf.Clamp01(predictedT);

        Vector3 myNextPos = splineContainer.EvaluatePosition(predictedT);
        Vector3 frontPos = splineContainer.EvaluatePosition(itemInFront.t);

        float distance = Vector3.Distance(myNextPos, frontPos);
        return distance > minDistance;
    }

    public ColorId GetColor(){
        return colorIdentifier.GetColor();
    }
}