using UnityEngine;

/**
 * Manager class for all particle system related tasks. This class is meant to manage particle systems as well as their
 * force fields
 */
public class PSManager : MonoBehaviour
{
    // Snare related particle system
    private ParticleSystem snarePS;
    // Snare related particle force field
    private ParticleSystemForceField snarePSF;

    // Fields for timing events
    private bool snareTimerStarted = false;
    private float snareTimer = 0f;
    // The original gravity of the snare force field
    private ParticleSystem.MinMaxCurve originalGravity;
    // The original attraction value of the snare force field
    private ParticleSystem.MinMaxCurve originalAttraction;
    // How long the gravity should be reduced upon a snare hit
    public float snareTimeInSeconds;

    void Start()
    {
        // Assign the snare particle system
        snarePS = GameObject.Find("Snare PS").GetComponent<ParticleSystem>();
        // Assign the snare particle system force field
        snarePSF = GameObject.Find("Snare PSF").GetComponent<ParticleSystemForceField>();
        // Store original values for resetting later
        originalGravity = snarePSF.gravity;
        originalAttraction = snarePSF.rotationAttraction;
    }

    void Update()
    {
        snare();
    }

    // Manages a snare hit. This sets the gravity of the force field to 0,
    // resulting in a explosion-like effect pushing the particles outwards.
    // The effect is increased by increasing the particles' velocities.
    public void triggerSnare()
    {
        if (!snareTimerStarted) snareTimerStarted = true;
        // Set gravity to low
        snarePSF.gravity = new ParticleSystem.MinMaxCurve(0f);
        snarePSF.rotationAttraction = new ParticleSystem.MinMaxCurve(0f);
        changeParticleVelocity(snarePS, 1.5f);
    }

    // In this case this method is only responsible for resetting the particle system force field values
    // After the specified effect time.
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
        }
    }

    // Changes the velocities of all the particles in a given particle system by multiplying it by the given velocityRatio
    void changeParticleVelocity(ParticleSystem ps, float velocityRatio)
    {
        // Get the total number of particles currently in the system
        int particleCount = ps.particleCount;
        // Create a new particle system for the changed values
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
        // Assign the newly generated particles
        ps.GetParticles(particles);

        // Increase/decrease the velocities
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
