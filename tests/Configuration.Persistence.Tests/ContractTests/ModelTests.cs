#pragma warning disable SA1402 // File may only contain a single type

namespace Kritikos.Configuration.Persistence.Tests.ContractTests;

using Kritikos.Configuration.Persistence.Contracts.Base;

public class ModelTests
{
  [Test]
  public async Task Equals_SameInstance_IsTrue()
  {
    var model = new SampleModel { Id = 1 };

    await Assert.That(model.Equals(model)).IsTrue();
  }

  [Test]
  public async Task Equals_SameIdentity_IsTrue()
  {
    var left = new SampleModel { Id = 7 };
    var right = new SampleModel { Id = 7 };

    await Assert.That(left.Equals(right)).IsTrue();
    await Assert.That(left.Equals((object)right)).IsTrue();
  }

  [Test]
  public async Task Equals_DifferentIdentity_IsFalse()
  {
    var left = new SampleModel { Id = 7 };
    var right = new SampleModel { Id = 8 };

    await Assert.That(left.Equals(right)).IsFalse();
  }

  [Test]
  public async Task Equals_NullModel_IsFalse()
  {
    var model = new SampleModel { Id = 1 };

    await Assert.That(model.Equals(null)).IsFalse();
  }

  [Test]
  public async Task Equals_NullObject_IsFalse()
  {
    var model = new SampleModel { Id = 1 };
    object? nullObject = null;

    await Assert.That(model.Equals(nullObject)).IsFalse();
  }

  [Test]
  public async Task Equals_UnrelatedType_IsFalse()
  {
    var model = new SampleModel { Id = 1 };

    await Assert.That(model.Equals("1")).IsFalse();
  }

  [Test]
  public async Task Equals_DifferentDerivedTypeWithTheSameIdentity_IsTrue()
  {
    var left = new SampleModel { Id = 3 };
    var right = new OtherSampleModel { Id = 3 };

    // Identity is the only discriminator, so sibling models sharing a key compare equal.
    await Assert.That(left.Equals(right)).IsTrue();
  }

  [Test]
  public async Task GetHashCode_SameIdentity_ProducesTheSameValue()
  {
    var left = new SampleModel { Id = 7 };
    var right = new SampleModel { Id = 7 };

    await Assert.That(left.GetHashCode()).IsEqualTo(right.GetHashCode());
  }

  [Test]
  public async Task EqualityOperator_SameIdentity_IsTrue()
  {
    var left = new SampleModel { Id = 7 };
    var right = new SampleModel { Id = 7 };

    await Assert.That(left == right).IsTrue();
    await Assert.That(left != right).IsFalse();
  }

  [Test]
  public async Task EqualityOperator_DifferentIdentity_IsFalse()
  {
    var left = new SampleModel { Id = 7 };
    var right = new SampleModel { Id = 8 };

    await Assert.That(left == right).IsFalse();
    await Assert.That(left != right).IsTrue();
  }

  [Test]
  public async Task EqualityOperator_BothNull_IsTrue()
  {
    SampleModel? left = null;
    SampleModel? right = null;

    await Assert.That(left == right).IsTrue();
    await Assert.That(left != right).IsFalse();
  }

  [Test]
  public async Task EqualityOperator_OneNull_IsFalse()
  {
    var model = new SampleModel { Id = 1 };
    SampleModel? missing = null;

    await Assert.That(model == missing).IsFalse();
    await Assert.That(missing == model).IsFalse();
    await Assert.That(model != missing).IsTrue();
  }
}

public class SampleModel : Model<int>;

public class OtherSampleModel : Model<int>;
