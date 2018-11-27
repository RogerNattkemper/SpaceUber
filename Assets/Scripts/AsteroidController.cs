using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;

/// <summary>
///  The Asteroid Manager (In the Scene Manager) instantiates and places asteroids around an orbital focus
///  The Asteroid Controller (attached to each asteroid) controls the initial movement of the asteroid based on
///  the gravitational constant, its own mass (and mass of the orbital focus) and the distance from the focus
/// </summary>

// This pulls in these scripts to this object, and makes them required for Asteroid Controller to work
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(DemoTrackable))]
[RequireComponent(typeof(Attractor))]
public class AsteroidController : MonoBehaviour
{
    // This is the velocity the asteroid is sent around the orbital focus
    public float velocity = 0;
    // This is the calculated direction, a tangential from the orbital focus
    public Vector3 direction;

    // A reference to the asteroid's Rigidbody
    Rigidbody rb;
    // A reference to the asteroid's MeshCollider (just used to make sure it is set up correctly)
    MeshCollider mc;

    float spinMult; // An adjustment variable to give the asteroid an appropriate spin speed
    float speedMult; // An adjustment variable to give the asteroid an appropriate velocity

    // A reference to the Asteroid Manager (for the orbital focus and gravitational constant)
    AsteroidManager Manager;

    // Use this for initialization
    void Start ()
    {
        // Get the Asteroid Manager using the path method
        Manager = GameObject.Find("LevelScripts/Asteroid Manager").GetComponent<AsteroidManager>();
        // Setting the reference to the asteroids own mesh collider
        mc = GetComponent<MeshCollider>();
        mc.convex = true;

        // Setting the reference to the asteroid's own Rigidbody
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Turn off gravity
        rb.drag = 0;  // Set to no drag (drag would slow and eventually stop movement)
        rb.angularDrag = 0;  // Set to no drag (drag would slow and eventually stop spin)

        // There are three sizes of asteroids, all set with the same mass, spinmult, speedmult and scale within their category
        Transform t = this.transform;
        if (this.gameObject.name.Contains("Huge"))
        {
            rb.mass = 6400;
            spinMult = rb.mass * Random.Range(2000, 8000);
            speedMult = 1.2f;
            t.localScale = new Vector3(20, 20, 20);
        }
        else if (this.gameObject.name.Contains("Medium"))
        {
            rb.mass = 800;
            spinMult = rb.mass * Random.Range(500, 2500);
            speedMult =1;
            t.localScale = new Vector3(10, 10, 10);
        }
        else 
        {
            rb.mass = 100;
            spinMult = rb.mass * Random.Range(112, 800);
            speedMult = 0.2f;
            t.localScale = new Vector3(5, 5, 5);
        }

        // This Adds the necessary force to the asteroid for it to be in a stable orbit at the placed distance
        rb.AddForce(CalculateVelocity());

        // GIves asteroid a random spin
        rb.AddRelativeTorque(new Vector3(RandSpinDir(), RandSpinDir(), RandSpinDir()));
    }

    
    float RandSpinDir()
    {
        // Random.value returns a random float value from 0 - 1, I am using it here to basically flip a coin about spin direction 
        int dir = (Random.value < 0.5) ? 1 : -1;
        return Random.value * dir * spinMult;
    }

    Vector3 CalculateVelocity()
    {
        // Velocity = Square Root (G * (Mainbody Mass + Asteroid Mass)/orbit radius)
        // IF there is an orbital focus (If not then the asteroid will be given no thrust)
        if (Manager.OrbitalFocus != null)
        {
            // Get distance (radius) to the Orbital Focus
            direction = rb.position - Manager.OrbitalFocus.transform.position;
            float distance = direction.magnitude;

            // Get Cross product of the result, zero out Y (This is a right hand rule for vectors)
            Vector3 asteroidDirection = Vector3.Cross(direction, Vector3.up);

            // Velocity = Square Root (G * (Mainbody Mass + Asteroid Mass)/orbit radius)
            velocity = Mathf.Sqrt(Manager.G * (rb.mass + Manager.OrbitalFocus.mass) / distance) * speedMult;

            return (asteroidDirection * velocity);

        }
        
        return Vector3.zero;
    }


}
