using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {


    public static CollisionManager collisionManager;
    public BoxCollider2D Floor;  
    public List<Polygon> polygons;
    
        
    // Use this for initialization
	void Start ()
    {
        collisionManager = this;
        Collisions.Floor = Floor.bounds;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        FloorCollisions();
	}

    void PolygonCollisions()
    {
        for(int i=0;i<polygons.Count;i++)
        {
            for (int j = 0; j < polygons.Count; i++)
            {
                if (polygons[i] != polygons[j])
                    Collisions.PolygonCheck();
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
