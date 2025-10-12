using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RecipeCalculator.UI;
using RecipeCalculator.UI.Services;
using RecipeCalculator.Engine;
using RecipeCalculator.Engine.Parser;
using RecipeCalculator.Engine.Formulas;
using RecipeCalculator.Engine.Function;
using Microsoft.Extensions.Logging;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register HttpClientFactory for LLM Service
builder.Services.AddHttpClient();

// Register Storage Services
builder.Services.AddScoped<FormulaStorageService>();
builder.Services.AddScoped<VariableStorageService>();
builder.Services.AddScoped<LLMSettingsService>();

// Register LLM Service with HttpClientFactory
builder.Services.AddScoped<LLMService>();

// Register Engine Services
builder.Services.AddScoped<IFunctionCache, DefaultFunctionCache>();
builder.Services.AddScoped<IFunctionResultCache, DefaultFunctionResultCache>();
builder.Services.AddScoped<IParseTreeCache, DefaultParseTreeCache>();
builder.Services.AddScoped<IFormulaResultCache, DefaultFormulaResultCache>();
builder.Services.AddScoped<IVariableCache, DefaultVariableCache>();
builder.Services.AddScoped<IParsingContext, DefaultParsingContext>();
builder.Services.AddScoped<IEngineRunner, EngineRunner>();

await builder.Build().RunAsync();
