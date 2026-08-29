# Configuration.Persistence.Converters

`ValueConverter` implementations for types Entity Framework Core does not map out of the box.

## Getting Started

```shell
dotnet add package Kritikos.Configuration.Persistence.Converters
```

Every converter is applied the same way, on the property that needs it.

```csharp
builder.Entity<Document>()
  .Property(x => x.Location)
  .HasConversion(new FileInfoToStringConverter('/', new DirectoryInfo("/srv/storage")));
```

## Capabilities

| Converter | Maps | Constructor arguments |
| --- | --- | --- |
| `DirectoryInfoToStringConverter` | `DirectoryInfo` to `string` | `separator`, optional `basePath` |
| `FileInfoToStringConverter` | `FileInfo` to `string` | `separator`, optional `basePath` |
| `RelativeUriToStringConverter` | `Uri` to `string` | `baseUri` |
| `TimeSpanToNumberConverter<T>` | `TimeSpan` to any numeric `T` | `interval` |
| `TimeSpanToDoubleConverter` | `TimeSpan` to `double` | `interval` |
| `TimeSpanToLongConverter` | `TimeSpan` to `long` | `interval` |
| `TimeSpanToIntConverter` | `TimeSpan` to `int` | `interval` |
| `EnumToDescriptionStringConverter<TEnum>` | `TEnum` to `string` | — |

All of them accept an optional `ConverterMappingHints` as their final argument, forwarded to EF Core so the column gets appropriate facets.

## Usage Examples

The filesystem converters take the separator used in storage, which decouples the persisted value from the separator of whatever operating system happens to be running. Supplying a `basePath` stores paths relative to it, so the same rows keep working when the storage root moves.

```csharp
builder.Entity<Document>()
  .Property(x => x.Folder)
  .HasConversion(new DirectoryInfoToStringConverter('/', new DirectoryInfo("/srv/storage")));
```

`RelativeUriToStringConverter` stores a URI relative to a base, and falls back to `about:blank` when a stored value can no longer be resolved against it.

```csharp
builder.Entity<Article>()
  .Property(x => x.Canonical)
  .HasConversion(new RelativeUriToStringConverter(new Uri("https://example.com/")));
```

The `TimeSpan` converters persist a duration as a plain number in the unit given by `DateInterval`, which is `Days`, `Hours`, `Minutes`, `Seconds`, `Milliseconds` or `Ticks`.

```csharp
builder.Entity<Session>()
  .Property(x => x.Duration)
  .HasConversion(new TimeSpanToLongConverter(DateInterval.Seconds));
```

`EnumToDescriptionStringConverter<TEnum>` persists the text of each member's `DescriptionAttribute`, falling back to the member name where none is present. The result is a column readable without the application, and stable against reordering of the enum.

```csharp
public enum Status
{
  [Description("Awaiting review")]
  Pending,

  [Description("Signed off")]
  Approved,
}
```

```csharp
builder.Entity<Request>()
  .Property(x => x.Status)
  .HasConversion(new EnumToDescriptionStringConverter<Status>());
```

## Caveats

> [!WARNING]
> A converted property cannot be translated to SQL in every query. Comparisons for equality generally work, but ordering, ranges and any function applied to the converted value are either evaluated client-side or rejected outright. Read the [EF Core value conversion documentation][ef-converters] before converting a property you intend to filter or sort on.

> [!IMPORTANT]
> `DateInterval` is a lossy choice, not a formatting one. Persisting as `Seconds` discards sub-second precision permanently, and `TimeSpanToIntConverter` overflows for spans beyond roughly 24 days at millisecond resolution. Prefer `Ticks` unless the column is meant to be read by a human.

> [!CAUTION]
> Changing a converter's `separator`, `basePath`, `baseUri` or `DateInterval` reinterprets every row already written with the previous setting. Treat these as part of the schema and migrate the data alongside any change.

> [!NOTE]
> `EnumToDescriptionStringConverter<TEnum>` builds its lookup once per closed generic type. Editing a `DescriptionAttribute` after rows exist orphans the values already stored, which materialise as the enum's default member.

> [!WARNING]
> `EnumToDescriptionStringConverter<TEnum>` never rejects stored text. Any value matching no member reads back as `default(TEnum)` rather than throwing, so an unmatched row is indistinguishable from one legitimately holding the default. A value outside the enum is written as its numeric text and therefore does not survive a round trip.

[ef-converters]: https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions
