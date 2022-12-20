package swap

import java.io.File

class Swap {
    private val path = "src/main/resources/input.txt"
    private val input = File(path).readLines()

    fun solve(): Pair<Int, Int> {
        val part1 = part1()
        val part2 = part2()
        return Pair(part1, part2)
    }

    private fun part1(): Int {

        return 1
    }

    private fun part2(): Int {

        return 1
    }
}