package lava

import java.io.File


class Lava {
    private val path = "src/main/resources/input.txt"
    private val input = File(path).readLines()

    // get all cube x,y,z positions into a set
    private val points = input.map { it.split(",").map(String::toInt) }.map { (x, y, z) -> Triple(x, y, z) }.toSet()

    fun solve(): Pair<Int, Int> {
        val part1 = part1()
        val part2 = part2()
        return Pair(part1, part2)
    }

    private fun part1(): Int {
        // for each cube this counts the neighbors that aren't in the input (exposed sides for a cube)
        // then we find the sum of all exposed sides
        return points.sumOf { point -> point.neighbours().count { it !in points } }
    }

    private fun part2(): Int {
        // the smallest and largest x value
        val xBounds = points.minOf { it.x } - 1..points.maxOf { it.x } + 1
        // the smallest and largest y value
        val yBounds = points.minOf { it.y } - 1..points.maxOf { it.y } + 1
        // the smallest and largest z value
        val zBounds = points.minOf { it.z } - 1..points.maxOf { it.z } + 1

        // list of xyz coordinates we will visit
        val visitNext = mutableListOf(Triple(xBounds.first, yBounds.first, zBounds.first))
        // set of the xyz coordinates we have visited
        val visited = mutableSetOf<Triple>()

        // the sides that are exposed to the air
        var sides = 0
        // keep looping while the list of coordinates to visit is not empty
        while (visitNext.isNotEmpty()) {
            // remove the first element from the list
            val current = visitNext.removeFirst()
            if (current !in visited) {
                // for our current cube, get all the neighbors in our x,y,z range
                // then if one of the neighbors is in the points add 1 to the sides
                // (neighbor is in input and our current position is not so this is an open side)
                // else add the neighbor to the next points to visit
                current.neighbours()
                    .filter { it.x in xBounds && it.y in yBounds && it.z in zBounds }
                    .forEach { neighbor -> if (neighbor in points) sides++ else visitNext.add(neighbor) }
                // add the current cube to our visited
                visited.add(current)
            }
        }
        return sides
    }

    // data class holding the xyz position of a cube
    data class Triple(val x: Int, val y: Int, val z: Int) {
        // a list of all the neighbors of a xyz position
        // if a neighbor is in the
        fun neighbours(): List<Triple> {
            return listOf(
                Triple(x - 1, y, z), Triple(x + 1, y, z),
                Triple(x, y - 1, z), Triple(x, y + 1, z),
                Triple(x, y, z - 1), Triple(x, y, z + 1),
            )
        }
    }
}