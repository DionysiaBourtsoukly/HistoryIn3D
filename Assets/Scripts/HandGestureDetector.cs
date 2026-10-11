using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Unity;

public class HandGestureDetector : HandLandmarkerResultAnnotationController
{
    public enum HandGesture { None, ClosedFist, ThumbsUp, ThumbsDown }

    [Header("Tuning Settings")]
    [Tooltip("Multiplier to determine if fingers are curled within the palm area.")]
    [Range(1.0f, 2.0f)] public float curledBoundaryMultiplier = 1.3f;
    [Tooltip("How much further away the thumb outlier must be compared to the curled fingers.")]
    [Range(1.1f, 2.0f)] public float outlierSeparationMultiplier = 1.3f;
    [Tooltip("Threshold for the thumb's X-axis shift (dx) to count as Up/Down.")]
    public float thumbDirectionThreshold = 0.04f;
    public bool invertVertical = false; // thumbs up / down was suspected to be inverted
    public int stableFramesThreshold = 5;

    [Header("Gesture Events")]
    public UnityEvent onThumbsUpStarted;
    public UnityEvent onThumbsUpEnded;
    public UnityEvent onThumbsDownStarted;
    public UnityEvent onThumbsDownEnded;
    public UnityEvent onClosedFistStarted;
    public UnityEvent onClosedFistEnded;

    private HandGesture currentGesture = HandGesture.None;
    private HandGesture candidateGesture = HandGesture.None;
    private int consecutiveFrames = 0;
    
    private float lastNoHandLogTime = 0f;

    protected override void SyncNow()
    {
        lock (_currentTargetLock)
        {
            if (_currentTarget.handLandmarks == null || _currentTarget.handLandmarks.Count == 0)
            {
                if (Time.time - lastNoHandLogTime > 2f)
                {
                    lastNoHandLogTime = Time.time;
                }
                return;
            }

            HandGesture detected = EvaluateGesture(_currentTarget.handLandmarks[0].landmarks);
            ApplySmoothing(detected);
        }
    }

    private HandGesture EvaluateGesture(IReadOnlyList<NormalizedLandmark> lm)
    {
        if (lm == null || lm.Count < 21) return HandGesture.None;

        Vector2 palmCenter = (Get2D(lm[5]) + Get2D(lm[9]) + Get2D(lm[13]) + Get2D(lm[17])) / 4f;
        float palmSize = Vector2.Distance(Get2D(lm[0]), Get2D(lm[9])); 

        float indexDist  = Vector2.Distance(Get2D(lm[8]), palmCenter);
        float middleDist = Vector2.Distance(Get2D(lm[12]), palmCenter);
        float ringDist   = Vector2.Distance(Get2D(lm[16]), palmCenter);
        float pinkyDist  = Vector2.Distance(Get2D(lm[20]), palmCenter);

        float maxFingerDist = Mathf.Max(indexDist, Mathf.Max(middleDist, Mathf.Max(ringDist, pinkyDist)));

        float thumbDist = Vector2.Distance(Get2D(lm[4]), palmCenter);

        if (maxFingerDist > (palmSize * curledBoundaryMultiplier))
        {
            return HandGesture.None;
        }

        bool isThumbOutlier = thumbDist > (maxFingerDist * outlierSeparationMultiplier);

        if (!isThumbOutlier)
        {
            return HandGesture.ClosedFist;
        }

        // Measure from Thumb Base (lm[2]) to Thumb Tip (lm[4])
        float dx = Get2D(lm[4]).x - Get2D(lm[2]).x;
        
        if (invertVertical) dx *= -1f;

        if (dx > thumbDirectionThreshold)
        {
            return HandGesture.ThumbsUp;
        }
        else if (dx < -thumbDirectionThreshold)
        {
            return HandGesture.ThumbsDown;
        }

        return HandGesture.None;
    }

    private void ApplySmoothing(HandGesture detected)
    {
        if (detected == candidateGesture)
        {
            consecutiveFrames++;
        }
        else
        {
            candidateGesture = detected;
            consecutiveFrames = 1;
        }

        if (consecutiveFrames == stableFramesThreshold && candidateGesture != currentGesture)
        {
            TriggerEvents(candidateGesture);
            currentGesture = candidateGesture;
        }
    }

    private void TriggerEvents(HandGesture newGesture)
    {
        if (currentGesture == HandGesture.ThumbsUp) onThumbsUpEnded?.Invoke();
        else if (currentGesture == HandGesture.ThumbsDown) onThumbsDownEnded?.Invoke();
        else if (currentGesture == HandGesture.ClosedFist) onClosedFistEnded?.Invoke();

        if (newGesture == HandGesture.ThumbsUp) onThumbsUpStarted?.Invoke();
        else if (newGesture == HandGesture.ThumbsDown) onThumbsDownStarted?.Invoke();
        else if (newGesture == HandGesture.ClosedFist) onClosedFistStarted?.Invoke();
    }

    private Vector2 Get2D(NormalizedLandmark lm) 
    {
        // convert into true screen pixel space to eliminate aspect-ratio stretching
        return new Vector2(lm.x * UnityEngine.Screen.width, lm.y * UnityEngine.Screen.height);
    }
}