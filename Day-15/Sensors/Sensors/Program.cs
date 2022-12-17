using System.Runtime.CompilerServices;

namespace Sensors
{
    class SensorsProgram
    {
        private static readonly Dictionary<string, Sensor> Sensors = new();
        private static readonly Dictionary<string, Beacon> Beacons = new();
        static void Main()
        {
            string[] input = File.ReadAllText(@"../../../../../input.txt").Split('\n');
            long result = SolvePart1(input);
            int result2 = SolvePart2(input);
            Console.WriteLine("Part 1: " + result);
            Console.WriteLine("Part 2: " + result2);
        }

        public static long SolvePart1(string[] input)
        {
            // go through each line
            foreach(string line in input)
            {
                // split between beacons and sensors
                string[] split = line.Split(": ");
                // beacon coordinates from string
                string beaconString = split[1][21..];
                Beacon beacon = ParseBeacon(beaconString);

                // sensor coordinates from string
                string sensorString = split[0][10..];
                ParseSensor(sensorString, beacon);
            }
            
            // list of beacons
            List<Beacon> beaconsList = Beacons.Values.ToList();
            // list of sensors
            List<Sensor> sensorsList = Sensors.Values.ToList();

            // sort the beacons in ascending order by x coordinate
            beaconsList.Sort((left, right) => left.Position.X.CompareTo(right.Position.X));
            // sort the sensors in ascending order by x coordinate
            sensorsList.Sort((left, right) => left.Position.X.CompareTo(right.Position.X));

            // x position of farthest left sensor x position minus the manhattan distance to the closest beacon
            long furthestLeftSensorRange = sensorsList.Select(sensor => sensor.Position.X - sensor.DistanceToBeacon).OrderBy(x => x).First();
            // x position of the farthest right sensor x position plus the mangattan distance to the closest beacon
            long furthestRightSensorRange = sensorsList.Select(sensor => sensor.Position.X + sensor.DistanceToBeacon).OrderByDescending(x => x).First();

            // find what is less between the farthest left beacon and the farthest left sensor range (determines starting range)
            long rangeStart = Math.Min(furthestLeftSensorRange, beaconsList[0].Position.X);
            // find what is more between the farthest right beacon and the farthest right sensor range (determines ending range)
            long rangeEnd = Math.Max(furthestRightSensorRange, beaconsList[^1].Position.X);

            // the number of positions a beacon cannot be present in a row
            long invalidPositionCount = 0;

            // go through each position in the range
            for (long x = rangeStart; x < rangeEnd; x++)
            {
                // the position in the range for the 2000000th row (y position)
                // CHANGE Y POSITION TO 10 FOR PROPER RESULTS FOR EXAMPLE INPUT
                var position = new Coordinate(x, 2000000);
                // if the current position is not equal the position of the closest beacon to a sensor (make sure it is not a beacon we are counting)
                // and the distance from a sensor to its closest beacon is greater than or equal to the distance of the current position (within sensor range)
                // then the current position is invalid
                if (sensorsList.Any(sensor => position != sensor.ClosestBeacon.Position && sensor.DistanceToBeacon >= sensor.DistanceTo(position)))
                {
                    invalidPositionCount++;
                }
            }

            return invalidPositionCount;
        }

        public static int SolvePart2(string[] input)
        {
            return 1;
        }

        private static Beacon ParseBeacon(string key)
        {
            // check if beacon is already in the dictionary
            if (!Beacons.ContainsKey(key))
            {
                // parse coordinates from string
                string[] segments = key.Split(", ");
                long x = long.Parse(segments[0]["x=".Length..]);
                long y = long.Parse(segments[1]["y=".Length..]);
                // create new beacon
                var beacon = new Beacon
                {
                    Position = new Coordinate(x, y)
                };
                // add beacon to dictionary
                Beacons.Add(key, beacon);
            }
            return Beacons[key];
        }

        private static void ParseSensor(string key, Beacon closestBeacon)
        {
            // check if sensor is already in the dictionary
            if (!Sensors.ContainsKey(key))
            {
                // parse coordinates from string
                string[] segments = key.Split(", ");
                long x = long.Parse(segments[0]["x=".Length..]);
                long y = long.Parse(segments[1]["y=".Length..]);
                // create new sensor
                var sensor = new Sensor
                {
                    Position = new Coordinate(x, y),
                    ClosestBeacon = closestBeacon,
                };
                // add sensor to dictionary
                Sensors.Add(key, sensor);
            }
        }
    }
}