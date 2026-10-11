using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class ARCameraToMediaPipe : MonoBehaviour
{
    [Header("AR Foundation Setup")]
    [SerializeField] private ARCameraManager cameraManager;

    [Header("MediaPipe Target")]
    [SerializeField] private HandLandmarkerRunner handLandmarkerRunner;

    private Texture2D arCameraTexture;
    private int frameCount = 0;

    private void OnEnable()
    {
        if (cameraManager != null)
        {
            cameraManager.frameReceived += OnCameraFrameReceived;
        }
    }

    private void OnDisable()
    {
        if (cameraManager != null)
        {
            cameraManager.frameReceived -= OnCameraFrameReceived;
        }
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        frameCount++;
        if (frameCount % 4 != 0) return;
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
            return;

        using (cpuImage)
        {
            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
                outputDimensions = new Vector2Int(cpuImage.width / 2, cpuImage.height / 2),
                outputFormat = TextureFormat.RGBA32,
                transformation = XRCpuImage.Transformation.MirrorY
            };


            cpuImage.ConvertAsync(conversionParams, (status, paramsUsed, rawBuffer) =>
            {
                if (status == XRCpuImage.AsyncConversionStatus.Ready)
                {
                    ProcessConvertedFrame(paramsUsed, rawBuffer);
                }
                
            });
        }
    }

    private void ProcessConvertedFrame(XRCpuImage.ConversionParams conversionParams, NativeArray<byte> rawBuffer)
    {
        if (arCameraTexture == null || 
            arCameraTexture.width != conversionParams.outputDimensions.x || 
            arCameraTexture.height != conversionParams.outputDimensions.y)
        {
            arCameraTexture = new Texture2D(
                conversionParams.outputDimensions.x,
                conversionParams.outputDimensions.y,
                TextureFormat.RGBA32,
                false
            );
        }

        arCameraTexture.LoadRawTextureData(rawBuffer);
        arCameraTexture.Apply();

        if (handLandmarkerRunner != null)
        {
            handLandmarkerRunner.ProcessTexture(arCameraTexture);
        }
    }
}