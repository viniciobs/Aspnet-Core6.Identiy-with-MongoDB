using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Data.Entities
{

	[CollectionName("Users")]
	public class User : MongoIdentityUser<Guid>
	{
	}
}
