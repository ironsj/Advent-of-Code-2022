using System.Runtime.CompilerServices;

namespace Sensors
{
    class SensorsProgram
    {
        private static readonly Dictionary<string, Sensor> Sensors = new();
        private static readonly Dictionary<string, Beacon> Beacons = new();
        static void Main()
        {
            string[] input = File.ReadAllText(@"../../../../../input_example.txt").Split('\n');
            long result = SolvePart1(input);
            long result2 = SolvePart2();
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

        public static long SolvePart2()
        {
            List<Sensor> sensorsList = Sensors.Values.ToList();
            var edgeCoordinates = sensorsList.Select(sensor => GetEdgeCoordinates(sensor)).SelectMany(coordinate => coordinate);

            // iterate through each coordinate that is on the perimeter of the sensor ranges
            foreach (Coordinate coordinate in edgeCoordinates)
            {
                // CHANGE 4000000 TO 20 FOR PROPER RESULTS FOR EXAMPLE INPUT
                // make sure the coordinate is in the range of the distress beacon
                if (coordinate.X > 0 && coordinate.X < 20 && coordinate.Y > 0 && coordinate.Y < 20)
                {
                    // check if the positions's distance to the sensor is greater than a sensor to it's closest beacon for each sensor
                    // find a position that is not in any sensor ranges
                    bool inSensorRange = sensorsList.Any(sensor => sensor.DistanceToBeacon >= sensor.DistanceTo(coordinate));

                    // if the distance to a coordinate is greater than than beacon distance for each sensor we return below
                    if (!inSensorRange)
                    {
                        return coordinate.X * 4000000 + coordinate.Y;
                    }
                }
            }
            
            return 1;
        }

        private static List<Coordinate> GetEdgeCoordinates(Sensor sensor)
        {
            // find the range of the sensor + 1 for manhattan distance
            // furthest left in sensor range plus 1
            Coordinate left = new(sensor.Position.X - sensor.DistanceToBeacon - 1, sensor.Position.Y);
            //furthest right in sensor range plus 1
            Coordinate right = new(sensor.Position.X + sensor.DistanceToBeacon + 1, sensor.Position.Y);
            // furthest up in sensor range plus 1
            Coordinate top = new(sensor.Position.X, sensor.Position.Y + sensor.DistanceToBeacon + 1);
            // furthest down in sensor range plus
            Coordinate bottom = new(sensor.Position.X, sensor.Position.Y - sensor.DistanceToBeacon - 1);

            // list of the coordinates of the perimeter of the range
            List<Coordinate> edges = new();

            // get the edges for the sensor range and add them to the lsit
            edges = edges
                .Concat(GetCoordinatesBetween(left, top))
                .Concat(GetCoordinatesBetween(bottom, right))
                .Concat(GetCoordinatesBetween(top, right))
                .Concat(GetCoordinatesBetween(left, bottom))
                .ToList();

            return edges;
        }

        private static IEnumerable<Coordinate> GetCoordinatesBetween(Coordinate start, Coordinate end)
        {
            // determine which coordinate betweent the start and end is the left and right
            var (left, right) = start.X <= end.X ? (start, end) : (end, start);
            //holds all coordinates between the start and end
            List<Coordinate> coordinates = new();

            // check if the left coordinate is higher or lower than the right coordinate
            if (left.Y <= right.Y)
            {
                // add the coordinates between the left and right coordinates
                for (long offset = 0; offset <= right.X - left.X; offset++)
                {
                    coordinates.Add(new Coordinate(left.X + offset, left.Y + offset));
                }
            }
            else
            {
                // add the coordinates between the left and right coordinates
                for (long offset = 0; offset <= right.X - left.X; offset++)
                {
                    coordinates.Add(new Coordinate(left.X + offset, left.Y - offset));
                }
            }

            // return the list of coordinates between the two positions
            return coordinates;
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