namespace Kritikos.Configuration.Persistence
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Kritikos.Configuration.Persistence.Abstractions;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.ChangeTracking;

	public static class ChangeTrackerExtensions
	{
#pragma warning disable CS0419 // Ambiguous reference in cref attribute
		/// <summary>
		/// Configures preset actions that will be applied on all matching entities.
		/// </summary>
		/// <typeparam name="T">The type of entity to operate on.</typeparam>
		/// <param name="entries">The List of <see cref="EntityEntry"/> records to operate on.</param>
		/// <param name="actions">A mapping of actions to each state.</param>
		/// <returns>The list of <see cref="EntityEntry"/> used as input, now modified as per <paramref name="actions"></paramref>.</returns>
		/// <remarks>This is meant to be used on overloaded <see cref="DbContext.SaveChanges"/>.</remarks>
#pragma warning restore CS0419 // Ambiguous reference in cref attribute
		public static List<EntityEntry> ConfigureEntity<T>(
			this List<EntityEntry> entries,
			Dictionary<EntityState, Action<T>> actions)
			where T : class
		{
			foreach (var entry in entries.Where(x => x.Entity is T).Where(x => actions.ContainsKey(x.State)))
			{
				var entity = entry.Entity as T ?? throw new InvalidCastException(nameof(T));
				actions[entry.State](entity);
			}

			return entries;
		}

		/// <summary>
		/// Timestamps entities that implement <see cref="ITimestamped"/>.
		/// </summary>
		/// <param name="entries">The List of <see cref="EntityEntry"/> records to operate on.</param>
		/// <param name="now"><see cref="DateTimeOffset"/> that will be used for stamping.</param>
		/// <returns>The list of <see cref="EntityEntry"/> used as input, now timestamped.</returns>
		[Obsolete("Use TimestampSaveChangesInterceptor instead, method is marked for removal")]
		public static List<EntityEntry> TimeStampEntityEntries(this List<EntityEntry> entries, DateTimeOffset now)
			=> entries.ConfigureEntity(new Dictionary<EntityState, Action<ITimestamped>>
			{
				{ EntityState.Modified, x => x.UpdatedAt = now },
				{
					EntityState.Added, x =>
					{
						x.UpdatedAt = now;
						x.CreatedAt = now;
					}
				},
			});

		/// <summary>
		/// Sets audit fields for entities that implement <see cref="IAuditable{T}"/>.
		/// </summary>
		/// <typeparam name="T">Type of auditor identifying field.</typeparam>
		/// <param name="entries">The List of <see cref="EntityEntry"/> records to operate on.</param>
		/// <param name="auditor">The auditor that will be set.</param>
		/// <returns>The list of <see cref="EntityEntry"/> used as input, now audit-timestamped.</returns>
		[Obsolete("Use AuditSaveChangesInterceptor instead, method is marked for removal")]
		public static List<EntityEntry> AuditEntities<T>(this List<EntityEntry> entries, T auditor)
			=> entries.ConfigureEntity(new Dictionary<EntityState, Action<IAuditable<T>>>
			{
				{ EntityState.Modified, x => x.UpdatedBy = auditor },
				{
					EntityState.Added, x =>
					{
						x.CreatedBy = auditor;
						x.UpdatedBy = auditor;
					}
				},
			});
	}
}
