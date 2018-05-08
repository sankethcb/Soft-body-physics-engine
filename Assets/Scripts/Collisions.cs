using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Collisions
{

    static Vector2 MTV;

    static List<PointMass> verticesA;
    static List<Vector3> normalsA;

    static List<PointMass> verticesB;
    static List<Vector3> normalsB;
    static float magnitude;
    public static Bounds Floor;
    public static List<PointMass> colPoints;


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

        if (AABB(poly.center))
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

        float distance = 0.0f;

        //Top left quadrant
        if (pm.position.x < Floor.center.x && pm.position.y >= Floor.center.y)
        {
            if (Mathf.Abs(Floor.min.x - pm.position.x) < Mathf.Abs(Floor.max.y - pm.position.y))
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
            if (Mathf.Abs(Floor.max.x - pm.position.x) < Mathf.Abs(Floor.max.y - pm.position.y))
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
            if (Mathf.Abs(Floor.max.x - pm.position.x) < Mathf.Abs(Floor.min.y - pm.position.y))
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
            if (Mathf.Abs(Floor.min.x - pm.position.x) < Mathf.Abs(Floor.min.y - pm.position.y))
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

    public static void PolygonCheck(Polygon polyA, Polygon polyB)
    {


        if (TestIntersection(polyA, polyB))
        {

            //DecoupleShapes(polyA, polyB);
            colPoints = CollisionPoint(polyA, polyB);
            ResolveCollision(polyA, polyB);

        }



    }

    //Testing for collision using SAT
    private static bool TestIntersection(Polygon A, Polygon B)
    {

        MTV = Vector2.zero;
        magnitude = 0;

        verticesA = A.vertices;
        normalsA = A.Normals;
        verticesB = B.vertices;
        normalsB = B.Normals;


        for (int i = 0; i < normalsA.Count; i++)
        {

            float min1, max1;

            min1 = max1 = Vector2.Dot(normalsA[i], verticesA[0].position);

            for (int j = 1; j < verticesA.Count; j++)
            {
                float current = Vector2.Dot(normalsA[i], verticesA[j].position);

                if (current < min1)
                    min1 = current;
                else if (current > max1)
                    max1 = current;
            }
            float min2, max2;

            min2 = max2 = Vector2.Dot(normalsA[i], verticesB[0].position);
            for (int j = 1; j < verticesB.Count; j++)
            {

                float current = Vector2.Dot(normalsA[i], verticesB[j].position);
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

            min1 = max1 = Vector3.Dot(normalsB[i], verticesA[0].position);

            for (int j = 1; j < verticesA.Count; j++)
            {
                float current = Vector3.Dot(normalsB[i], verticesA[j].position);
                if (current < min1)
                    min1 = current;
                else if (current > max1)
                    max1 = current;
            }
            float min2, max2;

            min2 = max2 = Vector2.Dot(normalsB[i], verticesB[0].position);
            for (int j = 1; j < verticesB.Count; j++)
            {

                float current = Vector2.Dot(normalsB[i], verticesB[j].position);
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

        Vector2 temp = A.center.position - B.center.position;
        if (Vector2.Dot(temp, MTV) < 0.0f)
            MTV *= -1.0f;


        return true;
    }

    /*
    static void DecoupleShapes(Polygon A, Polygon B)
    {
        float i1, i2, sum, mag1, mag2;
        /*
        float i1 = Mathf.Abs(Vector2.Dot(A.center.velocity, MTV));
        float i2 = Mathf.Abs(Vector2.Dot(B.center.velocity, MTV));

        float sum = i1 + i2;

        float mag1 = i1 / sum * magnitude;
        float mag2 = i2 / sum * magnitude;
        


        foreach (PointMass vertex in A.vertices)
        {
            i1 = Mathf.Abs(Vector2.Dot(vertex.velocity, MTV));
            i2 = Mathf.Abs(Vector2.Dot(B.center.velocity, MTV));

            sum = i1 + i2;

            mag1 = i1 / sum * magnitude;
            mag2 = i2 / sum * magnitude;

            vertex.position += new Vector3(MTV.x * mag1, MTV.y * mag1, 0.0f);
        }

        i1 = Mathf.Abs(Vector2.Dot(A.center.velocity, MTV));
        i2 = Mathf.Abs(Vector2.Dot(B.center.velocity, MTV));

        sum = i1 + i2;

        mag1 = i1 / sum * magnitude;
        mag2 = i2 / sum * magnitude;

        
        A.center.position += new Vector3(MTV.x * mag1, MTV.y * mag1, 0.0f);

        foreach (PointMass vertex in B.vertices)
        {
            i1 = Mathf.Abs(Vector2.Dot(A.center.velocity, MTV));
            i2 = Mathf.Abs(Vector2.Dot(vertex.velocity, MTV));

            sum = i1 + i2;

            mag1 = i1 / sum * magnitude;
            mag2 = i2 / sum * magnitude;
            vertex.position += new Vector3(MTV.x * mag2, MTV.y * mag2, 0.0f);
        }

        i1 = Mathf.Abs(Vector2.Dot(A.center.velocity, MTV));
        i2 = Mathf.Abs(Vector2.Dot(B.center.velocity, MTV));

        sum = i1 + i2;

        mag1 = i1 / sum * magnitude;
        mag2 = i2 / sum * magnitude;
        B.center.position += new Vector3(MTV.x * mag1, MTV.y * mag1, 0.0f);

    }
*/

    static List<PointMass> CollisionPoint(Polygon A, Polygon B)
    {
        float tolerance = 0.01f;
        float currentMin;
        float dot;
        PointMass current;
        List<PointMass> closestPointsA = new List<PointMass>();

        current = A.vertices[0];
        currentMin = Vector3.Dot(current.position, MTV);
        closestPointsA.Add(current);


        for (int i = 1; i < A.vertices.Count; i++)
        {
            current = A.vertices[i];

            dot = Vector3.Dot(current.position, MTV);

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
            return closestPointsA;

        List<PointMass> closestPointsB = new List<PointMass>();
        float currentMax;

        current = B.vertices[0];
        currentMax = Vector3.Dot(current.position, MTV);
        closestPointsB.Add(current);

        for (int i = 1; i < B.vertices.Count; i++)
        {
            current = B.vertices[i];

            dot = Vector3.Dot(current.position, MTV);

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
            return closestPointsB;

        Vector2 edge = new Vector3(MTV.y, -MTV.x);

        closestPointsA.AddRange(closestPointsB);

        currentMin = currentMax = Vector3.Dot(closestPointsA[0].position, edge);

        int minIndex = 0, maxIndex = 0;
        for (int i = 0; i < closestPointsA.Count; i++)
        {

            dot = Vector3.Dot(closestPointsA[i].position, edge);
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

        //PointMass closestPoint = (closestPointsA[0] + closestPointsA[1]) * 0.5f;
        return closestPointsA;





    }

    /*
    static bool NeedsResolution(Polygon A, Polygon B)
    {

        Vector2 relativeV = A.center.velocity - B.center.velocity;

        if (Vector2.Dot(MTV, relativeV) > 0.0f)
            return true;
        else
            return false;
    }
    */
    static void ResolveCollision(Polygon A, Polygon B)
    {


        Vector2 relativeV = A.center.velocity - B.center.velocity;

        //coefficient of restitution
        float e = 1.0f;
        float prelativeV = Vector2.Dot(relativeV, MTV);

        //"Magic" momentum equation

        float j = (-(1 + e) * prelativeV) /   //Numerator
            (1 / A.Mass + 1 / B.Mass);    //Summ of inverse masses





        Vector3 impulse = j * MTV;
        impulse.z = 0f;

        if (colPoints == null)
            return;
        foreach (PointMass pm in colPoints)
        {
            if (A.vertices.Contains(pm))
            {
                pm.impulse += impulse;
            }

            else if (B.vertices.Contains(pm))
            {
                pm.impulse -= impulse;
            }
        }

    }

}
