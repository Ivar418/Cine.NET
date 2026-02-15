using Cine.NET_WA.Dto;
using Cine.NET_WA.Domain;

namespace Cine.NET_WA.Mapping;

public static class UserMapper
{
    public static User ToDomain(UserDto dto)
        => new(dto.Id, dto.Name);
}