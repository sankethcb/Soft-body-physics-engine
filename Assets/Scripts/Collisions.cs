using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Collisions
{

    static Vector2 MTV;

    static List<Vector2> verticesA;
    static List<Vector2> normalsA;

    static List<Vector2> verticesB;
    static List<Vector2> normalsB;
    static float magnitude;
    public static Bounds Floor;



    //Polygon-floor collisions
    public static void FloorCheck(Polygon poly)
    {
        List<PointMass> vertices = poly.vertices;
        foreach (PointMass pm in vertices)
        {
            if (AABB(pm))
            {
                
                FloorDecouple(pm);
                ResolveFloorCollision(pm);

            }
        }

        if(AABB(poly.center))
        {
            FloorDecouple(poly.center);
            ResolveFloorCollision(poly.center);
        }
    }


    private static bool AABB(PointMass pm)
    {
        if (pm.position.x > Floor.min.x &&
            pm.position.x < Floor.max.x &&
            pm.position.y > Floor.min.y &&
            pm.position.y < Floor.max.y)
            return true;

        return false;
    }

    //Preventing tunneling
    private static void FloorDecouple(PointMass pm)
    {
        Vector3 moveDir = -pm.velocity.normalized;

        float distance=0.0f;
        
        //Top left quadrant
        if(pm.position.x<Floor.center.x && pm.position.y>=Floor.center.y)
        {
            if(Mathf.Abs(Floor.min.x-pm.position.x)< Mathf.Abs(Floor.max.y-pm.position.y))
            {
                distance = Mathf.Abs(Floor.min.x - pm.position.x);
            }
            else
            {
                distance = Mathf.Abs(Floor.max.y - pm.position.y);
            }
        }
        //Top right quadrant
        if (pm.position.x >= Floor.center.x && pm.position.y >= Floor.center.y)
        {
            if(Mathf.Abs(Floor.max.x-pm.position.x)< Mathf.Abs(Floor.max.y-pm.position.y))
            {
                distance = Mathf.Abs(Floor.max.x - pm.position.x);
            }
            else
            {
                distance = Mathf.Abs(Floor.max.y - pm.position.y);
            }
        }
        //Bottom right quadrant
        if (pm.position.x >= Floor.center.x && pm.position.y < Floor.center.y)
        {
            if(Mathf.Abs(Floor.max.x-pm.position.x)< Mathf.Abs(Floor.min.y-pm.position.y))
            {
                distance = Mathf.Abs(Floor.max.x - pm.position.x);
            }
            else
            {
                distance = Mathf.Abs(Floor.min.y - pm.position.y);
            }
        }
        //Bottom left quadrant
        if (pm.position.x < Floor.center.x && pm.position.y < Floor.center.y)
        {
            if (Mathf.Abs(Floor.min.x-pm.position.x) < Mathf.Abs(Floor.min.y-pm.position.y))
            {
                distance = Mathf.Abs(Floor.min.x - pm.position.x);
            }
            else
            {
                distance = Mathf.Abs(Floor.min.y - pm.position.y);
            }
        }

        pm.position += moveDir * distance;

    }

    private static void ResolveFloorCollision(PointMass pm)
    {
        //Rel v is just the pms v here
        Vector2 relativeV = pm.velocity;

        //coefficient of restitution
        float e = 1.0f;

        Vector3 floorMTV;
        floorMTV = Vector3.up;
        //floorMTV = -(Floor.center-pm.position).normalized;



        //Perpendicular  velocity
        float prelativeV = Vector3.Dot(relativeV, floorMTV);

        //"Magic" momentum equation
        float j = ((-(1 + e) * prelativeV) /   //Numerator
            (1 / pm.mass));      //Summ of inverse masses

        Vector3 impulse = j * floorMTV;
        impulse.z = 0;

        pm.impulse += impulse;
        
    }



    //Polygon-Polygon Collisions
    //
    public static void PolygonCheck()
    {

        /*
        for (int i = 0; i < A.CollisionList.Count; i++)
        {
            if (A.CollisionList[i].GetInstanceID()==B.GetInstanceID())
                return;     
        }
        A.CollisionList.Add(B);

        for (int i = 0; i < B.CollisionList.Count; i++)
        {
            if (B.CollisionList[i].GetInstanceID() == A.GetInstanceID())
                return;
        }
        B.CollisionList.Add(A);

        if (TestIntersection(colliderA, colliderB))
        {
            DecoupleShapes(colliderA, colliderB);
            Vector3 colPoint = CollisionPoint(colliderA, colliderB);
            
            if (NeedsResolution(colliderA, colliderB, colPoint))
            {
               ResolveCollision(colliderA, colliderB, colPoint);
            }
           
        }

        */
    }
    /*
    //Testing for collision using SAT
    private static bool TestIntersection(Polygon A, Polygon B)
    {

        MTV = Vector2.zero;
        magnitude = 0;

        verticesA = A.vertices;
        normalsA = A.GetNormals();
        verticesB = B.GetGVertices();
        normalsB = B.GetNormals();


        for (int i = 0; i < normalsA.Count; i++)
        {

            float min1, max1;

            min1 = max1 = Vector2.Dot(normalsA[i], verticesA[0]);

            for (int j = 1; j < verticesA.Count; j++)
            {
                float current = Vector2.Dot(normalsA[i], verticesA[j]);

                if (current < min1)
                    min1 = current;
                else if (current > max1)
                    max1 = current;
            }
            float min2, max2;

            min2 = max2 = Vector2.Dot(normalsA[i], verticesB[0]);
            for (int j = 1; j < verticesB.Count; j++)
            {

                float current = Vector2.Dot(normalsA[i], verticesB[j]);
                if (current < min2)
                    min2 = current;
                else if (current > max2)
                    max2 = current;
            }

            if (min1 < max2 && max1 > min2)
            {

                float overlap1 = max2 - min1;
                float overlap2 = max1 - min2;

                if (overlap1 > overlap2)
                    overlap1 = overlap2;


                if (i == 0 || overlap1 < magnitude)
                {
                    magnitude = overlap1;
                    MTV = normalsA[i];
                }
            }
            else
                return false;
        }

        for (int i = 0; i < normalsB.Count; i++)
        {
            float min1, max1;

            min1 = max1 = Vector2.Dot(normalsB[i], verticesA[0]);

            for (int j = 1; j < verticesA.Count; j++)
            {
                float current = Vector2.Dot(normalsB[i], verticesA[j]);
                if (current < min1)
                    min1 = current;
                else if (current > max1)
                    max1 = current;
            }
            float min2, max2;

            min2 = max2 = Vector2.Dot(normalsB[i], verticesB[0]);
            for (int j = 1; j < verticesB.Count; j++)
            {

                float current = Vector2.Dot(normalsB[i], verticesB[j]);
                if (current < min2)
                    min2 = current;
                else if (current > max2)
                    max2 = current;
            }

            if (min1 < max2 && max1 > min2)
            {

                float overlap1 = max2 - min1;
                float overlap2 = max1 - min2;

                if (overlap1 > overlap2)
                    overlap1 = overlap2;


                if (i == 0 || overlap1 < magnitude)
                {
                    magnitude = overlap1;
                    MTV = normalsB[i];
                }
            }
            else
                return false;
        }

        Vector2 temp = A.Position - B.Position;
        if (Vector2.Dot(temp, MTV) < 0.0f)
            MTV *= -1.0f;


        return true;
    }

    static void DecoupleShapes(ICollider A, ICollider B)
    {
        float i1 = Mathf.Abs(Vector2.Dot(A.Velocity, MTV));
        float i2 = Mathf.Abs(Vector2.Dot(B.Velocity, MTV));

        float sum = i1 + i2 ;

        float mag1 = i1 / sum * magnitude;
        float mag2 = i2 / sum * magnitude;



        A.Position = A.Position + new Vector3(MTV.x * mag1, MTV.y * mag1, 0.0f);

        B.Position = B.Position - new Vector3(MTV.x * mag2, MTV.y * mag2, 0.0f);


    }

    static Vector2 CollisionPoint(ICollider A, ICollider B)
    {
        float tolerance = 0.01f;
        float currentMin;
        float dot;
        Vector2 current;
        List<Vector2> closestPointsA = new List<Vector2>();

        current = A.GetGVertices()[0];
        currentMin = Vector2.Dot(current, MTV);
        closestPointsA.Add(current);


        for (int i = 1; i < A.GetGVertices().Count; i++)
        {
            current = A.GetGVertices()[i];

            dot = Vector2.Dot(current, MTV);

            if (Mathf.Abs(dot - currentMin) < Mathf.Epsilon + tolerance)
                closestPointsA.Add(current);
            else if (dot < currentMin - Mathf.Epsilon)
            {
                currentMin = dot;
                closestPointsA.Clear();
                closestPointsA.Add(current);
            }
        }

        if (closestPointsA.Count == 1)
            return closestPointsA[0];

        List<Vector2> closestPointsB = new List<Vector2>();
        float currentMax;

        current = B.GetGVertices()[0];
        currentMax = Vector2.Dot(current, MTV);
        closestPointsB.Add(current);

        for (int i = 1; i < B.GetGVertices().Count; i++)
        {
            current = B.GetGVertices()[i];

            dot = Vector2.Dot(current, MTV);

            if (Mathf.Abs(dot - currentMin) < Mathf.Epsilon + tolerance)
                closestPointsB.Add(current);
            else if (dot > currentMax + Mathf.Epsilon)
            {
                currentMax = dot;
                closestPointsB.Clear();
                closestPointsB.Add(current);
            }
        }

        if (closestPointsB.Count == 1)
            return closestPointsB[0];

        Vector2 edge = new Vector2(MTV.y, -MTV.x);

        closestPointsA.AddRange(closestPointsB);

        currentMin = currentMax = Vector2.Dot(closestPointsA[0], edge);

        int minIndex = 0, maxIndex = 0;

        for (int i = 0; i < A.GetGVertices().Count; i++)
        {
            dot = Vector2.Dot(closestPointsA[i], edge);
            if (dot < currentMin)
            {
                currentMin = dot;
                minIndex = i;
            }
            if (dot > currentMin)
            {
                currentMax = dot;
                maxIndex = i;
            }
        }

        closestPointsA.RemoveAt(minIndex);
        if (minIndex < maxIndex)
            --maxIndex;
        closestPointsA.RemoveAt(maxIndex);

        Vector2 closestPoint = (closestPointsA[0] + closestPointsA[1]) * 0.5f;
        return closestPoint;


    }

    static bool NeedsResolution(ICollider A, ICollider B, Vector2 cPoint)
    {
        Vector3 radiusA = new Vector3(cPoint.x, cPoint.y, 0.0f) - A.Position;
        Vector3 radiusB = new Vector3(cPoint.x, cPoint.y, 0.0f) - B.Position;

        Vector3 velA = new Vector3(A.Velocity.x, A.Velocity.y, 0.0f) + Vector3.Cross(A.AngularVelocity, radiusA);
        Vector3 velB = new Vector3(B.Velocity.x, B.Velocity.y, 0.0f) + Vector3.Cross(B.AngularVelocity, radiusB);

        Vector2 relativeV = velB - velA;

        if (Vector2.Dot(MTV, relativeV) > 0.0f)
            return true;
        else
            return false;
    }

    static void ResolveCollision(ICollider A, ICollider B, Vector2 cPoint)
    {
        Vector3 radiusA = new Vector3(cPoint.x, cPoint.y, 0.0f) - A.Position;
        Vector3 radiusB = new Vector3(cPoint.x, cPoint.y, 0.0f) - B.Position;

        Vector3 velA = new Vector3(A.Velocity.x, A.Velocity.y, 0.0f) + Vector3.Cross(A.AngularVelocity, radiusA);
        Vector3 velB = new Vector3(B.Velocity.x, B.Velocity.y, 0.0f) + Vector3.Cross(B.AngularVelocity, radiusB);

        Vector2 relativeV = velA - velB;

        //coefficient of restitution
        float e = 1.0f;
        float prelativeV = Vector2.Dot(relativeV, MTV);
        //Perpendicular radii and velocity
        float prelativeFinalV = -e * prelativeV;
        Vector3 pradiusA = Vector3.Cross(Vector3.forward, radiusA);
        Vector3 pradiusB = Vector3.Cross(Vector3.forward, radiusB);



        //"Magic" momentum equation
        
        float j = ((-(1+e)* prelativeV) /   //Numerator
            ((1 / A.Mass + 1 / B.Mass)+              //Summ of inverse masses
           (Mathf.Pow(Vector2.Dot(pradiusA, MTV), 2.0f) / A.MomentofInertia) +
            Mathf.Pow(Vector2.Dot(pradiusB, MTV), 2.0f) / B.MomentofInertia));
            

       

       Vector2 impulse = j * MTV;

    
        A.Impulse += impulse;
       A.AngularImpulse += Vector3.Cross(radiusA, impulse).z;

        impulse *= -1.0f;

        B.Impulse += impulse;
        B.AngularImpulse += Vector3.Cross(radiusA, impulse).z;

    }
    */
}
