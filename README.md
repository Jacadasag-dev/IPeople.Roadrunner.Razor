# IPeople.Roadrunner.Razor

## Wiki
https://ipeople.atlassian.net/l/cp/oU5efSX0

## wwwroot/index.html
<script src="_content/IPeople.Roadrunner.Razor/scripts/iprs.js"></script>

## _Imports.razor
@using IPeople.Roadrunner.Razor.Components
@using IPeople.Roadrunner.Razor.Models
@using IPeople.Roadrunner.Razor.Services
@inject IRrStateService RrStateService

## MauiProgram.cs
builder.Services.AddSingleton<IRrStateService, RrStateService>();

## Optional csproj if not using NuGet
<ItemGroup>
	<ProjectReference Include="..\IPeople.Roadrunner.Razor\IPeople.Roadrunner.Razor.csproj" />
</ItemGroup>