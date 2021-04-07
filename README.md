# Configuration.Persistence

[![Build Status](https://dev.azure.com/kritikos/DotNet%20Libaries/_apis/build/status/Configuration.Persistence?repoName=kritikos-io%2FConfiguration.Persistence&branchName=master)](https://dev.azure.com/kritikos/DotNet%20Libaries/_build/latest?definitionId=14&repoName=kritikos-io%2FConfiguration.Persistence&branchName=master)
[![codecov](https://codecov.io/gh/kritikos-io/Configuration.Persistence/branch/master/graph/badge.svg?token=zUKGBEw0Hs)](https://codecov.io/gh/kritikos-io/Configuration.Persistence)
[![Coverage Status](https://coveralls.io/repos/github/kritikos-io/Configuration.Persistence/badge.svg?branch=master)](https://coveralls.io/github/kritikos-io/Configuration.Persistence?branch=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=kritikos-io_Configuration.Persistence&metric=alert_status)](https://sonarcloud.io/dashboard?id=kritikos-io_Configuration.Persistence)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
![GitHub language count](https://img.shields.io/github/languages/count/kritikos-io/Configuration.Persistence)
![GitHub top language](https://img.shields.io/github/languages/top/kritikos-io/Configuration.Persistence)

Starting point and useful extensions to handle database persistence via Entity Framework Core.

## Persistence Abstractions

* ```IEntity<TKey>``` marks and groups primary entities in project having a primary key of type ```TKey```
* ```IConcurrent``` provides RowVersion used by Microsoft SQL Server for database concurrency
* ```ITimestamped``` and ```IAuditable<T>``` handle audit records and are compatible with provided interceptors

Examples can be found in [```BasicEntity```][BasicEntity], [```ConcurrentEntity```][ConcurrentEntity] and [```Entity```][Entity].

## Interceptors

A set of pre-transaction interceptors that provide helper functions. Current list includes:

* ```AuditSaveChangesInterceptor```: provides auditing for create/update events, requires an implementation of ```IAuditorProvider``` in order to work
* ```ITimeStampSaveChangesInterceptor```: populates fields provided by ```ITimestamped```, auditing create and update dates of each record
* ```ReadOnlyDbCommandInterceptor```: turns a database readonly by supressing all actions in SaveChanges

## Services

* ```MigrationService<TContext>``` is an oneshot ```HostedService``` that enforces database migration before kestrel starts serving requests.

## Extensions

Please refrain from using ChangeTracker extensions, they are considered obsolete and will be removed at some point in the future.

* ```EntitiesOfType``` is a ModelBuilder extension that allows configuring all entities inheriting common interfaces.

## Additional Features

Library ```Kritikos.Configuration.Peristence.IdentityServer``` contains an ApiAuthorizationDbContext wrapper for ```Microsoft.AspNetCore.ApiAuthorization.IdentityServer``` handling the missing overload for ```IdentityDbContext<TUser,TRole,TKey>```.

```KritikosConfiguration.Persistence.HealthCheck``` offers a basic DbContext IHealthCheck for use with ASP .NET Core.

## Converters

Use with care, not all converters support every available query execution at database level. Consult the [documentation][ef-converters] before applying.

* DirectoryInfo to string
* FileInfo to string
* Relative URI to string
* TimeSpan to number

[BasicEntity]: src/Configuration.Persistence/Base/BasicEntity.cs
[ConcurrentEntity]: src/Configuration.Persistence/Base/ConcurrentEntity.cs
[Entity]: src/Configuration.Persistence/Base/Entity.cs
[ef-converters]: https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations
