﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePoly : MonoBehaviour {

    public GameObject visualPoint;


    public int verticeCount = 4;
    public  float stiffness=200;
    public float damping =10;

    Polygon currentPoly;    


    // Use this for initialization
    void Start () {

        StartCoroutine(LateStart());
        

    }
	
    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(1);
        PolyCreation();
    }

	// Update is called once per frame
	void Update () {
		
	}

    //handles ploygon creation
    public void PolyCreation()
    {
        GameObject currentPolyObject = new GameObject();
        currentPoly = currentPolyObject.AddComponent<Polygon>() as Polygon;
        AddPointMasses(currentPolyObject);

        CollisionManager.collisionManager.polygons.Add(currentPoly);


    }

    //Adds point masses to the currently created polygon
    void AddPointMasses( GameObject polyObject)
    {
        float angle = 0;
        float dAngle = 360 / verticeCount;
        float x;
        float y;
        Vector3 position;
        PointMass pm;
        GameObject vp;
        for (int i = 0; i < verticeCount; i++)
        {
            //Position for each vertex
            x = Mathf.Cos(Mathf.Deg2Rad * angle);
            y = Mathf.Sin(Mathf.Deg2Rad * angle);
            position = new Vector3(x, y);

            //Creating point mass at each vertex
             pm = polyObject.AddComponent<PointMass>() as PointMass;
            pm.position = position;
            currentPoly.vertices.Add(pm);

            //Creativ a visual indicator of a vertex
            vp = Instantiate(visualPoint, position, Quaternion.identity);
            vp.transform.localScale = vp.transform.localScale * 0.2f;
            currentPoly.visualPoints.Add(vp);
            vp.transform.parent = polyObject.transform;

            //Attaching springs
            if(i!=0)
            {
                ConnectSpring(currentPoly.vertices[i - 1], currentPoly.vertices[i]);               
            }
            angle += dAngle;
        }
        ConnectSpring(currentPoly.vertices[verticeCount - 1], currentPoly.vertices[0]);


        //Creating Center point mass
        pm = polyObject.AddComponent<PointMass>() as PointMass;
        pm.position = polyObject.transform.position;
        currentPoly.center = pm;

        vp = Instantiate(visualPoint, pm.position, Quaternion.identity);
        vp.transform.localScale = vp.transform.localScale * 0.2f;
        currentPoly.visualPoints.Add(vp);
        vp.transform.parent = polyObject.transform;

        foreach(PointMass vertex in currentPoly.vertices)
        {
            ConnectSpring(vertex, pm);
        }



    }

    //Creates and connects spring between 2 vertices
    void ConnectSpring(PointMass A, PointMass B)
    {
        Spring spring = new Spring(A, B);

        spring.SetStats(stiffness, damping);


        A.attatchedSprings.Add(spring);
        B.attatchedSprings.Add(spring);
        currentPoly.springs.Add(spring);
    }


}