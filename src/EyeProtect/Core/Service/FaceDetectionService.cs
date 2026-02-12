using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using Emgu.CV;
using EyeProtect.Core.Helpers;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace EyeProtect.Core.Service
{
    /// <summary>
    /// Face Detection Service - detects if user is present using camera
    /// </summary>
    public class FaceDetectionService : IService, IDisposable
    {
        private InferenceSession _inferenceSession;
        private VideoCapture _camera;
        private readonly ConfigService _config;
        private Thread _detectionThread;
        private volatile bool _isRunning;
        private volatile bool _faceDetected;
        private volatile bool _wasFaceDetected; // Track previous state for edge detection
        private readonly Lock _lock = new();
        private Dispatcher _uiDispatcher;
        private string _tempModelPath; // Store temp file path for cleanup

        // Detection parameters
        private const int DetectionIntervalMs = 1000; // Check every second
        private const int ThreadJoinTimeoutMs = 2000; // Timeout for thread to finish

        /// <summary>
        /// Event fired when a face is detected (transition from no face to face detected)
        /// </summary>
        public event EventHandler FaceDetected;

        public FaceDetectionService(ConfigService config)
        {
            _config = config;
        }

        public void Init()
        {
            try
            {
                // Initialize ONNX model from embedded resources
                var modelUri = new Uri(ResourcePaths.Models.FaceDetection, UriKind.RelativeOrAbsolute);
                var resourceInfo = System.Windows.Application.GetResourceStream(modelUri);

                if (resourceInfo != null)
                {
                    // Create a unique temporary file for the ONNX model since InferenceSession requires a file path
                    _tempModelPath = Path.Combine(Path.GetTempPath(), $"Lightweight-Face-Detection-{Guid.NewGuid()}.onnx");
                    
                    using (var fileStream = File.Create(_tempModelPath))
                    using (var resourceStream = resourceInfo.Stream)
                    {
                        resourceStream.CopyTo(fileStream);
                    }

                    var sessionOptions = new SessionOptions();
                    sessionOptions.RegisterOrtExtensions();
                    _inferenceSession = new InferenceSession(_tempModelPath, sessionOptions);
                    LogHelper.Info("Face detection model loaded successfully from resources");
                }
                else
                {
                    LogHelper.Warning("Face detection model resource not found");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Failed to initialize face detection service: {ex.Message}");
            }
        }

        /// <summary>
        /// Start face detection
        /// </summary>
        public void Start()
        {
            if (!_config.options.Behavior.IsFaceDetectionEnabled || _inferenceSession == null)
            {
                return;
            }

            if (_isRunning)
            {
                return;
            }

            try
            {
                // Store the current dispatcher to marshal events back to UI thread
                _uiDispatcher = Dispatcher.CurrentDispatcher;

                // Initialize camera
                _camera = new VideoCapture(0); // Use default camera
                if (!_camera.IsOpened)
                {
                    LogHelper.Warning("Could not open camera for face detection");
                    return;
                }

                _isRunning = true;
                _detectionThread = new Thread(DetectionLoop)
                {
                    IsBackground = true,
                    Name = "FaceDetectionThread"
                };
                _detectionThread.Start();

                LogHelper.Info("Face detection service started");
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Failed to start face detection: {ex.Message}");
                _isRunning = false;
            }
        }

        /// <summary>
        /// Stop face detection
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
            {
                return;
            }

            _isRunning = false;
            _wasFaceDetected = false;

            // Wait for thread to finish
            if (_detectionThread != null && _detectionThread.IsAlive)
            {
                _detectionThread.Join(ThreadJoinTimeoutMs);
            }

            // Release camera
            _camera?.Dispose();
            _camera = null;

            LogHelper.Info("Face detection service stopped");
        }

        /// <summary>
        /// Check if a face is currently detected.
        /// Note: When face detection is disabled in settings, this method returns true
        /// to assume the user is always present (backward compatibility with existing behavior).
        /// </summary>
        public bool IsFaceDetected()
        {
            lock (_lock)
            {
                return _faceDetected;
            }
        }

        private void DetectionLoop()
        {
            while (_isRunning)
            {
                try
                {
                    if (_camera != null && _camera.IsOpened)
                    {
                        using (Mat frame = new Mat())
                        {
                            _camera.Read(frame);
                            if (!frame.IsEmpty)
                            {
                                bool detected = DetectFaceInFrame(frame);
                                lock (_lock)
                                {
                                    // Fire event if face was just detected (transition from no face to face)
                                    if (detected && !_wasFaceDetected)
                                    {
                                        // Marshal the event back to the UI thread
                                        if (_uiDispatcher != null && !_uiDispatcher.HasShutdownStarted)
                                        {
                                            _uiDispatcher.BeginInvoke(
                                                () => FaceDetected?.Invoke(this, EventArgs.Empty),
                                                DispatcherPriority.Normal);
                                        }
                                    }
                                    
                                    _faceDetected = detected;
                                    _wasFaceDetected = detected;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"Error in face detection loop: {ex.Message}");
                }

                Thread.Sleep(DetectionIntervalMs);
            }
        }

        private bool DetectFaceInFrame(Mat frame)
        {
            try
            {
                if (_inferenceSession == null)
                {
                    return false;
                }

                var originalImageWidth = frame.Width;
                var originalImageHeight = frame.Height;

                var inputMetadataName = _inferenceSession.InputNames[0];
                var inputDimensions = _inferenceSession.InputMetadata[inputMetadataName].Dimensions;

                int modelInputHeight = inputDimensions[2];
                int modelInputWidth = inputDimensions[3];

                using Bitmap resizedImage = BitmapFunctions.ResizeVideoFrameWithPadding(frame, modelInputWidth, modelInputHeight);

                Tensor<float> input = new DenseTensor<float>([.. inputDimensions]);
                input = BitmapFunctions.PreprocessBitmapForFaceDetection(resizedImage, input);

                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor(inputMetadataName, input)
                };

                using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _inferenceSession.Run(inputs);
                {
                    var predictions = FaceHelpers.PostprocessFacialResults(results, originalImageWidth, originalImageHeight);
                    return predictions.Count > 0;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error detecting face in frame: {ex.Message}");
                return false;
            }
        }

        public void Dispose()
        {
            Stop();
            _inferenceSession?.Dispose();
            _inferenceSession = null;

            // Clean up temporary model file
            if (!string.IsNullOrEmpty(_tempModelPath) && File.Exists(_tempModelPath))
            {
                try
                {
                    File.Delete(_tempModelPath);
                    LogHelper.Info("Temporary model file cleaned up");
                }
                catch (Exception ex)
                {
                    LogHelper.Warning($"Failed to delete temporary model file: {ex.Message}");
                }
            }
        }
    }
}
