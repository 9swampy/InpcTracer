namespace InpcTracer.NTests.Configuration
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq.Expressions;
  using NUnit.Framework;
  using Guard = InpcTracer.Framework.Guard;
  using InpcTracer.Configuration;

  [TestFixture]
  public class RepeatedTests
  {
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used reflectively.")]
    private object[] descriptionTestCases = TestCases.Create(
        new RepeatDescriptionTestCase()
        {
          Repeat = () => Notified.AtLeast.Once,
          ExpectedDescription = "at least once"
        },
        new RepeatDescriptionTestCase()
        {
          Repeat = () => Notified.AtLeast.Twice,
          ExpectedDescription = "at least twice"
        },
        new RepeatDescriptionTestCase()
        {
          Repeat = () => Notified.AtLeast.Times(3),
          ExpectedDescription = "at least 3 times"
        },
        new RepeatDescriptionTestCase()
        {
          Repeat = () => Notified.AtLeast.Times(10),
          ExpectedDescription = "at least 10 times"
        },
        new RepeatDescriptionTestCase()
        {
          Repeat = () => Notified.AtLeast.Times(10),
          ExpectedDescription = "at least 10 times"
        },
        new RepeatDescriptionTestCase()
        {
          Repeat = () => Notified.NoMoreThan.Once,
          ExpectedDescription = "no more than once"
        },
        new RepeatDescriptionTestCase()
        {
          Repeat = () => Notified.NoMoreThan.Twice,
          ExpectedDescription = "no more than twice"
        },
        new RepeatDescriptionTestCase()
        {
          Repeat = () => Notified.NoMoreThan.Times(10),
          ExpectedDescription = "no more than 10 times"
        },
        new RepeatDescriptionTestCase()
        {
          Repeat = () => Notified.Exactly.Once,
          ExpectedDescription = "exactly once"
        },
        new RepeatDescriptionTestCase()
        {
          Repeat = () => Notified.Exactly.Twice,
          ExpectedDescription = "exactly twice"
        },
        new RepeatDescriptionTestCase()
        {
          Repeat = () => Notified.Exactly.Times(99),
          ExpectedDescription = "exactly 99 times"
        }).AsTestCaseSource();

    [TestCase(1, 1, Result = true)]
    [TestCase(1, 2, Result = false)]
    public bool LikeShouldReturnInstanceThatDelegatesToExpression(int expected, int actual)
    {
      // Arrange
      Expression<Func<int, bool>> repeatPredicate = repeat => repeat == expected;

      // Act
      var happened = Notified.Like(repeatPredicate);

      // Assert
      return happened.Matches(actual);
    }

    [Test]
    public void LikeShouldReturnInstanceThatHasCorrectDescription()
    {
      // Arrange
      Expression<Func<int, bool>> repeatPredicate = repeat => repeat == 1;

      // Act
      var happened = Notified.Like(repeatPredicate);

      // Assert
      Assert.That(happened.ToString(), Is.EqualTo("the number of times specified by the predicate 'repeat => (repeat == 1)'"));
    }

    [TestCase(1, Result = true)]
    [TestCase(2, Result = false)]
    public bool ExactlyOnceShouldOnlyMatchOne(int actualRepeat)
    {
      // Arrange

      // Act
      var repeated = Notified.Exactly.Once;

      // Assert
      return repeated.Matches(actualRepeat);
    }

    [TestCase(2, Result = true)]
    [TestCase(0, Result = false)]
    public bool ExactlyTwiceShouldOnlyMatchTwo(int actualRepeat)
    {
      // Arrange

      // Act
      var repeated = Notified.Exactly.Twice;

      // Assert
      return repeated.Matches(actualRepeat);
    }

    [TestCase(0, 0, Result = true)]
    [TestCase(0, 1, Result = false)]
    public bool ExactlyNumberOfTimesShouldMatchAsExpected(int actualRepeat, int expectedNumberOfTimes)
    {
      // Arrange

      // Act
      var repeated = Notified.Exactly.Times(expectedNumberOfTimes);

      // Assert
      return repeated.Matches(actualRepeat);
    }

    [TestCase(1, Result = true)]
    [TestCase(0, Result = false)]
    [TestCase(2, Result = true)]
    public bool AtLeastOnceShouldMatchOneOrHigher(int actualRepeat)
    {
      // Arrange

      // Act
      var repeated = Notified.AtLeast.Once;

      // Assert
      return repeated.Matches(actualRepeat);
    }

    [TestCase(1, Result = false)]
    [TestCase(0, Result = false)]
    [TestCase(2, Result = true)]
    [TestCase(3, Result = true)]
    public bool AtLeastTwiceShouldOnlyMatchTwoOrHigher(int actualRepeat)
    {
      // Arrange

      // Act
      var repeated = Notified.AtLeast.Twice;

      // Assert
      return repeated.Matches(actualRepeat);
    }

    [TestCase(0, 0, Result = true)]
    [TestCase(1, 0, Result = true)]
    [TestCase(1, 1, Result = true)]
    [TestCase(0, 1, Result = false)]
    [TestCase(2, 1, Result = true)]
    public bool AtLeastNumberOfTimesShouldMatchAsExpected(int actualRepeat, int expectedNumberOfTimes)
    {
      // Arrange

      // Act
      var repeated = Notified.AtLeast.Times(expectedNumberOfTimes);

      // Assert
      return repeated.Matches(actualRepeat);
    }

    [TestCase(0, Result = true)]
    [TestCase(1, Result = true)]
    [TestCase(2, Result = false)]
    public bool NoMoreThanOnceShouldMatchZeroAndOneOnly(int actualRepeat)
    {
      // Arrange

      // Act
      var repeated = Notified.NoMoreThan.Once;

      // Assert
      return repeated.Matches(actualRepeat);
    }

    [TestCase(0, Result = true)]
    [TestCase(1, Result = true)]
    [TestCase(2, Result = true)]
    [TestCase(3, Result = false)]
    public bool NoMoreThanTwiceShouldMatchZeroOneAndTwoOnly(int actualRepeat)
    {
      // Arrange

      // Act
      var repeated = Notified.NoMoreThan.Twice;

      // Assert
      return repeated.Matches(actualRepeat);
    }

    [TestCase(0, 0, Result = true)]
    [TestCase(1, 0, Result = false)]
    [TestCase(1, 1, Result = true)]
    [TestCase(0, 1, Result = true)]
    [TestCase(2, 1, Result = false)]
    public bool NoMoreThanTimesShouldMatchAsExpected(int actualRepeat, int expectedNumberOfTimes)
    {
      // Arrange

      // Act
      var repeated = Notified.NoMoreThan.Times(expectedNumberOfTimes);

      // Assert
      return repeated.Matches(actualRepeat);
    }

    [TestCase(0, Result = true)]
    [TestCase(1, Result = false)]
    public bool NeverShouldMatchZeroOnly(int actualRepeat)
    {
      // Arrange

      // Act

      // Assert
      return Notified.Never.Matches(actualRepeat);
    }

    [TestCaseSource("descriptionTestCases")]
    public void ShouldProvideExpectedDescription(Func<Notified> repeated, string expectedDescription)
    {
      Guard.AgainstNull(repeated, "repeated");

      // Arrange
      var repeatedInstance = repeated.Invoke();

      // Act
      var description = repeatedInstance.ToString();

      // Assert
      Assert.That(description, Is.EqualTo(expectedDescription));
    }

    private class RepeatDescriptionTestCase
    {
      [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used reflecively.")]
      public Func<Notified> Repeat { get; set; }

      [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used reflecively.")]
      public string ExpectedDescription { get; set; }
    }
  }
}
