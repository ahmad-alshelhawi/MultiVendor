namespace AttarStore.WebApi.Authorization
{
    /// <summary>
    /// Exposes the current authenticated user’s ID (if any).
    /// </summary>
    public interface IUserContextAccessor
    {
        string? ActorId { get; }
        string? ActorRole { get; }
    }
}