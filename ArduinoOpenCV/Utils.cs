using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoOpenCV
{
    internal class Utils
    {
        static string cascadePath = "C:\\Users\\Ucenik2\\Desktop\\haarcascade_frontalface_alt.xml";

        public static void SaveFaces(String path, List<Mat> faces)
        {
            for (int i = 0; i < faces.Count; i++)
            {
                Cv2.ImWrite($"{path}\\face_{i}.png", faces[i]);
            }
        }

        public static Mat DrawFaceRects(Mat img, Rect[] faces)
        {
            foreach (var face in faces)
            {
                int padding = 20;
                Rect enlargedRect = new Rect(
                    Math.Max(0, face.X - padding),
                    Math.Max(0, face.Y - padding),
                    Math.Min(img.Cols - 1, face.Width + 2 * padding),
                    Math.Min(img.Rows - 1, face.Height + 2 * padding)
                );

                Cv2.Rectangle(img, enlargedRect, Scalar.Green, 2);
            }
            return img;
        }

        public static Rect[] DetectFacesOnImage(Mat img)
        {
            CascadeClassifier faceCascade = new CascadeClassifier(cascadePath);

            if (faceCascade.Empty())
            {
                Console.WriteLine("Error loading face cascade.");
                return new Rect[] { };
            }

            Mat grayImage = new Mat();
            Cv2.CvtColor(img, grayImage, ColorConversionCodes.BGR2GRAY);
            Cv2.EqualizeHist(grayImage, grayImage);

            Rect[] faces = faceCascade.DetectMultiScale(
                grayImage, 1.1, 2, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(30, 30)
            );

            grayImage.Dispose();

            return faces;
        }

        public static List<Mat> GetFaceFromRects(Mat img, Rect[] faces, int padding = 10)
        {
            List<Mat> faceROIs = new List<Mat>();

            foreach (var face in faces)
            {
                Rect enlargedRect = new Rect(
                    Math.Max(0, face.X - padding),
                    Math.Max(0, face.Y - padding),
                    Math.Min(img.Cols - 1, face.Width + 2 * padding),
                    Math.Min(img.Rows - 1, face.Height + 2 * padding)
                );

                Mat faceROI = img.Clone(enlargedRect);
                faceROIs.Add(faceROI);
            }

            return faceROIs;
        }

        public static Image ResizeImage(Image originalImage, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                graphics.DrawImage(originalImage, 0, 0, width, height);
            }

            return resizedImage;
        }
    }
}
