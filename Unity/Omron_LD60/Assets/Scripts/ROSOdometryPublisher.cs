using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Nav;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using Unity.Robotics.Core;


/// <summary>
// Class to publish nav_msgs/Odometry messages
// For message documentation
// refer: https://github.com/Unity-Technologies/ROS-TCP-Connector/blob/main/com.unity.robotics.ros-tcp-connector/Runtime/Messages/Nav/msg/OdometryMsg.cs
/// </summary>
public class ROSOdometryPublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "odom";
    public string odomFrameName = "odom";
    public GameObject robotBaseLink;
    public float publishMessageFrequency = 10f;
    private float timeElapsed = 0;
    private uint msgNumber = 0;

    void Start()
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

            msg.pose.pose.position.x = robotBaseLink.transform.position.x;
            msg.pose.pose.position.y = robotBaseLink.transform.position.y;
            msg.pose.pose.position.z = robotBaseLink.transform.position.z;

            msg.pose.pose.orientation.x = robotBaseLink.transform.rotation.x;
            msg.pose.pose.orientation.y = robotBaseLink.transform.rotation.y;
            msg.pose.pose.orientation.z = robotBaseLink.transform.rotation.z;
            msg.pose.pose.orientation.w = robotBaseLink.transform.rotation.w;

            ros.Publish(topicName, msg);
            timeElapsed = 0;
            msgNumber += 1;
        }
    }
}