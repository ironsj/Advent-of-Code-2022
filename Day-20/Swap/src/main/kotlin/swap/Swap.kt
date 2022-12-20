package swap

import java.io.File

class Swap {
    private val path = "src/main/resources/input.txt"
    private val input = File(path).readLines()
    private val numList = input.map { it.toInt() }

    fun solve(): Pair<Long, Long> {
        val part1 = part1()
        val part2 = part2()
        return Pair(part1, part2)
    }

    private fun part1(): Long {
        // our original list of nodes
        val original = linkInput(numList)
        return findGrove(original)
    }

    private fun part2(): Long {
        // our original list of nodes with decryption key
        val original = linkInput(numList, 811_589_153)
        return findGrove(original, 10)
    }

    private fun findGrove(original: List<Node>, numMix: Int = 1): Long{
        // the list of nodes that will be updated as the values are mixed
        val new = original.toMutableList()
        // mix 10 times if decryption key, else 1
        repeat(numMix) {
            // iterate through each node
            original.forEach { node ->
                // get the index of the node
                val index = new.indexOf(node)
                // remove the node
                new.removeAt(index)
                // add the node at its former index + its value modulo the size of the list
                // (the modulo is for if we need to wrap back to the beginning/end of the list
                new.add((index + node.data).mod(new.size), node)
            }
        }
        // get a list of all the values of a node
        val numbers = new.map { it.data }
        // get the index of 0
        val indexOfZero = numbers.indexOf(0)

        // find the value at the 1000th, 2000th, and 3000th index after 0
        return numbers[(1000 + indexOfZero) % numbers.size] +
                numbers[(2000 + indexOfZero) % numbers.size] +
                numbers[(3000 + indexOfZero) % numbers.size]
    }

    private fun linkInput(numbers: List<Int>, key: Int = 1): List<Node> {
        // a list of nodes based on the input
        // in the second part there is a decryption key. if it is provided we multiply the values by it
        return numbers.mapIndexed { index, data -> Node(index, data.toLong() * key) }
    }

    // each node with its index and value
    data class Node(val index: Int, var data: Long)
}