namespace Kritikos.Configuration.Persistence
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
			=> modelBuilder.EntitiesOfType(typeof(T), buildAction);

		private static ModelBuilder EntitiesOfType(
			this ModelBuilder modelBuilder,
			Type type,
			Action<EntityTypeBuilder> buildAction)
		{
			if (modelBuilder == null)
			{
				throw new ArgumentNullException(nameof(modelBuilder));
			}

			foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(x => type.IsAssignableFrom(x.ClrType)))
			{
				buildAction(modelBuilder.Entity(entityType.ClrType));
			}

			return modelBuilder;
		}
	}
}
