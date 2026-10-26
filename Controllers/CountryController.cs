using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImfFunding.Context;
using ImfFunding.Models;
using ImfFunding.Dtos;


namespace IMFFundingApp.Controllers;

[ApiController] // Automatically binds parameters from the request body, query string, or route. Automatically returns 400 Bad Request when your model is invalid.
[Route("api/[controller]")] // endpoint for the controller starts at /api/country since [controller] is replaced with the name of the controller minus the "Controller" suffix
public class CountryController : ControllerBase
{
    // This constructor tells ASP.NET Core's built-in dependency injection container
    // to find the registered ApplicationDbContext and pass it in.
    private readonly FundingContext _context;
    public CountryController(FundingContext context)
    {
        _context = context; // Assigning the injected instance to a private field for use
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
    {
        var countries = await _context.Countries
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CountryDto(c.CountryId, c.IsoCode, c.Name, c.Region))
            .ToListAsync();

        return Ok(countries);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CountryDto>> GetById(int id)
    {
        var country = await _context.Countries
            .Where(c => c.CountryId == id && c.IsActive)
            .Select(c => new CountryDto(c.CountryId, c.IsoCode, c.Name, c.Region))
            .FirstOrDefaultAsync();

        return country is null ? NotFound() : Ok(country);
    }

}


/*
using Microsoft.AspNetCore.Mvc;

namespace IMFFundingApp.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
*/