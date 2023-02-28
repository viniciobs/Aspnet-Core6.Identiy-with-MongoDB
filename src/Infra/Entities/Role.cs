using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Data.Entities
{
	[CollectionName("Roles")]
	public class Role : MongoIdentityRole<Guid>
	{
	}
}
