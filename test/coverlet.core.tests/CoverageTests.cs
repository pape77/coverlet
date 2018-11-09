using System;
using System.IO;
using Xunit;

namespace Coverlet.Core.Tests
{
    public class CoverageTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestCoverage(bool excludeNonCalledFilesValue)
        {
            string module = GetType().Assembly.Location;
            string pdb = Path.Combine(Path.GetDirectoryName(module), Path.GetFileNameWithoutExtension(module) + ".pdb");

            var directory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

            File.Copy(module, Path.Combine(directory.FullName, Path.GetFileName(module)), true);
            File.Copy(pdb, Path.Combine(directory.FullName, Path.GetFileName(pdb)), true);

            // TODO: Find a way to mimic hits

            // Since Coverage only instruments dependencies, we need a fake module here
            var testModule = Path.Combine(directory.FullName, "test.module.dll");

            var coverage = new Coverage(testModule, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), string.Empty, excludeNonCalledFilesValue);
            coverage.PrepareModules();

            var result = coverage.GetCoverageResult();

            if (excludeNonCalledFilesValue)
            {
                Assert.Empty(result.Modules);
            }
            else
            {
                Assert.NotEmpty(result.Modules);
            }

            directory.Delete(true);
        }
    }
}