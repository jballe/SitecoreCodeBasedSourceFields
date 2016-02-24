using System.Collections.Generic;
using NUnit.Framework;

namespace SitecoreCodeSourceFields.UnitTests
{
    public class DataLoaderShould
    {
        [Test]
        public void LoadEnumerableStringProperty()
        {
            //// Arrange
            var datasource = "SitecoreCodeSourceFields.UnitTests.TestDataModel.MyStringProperty, SitecoreCodeSourceFields.UnitTests";

            //// Act
            var result = DataLoader.GetKeyValue(datasource);

            //// Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EquivalentTo(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("1", "1"),
                new KeyValuePair<string, string>("2", "2"),
                new KeyValuePair<string, string>("3", "3")
            }));
        }

        [Test]
        public void LoadDictionaryStringProperty()
        {
            //// Arrange
            var datasource = "SitecoreCodeSourceFields.UnitTests.TestDataModel.MyDictionaryProperty, SitecoreCodeSourceFields.UnitTests";

            //// Act
            var result = DataLoader.GetKeyValue(datasource);

            //// Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EquivalentTo(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("2", "Two"),
                new KeyValuePair<string, string>("3", "Three")
            }));
        }

        [Test]
        public void LoadNullProperty()
        {
            //// Arrange
            var datasource = "SitecoreCodeSourceFields.UnitTests.TestDataModel.NullProperty, SitecoreCodeSourceFields.UnitTests";

            //// Act
            var result = DataLoader.GetKeyValue(datasource);

            //// Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EquivalentTo(new List<KeyValuePair<string, string>>(0)));
        }

    }

    public class TestDataModel
    {
        public static IEnumerable<string> MyStringProperty
        {
            get { return new List<string> {"1", "2", "3"}; }
        }

        public static Dictionary<string,string> MyDictionaryProperty
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"2", "Two"},
                    {"3", "Three"}
                };
            }
        }

        public static IEnumerable<string> NullProperty
        {
            get { return null; }
        }

    }
}
