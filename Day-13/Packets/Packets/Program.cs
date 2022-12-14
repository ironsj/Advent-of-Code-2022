using System.Text.Json.Nodes;

namespace Packets
{
    class PacketsProgram
    {
        static void Main()
        {
            string[] input = File.ReadAllText(@"../../../../../input.txt").Split("\r\n\r\n");
            int result = SolvePart1(input);
            int result2 = SolvePart2(input);
            Console.WriteLine("Part 1: " + result);
            Console.WriteLine("Part 2: " + result2);
        }

        public static int SolvePart1(string[] input)
        {
            int sum = 0;
            for(int i = 0; i < input.Length; i++)
            {
                // split into pairs
                var packets = input[i].Split("\r\n");
                // parse into json
                JsonNode left = JsonNode.Parse(packets[0])!;
                JsonNode right = JsonNode.Parse(packets[1])!;
                var correctOrder = CompareJson(left, right);
                // add index of pair if in correct order
                if (correctOrder == true)
                {
                    sum += i + 1;
                }
            }
            return sum;
        }

        public static int SolvePart2(string[] input)
        {
            return 1;
        }

        public static bool? CompareJson(JsonNode left, JsonNode right)
        {
            // if left is a value and right is a value
            if(left is JsonValue leftValue && right is JsonValue rightValue)
            {
                // convert to integers
                int leftInt = leftValue.GetValue<int>();
                int rightInt = rightValue.GetValue<int>();
                // if they are equal return null, if left is less than right return true
                return leftInt == rightInt ? null : leftInt < rightInt;
            }

            // if the left value is not an array (for example, is just an integer and right is a list, make left a list with the integer)
            if(left is not JsonArray leftArray) 
            {
                leftArray = new JsonArray(left.GetValue<int>());
            }
            // if the right value is not an array
            if(right is not JsonArray rightArray)
            {
                rightArray = new JsonArray(right.GetValue<int>());
            }

            // compare values in arrays
            for(int i = 0; i < Math.Min(leftArray.Count, rightArray.Count); i++)
            {
                // recursively call to compare at the index in the array
                var result = CompareJson(leftArray[i]!, rightArray[i]!);
                // if the result is not null
                if(result.HasValue)
                {
                    return result;
                }
            }

            // left array ran out of items, return true
            if(leftArray.Count < rightArray.Count) 
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