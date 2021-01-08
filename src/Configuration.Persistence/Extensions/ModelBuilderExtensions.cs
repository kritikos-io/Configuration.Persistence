namespace Kritikos.Configuration.Persistence.Extensions
{
	using System;
	using System.Linq;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	public static class ModelBuilderExtensions
	{
		public static ModelBuilder EntitiesOfType<T>(
			this ModelBuilder modelBuilder,
			Action<EntityTypeBuilder> buildAction)
			where T : class
		{
			if (modelBuilder == null)
			{
				throw new ArgumentNullException(nameof(modelBuilder));
			}

			foreach (var entityType in modelBuilder.Model.GetEntityTypes()
				.Where(x => typeof(T).IsAssignableFrom(x.ClrType)))
			{
				buildAction(modelBuilder.Entity(entityType.ClrType));
			}

			return modelBuilder;
		}
	}
}
