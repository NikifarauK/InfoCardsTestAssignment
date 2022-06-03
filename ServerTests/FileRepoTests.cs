using Server.Data;
using NUnit.Compatibility;
using NUnit.Framework;
using Server.Models;

namespace Server.Tests
{
    [TestFixture]
    internal class FileRepoTests
    {
        public JsonFileRepo Repo { get; set; } 
            = new JsonFileRepo(@".\test.json");

        [Test]
        public void InsertTest()
        {
            var insetrted = new CardDto() { Id = 1, Name = "name", Base64Image = "123456789" };
            Repo.Insert(insetrted);
            var getted = Repo.Get(1);
            Assert.That(getted.Id, Is.EqualTo(1));
        }

        [Test]
        public void EmptySavingTest()
        {
            var before = Repo.GetAll();
            Repo.SaveChanges();
            var after = Repo.GetAll();
            Assert.That(before, Is.EqualTo(after));
        }

        [Test]
        public void UpdateTest()
        {
            var after = new CardDto { Id = 1, Name = "name2", Base64Image = "987654321" };
            var before = Repo.Get(1);
            Repo.Update(after);
            var update = Repo.Get(1);
            Assert.That(after, Is.EqualTo(update));
            Assert.That(before, !Is.EqualTo(after));
        }

        [Test]
        public void DeleteTest()
        {
            Repo.Delete(1);
            var getted = Repo.Get(1);
            Assert.That(getted, Is.EqualTo(null));
        }
    }
}
