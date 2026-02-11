namespace EyeProtect.Models.FaceDetection
{
    public class Prediction
    {
        public BoundingBox Box { get; set; }
        public float Confidence { get; set; }

        public class BoundingBox
        {
            public float Xmin { get; set; }
            public float Ymin { get; set; }
            public float Xmax { get; set; }
            public float Ymax { get; set; }
        }
    }
}
