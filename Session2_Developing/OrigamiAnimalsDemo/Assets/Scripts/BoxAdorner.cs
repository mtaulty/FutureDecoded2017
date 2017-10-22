using HoloToolkit.Unity.InputModule;
using System;
using System.Linq;
using UnityEngine;

public class BoxAdorner : MonoBehaviour
{
  public Material LineMaterial;

  public BoxAdorner()
  {
  }
  void Start()
  {
    if (this.LineMaterial == null)
    {
      this.LineMaterial = Resources.Load<Material>("BoxMaterial");
    }
  }
  void OnRenderObject()
  {
    if (LineMaterial == null)
    {
      Debug.Log("No line material");
    }
    else
    {
      this.LineMaterial.SetPass(0);

      GL.Begin(GL.LINES);

      // Points from the first point
      GL.Vertex3(_coordinates[0].x, _coordinates[0].y, _coordinates[0].z);
      GL.Vertex3(_coordinates[1].x, _coordinates[1].y, _coordinates[1].z);

      GL.Vertex3(_coordinates[0].x, _coordinates[0].y, _coordinates[0].z);
      GL.Vertex3(_coordinates[3].x, _coordinates[3].y, _coordinates[3].z);

      GL.Vertex3(_coordinates[0].x, _coordinates[0].y, _coordinates[0].z);
      GL.Vertex3(_coordinates[4].x, _coordinates[4].y, _coordinates[4].z);

      // Points from the second point
      GL.Vertex3(_coordinates[1].x, _coordinates[1].y, _coordinates[1].z);
      GL.Vertex3(_coordinates[5].x, _coordinates[5].y, _coordinates[5].z);

      GL.Vertex3(_coordinates[1].x, _coordinates[1].y, _coordinates[1].z);
      GL.Vertex3(_coordinates[2].x, _coordinates[2].y, _coordinates[2].z);

      // Points from the third point
      GL.Vertex3(_coordinates[3].x, _coordinates[3].y, _coordinates[3].z);
      GL.Vertex3(_coordinates[7].x, _coordinates[7].y, _coordinates[7].z);

      GL.Vertex3(_coordinates[3].x, _coordinates[3].y, _coordinates[3].z);
      GL.Vertex3(_coordinates[2].x, _coordinates[2].y, _coordinates[2].z);

      // Points from the fourth point
      GL.Vertex3(_coordinates[4].x, _coordinates[4].y, _coordinates[4].z);
      GL.Vertex3(_coordinates[7].x, _coordinates[7].y, _coordinates[7].z);

      GL.Vertex3(_coordinates[4].x, _coordinates[4].y, _coordinates[4].z);
      GL.Vertex3(_coordinates[5].x, _coordinates[5].y, _coordinates[5].z);

      // Points from the fifth point
      GL.Vertex3(_coordinates[5].x, _coordinates[5].y, _coordinates[5].z);
      GL.Vertex3(_coordinates[6].x, _coordinates[6].y, _coordinates[6].z);

      // Points from the sixth point
      GL.Vertex3(_coordinates[6].x, _coordinates[6].y, _coordinates[6].z);
      GL.Vertex3(_coordinates[2].x, _coordinates[2].y, _coordinates[2].z);

      GL.Vertex3(_coordinates[6].x, _coordinates[6].y, _coordinates[6].z);
      GL.Vertex3(_coordinates[7].x, _coordinates[7].y, _coordinates[7].z);

      GL.End();
    }
  }
  void LateUpdate()
  {
    var collider = gameObject.GetComponentInChildren<BoxCollider>();

    if (collider != null)
    {
      _coordinates = Positions(collider.bounds);
    }
  }
  Vector3[] Positions(Bounds bounds)
  {
    Vector3[] verts = new Vector3[8];

    verts[0].x = bounds.center.x - bounds.extents.x;
    verts[0].y = bounds.center.y - bounds.extents.y;
    verts[0].z = bounds.center.z + bounds.extents.z;

    verts[1].x = bounds.center.x - bounds.extents.x;
    verts[1].y = bounds.center.y + bounds.extents.y;
    verts[1].z = bounds.center.z + bounds.extents.z;

    verts[2].x = bounds.center.x + bounds.extents.x;
    verts[2].y = bounds.center.y + bounds.extents.y;
    verts[2].z = bounds.center.z + bounds.extents.z;

    verts[3].x = bounds.center.x + bounds.extents.x;
    verts[3].y = bounds.center.y - bounds.extents.y;
    verts[3].z = bounds.center.z + bounds.extents.z;

    verts[4].x = bounds.center.x - bounds.extents.x;
    verts[4].y = bounds.center.y - bounds.extents.y;
    verts[4].z = bounds.center.z - bounds.extents.z;

    verts[5].x = bounds.center.x - bounds.extents.x;
    verts[5].y = bounds.center.y + bounds.extents.y;
    verts[5].z = bounds.center.z - bounds.extents.z;

    verts[6].x = bounds.center.x + bounds.extents.x;
    verts[6].y = bounds.center.y + bounds.extents.y;
    verts[6].z = bounds.center.z - bounds.extents.z;

    verts[7].x = bounds.center.x + bounds.extents.x;
    verts[7].y = bounds.center.y - bounds.extents.y;
    verts[7].z = bounds.center.z - bounds.extents.z;

    return verts;
  }
  Vector3? lastDelta;
  Vector3[] _coordinates;
  Vector3[][] _cubeCoordinates = new Vector3[8][];

  static BoxAdorner focusedObject;
}

