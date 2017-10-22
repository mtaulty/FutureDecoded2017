using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA;

public class ScaleRotateAdorner : MonoBehaviour, IManipulationHandler, IFocusable, ISpeechHandler
{
  public enum ManipulationMode
  {
    Rotate,
    Scale
  }
  public enum ManipulationState
  {
    Idle,
    Available,
    Active
  }

  [Serializable]
  public class ModeMaterial
  {
    public Material material;
    public ManipulationMode mode;
  }
  public ModeMaterial[] lineMaterials;

  public ManipulationState State { get; set; }

  public ManipulationMode? Mode { get; set; }

  public ScaleRotateAdorner()
  {
    this.State = ManipulationState.Idle;
  }

  void Start()
  {
    if ((this.lineMaterials == null) || (this.lineMaterials.Length == 0))
    {
      this.lineMaterials = new ModeMaterial[]
      {
        new ModeMaterial()
        {
          mode = ManipulationMode.Rotate,
          material = Resources.Load<Material>("RotateMaterial")
        },
        new ModeMaterial()
        {
          mode = ManipulationMode.Scale,
          material = Resources.Load<Material>("ScaleMaterial")
        }
      };
    }
    UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourceReleased += OnInteractionSourceReleased;
    UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourceLost += OnInteractionSourceLost;
  }
  public void OnSpeechKeywordRecognized(SpeechEventData eventData)
  {
    var phrase = Enum.Parse(typeof(ManipulationMode), eventData.RecognizedText, true);

    if (Enum.IsDefined(typeof(ManipulationMode), phrase))
    {
      this.Mode = (ManipulationMode)phrase;
      Activate();
    }
  }
  static void Activate()
  {
    // It's possible on start up for someone to issue a command but
    // the framework has not got around to telling us where focus
    // really is! Let's see if the gaze manager knows.
    if (focusedObject == null)
    {
      var hitObject = GazeManager.Instance.HitObject;

      if (hitObject != null)
      {
        focusedObject = hitObject.GetComponent<ScaleRotateAdorner>();
      }
    }
    if (focusedObject != null)
    {
      focusedObject.State = ManipulationState.Available;
    }
  }
  public void OnManipulationStarted(ManipulationEventData eventData)
  {
    if (this.State == ManipulationState.Available)
    {
      this.State = ManipulationState.Active;
      this.lastDelta = eventData.CumulativeDelta;

      // This is a bit of a hack. We destroy the world anchor off the component
      // if you manipulate it but we don't remove it from the store so
      // the object should reappear in the same place next time around.
      var worldAnchor = this.gameObject.GetComponent<WorldAnchor>();

      if (worldAnchor != null)
      {
        Destroy(worldAnchor);
      }
    }
  }
  public void OnManipulationUpdated(ManipulationEventData eventData)
  {
    if (this.State == ManipulationState.Active)
    {
      var delta = eventData.CumulativeDelta - this.lastDelta.Value;
      var xDelta = delta.x;
      var yDelta = delta.y;
      var zDelta = delta.z;
      var movement = new Vector3(xDelta, yDelta, zDelta);
      var magnitude =
        (xDelta > 0) ? movement.magnitude : 0 - movement.magnitude;

      switch (Mode.Value)
      {
        case ManipulationMode.Rotate:
          // Rotate around Z, X, Y
          this.gameObject.transform.Rotate(
            yDelta * ROTATE_FACTOR, (0 - xDelta) * ROTATE_FACTOR, 0, Space.World);
          break;
        case ManipulationMode.Scale:
          var newScale = (1 + (magnitude * SCALE_FACTOR)) * this.gameObject.transform.localScale;

          if ((newScale.magnitude >= MIN_SCALE) && (newScale.magnitude <= MAX_SCALE))
          {
            this.gameObject.transform.localScale = newScale;
          }
          break;
        default:
          break;
      }
      this.lastDelta = eventData.CumulativeDelta;
    }
  }
  public void OnManipulationCompleted(ManipulationEventData eventData)
  {
    this.Done();
  }
  public void OnManipulationCanceled(ManipulationEventData eventData)
  {
    this.Done();
  }
  void Done()
  {
    this.lastDelta = null;
    this.State = ManipulationState.Idle;
  }
  public void OnFocusEnter()
  {
    focusedObject = this;
  }
  public void OnFocusExit()
  {
    this.State = ManipulationState.Idle;
    focusedObject = null;
  }
  void OnInteractionSourceLost(UnityEngine.XR.WSA.Input.InteractionSourceLostEventArgs obj)
  {
    this.Done();
  }
  void OnInteractionSourceReleased(UnityEngine.XR.WSA.Input.InteractionSourceReleasedEventArgs obj)
  {
    this.Done();
  }
  void OnRenderObject()
  {
    if (lineMaterials == null)
    {
      Debug.Log("No line material");
    }
    else if (
      ((this.State == ManipulationState.Active) ||
       (this.State == ManipulationState.Available)) &&
      (Mode != null) &&
      (lineMaterials != null))
    {
      var materialEntry = lineMaterials.Where(m => m.mode == Mode).Single();

      if (materialEntry.material != null)
      {
        materialEntry.material.SetPass(0);

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

        for (int i = 0; i < 8; i++)
        {
          DrawCube(_cubeCoordinates[i]);
        }
      }
    }
  }

  void DrawCube(Vector3[] points)
  {
    GL.Begin(GL.QUADS);
    for (int i = 0; i < points.Length; i++)
    {
      GL.Vertex3(points[i].x, points[i].y, points[i].z);
    }
    GL.End();
  }
  void DrawCube(Vector3 centre, float size)
  {
    // draw a cube for the corners
    GL.Begin(GL.QUADS);

    GL.Vertex3(centre.x + size, centre.y + size, centre.z - size);
    GL.Vertex3(centre.x - size, centre.y + size, centre.z - size);
    GL.Vertex3(centre.x - size, centre.y + size, centre.z + size);
    GL.Vertex3(centre.x + size, centre.y + size, centre.z + size);

    GL.Vertex3(centre.x + size, centre.y - size, centre.z + size);
    GL.Vertex3(centre.x - size, centre.y - size, centre.z + size);
    GL.Vertex3(centre.x - size, centre.y - size, centre.z - size);
    GL.Vertex3(centre.x + size, centre.y - size, centre.z - size);

    GL.Vertex3(centre.x + size, centre.y + size, centre.z + size);
    GL.Vertex3(centre.x - size, centre.y + size, centre.z + size);
    GL.Vertex3(centre.x - size, centre.y - size, centre.z + size);
    GL.Vertex3(centre.x + size, centre.y - size, centre.z + size);

    GL.Vertex3(centre.x + size, centre.y - size, centre.z - size);
    GL.Vertex3(centre.x - size, centre.y - size, centre.z - size);
    GL.Vertex3(centre.x - size, centre.y + size, centre.z - size);
    GL.Vertex3(centre.x + size, centre.y + size, centre.z - size);

    GL.Vertex3(centre.x - size, centre.y + size, centre.z + size);
    GL.Vertex3(centre.x - size, centre.y + size, centre.z - size);
    GL.Vertex3(centre.x - size, centre.y - size, centre.z - size);
    GL.Vertex3(centre.x - size, centre.y - size, centre.z + size);

    GL.Vertex3(centre.x + size, centre.y + size, centre.z - size);
    GL.Vertex3(centre.x + size, centre.y + size, centre.z + size);
    GL.Vertex3(centre.x + size, centre.y - size, centre.z + size);
    GL.Vertex3(centre.x + size, centre.y - size, centre.z - size);

    GL.End();
  }

  void LateUpdate()
  {
    var collider = gameObject.GetComponentInChildren<BoxCollider>();

    if (collider != null)
    {
      _coordinates = Positions(collider.bounds);

      for (int i = 0; i < 8; i++)
      {
        _cubeCoordinates[i] = CreateCubePositions(_coordinates[i], 0.025f);
      }
    }
  }

  Vector3[] CreateCubePositions(Vector3 rawCoords, float size)
  {
    Vector3[] ret = new Vector3[24];

    // We want the corner cubes to scale with the gameobject but we don't want their
    // individual size to change in the process. So, remove the scale component..
    //
    var scale = transform.localScale;

    float sizex = size;
    float sizey = size;
    float sizez = size;

    ret[0] = new Vector3(rawCoords.x + sizex, rawCoords.y + sizey, rawCoords.z - sizez);
    ret[1] = new Vector3(rawCoords.x - sizex, rawCoords.y + sizey, rawCoords.z - sizez);
    ret[2] = new Vector3(rawCoords.x - sizex, rawCoords.y + sizey, rawCoords.z + sizez);
    ret[3] = new Vector3(rawCoords.x + sizex, rawCoords.y + sizey, rawCoords.z + sizez);

    ret[4] = new Vector3(rawCoords.x + sizex, rawCoords.y - sizey, rawCoords.z + sizez);
    ret[5] = new Vector3(rawCoords.x - sizex, rawCoords.y - sizey, rawCoords.z + sizez);
    ret[6] = new Vector3(rawCoords.x - sizex, rawCoords.y - sizey, rawCoords.z - sizez);
    ret[7] = new Vector3(rawCoords.x + sizex, rawCoords.y - sizey, rawCoords.z - sizez);

    ret[8] = new Vector3(rawCoords.x + sizex, rawCoords.y + sizey, rawCoords.z + sizez);
    ret[9] = new Vector3(rawCoords.x - sizex, rawCoords.y + sizey, rawCoords.z + sizez);
    ret[10] = new Vector3(rawCoords.x - sizex, rawCoords.y - sizey, rawCoords.z + sizez);
    ret[11] = new Vector3(rawCoords.x + sizex, rawCoords.y - sizey, rawCoords.z + sizez);

    ret[12] = new Vector3(rawCoords.x + sizex, rawCoords.y - sizey, rawCoords.z - sizez);
    ret[13] = new Vector3(rawCoords.x - sizex, rawCoords.y - sizey, rawCoords.z - sizez);
    ret[14] = new Vector3(rawCoords.x - sizex, rawCoords.y + sizey, rawCoords.z - sizez);
    ret[15] = new Vector3(rawCoords.x + sizex, rawCoords.y + sizey, rawCoords.z - sizez);

    ret[16] = new Vector3(rawCoords.x - sizex, rawCoords.y + sizey, rawCoords.z + sizez);
    ret[17] = new Vector3(rawCoords.x - sizex, rawCoords.y + sizey, rawCoords.z - sizez);
    ret[18] = new Vector3(rawCoords.x - sizex, rawCoords.y - sizey, rawCoords.z - sizez);
    ret[19] = new Vector3(rawCoords.x - sizex, rawCoords.y - sizey, rawCoords.z + sizez);

    ret[20] = new Vector3(rawCoords.x + sizex, rawCoords.y + sizey, rawCoords.z - sizez);
    ret[21] = new Vector3(rawCoords.x + sizex, rawCoords.y + sizey, rawCoords.z + sizez);
    ret[22] = new Vector3(rawCoords.x + sizex, rawCoords.y - sizey, rawCoords.z + sizez);
    ret[23] = new Vector3(rawCoords.x + sizex, rawCoords.y - sizey, rawCoords.z - sizez);

    return ret;
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

  static ScaleRotateAdorner focusedObject;

  // These are all really just fudge factors based on a small set of observations.
  const float ROTATE_FACTOR = 500.0f;
  const float SCALE_FACTOR = 10.0f;
  const float MAX_SCALE = 4.0f;
  const float MIN_SCALE = 0.25f;
}

