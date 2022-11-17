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
#region ʹ��Autofac
{
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>((context, buider) =>
    {
        // ����ʹ�õ���ע��
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

#region ��װAPI�ṩ��
app.MapPost("/api/order", async (IOrderService orderService, Order order) =>
{
    var result = await orderService.AddOrder(order);
    return new AjaxResult
    {
        Result = result ? true : false,
        Message = result ? "�����ɹ�" : "����ʧ��",
        StatusCode = result ? 30000 : -9999
    };
});

app.MapPost("/api/aop/order", (IOrderService orderService, Order order) =>
{
    var result = orderService.AddOrderForAOP(order);
    return new AjaxResult
    {
        Result = result ? true : false,
        Message = result ? "�����ɹ�" : "����ʧ��",
        StatusCode = result ? 30000 : -9999
    };
});
#endregion


#region ����APi

// ���峬ʱ���õ�APi
app.MapGet("/api/polly/timeout", () =>
{
    Thread.Sleep(6000);
    return "Polly Timeout11111111111";
});

// ����500�����APi
app.MapGet("/api/polly/500", (HttpContext context) =>
{
    context.Response.StatusCode = 500;
    return "fail";
});

// ����/api/user
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