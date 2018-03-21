using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.Text;
using ServiceStack.Auth;
using ServiceStack.OrmLite;
using System.Collections.Generic;

namespace FoxValleyMeetup.Web.Configurations.c04_UserInfoService
{
    [Authenticate] 
    public class UsersService : Service
    {
        public async Task<object> Get(UsersRequest request)
        {
            var users = await Db.SelectAsync<UserAuth>(Db.From<UserAuth>().OrderBy(u => u.UserName).ThenBy(u => u.DisplayName));
            return users.Map(u => new UserInfo().PopulateWithNonDefaultValues(u));
        }
    }

    public class UserInfo
    {
        public virtual string UserName { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string DisplayName { get; set; }
    }

    [Route("/users", "GET")] 
    public class UsersRequest: IReturn<List<UserInfo>>
    {
    }
}