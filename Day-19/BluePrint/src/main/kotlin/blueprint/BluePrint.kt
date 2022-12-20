package blueprint

import java.io.File
import kotlin.math.max

class BluePrint {
    private val path = "src/main/resources/input.txt"
    private val input = File(path).readLines()

    // for each line finds all the numbers, converts them to ints, and puts them in a list
    private val allBluePrintsRecipes =
        input.map { Regex("\\d+").findAll(it).map { char -> char.value.toInt() }.toList() }

    // creates a blueprint for each line from the input
    private val allBluePrints = allBluePrintsRecipes.map { SingleBluePrint.from(it) }
    private var maxScoreAdded = (0..40).map { (0..it).sum() }.toList()

    fun solve(): Pair<Int, Long> {
        val part1 = part1()
        val part2 = part2()
        return Pair(part1, part2)
    }

    private fun part1(): Int {
        // sum of the best score for each blueprint
        val result = allBluePrints.sumOf {
            // initial state with elapsed time as 1, 0 resources for each, 1 ore robot
            val initialState = State(1, mutableListOf(0, 0, 0, 0), mutableListOf(1, 0, 0, 0))
            // best score possible when starting in initial state
            val bestScore: Int = findMaxGeodes(it, initialState, 24)
            // best score times the index for the blueprint
            it.id * bestScore
        }
        return result
    }

    private fun part2(): Long {
        val result = allBluePrints.take(3).map {
            val initialState = State(1, mutableListOf(0, 0, 0, 0), mutableListOf(1, 0, 0, 0))
            val bestScore = findMaxGeodes(it, initialState, 32)
            bestScore.toLong()
        }.reduce{ a, b -> a * b }
        return result
    }

    // implements depth first search, pruning a branch when we cannot do better than our current best
    private fun findMaxGeodes(blueprint: SingleBluePrint, state: State, timeLimit: Int): Int {
        // holds the current best number of geodes
        var numGeodes = 0

        // finds the robot that requires the max number of ore
        val maxOreRobotCost = listOf(
            blueprint.oreRobotCost,
            blueprint.clayRobotCost,
            blueprint.obsidianRobotCost.first,
            blueprint.geodeRobotCost.first
        ).max()
        // only the obsidian robot requires clay
        val maxClayRobotCost = blueprint.obsidianRobotCost.second
        // only the geode robot requires obsidian
        val maxObsidianRobotCost = blueprint.geodeRobotCost.second

        fun findBest(state: State): Int {

            // how many turns until we can make a resource
            fun turnsToWait(remaining: Int, numRobots: Int): Int {
                // we have enough resources to make the robot
                if (remaining <= 0) {
                    return 0
                }
                // how many resources needed, divided by the number of robots that make the resource
                val fullTurns = remaining / numRobots
                // add the remainder (e.g. 3 resources needed and 2 robots will take 2 turns)
                return fullTurns + if (remaining % numRobots == 0) 0 else 1
            }

            // how many minutes we have remaining
            val turnsRemaining = timeLimit - state.time
            // current number of geodes created
            val currentGeodes = state.resourceCount[3]
            // the number of geodes the current geode robots can get for the rest of the round
            val addedGeodesFromCurrentRobots: Int = state.robotCount[3] * (turnsRemaining + 1)

            val maxAddedGeodesFromFutureRobots: Int = maxScoreAdded[turnsRemaining]
            // if the current geodes + geodes from current robots + possible future geodes is less
            // than our best possible so far, return 0 (stop now if we can't do better than our current best)
            if (currentGeodes + addedGeodesFromCurrentRobots + maxAddedGeodesFromFutureRobots < numGeodes - 1) {
                return 0
            }

            // we have reached our time limit
            if (state.time == timeLimit) {
                // final number of geodes for this branch
                val score = state.resourceCount[3] + state.robotCount[3]
                // if the number of geodes for this branch is better than the current best number of geodes
                if (score > numGeodes) {
                    // update number of geodes
                    numGeodes = score
                }
                return score
            }

            // keeps track of the best score in each state
            val resultList = mutableListOf<Int>()

            // check if we have enough ore robots to get ore
            if (state.robotCount[0] < maxOreRobotCost) {
                // how many turns until we can get the ore robot
                val oreTurns = turnsToWait(blueprint.oreRobotCost - state.resourceCount[0], state.robotCount[0])
                val turns = 1 + oreTurns

                // check we are still below time limit after all the turns required
                if (turns + state.time <= timeLimit) {
                    // how many of each resource we will have after all turns
                    val resources =
                        state.resourceCount.zip(state.robotCount).map { it.first + (it.second * turns) }.toMutableList()
                    // create an ore robot (current number of ore minus ore cost)
                    resources[0] -= blueprint.oreRobotCost
                    // increase the number of ore robots by 1
                    val robots = state.robotCount.toMutableList()
                    robots[0]++

                    // call findBest from new state and adds most geodes we can get from this state
                    resultList.add(findBest(State(state.time + turns, resources, robots)))
                }
            }

            // check if we have obsidian robots
            if (state.robotCount[2] > 0) {
                // find how many turns we need to wait for ores for geode robot
                val oreTurns = turnsToWait(blueprint.geodeRobotCost.first - state.resourceCount[0], state.robotCount[0])
                // find how many turns we need to wait for obsidian for geode robot
                val obsidianTurns =
                    turnsToWait(blueprint.geodeRobotCost.second - state.resourceCount[2], state.robotCount[2])
                // max number of turns needed for geode
                val turns = 1 + max(oreTurns, obsidianTurns)

                // make sure we are in time limit after turns
                if (turns + state.time <= timeLimit) {
                    // update resources after turns have completed
                    val resources =
                        state.resourceCount.zip(state.robotCount).map { it.first + (it.second * turns) }.toMutableList()
                    // create geode robot
                    resources[0] -= blueprint.geodeRobotCost.first
                    resources[2] -= blueprint.geodeRobotCost.second
                    val robots = state.robotCount.toMutableList()
                    robots[3]++

                    // call findBest from new state and add the most geodes we can get from the state
                    resultList.add(findBest(State(state.time + turns, resources, robots)))
                }
            }

            // if we have clay but not enough obsidian robots to get geodes
            if (state.resourceCount[1] > 0 && state.robotCount[2] < maxObsidianRobotCost) {
                // find how many turns needed to get ore for obsidian robots
                val oreTurns =
                    turnsToWait(blueprint.obsidianRobotCost.first - state.resourceCount[0], state.robotCount[0])
                // find how many turns needed to get clay for obsidian robots
                val clayTurns =
                    turnsToWait(blueprint.obsidianRobotCost.second - state.resourceCount[1], state.robotCount[1])
                val turns = 1 + max(oreTurns, clayTurns)

                // check that are under time limit after turns
                if (turns + state.time <= timeLimit) {
                    // update resources after turns has completed
                    val resources =
                        state.resourceCount.zip(state.robotCount).map { it.first + (it.second * turns) }.toMutableList()
                    // create an obsidian robot
                    resources[0] -= blueprint.obsidianRobotCost.first
                    resources[1] -= blueprint.obsidianRobotCost.second
                    val robots = state.robotCount.toMutableList()
                    robots[2]++

                    // call findBest from new state and add the most geodes we can get from the state
                    resultList.add(findBest(State(state.time + turns, resources, robots)))
                }
            }

            // check if we have enough clay robots to get obsidian
            if (state.robotCount[1] < maxClayRobotCost) {
                // how many turns to get enough clay
                val oreTurns = turnsToWait(blueprint.clayRobotCost - state.resourceCount[0], state.robotCount[0])
                val turns = 1 + oreTurns

                // make sure we are under time limit after turns
                if (turns + state.time <= timeLimit) {
                    // the number of resources after turns
                    val resources =
                        state.resourceCount.zip(state.robotCount).map { it.first + (it.second * turns) }.toMutableList()
                    // create clay robot
                    resources[0] -= blueprint.clayRobotCost
                    val robots = state.robotCount.toMutableList()
                    robots[1]++

                    // call findBest from new state and add the most geodes we can get from the state
                    resultList.add(findBest(State(state.time + turns, resources, robots)))
                }
            }

            // how much time is remaining in the current state
            val waitRounds = timeLimit - state.time + 1
            // add all the geodes we can create in the remaining time with the geode robots
            val waitScore = state.resourceCount[3] + (state.robotCount[3] * waitRounds)
            // add the number of geodes to the result list
            resultList.add(waitScore)
            // find the best number of geodes out of all the results
            val result = resultList.max()

            // if our result for this branch is better than the current best
            if (result == waitScore && waitScore > numGeodes) {
                numGeodes = result
            }

            return result
        }
        // find the best number of geodes for our initial state
        return findBest(state)
    }

    // class holding the amount of time elapsed, resources available, and number of robots
    data class State(val time: Int, val resourceCount: List<Int>, val robotCount: List<Int>)

    data class SingleBluePrint(
        val id: Int,
        val oreRobotCost: Int,
        val clayRobotCost: Int,
        val obsidianRobotCost: Pair<Int, Int>,
        val geodeRobotCost: Pair<Int, Int>
    ) {
        companion object {
            fun from(values: List<Int>): SingleBluePrint {
                return SingleBluePrint(
                    values[0],
                    values[1],
                    values[2],
                    Pair(values[3], values[4]),
                    Pair(values[5], values[6])
                )
            }
        }
    }
}