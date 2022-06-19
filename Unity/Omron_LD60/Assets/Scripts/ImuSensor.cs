using System;
using System.Collections.Generic;
using RosMessageTypes.Sensor;
using RosMessageTypes.Sensor;
using RosMessageTypes.Geometry;
using Unity.Robotics.Core;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

[RequireComponent(typeof(Rigidbody))]
public class IMU : MonoBehaviour
{
    private Rigidbody _rb;
    private Transform _trans;

    // Previous value
    private Vector3 _lastVelocity = Vector3.zero;

    private Vector4 _geometryQuaternion;
    private Vector3 _angularVelocity;
    private Vector3 _linearAcceleration;

    [SerializeField] private float _scanRate = 100f;
    public float scanRate { get => this._scanRate; }

    public Vector4 GeometryQuaternion { get => _geometryQuaternion; }
    public Vector3 AngularVelocity { get => _angularVelocity; }
    public Vector3 LinearAcceleration { get => _linearAcceleration; }


    private void Start()
    {
        this._trans = this.GetComponent<Transform>();
        this._rb = this.GetComponent<Rigidbody>();
        this._geometryQuaternion = new Vector4();
        this._angularVelocity = new Vector3();
        this._linearAcceleration = new Vector3();
    }

    public void UpdateIMU()
    {
        // Update Object State //
        // Calculate Move Element
        Vector3 localLinearVelocity = this._trans.InverseTransformDirection(this._rb.velocity);
        Vector3 acceleration = (localLinearVelocity - this._lastVelocity) / Time.deltaTime;
        this._lastVelocity = localLinearVelocity;
        // Add Gravity Element
        acceleration += this._trans.InverseTransformDirection(Physics.gravity);

        // Update //

        // Raw
        this._geometryQuaternion = new Vector4(this._trans.rotation.x, this._trans.rotation.y, this._trans.rotation.z, this._trans.rotation.w);
        this._angularVelocity = -1 * this.transform.InverseTransformVector(this.GetComponent<Rigidbody>().angularVelocity);
        this._linearAcceleration = acceleration;
    }
}

public class ImuPublisher : MonoBehaviour
{
    private string _topicName = "imu/raw_data";
    private string _frameID = "imu_link";

    private float _timeElapsed = 0f;
    private float _timeStamp = 0f;
    
    private ROSConnection _ros;
    public ImuMsg _message;

    private IMU _imu;

    // Start is called before the first frame update
    void Start()
    {
        this._imu = GetComponent<IMU>();

        this._ros = ROSConnection.GetOrCreateInstance();
        this._ros.RegisterPublisher<ImuMsg>(this._topicName);

        this._message = new ImuMsg();
        this._message.header.frame_id = this._frameID;
    }

    // Update is called once per frame
    void Update()
    {
        this._timeElapsed += Time.deltaTime;
                if (this._timeElapsed > (1f / this._imu.scanRate))
        {
            // Update time
            this._timeElapsed = 0;
            this._timeStamp = Time.time;

            // Update IMU data
            this._imu.UpdateIMU();

            // Update ROS Message
            uint sec = (uint)Math.Truncate(this._timeStamp);
            uint nanosec = (uint)((this._timeStamp - sec) * 1e+9);
            this._message.header.stamp.sec = sec;
            this._message.header.stamp.nanosec = nanosec;
            
            QuaternionMsg orientation =
                new QuaternionMsg(this._imu.GeometryQuaternion.x,
                                  this._imu.GeometryQuaternion.y,
                                  this._imu.GeometryQuaternion.z,
                                  this._imu.GeometryQuaternion.w);
            this._message.orientation = orientation;
            
            Vector3Msg angular_velocity =
                new Vector3Msg(this._imu.AngularVelocity.x,
                               this._imu.AngularVelocity.y,
                               this._imu.AngularVelocity.z);
            this._message.angular_velocity = angular_velocity;

            Vector3Msg linear_acceleration =
                new Vector3Msg(this._imu.LinearAcceleration.x,
                               this._imu.LinearAcceleration.y,
                               this._imu.LinearAcceleration.z);
            this._message.linear_acceleration = linear_acceleration;

            this._ros.Publish(this._topicName, this._message);
        }
        
    }
}
