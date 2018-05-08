using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMass: MonoBehaviour {

    public List<Spring> attatchedSprings=new List<Spring>();
    public float mass = 2f;
    public float drag = 0.5f;
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 totalForce;
    public Vector3 position;
    public Vector3 impulse;

	void Start ()
    {
        velocity = acceleration = totalForce = impulse= Vector3.zero;
	}
	
	void FixedUpdate ()
    {
        CalcForces();
        ApplyForce();
	}


    void ApplyForce()
    {
        acceleration = totalForce/mass;
        velocity += acceleration * Time.fixedDeltaTime + impulse/mass;
        position += velocity * Time.fixedDeltaTime + acceleration * 0.5f * Mathf.Pow(Time.fixedDeltaTime, 2);
        ResetForces();
        
    }

    void CalcForces()
    {
       
       Springs();
        Gravity();
        Drag();
    }

    private void Gravity()
    {
        totalForce += new Vector3(0.0f,-9.81f);
    }

    private void Springs()
    {
        
        foreach(Spring spring in attatchedSprings)
        {
            spring.GetForce(this);
        }

    }

    private void Drag()
    {
        totalForce += velocity * -drag;
    }

    private void ResetForces()
    {
        totalForce = impulse = Vector3.zero;
        
    }

}
