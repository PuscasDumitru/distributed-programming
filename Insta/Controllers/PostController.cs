using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Entities;
using Data.Repositories.Implementation;
using Data.Repositories.Interfaces;
using Insta.Models;
using Microsoft.AspNetCore.Http;
using Insta.Interfaces;
using AutoMapper;
using Insta.DTOs;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Insta.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    public class PostController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        private readonly IConnectionMultiplexer _redis;

        public PostController(RepositoryDbContext context, IPhotoService photoService, IMapper mapper, IConnectionMultiplexer redis)
        {
            _unitOfWork = new UnitOfWork(context);
            _photoService = photoService;
            _mapper = mapper;
            _redis = redis;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAllPosts()
        {
            try
            {
                
                var allPosts = await _unitOfWork.PostRepository.GetAllPostsAsync();

                return new SuccessModel
                {
                    Data = allPosts,
                    Message = "Posts retrieved",
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetPostByIdAsync(int postId)
        {
            try
            {
                var db = _redis.GetDatabase();
                string serializedPost = db.StringGet($"post:{postId}");
                if (serializedPost != null)
                {
                    return new SuccessModel
                    {
                        Data = serializedPost,
                        Message = "Post retrieved",
                        Success = true
                    };
                }

                var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
                db.StringSet($"post:{postId}", JsonConvert.SerializeObject(post), TimeSpan.FromMinutes(1));
                return new SuccessModel
                {
                    Data = post,
                    Message = "Post retrieved",
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> Create(Post post)
        {
            try
            {
                _unitOfWork.PostRepository.Create(post);
                await _unitOfWork.SaveChangesAsync();
                var db = _redis.GetDatabase();
                db.StringSet($"post:{post.Id}", JsonConvert.SerializeObject(post), TimeSpan.FromMinutes(1));
                return new SuccessModel
                {
                    Data = post,
                    Message = "Post created",
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> Update(Post post)
        {
            try
            {
                _unitOfWork.PostRepository.Update(post);
                await _unitOfWork.SaveChangesAsync();
                var db = _redis.GetDatabase();
                db.StringSet($"post:{post.Id}", JsonConvert.SerializeObject(post), TimeSpan.FromMinutes(1));
                return new SuccessModel
                {
                    Data = post,
                    Message = "Post updated",
                    Success = true
                };

            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }

        [HttpDelete]
        public async Task<ActionResult<object>> Delete(int id)
        {
            try
            {
                Post post = _unitOfWork.PostRepository.GetById(id);
                _unitOfWork.PostRepository.Delete(post);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = post,
                    Message = "Post deleted",
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<object>> AddPhoto(IFormFile file, int postId)
        {
            try
            {
                var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
                var result = _photoService.AddPhotoAsync(file);

                var photo = new Photo
                {
                    Url = result.Result.SecureUrl.AbsoluteUri,
                    PublicId = result.Result.PublicId
                };

                post.Photos.Add(photo);

                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = photo,
                    Message = "Photo added",
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult<object>> DeletePhoto(int postId, int photoId)
        {

            try
            {
                var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);

                var photo = post.Photos.FirstOrDefault(x => x.Id == photoId);

                if (photo == null) return NotFound();

                if (photo.PublicId != null)
                {
                    var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                    if (result.Error != null) return BadRequest(result.Error.Message);
                }

                post.Photos.Remove(photo);

                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = photo,
                    Message = "Photo deleted",
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }
    }
}
