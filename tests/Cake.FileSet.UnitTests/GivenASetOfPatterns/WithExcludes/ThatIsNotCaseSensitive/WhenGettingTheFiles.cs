using System.Collections.Generic;
using System.Linq;
using Cake.Core.IO;
using Cake.FileSet.UnitTests.Framework;
using NUnit.Framework;

namespace Cake.FileSet.UnitTests.GivenASetOfPatterns.WithExcludes.ThatIsNotCaseSensitive
{
    [TestFixture]
    public class WhenGettingTheFiles
    {
        private List<FilePath> _result;

        [SetUp]
        public void SetUp()
        {
            var testDirectory = TestDirectory.Root("Cake.FileSet", "D:/code/git/Cake.FileSet")
                .Add("src/a/one.csproj")
                .Add("src/b/ONE.csproj")
                .Add("src/c/one.csproj")
                .Add("src/c/two.csproj");

            var includes = new List<string>
            {
                "**/*.csproj"
            };

            var excludes = new List<string>
            {
                "**/c/*.csproj"
            };

            var subject = new FileSet(testDirectory, includes, excludes, false);
            _result = subject.GetFiles().ToList();
        }

        [Test]
        public void ThenOnlyMatchingFilesAreReturned()
        {
            Assert.That(_result.Count, Is.EqualTo(2));
        }

        [Test]
        public void ThenTheMatchingFilesAreReturned()
        {
            Assert.That(_result.Count(x => x.FullPath == "D:/code/git/Cake.FileSet/src/a/one.csproj"), Is.EqualTo(1));
            Assert.That(_result.Count(x => x.FullPath == "D:/code/git/Cake.FileSet/src/b/ONE.csproj"), Is.EqualTo(1));
        }
    }
}