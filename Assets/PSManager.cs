using UnityEngine;

/**
 * Manager class for all particle system related tasks
 */
public class PSManager : MonoBehaviour
{
    // Snare related
    private ParticleSystem snarePS;
    private ParticleSystemForceField snarePSF;
    private bool snareTimerStarted = false;
    private float snareTimer = 0f;
    private ParticleSystem.MinMaxCurve originalGravity;
    private ParticleSystem.MinMaxCurve originalAttraction;
    public float snareTimeInSeconds;

    void Start()
    {
        snarePS = GameObject.Find("Snare PS").GetComponent<ParticleSystem>();
        snarePSF = GameObject.Find("Snare PSF").GetComponent<ParticleSystemForceField>();
        originalGravity = snarePSF.gravity;
        originalAttraction = snarePSF.rotationAttraction;
    }

    void Update()
    {
        snare();
    }

    public void triggerSnare()
    {
        if (!snareTimerStarted) snareTimerStarted = true;
        // Set gravity to low
        snarePSF.gravity = new ParticleSystem.MinMaxCurve(0f);
        snarePSF.rotationAttraction = new ParticleSystem.MinMaxCurve(0f);
        changeParticleVelocity(snarePS, 1.5f);
    }

    void snare()
    {
        if (snareTimerStarted)
        {
            snareTimer += Time.deltaTime;

            if (snareTimer >= snareTimeInSeconds)
            {
                // Reset timer
                snareTimer = 0f;
                snareTimerStarted = false;
                snarePSF.gravity = originalGravity;
                snarePSF.rotationAttraction = originalAttraction;
                changeParticleVelocity(snarePS, 1f/1.5f);
            } 
            //else
            //{
            //    snareForceField.gravity = map(snareTimer, 0f, snareTimeInSeconds, 0f, originalGravity.constant);
            //}
        }
    }

    // Changes the velocities of all the particles in a given particle system by multiplying it by the given velocityRatio
    void changeParticleVelocity(ParticleSystem ps, float velocityRatio)
    {
        int particleCount = ps.particleCount;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
        ps.GetParticles(particles);

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].velocity *= velocityRatio;
        }

        // Apply changed velocities
        ps.SetParticles(particles, particleCount);
    }

    private static float map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }
}
