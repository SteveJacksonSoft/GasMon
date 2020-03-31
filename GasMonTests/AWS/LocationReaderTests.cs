using System.Linq;
using GasMonPersonal.AWS;
using NUnit.Framework;

namespace GasMonTests.AWS
{
    public class LocationReaderTests
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void ParsesASeriesOfLocations()
        {
            // given
            var locationsJson =
                @"[{""x"":3.4,""y"":1.3,""id"":""testLocation1""},{""x"":44.331,""y"":877,""id"":""testLocation2""}]";
            
            // when
            var parsedJson = new LocationReader().Read(locationsJson).ToList();
            
            // then
            Assert.AreEqual(3.4, parsedJson.First().X);
            Assert.AreEqual(1.3, parsedJson.First().Y);
            Assert.AreEqual("testLocation1", parsedJson.First().Id);

            Assert.AreEqual(44.331, parsedJson.Last().X);
            Assert.AreEqual(877, parsedJson.Last().Y);
            Assert.AreEqual("testLocation2", parsedJson.Last().Id);
        }
    }
}