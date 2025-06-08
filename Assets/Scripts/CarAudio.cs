using UnityEngine;

public class CarAudio : MonoBehaviour
{
    [SerializeField] private float idlePitch;
    [SerializeField] private float maxSpeedPitch;
    [SerializeField] private float idleVolume;
    [SerializeField] private float maxSpeedVolume;
    private AudioSource audioSource;
    private CarController carController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (carController == null) return;
        var t = carController.GetForwardSpeed() / carController.MaxSpeed;
        audioSource.pitch = Mathf.Lerp(idlePitch, maxSpeedPitch, t);
        audioSource.volume = Mathf.Lerp(idleVolume, maxSpeedVolume, t);
    }
}
