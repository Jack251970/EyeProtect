using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using EyeProtect.Models.FaceDetection;

namespace EyeProtect.Core.Helpers
{
    public static class FaceHelpers
    {
        private const float ConfidenceThreshold = 0.7f;
        private const float IouThreshold = 0.5f;

        public static List<Prediction> PostprocessFacialResults(
            IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results,
            int originalWidth,
            int originalHeight)
        {
            var predictions = new List<Prediction>();

            try
            {
                // Extract scores and boxes from the model output
                var scoresOutput = results.FirstOrDefault(r => r.Name == "scores");
                var boxesOutput = results.FirstOrDefault(r => r.Name == "boxes");

                if (scoresOutput == null || boxesOutput == null)
                {
                    return predictions;
                }

                var scores = scoresOutput.AsEnumerable<float>().ToArray();
                var boxes = boxesOutput.AsEnumerable<float>().ToArray();

                // Process predictions
                int numDetections = scores.Length;
                for (int i = 0; i < numDetections; i++)
                {
                    float confidence = scores[i];
                    if (confidence < ConfidenceThreshold)
                        continue;

                    // Extract bounding box coordinates
                    // Format depends on model output, typically [x_min, y_min, x_max, y_max]
                    int boxOffset = i * 4;
                    if (boxOffset + 3 >= boxes.Length)
                        continue;

                    var prediction = new Prediction
                    {
                        Confidence = confidence,
                        Box = new Prediction.BoundingBox
                        {
                            Xmin = boxes[boxOffset] * originalWidth,
                            Ymin = boxes[boxOffset + 1] * originalHeight,
                            Xmax = boxes[boxOffset + 2] * originalWidth,
                            Ymax = boxes[boxOffset + 3] * originalHeight
                        }
                    };

                    predictions.Add(prediction);
                }

                // Apply Non-Maximum Suppression (NMS) to remove overlapping boxes
                predictions = ApplyNMS(predictions, IouThreshold);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error postprocessing face detection results: {ex.Message}");
            }

            return predictions;
        }

        private static List<Prediction> ApplyNMS(List<Prediction> predictions, float iouThreshold)
        {
            // Sort by confidence descending
            predictions = predictions.OrderByDescending(p => p.Confidence).ToList();
            
            var selected = new List<Prediction>();
            
            while (predictions.Count > 0)
            {
                var current = predictions[0];
                selected.Add(current);
                predictions.RemoveAt(0);

                predictions = predictions.Where(p => CalculateIoU(current.Box, p.Box) < iouThreshold).ToList();
            }

            return selected;
        }

        private static float CalculateIoU(Prediction.BoundingBox box1, Prediction.BoundingBox box2)
        {
            float x1 = Math.Max(box1.Xmin, box2.Xmin);
            float y1 = Math.Max(box1.Ymin, box2.Ymin);
            float x2 = Math.Min(box1.Xmax, box2.Xmax);
            float y2 = Math.Min(box1.Ymax, box2.Ymax);

            float intersectionArea = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
            float box1Area = (box1.Xmax - box1.Xmin) * (box1.Ymax - box1.Ymin);
            float box2Area = (box2.Xmax - box2.Xmin) * (box2.Ymax - box2.Ymin);
            float unionArea = box1Area + box2Area - intersectionArea;

            // Guard against division by zero
            if (unionArea <= 0.0f)
            {
                return 0.0f;
            }

            return intersectionArea / unionArea;
        }
    }
}
