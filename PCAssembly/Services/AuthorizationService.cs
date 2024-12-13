namespace PCAssembly.Services
{
    public class AuthorizationService
    {
        public bool IsOwner(Assembly assembly, string currentUserId)
        {
            return assembly.UserId == currentUserId;
        }
    }
}
