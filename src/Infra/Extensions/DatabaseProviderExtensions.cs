using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Infra.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Infra.Extensions
{
	public static class DatabaseProviderExtensions
	{
		public static IdentityBuilder AddIdentityDatabase(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
			BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));

			return services
				.ConfigureMongoDbIdentity<User, Role, Guid>(new MongoDbIdentityConfiguration
				{
					MongoDbSettings = configuration
							.GetSection(nameof(MongoDbSettings))
							.Get<MongoDbSettings>()
				});
		}

	}
}
