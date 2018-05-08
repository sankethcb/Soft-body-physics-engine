using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://docs.unity3d.com/ScriptReference/GL.html
public class DrawPoly
{
    static Shader lineShader = Shader.Find("Hidden/Internal-Colored");
    static Material lineMaterial;


    //Static function to draw a polygon
    public static void Draw(Polygon poly, Matrix4x4 localtoworld)
    {
        if (!lineMaterial)
        {
            CreateLineMat();
        }
        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(localtoworld);
        GL.Begin(GL.LINES);

        float pointCount = poly.vertices.Count;
        Vector3 pos1;
        Vector3 pos2;
        Vector3 center = poly.center.position;

        //Edge Lines
        for (int i = 0; i < pointCount; i++)
        {

            pos1 = poly.vertices[i].position;
            if (i + 1 == pointCount)
                pos2 = poly.vertices[0].position;
            else
                pos2 = poly.vertices[i + 1].position;

            GL.Vertex3(pos1.x, pos1.y, pos1.z);
            GL.Vertex3(pos2.x, pos2.y, pos2.z);

            GL.Vertex3(pos1.x, pos1.y, pos1.z);
            GL.Vertex3(center.x, center.y, center.z);
        }




        GL.End();
        GL.PopMatrix();
    }

    //Creates a mterial for the GL line
    static void CreateLineMat()
    {

        lineMaterial = new Material(lineShader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        // Turn on alpha blending
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // Turn backface culling off
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        // Turn off depth writes
        lineMaterial.SetInt("_ZWrite", 0);
    }

}
