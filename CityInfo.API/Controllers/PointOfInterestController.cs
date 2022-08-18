using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [Authorize(Policy= "MustBeFromAntwerp")]
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
            var citeName = User.Claims.FirstOrDefault(c => c.Type == "City")?.Value;
            if(!await _cityInfoRepository.CityNameMatchCityId(citeName, cityId))
            {
                return Forbid();
            }

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation("City not found");
                return NotFound();
            }
            var pointOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestForCity));
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int PointOfInterestId)
        {
            // find city
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, PointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, PointOfInterestForCreation pointOfInterest)
        {

            // find city
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);// Here the data are nor persisted
            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync(); // Here the data are persisted
            var createdPointOfInterestToRetorn = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId = cityId,
                pointOfInterestId = createdPointOfInterestToRetorn.Id
            }, createdPointOfInterestToRetorn);
        }

        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult<PointOfInterestDto>> UpdatePointOfInterest(int cityId, int PointOfInterestId, PointOfInterestForUpdate pointOfInterest)
        {
            // find city
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            // Find point of interest
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, PointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(pointOfInterest, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync(); // Save changes to the database

            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartaillyUpdatePointOfInterest(int cityId, int PointOfInterestId, JsonPatchDocument<PointOfInterestForUpdate> patchDocument)
        {
            // find city
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            // Find point of interest
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, PointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdate>(pointOfInterestEntity);
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(pointOfInterestToPatch)) { return BadRequest(ModelState); }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int PointOfInterestId)
        {
            // find city
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            // Find point of interest
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, PointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();
            _mailservice.Send("Point of interest deleted", $" Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted");
            return NoContent();

        }
    }
}
