Part 1: Flocking
- For this part, I created a group of flocking objects that would generally revolve around a center bird.
- The center bird can be controlled via WASD
- The birds are all dependent on the positions and velocities of the birds. However, I wanted them to revolve around the center bird most, so I made the weight of the 
center bird 5x the weight of the other birds. This way, the small birds will still be dependent on each other, but the larger scope of the flock would still be controlled by the 
center bird
Part 2: Cone check and Collision Prediction for Obstacle Avoidance
- To avoid the objects using cone check, it checks if there is another object within a cone shape in front of the current object
- If there is it attempts to avoid it based on the current location of the user and the target pathfinding location
- Otherwise it attempts to pathfind to a location relative to the center bird in each group
- For collision prediction, I set up the map so it would be very easy to predict the collision location (at y = 0 and different x's)
- Then the user would simply strafe around that location. If it is to the bottom, it strafes right of the point, if it is at the top it strafes left
- Collision prediction is much more efficient since the birds can move much quicker through the map compared to
Part 3: Raycast obstacle avoidance
- To create the map, I had to create the sprites in a different program and then bring them into this project
- I decided to use 3 rays for this project for a few reasons.
	- The collision and distance of the 3 rays would be a much better detection than 1 or 2 rays.
	- Having a seperate center ray can tell the object if it's about to crash into another object and will severely steer the object to prevent this
	- Avoid dealing with the corner trap, since the center ray would tell me if I am approaching a corner (since all 3 would be lit up) and biases to a left turn