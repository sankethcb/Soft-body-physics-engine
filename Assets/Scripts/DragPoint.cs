using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPoint : MonoBehaviour {

    public bool isDragging;
    Vector3 offset;

	// Use this for initialization
	void Start () {
        isDragging = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
    }


    private void OnMouseDrag()
    {
        isDragging = true;

        //https://stackoverflow.com/questions/23152525/drag-object-in-unity-2d
        Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);

        transform.position  = Camera.main.ScreenToWorldPoint(newPosition) + offset;



    }

    private void OnMouseUp()
    {
        isDragging = false;
    }
}
