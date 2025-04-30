using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;

[RequireComponent(typeof(SplineContainer))]
public class ResampleSpline : MonoBehaviour
{
    public float distance = 10;

    void Start()
    {
        var container = GetComponent<SplineContainer>();
        var originalSpline = container.Spline;

        var localToWorldMatrix = container.transform.localToWorldMatrix;
        float splineLength = container.Splines[0].CalculateLength(localToWorldMatrix);
        int knotCount = (int)(splineLength / distance);
        var newSpline = new Spline();

        for (int i = 0; i < knotCount; i++)
        {
            float t = i / (float)(knotCount - 1);

            float3 position, tangent, up;
            originalSpline.Evaluate(t, out position, out tangent, out up);

            var knot = new BezierKnot(position, float3.zero, float3.zero);
            newSpline.Add(knot);
        }

        container.Spline = newSpline;
        Debug.Log("Resampled spline with " + knotCount + " knots using normalized t.");
    }
}