using System.Reflection;
using NUnit.Framework;

namespace SitecoreCodeSourceFields.UnitTests
{
    public class DataSourceModelShould
    {
        [Test]
        public void ParseClassMethodAndAssembly()
        {
            //// Arrange
            var datasource = "Mynamespace.MyClass.MyMethod(), MyName.MyAssembly";

            //// Act
            var result = DataSourceModel.Parse(datasource);

            //// Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ClassName, Is.EqualTo("Mynamespace.MyClass"));
            Assert.That(result.AssemblyName, Is.EqualTo("MyName.MyAssembly"));
            Assert.That(result.MemberName, Is.EqualTo("MyMethod"));
            Assert.That(result.MemberType, Is.EqualTo(MemberTypes.Method));
        }

        [Test]
        public void ParseClassPropertyAndAssembly()
        {
            //// Arrange
            var datasource = "Mynamespace.MyClass.MyProp, MyName.MyAssembly";

            //// Act
            var result = DataSourceModel.Parse(datasource);

            //// Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ClassName, Is.EqualTo("Mynamespace.MyClass"));
            Assert.That(result.AssemblyName, Is.EqualTo("MyName.MyAssembly"));
            Assert.That(result.MemberName, Is.EqualTo("MyProp"));
            Assert.That(result.MemberType, Is.EqualTo(MemberTypes.Property));
        }

        [Test]
        public void ParseClassAndMethod()
        {
            //// Arrange
            var datasource = "Mynamespace.MyClass.MyMethod()";

            //// Act
            var result = DataSourceModel.Parse(datasource);

            //// Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ClassName, Is.EqualTo("Mynamespace.MyClass"));
            Assert.That(result.AssemblyName, Is.Null);
            Assert.That(result.MemberName, Is.EqualTo("MyMethod"));
            Assert.That(result.MemberType, Is.EqualTo(MemberTypes.Method));
        }

        [Test]
        public void ParseClassAndProperty()
        {
            //// Arrange
            var datasource = "Mynamespace.MyClass.MyProp";

            //// Act
            var result = DataSourceModel.Parse(datasource);

            //// Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ClassName, Is.EqualTo("Mynamespace.MyClass"));
            Assert.That(result.AssemblyName, Is.Null);
            Assert.That(result.MemberName, Is.EqualTo("MyProp"));
            Assert.That(result.MemberType, Is.EqualTo(MemberTypes.Property));
        }

    }
}
