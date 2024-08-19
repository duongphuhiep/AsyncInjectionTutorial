using SampleApi.AsyncInjection;


var builder = WebApplication.CreateBuilder(args);

// Uses Composition as an alternative IServiceProviderFactory
builder.Host.UseServiceProviderFactory(new Composition());

builder.Services.AddMvc();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc(opt => opt.EnableEndpointRouting = false).AddControllersAsServices();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseMvc();

await app.RunAsync();
