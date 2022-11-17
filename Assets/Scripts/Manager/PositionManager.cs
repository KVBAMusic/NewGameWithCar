using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PositionManager : MonoBehaviour
{
    private CarPositionTracker[] cars, carsSorted;
    void Start()
    {
        cars = FindObjectsOfType<CarPositionTracker>();
        StartCoroutine(nameof(CalculatePosition));
    }

    IEnumerator CalculatePosition()
    {
        for (;;)
        {
            carsSorted = cars.OrderBy(a => a.distanceToPoint)
            .OrderByDescending(a => a.nextPointOnPath)
            .OrderByDescending(a => a.lap)
            .ToArray();

            for (int i = 0; i < carsSorted.Length; i++)
            {
                carsSorted[i].Position = i + 1;
            }
            yield return new WaitForSecondsRealtime(.1f);
        }
    }

    private void OnDestroy() {
        StopCoroutine(nameof(CalculatePosition));
    }
}
