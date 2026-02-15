using WebApi_PocV1.Domain.Entities;
using WebApi_PocV1.DTOs.Responses;

namespace WebApi_PocV1.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToResponse(User user)
        {
            return new UserResponse(user.Id, user.Name);
        }

        public static IEnumerable<UserResponse> ToResponses(IEnumerable<User> users)
        {
            return users.Select(ToResponse);
        }
    }
}
