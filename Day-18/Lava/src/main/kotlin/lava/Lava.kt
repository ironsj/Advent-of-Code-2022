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
        // for each cube it counts the neighbors that aren't in the input (exposed sides for a cube)
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
        val toVisit = mutableListOf(Triple(xBounds.first, yBounds.first, zBounds.first))
        // set of the xyz coordinates we have visited
        val visited = mutableSetOf<Triple>()

        //
        var sides = 0
        // keep looping while the list of coordinates to visit is not empty
        while (toVisit.isNotEmpty()) {
            // remove the first element from the list
            val current = toVisit.removeFirst()
            if (current !in visited) {
                current.neighbours()
                    .filter { it.x in xBounds && it.y in yBounds && it.z in zBounds }
                    .forEach { next -> if (next in points) sides++ else toVisit += next }
                visited += current
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