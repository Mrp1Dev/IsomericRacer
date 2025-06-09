using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

[RequireComponent(typeof(Rigidbody), typeof(CarInput))]
public class CarController : MonoBehaviour
{
    private new Rigidbody rigidbody;
    [Header("Car Parameters")]
    [SerializeField] private float mass = 1.0f;
    [SerializeField] private float acceleration = 20.0f;
    [SerializeField] private float topSpeed = 30.0f;
    [SerializeField] private float lateralDragFactor = 1.0f;
    [SerializeField] private float dragExponent = 2.0f;
    //[SerializeField, Tooltip("Degrees")] private float angularAcceleration;
    //[SerializeField, Tooltip("Degrees per second")] private float angularTopSpeed;
    //[SerializeField] private float turningRadius;

    [SerializeField, Tooltip("Distance between back and front tires.")] private float wheelBase = 1.0f;
    [SerializeField] private float steeringAngleDeg = 15.0f;
    [SerializeField] private float torqueStrength = 1.0f;
    [SerializeField] private float torqueDragFactor = 1.0f;

    [Header("Effects")]
    [SerializeField] private Transform[] wheels;
    [SerializeField] private float wheelRotationMultiplier = 1.0f;
    private CarInput playerInput;
    private Vector2 input;

    public float MaxSpeed => topSpeed;
    private void OnValidate()
    {
        GetComponent<Rigidbody>().mass = mass;
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<CarInput>();
        rigidbody.linearDamping = 0.0f;
        rigidbody.angularDamping = 0.0f;
    }

    private void FixedUpdate()
    {
        input = new Vector2(playerInput.HorizontalAxis, playerInput.VerticalAxis);

        //at top speed equillibrium, engineForce == dragForce
        Vector3 v = rigidbody.linearVelocity;

        float steeringAngle = steeringAngleDeg * Mathf.Deg2Rad * input.x;
        Vector3 lateralForceDirection = Quaternion.AngleAxis(steeringAngle * Mathf.Rad2Deg, transform.up) * transform.right;
        Vector3 forceDirection = GetSteeringForward();

        //assume mass 1 and just use forcemode at the end.
        Vector3 engineForce = forceDirection * acceleration * input.y;
        Vector3 lateralVelocity = Vector3.Project(v, lateralForceDirection);
        Vector3 dragForce = -1.0f * (acceleration / (Mathf.Pow(topSpeed, dragExponent)) * (v.normalized * Mathf.Pow(v.magnitude, dragExponent)));
        Vector3 lateralDragForce = -1.0f * lateralDragFactor * (lateralVelocity.normalized * Mathf.Pow(lateralVelocity.magnitude, dragExponent));

        rigidbody.AddForce(engineForce, ForceMode.Acceleration);
        rigidbody.AddForce(dragForce + lateralDragForce, ForceMode.Acceleration);

        float turningRadius = wheelBase / Mathf.Tan(steeringAngle); //tan theta = L/R
        float forwardVelocity = Vector3.Project(v, forceDirection).magnitude;
        float targetAngularVelocity = forwardVelocity / turningRadius;
        rigidbody.AddTorque(transform.up * (targetAngularVelocity - rigidbody.angularVelocity.y) * torqueStrength, ForceMode.Acceleration);
        rigidbody.AddTorque(-1.0f * rigidbody.angularVelocity * torqueDragFactor, ForceMode.Acceleration);

        //effects
        foreach(var wheel in wheels)
        {
            wheel.localRotation = Quaternion.AngleAxis(steeringAngle * wheelRotationMultiplier * Mathf.Rad2Deg, Vector3.up);
        }
    }

    public Vector3 GetSteeringForward()
    {
        float steeringAngle = steeringAngleDeg * Mathf.Deg2Rad * input.x;
        return Quaternion.AngleAxis(steeringAngle * Mathf.Rad2Deg, transform.up) * transform.forward;
    }

    public float GetForwardSpeed()
    {
        return Vector3.Project(GetComponent<Rigidbody>().linearVelocity, GetSteeringForward()).magnitude;
    }
}
