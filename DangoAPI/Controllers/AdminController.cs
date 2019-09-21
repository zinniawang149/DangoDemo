using DangoAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DangoAPI.Models;
using System.Collections.Generic;
using DangoAPI.Dtos;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DangoAPI.Helpers;
using Microsoft.Extensions.Options;


namespace DangoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public AdminController(DataContext context, UserManager<User> userManager,IDatingRepository repo, IMapper mapper, 
            IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _context = context;
            _userManager = userManager;
            _repo = repo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
          _cloudinaryConfig.Value.CloudName,
          _cloudinaryConfig.Value.ApiKey,
          _cloudinaryConfig.Value.ApiSecret
          );
            _cloudinary = new Cloudinary(acc);
        }

        [Authorize(Policy = "RequireAdminRole")] //Same name as AddAuthorization in Startup.cs
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {

            var userList = await (from user in _context.Users
                                  orderby user.UserName
                                  select new
                                  {
                                      Id = user.Id,
                                      UserName = user.UserName,
                                      Roles = (from userRole in user.UserRoles
                                               join role in _context.Roles
                                               on userRole.RoleId equals role.Id
                                               select role.Name).ToList()
                                  }).ToListAsync();
            return Ok(userList);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            User user = await _userManager.FindByNameAsync(userName);
            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            string[] selectedRoles = roleEditDto.RoleNames;

            selectedRoles = selectedRoles ?? new string[] { }; // Same as: selectedRoles != null ? selectedRoles : new string[] { }, use for remove all roles
            IdentityResult result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded) return BadRequest("Failed to remove to roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            List<Photo> photos = await _repo.GetUnapprovedPhotosForModorator();
            if (photos.Count == 0) return NotFound();
            IEnumerable<PhotosForReturnToApprovedDto> photosToReturn = _mapper.Map<IEnumerable<PhotosForReturnToApprovedDto>>(photos);

            return Ok(photosToReturn);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("photo/approved/{photoId}")]
        public async Task<IActionResult> SetApprovedPhoto(int photoId)
        {
            Photo photoFromRepo = await _repo.GetPhoto(photoId);
            if (photoFromRepo == null) return BadRequest("This photo is not exist");

            if (photoFromRepo.IsApproved == true) return BadRequest("This photo is already approved");
            photoFromRepo.IsApproved = true;
            if (await _repo.SaveAll()) return NoContent();

            return BadRequest("Failed to approve the photo");
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("photo/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {

            Photo photoFromRepo = await _repo.GetPhoto(photoId);
            if (photoFromRepo.IsMain) return BadRequest("You cannot delete the main photo");

            if (photoFromRepo.PublicId != null)
            {
                DeletionParams deletionParams = new DeletionParams(photoFromRepo.PublicId);
                DeletionResult deletionResult = _cloudinary.Destroy(deletionParams);

                if (deletionResult.Result == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }

            if (photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll()) return Ok();
            return BadRequest("Failed to delete the photo");
        }
    }
}