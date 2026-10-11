// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;

using Mediapipe.Tasks.Vision.HandLandmarker;

namespace Mediapipe.Unity
{
  public class HandLandmarkerResultAnnotationController : AnnotationController<MultiHandLandmarkListAnnotation>
  {
    [SerializeField] private bool _visualizeZ = false;

    [SerializeField] private HandSurfaceMesh _handMesh;
    [SerializeField] private RectTransform _dummyScreenRect;

    protected readonly object _currentTargetLock = new object();
    protected HandLandmarkerResult _currentTarget;

    public void DrawNow(HandLandmarkerResult target)
    {
      target.CloneTo(ref _currentTarget);
      SyncNow();
    }

    public void DrawLater(HandLandmarkerResult target) => UpdateCurrentTarget(target);

    protected void UpdateCurrentTarget(HandLandmarkerResult newTarget)
    {
      lock (_currentTargetLock)
      {
        newTarget.CloneTo(ref _currentTarget);
        isStale = true;
      }
    }

    protected override void SyncNow()
    {
      lock (_currentTargetLock)
      {
        isStale = false;
        annotation.SetHandedness(_currentTarget.handedness);
        annotation.Draw(_currentTarget.handLandmarks, _visualizeZ);

        if (_handMesh == null)
        {
            _handMesh = FindObjectOfType<HandSurfaceMesh>();
        }

        if (_dummyScreenRect == null)
        {
            GameObject dummy = GameObject.Find("DummyScreen");
            if (dummy != null) _dummyScreenRect = dummy.GetComponent<RectTransform>();
        }

        if (_currentTarget.handLandmarks != null && _currentTarget.handLandmarks.Count > 0)
        {
            _handMesh.gameObject.SetActive(true);

            // RectTransform boundsToUse = _dummyScreenRect != null 
            //     ? _dummyScreenRect 
            //     : GetComponent<RectTransform>();

            // if (boundsToUse != null)
            // {
                // _handMesh.UpdateMesh(_currentTarget.handLandmarks[0].landmarks, boundsToUse);
            // }
        }
        else
        {
            _handMesh.gameObject.SetActive(false);
        }
      }
    }
  }
}
