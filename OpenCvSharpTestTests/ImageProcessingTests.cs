using Xunit;
using OpenCvSharpTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using Xunit.Abstractions;
using OpenCvSharp;
using System.Linq;

namespace OpenCvSharpTest
{
    public class ImageProcessingTests : BaseTestClass
    {
        public ImageProcessingTests(ITestOutputHelper output) : base(output)
        { }
        [MemberData(nameof(ImagePaths))]
        [Theory()]
        public async Task DrawTransparentContourOnImage_Test(string imagePath)
        {
            var testObject = ImageProcessing.LoadImage(imagePath);

            var biggestContour = await testObject.FindContours_MultiChannel();
            using var contoured = ImageProcessing.DrawTransparentContour(testObject.Clone(), biggestContour);
            using var contouredImageOriginal = ImageProcessing.DrawContour(testObject.Clone(), biggestContour);

            testObject = testObject.CvtColor(ColorConversionCodes.BGR2BGRA);

            Cv2.AddWeighted(testObject, 0, contoured, 1, 0, testObject);

            SaveAndCheckIfSavedCorrect(contoured, Path.GetFileName(imagePath) + "contour");
            SaveAndCheckIfSavedCorrect(contouredImageOriginal, Path.GetFileName(imagePath) + "oryginalContour");
            SaveAndCheckIfSavedCorrect(testObject, Path.GetFileName(imagePath) + "transparentContour");
        }
        [MemberData(nameof(ImagePaths))]
        [Theory()]
        public async Task DrawTransparentContour_Test(string imagePath)
        {
            var testObject = ImageProcessing.LoadImage(imagePath);

            var biggestContour = await testObject.FindContours_MultiChannel();
            testObject = testObject.DrawTransparentContour(biggestContour);

            SaveAndCheckIfSavedCorrect(testObject, Path.GetFileName(imagePath));
        }
        [MemberData(nameof(ImagePaths))]
        [Theory()]
         public  async  Task  DrawContoursWithTransparentBackground_Test(string imagePath)
        {
            var testObject = ImageProcessing.LoadImage(imagePath);
            var testObject2 = testObject.Clone();
            
            //find contours
            var contour = await testObject.FindContours_MultiChannel();
            testObject = testObject.DrawContour(contour);
            //create founded rectangle on black background
            Cv2.AddWeighted(testObject, 1, testObject2, -1, 0, testObject);

            //create alpha channel
            var gray = testObject.CvtColor(ColorConversionCodes.BGR2GRAY);
            //SaveAndCheckIfSavedCorrect(gray, Path.GetFileName(imagePath) + "gray");
            var alpha = new Mat();
            Cv2.Threshold(gray, alpha, 10, 255, ThresholdTypes.Binary);
            SaveAndCheckIfSavedCorrect(alpha, Path.GetFileName(imagePath) + "treshold");
            //merge alpha and other channels
            var channels = testObject.Split().ToList();
            channels.Add(alpha);

            channels[0] = Mat.Zeros(new Size(channels[0].Cols, channels[0].Rows), MatType.CV_8UC1);
            channels[1] = Mat.Zeros(new Size(channels[1].Cols, channels[1].Rows), MatType.CV_8UC1);
            channels[2] = alpha;
            Cv2.Merge(channels.ToArray(), testObject);

            SaveAndCheckIfSavedCorrect(testObject, Path.GetFileName(imagePath));
            testObject.Dispose();
            testObject2.Dispose();
        }
        [MemberData(nameof(ImagePaths))]
        [Theory()]
        public void OpenCvCorrectLoadTest_ShouldLoadAndSaveImage_NoChangesToImage(string imagePath)
        {
            using var testObject = ImageProcessing.LoadImage(imagePath);
            Assert.NotNull(testObject);

            SaveAndCheckIfSavedCorrect(testObject, Path.GetFileName(imagePath));
        }

        [MemberData(nameof(ImagePaths))]
        [Theory()]
        public void ProccessToGrayContuourTest(string imagePath)
        {
            using var testObject = ImageProcessing.LoadImage(imagePath);
            var result = testObject.ProccessToGrayContuour();

            SaveAndCheckIfSavedCorrect(result, Path.GetFileName(imagePath));
        }

        [MemberData(nameof(ImagePaths))]
        [Theory()]
        public async Task FindContours_MultiChannel_DrawContour_Test(string imagePath)
        {
            var testObject = ImageProcessing.LoadImage(imagePath);

            var contours = await testObject.FindContours_MultiChannel();

            testObject = testObject.DrawContour(contours);
            SaveAndCheckIfSavedCorrect(testObject, Path.GetFileName(imagePath));
            testObject.Dispose();
        }

        [InlineData(10, 1000, 10, 1000, TestImageRecipe)]
        [Theory()]
        public  void DrawContourTest_RandomContour(int xLeft, int xRight, int yUp, int yDown, string imagePath)
        {
            using var testObject = ImageProcessing.LoadImage(imagePath);
            using var result = ImageProcessing.DrawContour(testObject.Clone(), new List<Point>
            {
                new Point(xLeft, yUp) ,
                new Point(xRight, yUp) ,
                new Point(xRight, yDown),
                new Point(xLeft, yDown) ,
            });

            SaveAndCheckIfSavedCorrect(result, $"{xLeft}x{xRight}x{yUp}x{yDown}_{Path.GetFileName(imagePath)}");
        }

        [InlineData(10, 1000, 10, 1000, TestImageRecipe)]
        [InlineData(2000, 1000, 10, 1000, TestImageRecipe)]
        [InlineData(2000, 4000, 2000, 4000, TestImageRecipe)]
        [InlineData(8000, 1000, 8000, 1000, TestImageRecipe)]
        [Theory()]
        public  void TransformTest_Rectangle(int xLeft, int xRight, int yUp, int yDown, string imagePath)
        {
            using var testObject = ImageProcessing.LoadImage(imagePath);

            var pointsOfFragment = new Point2f[]
            {
                new Point2f(xLeft, yUp) ,
                new Point2f(xRight, yUp) ,
                new Point2f(xRight, yDown),
                new Point2f(xLeft, yDown) ,
            };
            var pointsOfDestination = new Point2f[]
            {
                new Point2f(0, 0),
                new Point2f(testObject.Width, 0),
                new Point2f(testObject.Width, testObject.Height),
                new Point2f(0, testObject.Height)
            };

            using var contouredImage = ImageProcessing.DrawContour(testObject.Clone(), new List<Point>
            {
                new Point(xLeft, yUp) ,
                new Point(xRight, yUp) ,
                new Point(xRight, yDown),
                new Point(xLeft, yDown)
            });
            using var result = ImageProcessing.Transform(testObject, pointsOfFragment.ToList());


            SaveAndCheckIfSavedCorrect(contouredImage, $"{xLeft}x{xRight}x{yUp}x{yDown}_{Path.GetFileName(imagePath)}_orgin");
            SaveAndCheckIfSavedCorrect(result, $"{xLeft}x{xRight}x{yUp}x{yDown}_{Path.GetFileName(imagePath)}_result");
        }

        [InlineData(10, 10, 1000, 1000, 1200, 2000, 500, 1800, TestImageRecipe)]
        [Theory()]
        public  void TransformTest_UnnormalShape(int point1x, int point1y, int point2x, int point2y, int point3x, int point3y, int point4x, int point4y, string imagePath)
        {
            using var testObject = ImageProcessing.LoadImage(imagePath);

            var pointsOfFragment = new Point2f[]
            {
                new Point2f(point1x, point1y) ,
                new Point2f(point2x, point2y) ,
                new Point2f(point3x, point3y),
                new Point2f(point4x, point4y)
            };
            var pointsOfDestination = new Point2f[]
            {
                new Point2f(0, 0),
                new Point2f(testObject.Width, 0),
                new Point2f(testObject.Width, testObject.Height),
                new Point2f(0, testObject.Height)
            };

            using var contouredImage = ImageProcessing.DrawContour(testObject.Clone(), new List<Point>
            {
                new Point(point1x, point1y) ,
                new Point(point2x, point2y) ,
                new Point(point3x, point3y),
                new Point(point4x, point4y)
            });
            using var result = ImageProcessing.Transform(testObject, pointsOfFragment.ToList());


            SaveAndCheckIfSavedCorrect(contouredImage, $"{point1x}x{point2x}x{point3x}x{point4x}_{Path.GetFileName(imagePath)}_orgin");
            SaveAndCheckIfSavedCorrect(result, $"{point1x}x{point2x}x{point3x}x{point4x}_{Path.GetFileName(imagePath)}_result");
        }

    }
}