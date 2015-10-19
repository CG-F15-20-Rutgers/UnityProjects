CG-F15-20-Rutgers
b1 - Unity: Navigation & Animation
https://github.com/CG-F15-20-Rutgers/UnityProjects

Blog: https://medium.com/@CG.F15.20.Rutgers
Post: <TODO -insert->

Documentation:

Extra Credit Attempts:

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

