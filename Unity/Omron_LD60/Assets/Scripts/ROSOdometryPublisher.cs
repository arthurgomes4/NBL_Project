using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Nav;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using Unity.Robotics.Core;


// <summary>
// Class to publish nav_msgs/Odometry messages
// For message documentation
// refer: https://github.com/Unity-Technologies/ROS-TCP-Connector/blob/main/com.unity.robotics.ros-tcp-connector/Runtime/Messages/Nav/msg/OdometryMsg.cs
// </summary>

public class ROSOdometryPublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "odom";
    public string odomFrameName = "odom";
    public GameObject robotBaseLink;
    public float publishMessageFrequency = 10f;
    private uint msgNumber = 0;
    private float prev_x = 0, prev_y = 0, prev_time = 0, prev_theta = 0, timeElapsed = 0;
    private void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<OdometryMsg>(topicName);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > 1f/publishMessageFrequency)
        {
            OdometryMsg msg = new OdometryMsg();
            msg.header = new HeaderMsg(msgNumber, new TimeStamp(Clock.time), odomFrameName);

            msg.child_frame_id = odomFrameName;

            msg.pose.pose.position = robotBaseLink.transform.position.To<FLU>();

            msg.pose.pose.orientation = robotBaseLink.transform.rotation.To<FLU>();

            float theta = robotBaseLink.transform.rotation.eulerAngles.y*Mathf.Deg2Rad;
            msg.twist.twist.angular.z = -(theta - prev_theta)/(Time.time - this.prev_time);
            this.prev_theta = theta;
            
            float x = robotBaseLink.transform.position.z;
            float y = -robotBaseLink.transform.position.x;
            msg.twist.twist.linear.x = ((x-this.prev_x)*Mathf.Cos(theta) - (y-this.prev_y)*Mathf.Sin(theta))/(Time.time - this.prev_time);
            this.prev_x = x;
            this.prev_y = y;
            
            this.prev_time = Time.time;

            ros.Publish(topicName, msg);
            timeElapsed = 0;
            msgNumber += 1;
        }
    }
}