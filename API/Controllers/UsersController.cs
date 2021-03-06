using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;
using API.Extensions;
using Microsoft.AspNetCore.Http;
using API.Helpers;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {

        private readonly IMapper _mapper;
        public readonly IPhotoService _PhotoService;

        private readonly IUnitOfWork _unitofWork;
     
        public UsersController(IUnitOfWork unitofWork, IMapper mapper, IPhotoService photoService)
        {
            _unitofWork = unitofWork;
            _PhotoService = photoService;
            _mapper = mapper;
     
          
        }
    
     
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
       
            var gender = await _unitofWork.UserRepository.GetUserGender(User.GetUsername());
            userParams.CurrentUsername = User.GetUsername();
            if(string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = gender == "male" ? "female" : "male";
            var users = await _unitofWork.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }
 
     
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _unitofWork.UserRepository.GetMemberAsync(username);
  
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) {
            

            var user = await _unitofWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            

            _mapper.Map(memberUpdateDto, user);
            _unitofWork.UserRepository.Update(user);

            if (await _unitofWork.Complete()) return NoContent();

            return BadRequest("FAILED TO UPDATE USER");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file) {
            var user = await _unitofWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());


            var result  = await _PhotoService.AddPhotoAsync(file);


            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);
            if(await _unitofWork.Complete())
            {
                //   return _mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("GetUser", new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
            } 
              


                return BadRequest("Problem adding photo");

           
        }


        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId) 
        {

            var user = await _unitofWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            

            //save again
            if (await _unitofWork.Complete()) return NoContent();
            return BadRequest("There something wrong. Failed to set new main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitofWork.UserRepository.GetUserByUsernameAsync(User.GetUsername()); 
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);


            if(photo == null) return NotFound();
            if(photo.IsMain) return BadRequest("you cannot delete you main photo");
            if(photo.PublicId != null)
            {
                var result = await _PhotoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
                
            }
            user.Photos.Remove(photo);
            if (await _unitofWork.Complete()) return Ok();
            return BadRequest("FAILED TO DELETE THE PHOTO");
        }


    }
}
