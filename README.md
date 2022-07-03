# NBL_Project
Nokia Bell Labs Project repo containing both unity project and required ROS packages

## Branch
This branch contains the full factory exploration scene.

## Usage

1. Clone into local workspace `src`. Build and source.

2. Open the unity project [Omron_LD60](./Unity/Omron_LD60) from the Unity Hub. Using editor version **2020.3.36f1**.

3. Launch the TCP endpoint on the ROS side with `roslaunch ros_tcp_endpoint endpoint.launch`.

4. Press the play button to run the scene in the Unity Editor.

5. Launch Nav stack with `roslaunch omron_ld60_navigation omron_navigation_demo.launch`. This will launch move_base, gmapping, explore_lite. 

6. Open up RVIZ for visualization with `roslaunch omron_ld60_description load_visualization.launch`

## Lidar Specifics
The lidar script is added as a component of the `upper_laser` childObject. All other scripts are components of the `ld60_castors` GameObject.

<p align="left">
<img src="./README_images/hierachy.png">
</p>

The publishing rate can be set in the inspector window.
<p align="left">
<img src="./README_images/lidar.png">
</p>

**Note**: Decreasing publishing period (increasing update rate) is not guaranteed to reflect in simulation. In the event that the actual publishing rate lags behind the set publishing rate this warning will be visible in the console window.

<p align="left">
<img src="./README_images/warning.png">
</p>

## Important params
* In the [exploration launch file](./ROS/omron_ld60_navigation/launch/exploration.launch) the param `min_frontier_size` can be changed depending on environment dimensions.
* In the [costmap params file](./ROS/omron_ld60_navigation/param/move_base/costmap_common.yaml) the params `inflation_radius` and `cost_scaling_factor` can be edited to ensure that the global path is always planned in the centre of passageways and avoids narrow gaps between obstacles. 