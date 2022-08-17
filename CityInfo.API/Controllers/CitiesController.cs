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
        public ActionResult<CityDto> GetCity(int id)
        {
            // find city
            //var cityToReturn = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == id);
            //if(cityToReturn == null)
            //{
            //    return NotFound();
            //}
            //return Ok(cityToReturn);
            return Ok();
        }
    }
}
