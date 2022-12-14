using System.Text.Json.Nodes;

namespace Packets
{
    class PacketsProgram
    {
        static void Main()
        {
            string input = File.ReadAllText(@"../../../../../input.txt");
            string[] input2 = input.Split("\r\n\r\n");
            int result = SolvePart1(input2);
            int result2 = SolvePart2(input);
            Console.WriteLine("Part 1: " + result);
            Console.WriteLine("Part 2: " + result2);
        }

        public static int SolvePart1(string[] input)
        {
            int sum = 0;
            for (int i = 0; i < input.Length; i++)
            {
                // split into pairs
                string[] packets = input[i].Split("\r\n");
                // parse into json
                JsonNode left = JsonNode.Parse(packets[0])!;
                JsonNode right = JsonNode.Parse(packets[1])!;
                bool? correctOrder = CompareJson(left, right);
                // add index of pair if in correct order
                if (correctOrder == true)
                {
                    sum += i + 1;
                }
            }
            return sum;
        }

        public static int SolvePart2(string input)
        {
            // Get list of all packets
            List<JsonNode?> packetsList = input.Split("\r\n").Where(line => !string.IsNullOrEmpty(line)).Select(packet => JsonNode.Parse(packet)).ToList();

            // Add the divider packets
            JsonNode divider = JsonNode.Parse("[[2]]")!;
            JsonNode divider2 = JsonNode.Parse("[[6]]")!;

            packetsList.Add(divider);
            packetsList.Add(divider2);

            // Compare each packet and sort them
            packetsList.Sort((left, right) => CompareJson(left!, right!) == true ? -1 : 1);

            // Return the product of the indices of the divider packets
            int result = (packetsList.IndexOf(divider) + 1) * (packetsList.IndexOf(divider2) + 1);
            return result;
        }

        public static bool? CompareJson(JsonNode left, JsonNode right)
        {
            // RULE 1
            // if left is a value and right is a value
            if (left is JsonValue leftValue && right is JsonValue rightValue)
            {
                // convert to integers
                int leftInt = leftValue.GetValue<int>();
                int rightInt = rightValue.GetValue<int>();
                // if they are equal return null, if left is less than right return true
                return leftInt == rightInt ? null : leftInt < rightInt;
            }

            // RULE 3
            // if the left value is not an array (for example, is just an integer and right is a list, make left a list with the integer)
            if (left is not JsonArray leftArray)
            {
                leftArray = new JsonArray(left.GetValue<int>());
            }
            // if the right value is not an array
            if (right is not JsonArray rightArray)
            {
                rightArray = new JsonArray(right.GetValue<int>());
            }

            // RULE 2
            // compare values in arrays
            for (int i = 0; i < Math.Min(leftArray.Count, rightArray.Count); i++)
            {
                // recursively call to compare at the index in the array
                bool? result = CompareJson(leftArray[i]!, rightArray[i]!);
                // if the result is not null
                if (result.HasValue)
                {
                    return result;
                }
            }

            // left array ran out of items, return true
            if (leftArray.Count < rightArray.Count)
            {
                return true;
            }
            // right array ran out of items, return false
            if (leftArray.Count > rightArray.Count)
            {
                return false;
            }

            return null;
        }
    }
}