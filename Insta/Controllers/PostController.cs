using AutoMapper;
using Data;
using Data.Entities;
using Data.Repositories.Implementation;
using Data.Repositories.Interfaces;
using Insta.Interfaces;
using Insta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Insta.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    public class PostController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly IDistributedCache _cache;
        private const int Timeout = 4000;

        public PostController(RepositoryDbContext context, IPhotoService photoService,  IDistributedCache cache)
        {
            _unitOfWork = new UnitOfWork(context);
            _photoService = photoService;
            _cache = cache;
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
                string serializedPost = await _cache.GetStringAsync($"post:{postId}");

                if (serializedPost == "null")
                {
                    return new ErrorModel
                    {
                        Error = "PostNotFound",
                        Success = false
                    };
                }

                if (serializedPost != null)
                {
                    return new SuccessModel
                    {
                        Data = serializedPost,
                        Message = "Post retrieved",
                        Success  = true
                    };
                }

                var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(20));
                await _cache.SetStringAsync($"post:{postId}", JsonConvert.SerializeObject(post), options);
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
        [Authorize]
        public async Task<ActionResult<object>> Create(Post post)
        {
            try
            {
                var createPostTask = Task.Run(async () =>
                {
                    if (HttpContext.User.Identity is ClaimsIdentity identity)
                    {
                        IEnumerable<Claim> claims = identity.Claims;
                        var userName = identity.FindFirst("userName").Value;
                        var userId = identity.FindFirst("userId").Value;
                        post.UserId = new Guid(userId);
                    }
                    _unitOfWork.PostRepository.Create(post);
                    await _unitOfWork.SaveChangesAsync();
                    var options = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(20));
                    await _cache.SetStringAsync($"post:{post.Id}", JsonConvert.SerializeObject(post), options);
                    return new SuccessModel
                    {
                        Data = post,
                        Message = "Post created",
                        Success = true
                    };
                });

                if (await Task.WhenAny(createPostTask, Task.Delay(Timeout)) == createPostTask)
                {
                    return await createPostTask;
                }

                return new ErrorModel()
                {
                    Success = false,
                    Error = "Task timeout"
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
        [Authorize]
        public async Task<ActionResult<object>> Update(Post post)
        {
            try
            {
                var updatePostTask = Task.Run(async () =>
                {
                    if (HttpContext.User.Identity is ClaimsIdentity identity)
                    {
                        IEnumerable<Claim> claims = identity.Claims;
                        var userName = identity.FindFirst("userName").Value;
                        var userId = identity.FindFirst("userId").Value;
                        post.UserId = new Guid(userId);
                    }

                    _unitOfWork.PostRepository.Update(post);
                    await _unitOfWork.SaveChangesAsync();
                    var options = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(20));
                    await _cache.SetStringAsync($"post:{post.Id}", JsonConvert.SerializeObject(post), options);
                    return new SuccessModel
                    {
                        Data = post,
                        Message = "Post updated",
                        Success = true
                    };
                });

                if (await Task.WhenAny(updatePostTask, Task.Delay(Timeout)) == updatePostTask)
                {
                    return await updatePostTask;
                }

                return new ErrorModel()
                {
                    Success = false,
                    Error = "Task timeout"
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
        [Authorize]
        public async Task<ActionResult<object>> Delete(int id)
        {
            try
            {
                var deletePostTask = Task.Run(async () =>
                {
                    Post post = _unitOfWork.PostRepository.GetById(id);
                    _unitOfWork.PostRepository.Delete(post);
                    await _unitOfWork.SaveChangesAsync();
                    await _cache.RemoveAsync($"post:{post.Id}");

                    return new SuccessModel
                    {
                        Data = post,
                        Message = "Post deleted",
                        Success = true
                    };
                });

                if (await Task.WhenAny(deletePostTask, Task.Delay(Timeout)) == deletePostTask)
                {
                    return await deletePostTask;
                }

                return new ErrorModel()
                {
                    Success = false,
                    Error = "Task timeout"
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
        [Authorize]
        public async Task<ActionResult<object>> GetPostsByUserId()
        {
            try
            {
                var updatePostTask = Task.Run(async () =>
                {
                    if (HttpContext.User.Identity is ClaimsIdentity identity)
                    {
                        IEnumerable<Claim> claims = identity.Claims;
                        var userId = identity.FindFirst("userId").Value;
                        var result = new SuccessModel()
                        {
                            Message = "Posts retrieved",
                            Success = true
                        };

                        string serializedPosts = await _cache.GetStringAsync($"posts:{userId}");

                        if (serializedPosts == null)
                        {
                            var posts = await _unitOfWork.PostRepository.GetPostByUserIdAsync(new Guid(userId));
                            await _unitOfWork.SaveChangesAsync();
                            var options = new DistributedCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromSeconds(20));
                            await _cache.SetStringAsync($"posts:{userId}", JsonConvert.SerializeObject(posts), options);
                            result.Data = posts;
                            return result;
                        }

                        result.Data = serializedPosts;
                        return result;
                    }

                    return new SuccessModel()
                    {
                        Success = false,
                        Message = "Claim userId missing"
                    };
                });

                if (await Task.WhenAny(updatePostTask, Task.Delay(Timeout)) == updatePostTask)
                {
                    return await updatePostTask;
                }

                return new ErrorModel()
                {
                    Success = false,
                    Error = "Task timeout"
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
                var result = await _photoService.AddPhotoAsync(file);

                var photo = new Photo
                {
                    Url = result.SecureUrl.AbsoluteUri,
                    PublicId = result.PublicId
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
