using UnityEngine;

public class FloatingBehaviour : MonoBehaviour
{
    public Collider volumeCollider;

    public float neutralY = 0;
    public float minWaveHeight = 0.2f;
    public float maxWaveHeight = 0.5f;
    public float minWaveDuration = 0.5f;
    public float maxWaveDuration = 2;

    private AnimationCurve curve;

    private bool currentWaveDirectionUp;
    private float currentWaveStartY;
    private float currentWaveHeight;
    private float currentWaveStartTime;
    private float currentWaveDuration;

    private void Awake()
    {
        volumeCollider = GetComponentInChildren<Collider>();
        curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    private void OnEnable()
    {
        currentWaveDirectionUp = Random.Range(0, 2) == 1;
        ReverseWave(Time.fixedTime);
    }

    private void FixedUpdate()
    {
        float t = Time.fixedTime - currentWaveStartTime;
        if (t > currentWaveDuration)
        {
            t -= currentWaveDuration;
            ReverseWave(currentWaveStartTime + currentWaveDuration);
        }

        float centerY = currentWaveStartY + (neutralY + currentWaveHeight - currentWaveStartY) * curve.Evaluate(t / currentWaveDuration);
        Vector3 pos = transform.position;
        pos.y = centerY - (GetCenterPosition().y - pos.y);
        transform.position = pos;
    }

    private void ReverseWave(float when)
    {
        currentWaveDirectionUp = !currentWaveDirectionUp;
        currentWaveStartY = GetCenterPosition().y;
        currentWaveHeight = Random.Range(minWaveHeight, maxWaveHeight);
        if (!currentWaveDirectionUp)
        {
            currentWaveHeight = -currentWaveHeight;
        }
        currentWaveStartTime = when;
        currentWaveDuration = Random.Range(minWaveDuration, maxWaveDuration);
    }

    private Vector3 GetCenterPosition()
    {
        return volumeCollider.bounds.center;
    }
}
