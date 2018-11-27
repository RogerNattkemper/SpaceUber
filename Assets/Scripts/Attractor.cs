using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    // THe gravitational constant
    const float G = 6.674f;
    // Static list of all attractors in the scene (Static means there's only one of these lists)
    public static List<Attractor> Attractors;
    // Reference to the rigidbody of THIS attractor
    public Rigidbody rb;

    private void FixedUpdate()
    {
        // This originally has ALL attractors attracting all attractors
        // but to streamline things, everything only calculates attraction to the planet and ship
        foreach (Attractor a in Attractors)
        {
            if (a.gameObject.name == "Space Fighter 2 Friendly" || a.gameObject.name == "Dead Planet") Attract(a);
        }
    }

    private void OnEnable()
    {
        // If the attractor list doesn't exist, then create a new attractor list
        if (Attractors == null) Attractors = new List<Attractor>();

        // this attractor adds itself to the list
        Attractors.Add(this);
        // Set the reference to its own rigidbody
        rb = GetComponent<Rigidbody>();

    }

    private void OnDisable()
    {
        // If this attractor is destroyed or disabled remove it from the attractor list
        Attractors.Remove(this);

    }

    void Attract(Attractor objToAttract)
    {
        // Get reference to target attractor's rigidbody
        Rigidbody rbToAttract = objToAttract.rb;

        // Get the direction to the target
        Vector3 direction = rb.position - rbToAttract.position;

        // Determine the distance from the target
        float distance = direction.magnitude;

        if (distance == 0) return; // If looking at self, ignore

        // Calculate the force of attraction
        float forceMagnitude = G * (rb.mass + rbToAttract.mass) / Mathf.Pow(distance, 2);
        // Calculate the direction the force needs to be applied and multiply by the magnitude
        Vector3 force = direction.normalized * forceMagnitude;

        // Push the target towards this attractor
        rbToAttract.AddForce(force);
    }
}
