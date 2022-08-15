﻿using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointOfInterestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterest>> GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{pointofinterestid}", Name ="GetPointOfInterest")]
        public ActionResult<PointOfInterest> GetPointOfInterest(int cityId, int PointOfInterestId)
        {
            // find city
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            // find point of interest

            var point = city.PointsOfInterest.FirstOrDefault(c => c.Id == PointOfInterestId);
            if (point == null)
            {
                return NotFound();
            }

            return Ok(point);
        }

        [HttpPost]
        public ActionResult<PointOfInterest> CreatePointOfInterest(int cityId, PointOfInterestForCreation pointOfInterest)
        {
           
            // find city
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            // Calculate the highest id of all points of interests 
            var MaxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);
            var finalPointOfInterest = new PointOfInterest()
            {
                Id = ++MaxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId = cityId,
                pointOfInterestId = finalPointOfInterest.Id 
            }, finalPointOfInterest);
        }

        [HttpPut("{pointofinterestid}")]
        public ActionResult<PointOfInterest> UpdatePointOfInterest(int cityId, int PointOfInterestId, PointOfInterestForUpdate pointOfInterest)
        {

            // find city
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            // Find point of interest
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == PointOfInterestId);
            if(pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;
            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public ActionResult PartaillyUpdatePointOfInterest(int cityId, int PointOfInterestId, JsonPatchDocument<PointOfInterestForUpdate> patchDocument)
        {
            // find city
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            // Find point of interest
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == PointOfInterestId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInteresttoPatch = new PointOfInterestForUpdate()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };
            patchDocument.ApplyTo(pointOfInteresttoPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(pointOfInteresttoPatch)) { return BadRequest(ModelState); }

            pointOfInterestFromStore.Name = pointOfInteresttoPatch.Name;
            pointOfInterestFromStore.Description = pointOfInteresttoPatch.Description;
            return NoContent();
        }
        
        [HttpDelete("{pointofinterestid}")]
        public ActionResult DeletePointOfInterest(int cityId, int PointOfInterestId)
        {
            // find city
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            // Find point of interest
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == PointOfInterestId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterestFromStore);
            return NoContent();

        }
    }
}
