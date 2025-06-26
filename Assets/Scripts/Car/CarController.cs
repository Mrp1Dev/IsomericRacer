using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class BoostData
{
    public StatModifierSO accelerationModifier;
    public StatModifierSO topSpeedModifier;
    public float duration = 1.0f;
    public float cooldown = 1.0f;
}

[RequireComponent(typeof(Rigidbody), typeof(CarInput))]
public class CarController : MonoBehaviour
{
    private new Rigidbody rigidbody;
    [Header("Car Parameters")]
    [SerializeField] private float mass = 1.0f;
    [SerializeField] private Stat acceleration = new Stat { realValue = 20.0f };
    [SerializeField] private Stat topSpeed = new Stat { realValue = 50.0f };
    [SerializeField] private float lateralDragFactor = 1.0f;
    [SerializeField] private float dragExponent = 2.0f;
    //[SerializeField, Tooltip("Degrees")] private float angularAcceleration;
    //[SerializeField, Tooltip("Degrees per second")] private float angularTopSpeed;
    //[SerializeField] private float turningRadius;

    [SerializeField, Tooltip("Distance between back and front tires.")] private float wheelBase = 1.0f;
    [SerializeField] private float steeringAngleDeg = 15.0f;
    [SerializeField] private float torqueStrength = 1.0f;
    [SerializeField] private float torqueDragFactor = 1.0f;
    [SerializeField] private bool invertInReverse;


    [Header("Boost")]
    [SerializeField] private BoostData boost = new BoostData();
    [SerializeField] private ParticleSystem boostParticle;
    private bool isBoosting;
    private float boostTimer;
    private float boostCooldownTimer;

    [Header("Effects")]
    [SerializeField] private Transform[] wheels;
    [SerializeField] private float wheelRotationMultiplier = 1.0f;


    private CarInput playerInput;
    private Vector2 input;

    public float MaxSpeed => topSpeed.GetValue();
    private void OnValidate()
    {
        GetComponent<Rigidbody>().mass = mass;
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<CarInput>();
        playerInput.BoostPressed += OnBoostInput;
        rigidbody.linearDamping = 0.0f;
        rigidbody.angularDamping = 0.0f;
    }

    private void FixedUpdate()
    {
        input = new Vector2(playerInput.HorizontalAxis, playerInput.VerticalAxis);
        /*var input3D = new Vector3(playerInput.HorizontalAxis, 0.0f, 0.0f);
        input3D = transform.InverseTransformDirection(input3D);
        if (input3D.x <= 0.001f) input.x = 0.0f;
        else input.x = Mathf.Sign(input3D.x);*/

        /*Vector2 input = new Vector2(playerInput.HorizontalAxis, playerInput.VerticalAxis);

        // Forward/backward input stays local (Z axis of car)
        float forward = input.y;

        // Get car's right direction in world space (XZ only)
        Vector3 carRight = transform.right;
        carRight.y = 0;
        carRight.Normalize();

        // Get the input direction in world space (XZ only)
        Vector3 inputDir = new Vector3(input.x, 0, 0.0f);
        //inputDir.Normalize();

        // Project input onto car's right to get steering value
        float steering = Vector3.Dot(inputDir, carRight);

        // Now you have:
        input = new Vector2(steering, forward);*/



        //at top speed equillibrium, engineForce == dragForce
        Vector3 v = rigidbody.linearVelocity;

        float steeringAngle = steeringAngleDeg * Mathf.Deg2Rad * input.x;
        Vector3 lateralForceDirection = Quaternion.AngleAxis(steeringAngle * Mathf.Rad2Deg, transform.up) * transform.right;
        Vector3 forceDirection = GetSteeringForward();

        //assume mass 1 and just use forcemode at the end.
        Vector3 engineForce = forceDirection * acceleration.GetValue() * input.y;
        Vector3 lateralVelocity = Vector3.Project(v, lateralForceDirection);
        Vector3 dragForce = -1.0f * (acceleration.GetValue() / (Mathf.Pow(topSpeed.GetValue(), dragExponent)) * (v.normalized * Mathf.Pow(v.magnitude, dragExponent)));
        Vector3 lateralDragForce = -1.0f * lateralDragFactor * (lateralVelocity.normalized * Mathf.Pow(lateralVelocity.magnitude, dragExponent));

        rigidbody.AddForce(engineForce, ForceMode.Acceleration);
        rigidbody.AddForce(dragForce + lateralDragForce, ForceMode.Acceleration);

        float turningRadius = wheelBase / Mathf.Tan(steeringAngle); //tan theta = L/R
        float forwardVelocity = Vector3.Project(v, forceDirection).magnitude;
        float multiplier = Mathf.Sign(Vector3.Dot(rigidbody.linearVelocity.normalized, GetSteeringForward()));
        float targetAngularVelocity = forwardVelocity / turningRadius * (invertInReverse ? multiplier : 1.0f);
        rigidbody.AddTorque(transform.up * (targetAngularVelocity - rigidbody.angularVelocity.y) * torqueStrength, ForceMode.Acceleration);
        rigidbody.AddTorque(-1.0f * rigidbody.angularVelocity * torqueDragFactor, ForceMode.Acceleration);

        //effects
        foreach (var wheel in wheels)
        {
            wheel.localRotation = Quaternion.AngleAxis(steeringAngle * wheelRotationMultiplier * Mathf.Rad2Deg, Vector3.up);
        }
    }

    private void Update()
    {
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0.0f)
            {
                isBoosting = false;
                boostTimer = 0.0f;
                acceleration.RemoveModifier(boost.accelerationModifier);
                topSpeed.RemoveModifier(boost.topSpeedModifier);
                boostParticle.Stop();
                boostCooldownTimer = boost.cooldown;
            }
        }
        if (boostCooldownTimer > 0.0f) { boostCooldownTimer -= Time.deltaTime; }
    }

    private void OnBoostInput()
    {
        if (isBoosting) return;
        if (boostCooldownTimer > Mathf.Epsilon) return;
        isBoosting = true;
        boostTimer = boost.duration;
        acceleration.AddModifier(boost.accelerationModifier);
        topSpeed.AddModifier(boost.topSpeedModifier);
        boostParticle.Play();
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

    private void OnDestroy()
    {
        playerInput.BoostPressed -= OnBoostInput;
    }
}
