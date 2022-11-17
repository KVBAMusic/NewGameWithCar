using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : CarComponent
{
    // wheel colliders
    // add in order: FR, FL, RR, RL
    [SerializeField] private WheelCollider[] wheels_col;

    // wheel models
    [SerializeField] private Transform[] wheels_geom;

    // properties
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float downforce;

    public bool isReversing {
        get{
            return Vector3.Dot(transform.forward, car.RB.velocity.normalized) < 0;
        }
    }

    private float axisV;
    private float axisH;
    private bool reversing;

    public bool IsOnGround
    {
        get {
            bool o = false;
            foreach(var w in wheels_col)
            {
                o |= w.isGrounded;
            }
            return o;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
       // fr_col.ConfigureVehicleSubsteps(10, 2, 2);
    }

    void Update()
    {
        if (!car.isAI)
        {
            axisH = Input.GetAxis("Horizontal");
            axisV = Input.GetAxis("Vertical");
        }

        wheels_col[0].steerAngle = maxSteerAngle * axisH;
        wheels_col[1].steerAngle = maxSteerAngle * axisH;

        for (int i = 0; i < 4; i++)
        {
            wheels_col[i].GetWorldPose(out var pos, out var rot);
            wheels_geom[i].SetPositionAndRotation(pos, rot);
        }
    }

    private void FixedUpdate() {
        float vel = car.RB.velocity.magnitude;
        if (IsOnGround)
            car.RB.velocity += Mathf.Lerp(acceleration * axisV, 0, vel / maxSpeed) * Time.fixedDeltaTime * transform.forward;

        car.RB.AddForce(-transform.up * (vel * downforce));
    }

    public void GetAxisFromAI(float horizontal, float vertical)
    {
        axisH = horizontal;
        axisV = vertical;
    }
}
