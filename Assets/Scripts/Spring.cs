using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring
{
    float k;
    float currLength;
    float eqLength;
    float dampening;
    Vector3 force;
    Vector3 dir;
    PointMass pmA;
    PointMass pmB;

    public bool isCalled;

    public Spring(PointMass A, PointMass B)
    {
        pmA = A;
        pmB = B;
        eqLength = currLength = Vector3.Distance(A.position, B.position);
        force = Vector3.zero;
        isCalled = false;
    }

    public void SetStats(float newK, float d)
    {
        k = newK;
        dampening = d;
    }

    public void GetForce(PointMass parent)
    {
        if (!isCalled)
        {
            dir = pmA.position - pmB.position;

            if (dir == Vector3.zero)
                return;

            currLength = Vector3.Distance(pmA.position, pmB.position);

            dir.Normalize();

            //Hooke's Law with damping
            force = (-k * (currLength - eqLength) + (-dampening * Vector3.Dot(pmA.velocity - pmB.velocity, dir))) * dir;



            pmA.totalForce += force;
            pmB.totalForce -= force;


            //isCalled = true;
        }
        

        //if (parent == pmA)
            //pmA.totalForce += force;
       // else if(parent==pmB)
            //pmB.totalForce -= force;



    }
}
     
        
 