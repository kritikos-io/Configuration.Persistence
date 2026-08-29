#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.Tests.EntityTests.ManyToMany;

using System.Collections.Generic;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;

public class Author : IEntity<long>
{
  public long Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public ICollection<AuthorBook> AuthorBooks { get; } = [];
}

public class Book : IEntity<long>
{
  public long Id { get; set; }

  public string Title { get; set; } = string.Empty;

  public ICollection<AuthorBook> AuthorBooks { get; } = [];
}

public class AuthorBook : IJoinEntity<Author, long, Book, long>
{
  public Author? Author { get; set; }

  public Book? Book { get; set; }
}

public class Painter : IEntity<long>
{
  public long Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public ICollection<Gallery> Galleries { get; } = [];
}

public class Gallery : IEntity<long>
{
  public long Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public ICollection<Painter> Painters { get; } = [];
}

public class PainterGallery : IJoinEntity<Painter, long, Gallery, long>
{
  public Painter? Painter { get; set; }

  public Gallery? Gallery { get; set; }
}
