namespace Sensors
{
    public class Sensor
    {
        public Coordinate Position { get; set; } = new(0, 0);
        public Beacon ClosestBeacon { get; set; } = new();
        // find the distance from the sensor to the beacon
        public long DistanceToBeacon => DistanceTo(ClosestBeacon.Position);
        public long DistanceTo(Coordinate position) =>
            Math.Abs(Position.X - position.X) + Math.Abs(Position.Y - position.Y);
    }
}
