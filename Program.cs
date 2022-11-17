using Autofac.Extensions.DependencyInjection;
using Autofac;
using StevenPollyApi.PollyExtend;
using Autofac.Extras.DynamicProxy;
using StevenPollyApi.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region 使用Autofac
{
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>((context, buider) =>
    {
        // 必须使用单例注册
        buider.RegisterType<UserService>().As<IUserService>().SingleInstance().EnableInterfaceInterceptors();

        buider.RegisterType<OrderService>().As<IOrderService>();

        buider.RegisterType<PollyPolicyAttribute>();

    });
}
#endregion 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

#region 封装API提供区
app.MapPost("/api/order", async (IOrderService orderService, Order order) =>
{
    var result = await orderService.AddOrder(order);
    return new AjaxResult
    {
        Result = result ? true : false,
        Message = result ? "新增成功" : "新增失败",
        StatusCode = result ? 30000 : -9999
    };
});

app.MapPost("/api/aop/order", (IOrderService orderService, Order order) =>
{
    var result = orderService.AddOrderForAOP(order);
    return new AjaxResult
    {
        Result = result ? true : false,
        Message = result ? "新增成功" : "新增失败",
        StatusCode = result ? 30000 : -9999
    };
});
#endregion


#region 测试APi

// 定义超时调用的APi
app.MapGet("/api/polly/timeout", () =>
{
    Thread.Sleep(6000);
    return "Polly Timeout11111111111";
});

// 定义500结果的APi
app.MapGet("/api/polly/500", (HttpContext context) =>
{
    context.Response.StatusCode = 500;
    return "fail";
});

// 定义/api/user
app.MapGet("/api/user/1", () =>
{
    var user = new User
    {
        Id = 20001,
        Name = "Steven Wang",
    };

    return user;
});
#endregion


app.MapControllers();

app.Run();
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Account { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime LoginTime { get; set; }
}