using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;

namespace API.Mappers {
    public static class UserMapper {
        public static UserResponse ToResponse(User user) {
            return new UserResponse {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FavoriteMovies = FavoriteMappers.ToFavoriteListResponse(user)
            };
        }

        public static IEnumerable<UserResponse> ToResponses(IEnumerable<User> users) {
            return users.Select(ToResponse);
        }
    }
}