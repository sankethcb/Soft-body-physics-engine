using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon : MonoBehaviour {

    public List<PointMass> vertices = new List<PointMass>();
    public PointMass center;
    public List<GameObject> visualPoints = new List<GameObject>();
    public List<Spring> springs = new List<Spring>();
     public List<Vector3> normals=new List<Vector3>();
    // Use this for initialization
    

    void Start ()
    {
    
    }

    private void Update()
    {
        for (int i = 0; i < springs.Count; i++)
        {
            springs[i].isCalled = false;
        }
        this.transform.position = visualPoints[visualPoints.Count - 1].transform.position;

        List<Vector3> temp = Normals;
        MoveSprites();
    }

    public float Mass
    {
        get
        {
            float sum=0;
            foreach (PointMass vertex in vertices)
                sum += vertex.mass;

            sum += center.mass;

            return sum;
        }
    }

    //Local normals
    public List<Vector3> Normals
    {
        get
        {
            normals.Clear();
            Vector3 edge=Vector3.zero;
            Vector3 normal = Vector3.zero;
            
            
            for (int i=0;i<vertices.Count;i++)
            {
                if (i + 1 == vertices.Count)
                    edge = transform.worldToLocalMatrix * vertices[0].position - transform.worldToLocalMatrix * vertices[i].position;
                    
                else
                    edge = transform.worldToLocalMatrix * vertices[i + 1].position - transform.worldToLocalMatrix * vertices[i].position;

                
                normal = new Vector3(edge.y, -edge.x).normalized;

                normals.Add(normal);
            }

            return normals;
        }
    }


    //Move sprites to the points OR move point to a dragged sprite
    void MoveSprites()
    {
        for(int i=0;i<vertices.Count;i++)
        {
           
            if(visualPoints[i].GetComponent<DragPoint>().isDragging)
            {
                vertices[i].position = visualPoints[i].transform.position;
            }
            else
                visualPoints[i].transform.position = vertices[i].position;
        }

        //Center point

        if (visualPoints[visualPoints.Count-1].GetComponent<DragPoint>().isDragging)
        {
            center.position = visualPoints[visualPoints.Count-1].transform.position;
        }
        else
            visualPoints[visualPoints.Count-1].transform.position = center.position;
    }

    public void OnRenderObject()
    {
        DrawPoly.Draw(this, transform.localToWorldMatrix);
    }
}
