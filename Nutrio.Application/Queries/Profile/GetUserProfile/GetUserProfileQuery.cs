using MediatR;

namespace Nutrio.Application.Queries.Profile.GetUserProfile;

public record GetUserProfileQuery(
    Guid UserId
) : IRequest<UserProfileDTO>;