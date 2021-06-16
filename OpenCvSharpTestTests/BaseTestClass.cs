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

namespace OpenCvSharpTest
{
    public class BaseTestClass
    {
        public readonly ITestOutputHelper output;

        public BaseTestClass(ITestOutputHelper output)
        {
            this.output = output;
        }
        public const string TestPath = @"C:\Users\mateu\OneDrive\Desktop\QR\documents";
        public const string TestImageRecipe = @"C:\Users\mateu\OneDrive\Desktop\QR\documents\recepta_test1.png";
        public const string TestImageRevo = @"C:\Users\mateu\OneDrive\Desktop\QR\documents\rewol_test.png";
        public const string TestImageRevo2 = @"C:\Users\mateu\OneDrive\Desktop\QR\documents\revol2_test.png";
        public const string TestImageRevo3 = @"C:\Users\mateu\OneDrive\Desktop\QR\documents\revol3_test.png";
        public static IEnumerable<object[]> ImagePaths =>
            new List<object[]>
            {
            new object[] { TestImageRecipe },
            new object[] { TestImageRevo },
            new object[] { TestImageRevo2 },
            new object[] { TestImageRevo3 },
            };

        public static string TestImageDirectory(string additionalInfo = "", [CallerMemberName] string testName = "")
        {

            var path = TestPath;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            path = Path.Combine(path, testName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var nameOfFile = string.IsNullOrWhiteSpace(additionalInfo) ? $"{testName}.png" : $"{additionalInfo}_{testName}.png";

            return Path.Combine(path, nameOfFile);
        }
        public static void SaveAndCheckIfSavedCorrect(OpenCvSharp.Mat result, string additionalInfo = "", [CallerMemberName] string name = "")
        {
            var savePath = TestImageDirectory(additionalInfo, name);
            result.SaveImage(savePath);
            Assert.True(File.Exists(savePath));
        }
    }
}
