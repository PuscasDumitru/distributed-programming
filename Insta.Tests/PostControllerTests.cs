using AutoMapper;
using Data;
using Data.Entities;
using Data.Repositories.Implementation;
using Insta.Controllers;
using Insta.Interfaces;
using Insta.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using StackExchange.Redis;

namespace Insta.Tests
{
    public class PostControllerTests
    {
        [Fact]
        public async void GetAllPosts_ReturnsAllPosts()
        {
            var mockPhotoService = new Mock<IPhotoService>();
            var mockMapper = new Mock<IMapper>();
            var mockRedis = new Mock<IConnectionMultiplexer>();
            
            var options = new DbContextOptionsBuilder<RepositoryDbContext>()
                .UseInMemoryDatabase(databaseName: "PostListDatabase")
                .Options;

            using (var context = new RepositoryDbContext(options))
            {
                context.Posts.Add(new Post { Id = 99, Title = "something" });
                context.Posts.Add(new Post { Id = 100, Title = "something else" });
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(options))
            {
                var controller = new PostController(context, mockPhotoService.Object, mockMapper.Object, mockRedis.Object);

                var result = await controller.GetAllPosts();
                var actionResult = Assert.IsType<ActionResult<object>>(result);
                var returnValue = Assert.IsType<SuccessModel>(actionResult.Value);
                var exctractedPosts = Assert.IsAssignableFrom<ICollection<Post>>(returnValue.Data);

                Assert.Equal(2, exctractedPosts.Count);
            }
        }

        [Fact]
        public async void GetPostByIdAsync_ReturnsAPostById()
        {
            var testPostId = 99;
            var mockPhotoService = new Mock<IPhotoService>();
            var mockMapper = new Mock<IMapper>();
            var mockRedis = new Mock<IConnectionMultiplexer>();
            
            var options = new DbContextOptionsBuilder<RepositoryDbContext>()
                .UseInMemoryDatabase(databaseName: "PostListDatabase")
                .Options;

            using (var context = new RepositoryDbContext(options))
            {
                context.Posts.Add(new Post { Id = testPostId, Title = "something" });
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(options))
            {
                var controller = new PostController(context, mockPhotoService.Object, mockMapper.Object, mockRedis.Object);

                var result = await controller.GetPostByIdAsync(testPostId);
                var actionResult = Assert.IsType<ActionResult<object>>(result);
                var returnValue = Assert.IsType<SuccessModel>(actionResult.Value);
                var extractedPost = Assert.IsAssignableFrom<Post>(returnValue.Data);

                Assert.Equal(testPostId, extractedPost.Id);
            }
        }

        [Fact]
        public async void AddPhoto_ReturnsANewPhoto()
        {
            var testPostId = 99;
            var mockPhotoService = new Mock<IPhotoService>();
            var mockMapper = new Mock<IMapper>();
            var mockRedis = new Mock<IConnectionMultiplexer>();
            var mockFormFile = new Mock<IFormFile>();

            var options = new DbContextOptionsBuilder<RepositoryDbContext>()
                .UseInMemoryDatabase(databaseName: "PostListDatabase")
                .Options;

            using (var context = new RepositoryDbContext(options))
            {
                context.Posts.Add(new Post { Id = testPostId, Title = "something" });
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(options))
            {
                var controller = new PostController(context, mockPhotoService.Object, mockMapper.Object, mockRedis.Object);

                var result = await controller.AddPhoto(mockFormFile.Object, testPostId);
                var actionResult = Assert.IsType<ActionResult<object>>(result);
                var returnValue = Assert.IsType<SuccessModel>(actionResult.Value);

                Assert.IsType<SuccessModel>(returnValue);
            }
        }

        [Fact]
        public async void DeletePhoto_DeletesTheGivenPhoto()
        {
            var testPostId = 1;
            var testPhotoId = 1;

            var mockPhotoService = new Mock<IPhotoService>();
            var mockMapper = new Mock<IMapper>();
            var mockRedis = new Mock<IConnectionMultiplexer>();
            var mockFormFile = new Mock<IFormFile>();

            var options = new DbContextOptionsBuilder<RepositoryDbContext>()
                .UseInMemoryDatabase(databaseName: "PostListDatabase")
                .Options;

            using (var context = new RepositoryDbContext(options))
            {
                context.Posts.Add(new Post { Id = testPostId, Title = "something" });
                context.Posts.FirstOrDefault(x => x.Id == testPostId).Photos.Add(new Photo { Id = testPhotoId });
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(options))
            {
                var controller = new PostController(context, mockPhotoService.Object, mockMapper.Object, mockRedis.Object);

                var result = await controller.DeletePhoto(testPostId, testPhotoId);
                var actionResult = Assert.IsType<ActionResult<object>>(result);
                var returnValue = Assert.IsType<SuccessModel>(actionResult.Value);

                Assert.IsType<SuccessModel>(returnValue);
            }
        }

    }
}