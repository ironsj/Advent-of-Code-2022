package day16

import java.io.File
import java.nio.file.Paths
import kotlin.math.max

class Valve2 {
    // the most pressure we can release in 30 minutes
    private var finalPressure = 0

    fun solve(): Int {
        val path = Paths.get("").toAbsolutePath().toString()
        val input = File("$path/../input.txt").readLines()
        // a list of all the valves (includes their id, flow rate, and neighbor ids')
        val valves = input.map { SingleValve.from(it) }
        // a map where the key is the valve id and the value is the valve itself
        val idMap = valves.associateBy { it.id }
        // mutable map where the key is the valve id and the value is a mutable map
        // where the keys are the neighbor ids and the values are 1
        val valvesMap = valves.associate { it.id to it.neighborIds.associateWith { 1 }.toMutableMap() }.toMutableMap()
        println(valvesMap)
        // uses the floyd warshall algorithm to find the shortest paths
        // returns a mutable map with the key as the valve id and the value as a mutable map
        // with key as neighbor id and value as distance
        val shortestPaths = floydWarshall(valvesMap, idMap)
        println(shortestPaths)
        getPressure(0, "AA", emptySet(), 0, shortestPaths, idMap)
        return finalPressure
    }

    private fun getPressure(
        currentPressure: Int,
        currentValve: String,
        visited: Set<String>,
        currentTime: Int,
        shortestPaths: MutableMap<String, MutableMap<String, Int>>,
        idMap: Map<String, SingleValve>
    ) {
        // set the final pressure equal to the max between it and the current pressure
        finalPressure = max(currentPressure, finalPressure)
        // goes through each neighbor, distance for a valve
        for ((valve, dist) in shortestPaths[currentValve]!!) {
            // if the neighbor hasn't been visited and we haven't gone over the time limit enter
            // time is checked with the current time +
            // the distance (since its one minute to visit) +
            // 1 (since that is how long it takes to open a valve)
            if (!visited.contains(valve) && currentTime + dist + 1 < 30) {
                // recursively call to visit other neighbors
                // currentPressure is the time remaining * the flow rate of the neighbor + the current pressure
                // valve is the neighbor we are currently visiting
                // visited has the current neighbor added to it
                // current time has the distance to the new point and 1 added to it
                // the maps stay the same
                getPressure(
                    currentPressure + (30 - currentTime - dist - 1) * idMap[valve]?.flowRate!!,
                    valve,
                    visited.union(listOf(valve)),
                    currentTime + dist + 1,
                    shortestPaths,
                    idMap
                )
            }
        }
    }

    // floyd warshall algorithm https://www.programiz.com/dsa/floyd-warshall-algorithm
    private fun floydWarshall(
        shortestPaths: MutableMap<String, MutableMap<String, Int>>,
        valves: Map<String, SingleValve>
    ): MutableMap<String, MutableMap<String, Int>> {
        // see link above for explanation
        for (k in shortestPaths.keys) {
            for (i in shortestPaths.keys) {
                for (j in shortestPaths.keys) {
                    val ik = shortestPaths[i]?.get(k) ?: INF
                    val kj = shortestPaths[k]?.get(j) ?: INF
                    val ij = shortestPaths[i]?.get(j) ?: INF
                    if (ik + kj < ij)
                        shortestPaths[i]?.set(j, ik + kj)
                }
            }
        }

        // each step to a new neighbor will be 1
        // remove all paths that lead to a valve with rate 0
        shortestPaths.values.forEach {
            // gets a list of each neighbor and replaces those with flow rate != 0 with ""
            it.keys.map { key -> if (valves[key]?.flowRate == 0) key else "" }
                // if the key is not "" remove it
                .forEach { toRemove -> if (toRemove != "") it.remove(toRemove) }
        }
        return shortestPaths
    }

    // data class that holds information for a valve
    // parses each line of input to get the valve id, flow rate, and it's neighbors' id
    data class SingleValve(val id: String, val flowRate: Int, val neighborIds: List<String>) {
        companion object {
            fun from(line: String): SingleValve {
                val (name, rate) = line.split("; ")[0].split(" ").let { it[1] to it[4].split("=")[1].toInt() }
                val neighbors = line.split(", ").toMutableList()
                neighbors[0] = neighbors[0].takeLast(2)
                return SingleValve(name, rate, neighbors)
            }
        }
    }

    // defining INF as 9999 in this class
    companion object {
        private const val INF = 9999
    }
}

