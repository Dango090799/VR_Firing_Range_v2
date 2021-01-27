using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePhysics : MonoBehaviour
{
    public enum TurnDirection { RightHand, LeftHand };

    public bool Gravity, Drag, SpinDrift, CoriolisEffect;
    public float projectileTipArea, projectileWeight, projectileCaliber, projectileLength, riflingTwist, muzzleVelocity, ballisticCoefficient, currentLatitude;
    public TurnDirection turnDirection;

    private Rigidbody rb;
    private readonly float G = -9.81f;
    private readonly double earthAngularVel = 0.000072921159;
    private float airDens, ToF;

    private int Calcs { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        airDens = Random.Range(1.2f, 1.27f);
        ToF = 0;
    }

    private void Update()
    {
        ToF += Time.deltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Gravity)
        {
            EnactGravity();
        }if (Drag)
        {
            EnactDrag();
        }if (SpinDrift)
        {
            EnactSpinDrift();
        }if (CoriolisEffect)
        {
            EnactCoriolisEffect();
        }
        //print($"Calculations so far: {Calcs}");
    }

    void EnactGravity()
    {
        // Add gravitational constant force onto bullet trajectory
        rb.AddForce(new Vector3(0, G, 0));
        Calcs++; // increment calculation
    }

    void EnactDrag()
    {
        Vector3 curVel = rb.velocity;
        if (curVel.magnitude == 0)
        {
            return;
        }Calcs++; //Increment amount of calculations

        // Adds air resistance force to bullet trajectory
        double dragVec = curVel.magnitude*((airDens * ballisticCoefficient * projectileTipArea) / 2); // air resistance formula
        Calcs++; //Increment amount of calculations

        // Determine X direction
        if (curVel.x < 0)
        {
            rb.AddForce(new Vector3((float)dragVec, 0, 0));
        }else
        {
            rb.AddForce(new Vector3((float)-dragVec, 0, 0));
        }Calcs++; //Increment amount of calculations

        //Determine Y direction
        if (curVel.y < 0)
        {
            rb.AddForce(new Vector3(0, (float)dragVec, 0));
        }
        else
        {
            rb.AddForce(new Vector3(0, (float)-dragVec, 0));
        }Calcs++; //Increment amount of calculations

        //Determine Z direction
        if (curVel.z < 0)
        {
            rb.AddForce(new Vector3(0, 0, (float)dragVec));
        }
        else
        {
            rb.AddForce(new Vector3(0, 0, (float)-dragVec));
        }Calcs++; //Increment amount of calculations

    }

    void EnactSpinDrift()
    {
        double d1 = ((8 * Mathf.PI) / (airDens * Mathf.Pow(riflingTwist, 2) * Mathf.Pow(projectileCaliber, 5) * ballisticCoefficient));
        double d2 = (1 / 12) * projectileWeight * projectileLength;

        double Sg = d1 * d2;
        Calcs++;

        double SpinDrift = 1.25f * (Sg + 1.2f) * Mathf.Pow(ToF, 1.83f);

        if (turnDirection == TurnDirection.RightHand)
        {
            rb.AddRelativeTorque(new Vector3(0, 0, (float)-SpinDrift));
        }
        else if (turnDirection == TurnDirection.LeftHand)
        {
            rb.AddRelativeTorque(new Vector3(0, 0, (float)SpinDrift));
        }
        Calcs++;
    }

    void EnactCoriolisEffect()
    {
        if(currentLatitude == 0)
        {
            return;
        }

        Vector3 Fc = 2 * rb.velocity * (float)earthAngularVel * currentLatitude;
        if(currentLatitude > 0)
        {
            rb.AddRelativeForce(Fc);
        }else if(currentLatitude < 0)
        {
            rb.AddRelativeForce(-Fc);
        }
    }
}
