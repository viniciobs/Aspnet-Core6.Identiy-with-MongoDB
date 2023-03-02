using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Infra.Entities
{

	[CollectionName("Users")]
	public class User : MongoIdentityUser<Guid>
	{
	}
}
