using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Infra.Entities
{
	[CollectionName("Roles")]
	public class Role : MongoIdentityRole<Guid>
	{
	}
}
