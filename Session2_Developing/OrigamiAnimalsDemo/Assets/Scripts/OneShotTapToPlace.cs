using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class OneShotTapToPlace : TapToPlace
{
  public event EventHandler Placed;

  public OneShotTapToPlace()
  {
    base.IsBeingPlaced = true;
  }
  public override void OnInputClicked(InputClickedEventData eventData)
  {
    base.OnInputClicked(eventData);

    if (this.Placed != null)
    {
      this.Placed(this, EventArgs.Empty);
    }
  }
  protected override void OnEnable()
  {
    base.OnEnable();
    base.UseColliderCenter = true;
    base.AllowMeshVisualizationControl = true;
    base.PlacementPosOffset *= 2.0f;
  }
}
