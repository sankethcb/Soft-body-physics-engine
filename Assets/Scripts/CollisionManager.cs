using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {


    public static CollisionManager collisionManager;
    public BoxCollider2D Floor;  
    public List<Polygon> polygons;
    public List<PointMass> collisionPoints;
    
        
    // Use this for initialization
	void Start ()
    {
        collisionManager = this;
        Collisions.Floor = Floor.bounds;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        PolygonCollisions();
        FloorCollisions();
        collisionPoints = Collisions.colPoints;
	}

    void PolygonCollisions()
    {
        if (polygons.Count > 0)
        {
            for (int i = 0; i < polygons.Count; i++)
            {
                for (int j = 0; j < polygons.Count; j++)
                {
                    if (polygons[i] != polygons[j])
                        Collisions.PolygonCheck(polygons[i], polygons[j]);
                }
            }
        }
    }

    void FloorCollisions()
    {
        for (int i = 0; i < polygons.Count; i++)
        {
            Collisions.FloorCheck(polygons[i]);
        }
    }
}
