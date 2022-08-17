using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {       
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            this._cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities()
        {
            var CityEntity = await _cityInfoRepository.GetCtiesAsync();
           
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(CityEntity));
            //return Ok(_citiesDataStore.Cities);
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
