namespace Kritikos.Configuration.Persistence.Dto.Base
{
  using System;
  using System.Collections.Generic;

  using Kritikos.Configuration.Persistence.Dto.Abstractions;

  public abstract record BaseRecordModel<TKey>(TKey Id);

  public abstract class BaseModel<TKey>
    : IIdentity<TKey>,
      IEquatable<BaseModel<TKey>>,
      IEqualityComparer<BaseModel<TKey>>
    where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
  {
#nullable disable // Handled by EF and/or deserialization
    public TKey Id { get; init; }
#nullable enable // Handled by EF and/or deserialization

    public virtual bool Equals(BaseModel<TKey>? other) =>
      other is not null && (ReferenceEquals(this, other)
                            || (other.GetType() == this.GetType()
                                && EqualityComparer<TKey>.Default.Equals(Id, other.Id)));

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
      obj is not null && (ReferenceEquals(this, obj)
                          || (obj is BaseModel<TKey> model && EqualityComparer<TKey>.Default.Equals(Id, model.Id)));

    public override int GetHashCode() => EqualityComparer<TKey>.Default.GetHashCode(Id);

    public bool Equals(BaseModel<TKey>? x, BaseModel<TKey>? y)
      => x?.Equals(y) ?? false;

    public int GetHashCode(BaseModel<TKey> obj) => EqualityComparer<TKey>.Default.GetHashCode(obj.Id);
  }
}
