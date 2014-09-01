namespace InpcTracer.NTests.Tracing
{
  using FakeItEasy;
  using InpcTracer.Tracing;
  using NUnit.Framework;
  using System.Collections.Generic;

  [TestFixture]
  public class NotificationComparerTests
  {
    private IEqualityComparer<INotification> notificationComparer;

    [SetUp]
    public void Setup()
    {
      this.notificationComparer = new NotificationComparer();
    }

    [Test]
    public void Should_validate_when_same()
    {
      //Arrange
      INotification a = A.Fake<INotification>();
      A.CallTo(() => a.PropertyName).Returns("a");
      INotification b = A.Fake<INotification>();
      A.CallTo(() => b.PropertyName).Returns("a");

      //Act

      //Assert
      Assert.That(this.notificationComparer.Equals(a, b), Is.True);
      Assert.That(this.notificationComparer.GetHashCode(a), Is.EqualTo(this.notificationComparer.GetHashCode(b)));
    }

    [Test]
    public void Should_invalidate_when_different()
    {
      //Arrange
      INotification a = A.Fake<INotification>();
      A.CallTo(() => a.PropertyName).Returns("a");
      INotification b = A.Fake<INotification>();
      A.CallTo(() => b.PropertyName).Returns("b");

      //Act

      //Assert
      Assert.That(this.notificationComparer.Equals(a, b), Is.False);
      Assert.That(this.notificationComparer.GetHashCode(a), Is.Not.EqualTo(this.notificationComparer.GetHashCode(b)));
    }
  }
}