using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePoly : MonoBehaviour {

    public GameObject visualPoint;


    public int verticeCount = 4;
    public float PolygonStiffness = 200f;
    float stiffness;
    public float damping =10;
    Vector3 spawn=Vector3.zero;
    Polygon currentPoly;    


    // Use this for initialization
    void Start () {

        stiffness = verticeCount * PolygonStiffness;
        

    }
	


	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
            spawn = Camera.main.ScreenToWorldPoint(newPosition);
            PolyCreation();

        }
    }

    //handles ploygon creation
    public void PolyCreation()
    {
        GameObject currentPolyObject = new GameObject();
        currentPolyObject.transform.position = spawn;
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
            position = spawn + new Vector3(x, y);

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

        float temp1 = stiffness;
        float temp2 = damping;
        //if(stiffness<1600)
           // stiffness = PolygonStiffness * verticeCount * 1.5f;
       // damping = 10f;

        foreach (PointMass vertex in currentPoly.vertices)
        {
            ConnectSpring(vertex, pm);
        }
        stiffness = temp1;
        damping = temp2;


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
