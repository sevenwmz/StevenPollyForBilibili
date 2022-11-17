using Microsoft.AspNetCore.Mvc;

namespace StevenPollyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "User")]
        public User Get()
        {
            Random random = new Random();
            int id = random.Next(1, 99);
            Console.WriteLine($"ID:{id}");
            return new User { Id = id, Name = "Steven Wang" };
        }
    }
}