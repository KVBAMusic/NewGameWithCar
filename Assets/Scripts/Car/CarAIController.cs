using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAIController : AbstractCarComponent
{
    // this is gonna be a bot
    // 1. get a point on path
    // 2. navigate towards a point on path
    // 3. if stuck, then ???

    [SerializeField] private float distanceThreshold;
    [SerializeField] private float angleThreshold;
    [SerializeField] private float turnMultiplier;

    private int currentPoint = 0;
    private Vector3 nextPos;
    private Vector3 closestPoint;
    private Vector3 closestTangent;

    private bool frontStuck = false;
    private int wallLayerMask = 1 << 3;
    private int carLayerMask = 1 << 6;

    private void Update() 
    {
        float vert;
        float horiz;
        if (!frontStuck) 
        { 
            closestPoint = car.path.GetClosestPointOnPath(transform.position);
            for (int i = 0; i < car.path.NumPoints; i++)
            {
                if (closestPoint == car.path.GetPoint(i))
                {
                    closestTangent = car.path.GetTangent(i);
                    break;
                }
            }
        }
        if (car.isAI)
        {
            nextPos = car.path.GetPoint(car.Position.nextPointOnPath);
            var relativeNextPos = transform.InverseTransformPoint(nextPos);
            if (car.Position.distanceToPoint < distanceThreshold)
            {
                currentPoint = (currentPoint + 1) % car.path.NumPoints;
            }


            horiz = Mathf.Clamp(relativeNextPos.x * turnMultiplier / 6f, -1, 1);

            var toNextPoint = (nextPos - transform.position).normalized;
            if (Vector3.Dot(transform.forward, toNextPoint) > 0)
            {
                vert = 1;
            }
            else vert = -1;

            if (Physics.Raycast(transform.position, transform.forward, out var hit, 8, wallLayerMask | carLayerMask) && !frontStuck)
            {
                var normalTransformed = transform.InverseTransformDirection(hit.normal);
                horiz = Mathf.Sign(normalTransformed.x) * .5f;
                if (car.RB.velocity.magnitude <= 5)
                {
                    frontStuck = true;
                }
            }

            if (Physics.Raycast(transform.position, (transform.forward + transform.right).normalized, out hit, 8, wallLayerMask | carLayerMask))
            {
                horiz = -1;
            }

            if (Physics.Raycast(transform.position, (transform.forward - transform.right).normalized, out hit, 8, wallLayerMask | carLayerMask))
            {
                horiz = 1;
            }

            if (frontStuck)
            {
                vert = -1;
                var lcp = transform.InverseTransformPoint(closestPoint);
                horiz = Mathf.Clamp(lcp.x, -1, 1) * -1;
                if (Mathf.Abs(Vector3.Dot(transform.position, closestTangent)) >= .75f)
                {
                    frontStuck = false;
                }
            }

            car.Movement.GetAxisFromAI(horiz, vert);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, nextPos);
    }
}
