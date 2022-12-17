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

    public fun solve(): Int {
        val answer = part1()
        return answer
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