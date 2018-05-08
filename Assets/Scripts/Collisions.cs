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
    public static List<PointMass> colPoints = new List<PointMass>();
    public static PointMass[] Edge = new PointMass[2];

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

    //Bounding box-point check
    private static bool AABB(PointMass pm)
    {
        if (pm.position.x > Floor.min.x &&
            pm.position.x < Floor.max.x &&
            pm.position.y > Floor.min.y &&
            pm.position.y < Floor.max.y)
            return true;

        return false;
    }

    //Preventing tunneling with floor
    private static void FloorDecouple(PointMass pm)
    {
        Vector3 moveDir = Vector3.up;// -pm.velocity.normalized;

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

    //Resolves point mass collisions with floor
    private static void ResolveFloorCollision(PointMass pm)
    {
        //Rel v is just the pms v here
        Vector2 relativeV = pm.velocity;

        //coefficient of restitution
        float e = 1f;

        Vector3 floorMTV;
        floorMTV = Vector3.up;
        // floorMTV = -(Floor.center-pm.position).normalized;



        //Perpendicular  velocity
        float prelativeV = Vector3.Dot(relativeV, floorMTV);

        //"Magic" momentum equation
        float j = ((-(1 + e) * prelativeV) /   //Numerator
            (1 / pm.mass));      //Summ of inverse masses

        Vector3 impulse = j * floorMTV;
        impulse.z = 0;

        if (pm.velocity.y < 0)
            pm.velocity.y = 0;

        pm.impulse += impulse;



    }





    //Polygon-Polygon Collisions
    public static void PolygonCheck(Polygon polyA, Polygon polyB)
    {
        colPoints.Clear();

        //if (polyA.collisionList.Contains(polyB) || polyB.collisionList.Contains(polyA))
            //return;

        if (TestIntersection(polyA, polyB))
        {
            CollisionPoints(polyA, polyB);
            DecouplePoints(polyA, polyB);
            ResolveCollision(polyA, polyB);

            polyA.collisionList.Add(polyB);
            polyB.collisionList.Add(polyA);
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

    static void DecouplePoints(Polygon A, Polygon B)
    {
        Vector3 dir=Vector3.zero;
        Vector3 p1, p2;
        float dist;
        float mindist = 1000;
        foreach (PointMass point in colPoints)
        {
            

            if (A.vertices.Contains(point))
            {
                dir = (A.center.position - point.position).normalized;
                for (int i = 0; i < B.vertices.Count; i++)
                {
                    p2 = B.vertices[i].position;
                    if (i + 1 == B.vertices.Count)
                        p1 = B.vertices[0].position;
                    else
                        p1 = B.vertices[i + 1].position;

                    dist = PointLineDistance(p1, p2, point.position);

                    if (dist < mindist)
                    {
                        mindist = dist;
                    }
                }

            }
            if (B.vertices.Contains(point))
            {
                dir = (B.center.position - point.position).normalized;
                for (int i = 0; i < A.vertices.Count; i++)
                {
                    p2 = A.vertices[i].position;
                    if (i + 1 == A.vertices.Count)
                        p1 = B.vertices[0].position;
                    else
                        p1 = A.vertices[i + 1].position;

                    dist = PointLineDistance(p1, p2, point.position);

                    if (dist < mindist)
                    {
                        mindist = dist;
                    }
                }
            }

            point.position += dir*mindist;
        }

    }

    //Adds point masses of one polygon in another to a list
    static void CollisionPoints(Polygon A, Polygon B)
    {
        foreach (PointMass pm in A.vertices)
            if (PointinPolygon(pm.position, B))
                colPoints.Add(pm);

        foreach (PointMass pm in B.vertices)
            if (PointinPolygon(pm.position, A))
                colPoints.Add(pm);
    }

    static void ResolveCollision(Polygon A, Polygon B)
    {

        Vector2 relativeV = A.center.velocity - B.center.velocity;

        //coefficient of restitution
        float e = 1.0f;
        float prelativeV = Vector2.Dot(relativeV, MTV);

        //"Magic" momentum equation

        float j;

        Vector3 impulse;

        j = (-(1 + e) * prelativeV) / (1 / A.Mass + 1 / B.Mass);
        impulse = j * MTV;
        impulse.z = 0f;
        impulse /= (colPoints.Count + 2);

        foreach (PointMass point in colPoints)
        {           
            if (A.vertices.Contains(point))
            {

                point.impulse += impulse;
                B.center.impulse -= impulse;

            }
            if (B.vertices.Contains(point))
            {
                A.center.impulse += impulse;
                point.impulse -= impulse;
            }
        }

    }

    //https://www.geeksforgeeks.org/how-to-check-if-a-given-point-lies-inside-a-polygon/
    //Checks if a point is in a polygon
    static bool PointinPolygon(Vector3 point, Polygon poly)
    {
        Vector3 extreme = new Vector3(1000, point.y);
        Vector3 p1;
        Vector3 p2;
        float count = 0;
        for (int i = 0; i < poly.vertices.Count; i++)
        {
            if (i + 1 == poly.vertices.Count)
                p1 = poly.vertices[0].position;

            else
                p1 = poly.vertices[i + 1].position;

            p2 = poly.vertices[i].position;

            if (LineIntersection(p1, p2, point, extreme))
            {
                if (Orientation(p2, point, p1) == 0)
                    return OnLine(p2, point, p1);

                count++;
            }

        }

        if (count % 2 == 0)
            return false;

        else
            return true;
    }


    //Checks if a point is on a line
    static bool OnLine(Vector3 p, Vector3 q, Vector3 r)
    {
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) && q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y))
            return true;

        return false;
    }

    //Colinear orientation of 3 points
    static int Orientation(Vector3 p, Vector3 q, Vector3 r)
    {
        float result = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

        if (result == 0)
            return 0;  // colinear

        if (result > 0)
            return 1; //clock wise

        return 2; //counterclockwise


    }

    //Check if p1q1 intersects with p2q2
    static bool LineIntersection(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
    {

        int o1 = Orientation(p1, q1, p2);
        int o2 = Orientation(p1, q1, q2);
        int o3 = Orientation(p2, q2, p1);
        int o4 = Orientation(p2, q2, q1);

        if (o1 != o2 && o3 != o4)
            return true;

        //p2 lies on line p1q1 while they are coloinear
        if (o1 == 0 && OnLine(p1, p2, q1))
            return true;

        // q2 lies on line p1q1 while they are coloinear
        if (o2 == 0 && OnLine(p1, q2, q1)) return true;

        //p1 lies on line p2q2 while they are coloinear
        if (o3 == 0 && OnLine(p2, p1, q2)) return true;

        //q1 lies on line p2q2 while they are coloinear
        if (o4 == 0 && OnLine(p2, q1, q2)) return true;

        return false; // Doesn't fall in any of the above cases
    }


    static float PointLineDistance(Vector3 p1, Vector3 p2, Vector3 point)
    {
        //herons 
        float a = Vector3.Distance(p1, p2);
        float b = Vector3.Distance(p2, point);
        float c = Vector3.Distance(point, p1);

        float s = (a+b+c)/2;

        float area = Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));

        float height = (area * 2) * a;

        return height;
    }

    //Currently not used
    /* static List<PointMass> CollisionPoint(Polygon A, Polygon B)
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
    */

    /*
static void DecouplePoints(Polygon A, Polygon B)
{
    //float i1, i2, sum, mag1, mag2;


    //i1 = Mathf.Abs(Vector2.Dot(pm.velocity, MTV));
    //  i2 = Mathf.Abs(Vector2.Dot(closestVertex.velocity, MTV));

    // sum = i1 + i2;

    // mag1 = i1 / sum * magnitude;
    // mag2 = i2 / sum * magnitude;

    // pm.position += new Vector3(MTV.x * mag1, MTV.y * mag1, 0.0f);

     PointMass p1,p2;
        Vector3 dir=Vector3.zero;
        float mindist = 1000;
       
        float dist;
        
        foreach (PointMass point in colPoints)
        {
            if (A.vertices.Contains(point))
            {
                for (int i = 0; i < B.vertices.Count; i++)
                {
                    p2 = B.vertices[i];
                    if (i + 1 == B.vertices.Count)
                        p1 = B.vertices[0];
                    else
                        p1 = B.vertices[i + 1];

                    dist = PointLineDistance(p1.position, p2.position, point.position);

                    if (dist < mindist)
                    {
                        mindist = dist;
                        Edge[0] = p1;
                        Edge[1] = p2;
                    }
                }


            }

            else if(B.vertices.Contains(point))
            {
                for (int i = 0; i < A.vertices.Count; i++)
                {
                    p2 = A.vertices[i];
                    if (i + 1 == A.vertices.Count)
                        p1 = A.vertices[0];
                    else
                        p1 = A.vertices[i + 1];

                    dist = PointLineDistance(p1.position, p2.position, point.position);

                    if (dist < mindist)
                    {
                        mindist = dist;
                        Edge[0] = p1;
                        Edge[1] = p2;
                    }
                }

            }

            float scalarProj = Vector3.Dot((point.position - Edge[0].position), (Edge[1].position - Edge[0].position).normalized);
            Vector3 vecProj = scalarProj * (Edge[1].position - Edge[0].position).normalized;          

            dir = ((vecProj + Edge[0].position)-point.position).normalized;


            point.position += mindist * dir;
    
        }      

}
*/
}


