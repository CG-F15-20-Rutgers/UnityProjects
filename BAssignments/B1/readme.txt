CG-F15-20-Rutgers
b1 - Unity: Navigation & Animation
https://github.com/CG-F15-20-Rutgers/UnityProjects

Blog: https://medium.com/@CG.F15.20.Rutgers
Post: https://medium.com/@CG.F15.20.Rutgers/unity-navigation-and-animation-f650be55fc5a

Documentation:
B1: Navigation and Animation
This is a fun project meant meant as an introduction to Unity's navigation and animation system. We leverage these systems in order to build a simple crowd simulator, where you control humanoid robots in a complex environment. You can even move obstacles in and out of a robot's way and their path will be updated accordingly.    


There are 3 components to this project: 

   * Part 1: Navigate capsules + move obstacles.
      * Features:
         * Using Unity's NavMeshAgent system, we made it possible to move agents by clicking on them and then clicking on a surface where you want them to go.
      * Controls:
         * WASD + Q/E to control the camera. Arrow keys to move selected obstacles.
         * Select and deselect agents/obstacles by clicking on them
            1. Move selected agents by clicking on where you want them to go
            2. Move selected obstacles via the arrow keys

   * Part 2: Walk, Run, and Jump around as a humanoid robot.
      * Features:
         * Using Unity's Animation system, we made a player controlled humanoid robot. He has animations for walking (forward and backward), running, and jumping.
      * Controls:
         * Move via WASD key
         * Run with SHIFT
         * SPACE to jump (only works if you are moving forward)

   * Part 3: Navigate humanoid robots around the map
      * Features:
         * Combining the two features above, we have a group of robots that are able to navigate around the world in a natural way, with smooth animations.
		 * As an added bonus, when moving robots around the scene, holding shift while clicking will make the robots run. Clicking without shift will still move
		   the robots, albeit at a slower pace.


Extra Credit Attempts:
1. We've integrated the 3 scenes (all 3 parts) into one single demo with a game menu
allowing users to navigate between part 1, 2, and 3.
2. Our character can walk backwards (press the S key in part 2)
3. Our character uses a backflip animation for jumping in Part 2. 


Answers to Questions:
8. When carving is used, the area taken up by the obstacle will be removed (carved out) of the
mesh, meaning the agent won't be able to travel there - it will plan its path around it. Otherwise,
if carving isn't used, the agent will ignore the obstacle when plotting a path and will just try to
side-step it when moving along the path (like steering). Whether or not you use carving depends on
your use-case; steering might be more effective or more realistic for crowd simulations. For the
optimal route though, carving should be used. Steering might also be better for obstacles that move
around.

  a. If all obstacles are carved, the agent will plan its path around all of them in advance. It
  will find the optimal path that does not touch an obstacle.

  b. If all obstacles are not carved, then the agent may get stuck in an area full of obstacles,
  as it might try to steer away from them as it heads towards its path and get stuck in a local loop
  of sorts.

9. One possible implementation would be to use goal directed collision avoidance and/or slope
fields, as mentioned in class.

