using Newtonsoft.Json;
using Polly;
using Polly.Timeout;
using Polly.Wrap;

namespace StevenPollyApi.Service
{
    /// <summary>
    /// 调用远程API获取用户信息
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private AsyncPolicyWrap<User>? _policyWrap;

        public UserService(ILogger<UserService> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 降级方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User UserServiceFallback(int id)
        {
            Console.WriteLine("调用了->UserServiceFallback方法进行降级处理");
            return new User { Id = 1 , Name = "Steven降级了"};
        }


        /// <summary>
        /// 【AOP方式定义】根据用户ID查询用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public User AOPGetById(int id)
        {
            string url = $"http://localhost:7778/User";
            string content = InvokeApi(url);
            var user = JsonConvert.DeserializeObject<User>(content)!;
            return user;
        }

        public Task<User> GetById(int id)
        {
            return Task.FromResult(new User { Id = 1, Name = "Steven没有AOP" });
        }

        /// <summary>
        /// 给个URL，然后发起Http请求，拿到结果
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string InvokeApi(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage();
                message.Method = HttpMethod.Get;
                message.RequestUri = new Uri(url);
                var result = httpClient.SendAsync(message).Result;
                string content = result.Content.ReadAsStringAsync().Result;
                return content;
            }
        }
    }
}
