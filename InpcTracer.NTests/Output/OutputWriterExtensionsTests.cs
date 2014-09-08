namespace InpcTracer.NTests.Output
{
  using System;
  using FluentAssertions;
  using NUnit.Framework;
  using FakeItEasy;

  [TestFixture]
  public class OutputWriterExtensionsTests
  {
    [Test]
    public void ShouldAppendLineBreakWhenCallingWriteLine()
    {
      // Arrange
      var writer = A.Fake<IOutputWriter>();

      // Act
      writer.WriteLine();

      // Assert
      A.CallTo(() => writer.Write(Environment.NewLine)).MustHaveHappened();
    }

    [Test]
    public void ShouldReturnSameInstanceWhenCallingWriteLine()
    {
      // Arrange
      var writer = A.Dummy<IOutputWriter>();

      // Act
      var result = writer.WriteLine();

      // Assert
      result.Should().BeSameAs(writer);
    }
  }
}