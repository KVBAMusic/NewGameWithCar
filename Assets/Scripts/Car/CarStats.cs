public struct CarStats
{
    public readonly float maxSpeed;
    public readonly float acceleration;
    public readonly float steerAngle;

    public CarStats(float maxSpeed, float acceleration, float steerAngle)
    {
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.steerAngle = steerAngle;
    }
}