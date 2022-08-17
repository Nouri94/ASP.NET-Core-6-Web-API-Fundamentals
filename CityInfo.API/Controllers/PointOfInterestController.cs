using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointOfInterestController : ControllerBase
    {
        private readonly ILogger<PointOfInterestController> _logger;
        private readonly IMailService _mailservice;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointOfInterestController(ICityInfoRepository cityInfoRepository, IMapper mapper, ILogger<PointOfInterestController> logger, IMailService mailservice)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailservice = mailservice ?? throw new ArgumentNullException(nameof(mailservice));
            this._cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation("City not found");
                return NotFound();
            }
            var pointOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestForCity));
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task< ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int PointOfInterestId)
        {
            // find city
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {              
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, PointOfInterestId);
            if(pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        //[HttpPost]
        //public async ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreation pointOfInterest)
        //{

        //    // find city
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    // Calculate the highest id of all points of interests 
        //    var MaxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);
        //    var finalPointOfInterest = new PointOfInterestDto()
        //    {
        //        Id = ++MaxPointOfInterestId,
        //        Name = pointOfInterest.Name,
        //        Description = pointOfInterest.Description
        //    };

        //    city.PointsOfInterest.Add(finalPointOfInterest);
        //    return CreatedAtRoute("GetPointOfInterest", new
        //    {
        //        cityId = cityId,
        //        pointOfInterestId = finalPointOfInterest.Id
        //    }, finalPointOfInterest);
        //}

        //[HttpPut("{pointofinterestid}")]
        //public async ActionResult<PointOfInterestDto> UpdatePointOfInterest(int cityId, int PointOfInterestId, PointOfInterestForUpdate pointOfInterest)
        //{

        //    // find city
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    // Find point of interest
        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == PointOfInterestId);
        //    if (pointOfInterestFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    pointOfInterestFromStore.Name = pointOfInterest.Name;
        //    pointOfInterestFromStore.Description = pointOfInterest.Description;
        //    return NoContent();
        //}

        //[HttpPatch("{pointofinterestid}")]
        //public async ActionResult PartaillyUpdatePointOfInterest(int cityId, int PointOfInterestId, JsonPatchDocument<PointOfInterestForUpdate> patchDocument)
        //{
        //    // find city
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    // Find point of interest
        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == PointOfInterestId);
        //    if (pointOfInterestFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    var pointOfInteresttoPatch = new PointOfInterestForUpdate()
        //    {
        //        Name = pointOfInterestFromStore.Name,
        //        Description = pointOfInterestFromStore.Description
        //    };
        //    patchDocument.ApplyTo(pointOfInteresttoPatch, ModelState);
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    if (!TryValidateModel(pointOfInteresttoPatch)) { return BadRequest(ModelState); }

        //    pointOfInterestFromStore.Name = pointOfInteresttoPatch.Name;
        //    pointOfInterestFromStore.Description = pointOfInteresttoPatch.Description;
        //    return NoContent();
        //}

        //[HttpDelete("{pointofinterestid}")]
        //public async ActionResult DeletePointOfInterest(int cityId, int PointOfInterestId)
        //{
        //    // find city
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    // Find point of interest
        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == PointOfInterestId);
        //    if (pointOfInterestFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    city.PointsOfInterest.Remove(pointOfInterestFromStore);
        //    _mailservice.Send("Point of interest deleted", $" Point of interest {pointOfInterestFromStore.Name} with id {PointOfInterestId} was deleted");
        //    return NoContent();

        //}
    }
}
