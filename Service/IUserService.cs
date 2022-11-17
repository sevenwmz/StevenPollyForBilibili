using Autofac.Extras.DynamicProxy;
using StevenPollyApi.PollyExtend;

namespace StevenPollyApi.Service
{
    [Intercept(typeof(PollyPolicyAttribute))]//表示要polly生效
    public interface IUserService
    {
        [PollyPolicyConfig(FallBackMethod = "UserServiceFallback",
            IsEnableCircuitBreaker = true,
            ExceptionsAllowedBeforeBreaking = 3,
            MillisecondsOfBreak = 1000 * 5
            , CacheTTLMilliseconds = 1000 * 3)]
        User AOPGetById(int id);

        Task<User> GetById(int id);
    }
}
