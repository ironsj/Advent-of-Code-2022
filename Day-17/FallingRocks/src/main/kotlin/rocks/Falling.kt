package rocks

import java.io.File


class Falling {
    private val path = "src/main/resources/input.txt"
    private val input = File(path).readText()
    // create array where '>' is 1 and '<' is -1
    private val jetPattern = input.map { if (it == '>') 1 else -1 }
    // keeps track of where we are in the jetPattern
    private var patternIndex = 0
    // height of the tower of rocks
    private var height = 0
    // set holding all the positions that are blocked (the floor and the rock positions)
    private val chamber = mutableSetOf<Position>()

    public fun solve(): Pair<Int, Long> {
        val part1 = part1()
        val part2 = part2()
        return Pair(part1, part2)
    }

    private fun part1(): Int{
        // add the floor position
        for (i in 0..6){
            chamber.add(Position(i, 0))
        }

        // drop rocks 2022 times
        for (i in 0..2021){
            // alternate between dropping each of the 5 rocks
            dropRock(Rock.values()[i % 5])
        }

        return height
    }

    private fun part2(): Long{
        // reset variables
        height = 0
        chamber.clear()
        patternIndex = 0

        // do the same as we did in part 1
        for (i in 0..6){
            chamber.add(Position(i, 0))
        }

        // a list that keeps track of the max height after each drop
        val maxHeightHistory = mutableListOf(0)
        for (i in 0 until 2022){
            dropRock(Rock.values()[i % 5])
            // add the current max after the rock has come to a rest
            maxHeightHistory.add(height)
        }

        // a list containing the difference between each max height
        // first it creates a list full of pairs of each adjacent element
        // then it find the difference for each pair
        val maxHeightDifferences = maxHeightHistory.zipWithNext().map { (a, b) -> b - a }

        // We are now looking for a pattern in the difference of maximum heights
        // If we find one, it can be applied to find the height after 1,000,000,000,000 rocks are dropped

        // a value to start searching for the pattern
        // do not know the best way to find this starting pattern, so it is trial and error
        val patternStart = 225
        // length of the sublist we will take from the height differences
        val subListLength = 10
        // 10 of the height differences in a list
        // we are going to search for this sequence again
        // this will indicate the pattern starting again
        val subList = maxHeightDifferences.subList(patternStart, patternStart + subListLength)

        // this will be equal to how much difference there is in height between when the pattern starts and ends
        var patternHeight = 0
        // this will equal how many rock are dropped from when the pattern starts to when the pattern ends
        var patternLength = 0
        // the height before the pattern began
        val heightBeforePattern = maxHeightHistory[patternStart - 1]
        // look for sequence in the rest of the list
        for (i in patternStart + subListLength until maxHeightDifferences.size) {
            // if our earlier sequence equals the new sublist of 10
            if (subList == maxHeightDifferences.subList(i, i + subListLength)) {
                // The sequence starts again at i. Therefore, the length of the pattern is i minus
                // where we started the pattern
                patternLength = i - patternStart
                // The difference in height between the beginning of the sequence and the end
                patternHeight = maxHeightHistory[i - 1] - heightBeforePattern
                break
            }
        }

        // number of rocks we are dropping
        val numRocks = 1_000_000_000_000
        // This is the number of times the pattern that can fit between the total number of rocks
        // and when we started searching for the pattern
        val numPatterns = (numRocks - patternStart) / patternLength
        // The remainder of height for the last pattern
        // This is because the last pattern may not fit fully
        val offsetIntoLastPattern = ((numRocks - patternStart) % patternLength).toInt()
        // the rest of the height in the pattern
        val extraHeight = maxHeightHistory[patternStart + offsetIntoLastPattern] - heightBeforePattern
        // the height when we started searching for the pattern plus
        // the height for each full pattern together plus
        // the leftover height at the end
        return heightBeforePattern + (patternHeight * numPatterns) + extraHeight
    }

    private fun dropRock(rock: Rock){
        // every rock drops two to the right of the left chamber wall
        var xPos = 2
        // bottom edge of rock has 3 units of space between the highest rock
        var yPos = height + 4

        // keep looping until a rock can no longer be dropped
        while (true){
            // add the affect of the jet stream to the rock (plus or minus one)
            val newX = xPos + jetPattern[patternIndex % jetPattern.size]
            // iterate the index for next time
            patternIndex++

            // add the new position to the rock and check
            // if all the rock positions are in the chamber walls and not in a position a rock exists
            // the rock will move left or right
            if (rock.positions.map { it + Position(newX, yPos) }.all { it.x in 0..6 && it !in chamber}) {
                xPos = newX
            }

            // if the rock is above the floor and its next position will not be in the position of
            // another existing rock, drop it 1 lower
            if (yPos > 1 && rock.positions.map { it + Position(xPos, yPos - 1) }.none { it in chamber }) {
                yPos--
            }
            // when the rock can no longer be dropped
            else {
                // add the final positions of the rock to the set of blocked positions
                rock.positions.map { it + Position(xPos, yPos) }.forEach { chamber.add(it)}
                // find the max height of the rock tower
                val highest = chamber.maxBy { it.y }
                // update the maximum height
                height = highest.y
                break
            }
        }

    }

    // enum for each rock
    // list of positions for the space it fills
    enum class Rock(val height: Int, val positions: List<Position>){
        Line(1, listOf(Position(0, 0), Position(1, 0), Position(2, 0), Position(3, 0))),
        Cross(3, listOf(Position(1, 0), Position(0, 1), Position(1, 1), Position(2, 1), Position(1, 2))),
        BackwardsL(3, listOf(Position(0, 0), Position(1, 0), Position(2, 0), Position(2, 1), Position(2, 2))),
        Pipe(4, listOf(Position(0, 0), Position(0, 1), Position(0, 2), Position(0, 3))),
        Square(2, listOf(Position(0, 0), Position(0, 1), Position(1, 0), Position(1, 1)))
    }

    // data class for a x,y coordinate
    data class Position(val x: Int, val y: Int){
        // allows you to add positions together to get a new position
        operator fun plus(other: Position) = Position(x + other.x, y + other.y)
    }
}