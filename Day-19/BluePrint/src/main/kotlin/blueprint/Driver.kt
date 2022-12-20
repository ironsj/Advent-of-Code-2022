package blueprint

fun main(){
    val bluePrint = BluePrint()
    val result = bluePrint.solve()
    val part1 = result.first
    val part2 = result.second
    println("Part 1: $part1")
    println("Part 2: $part2")
}