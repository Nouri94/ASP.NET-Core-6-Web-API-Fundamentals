using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {       
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            this._cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities(string? name,
            string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }
            var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCtiesAsync(name, searchQuery, pageNumber, pageSize);    
          
            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cityEntities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool IncludePointOfInterest=false)
        {
            var city = await _cityInfoRepository.GetCityAsync(id, IncludePointOfInterest);
            if(city == null)
            {
                return NotFound();
            }

            if (IncludePointOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityWithoutPointOfInterestDto>(city));
        }
    }
}
