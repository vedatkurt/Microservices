using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Helpers;
using FreeCourse.Web.Models.Catalog;
using FreeCourse.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IPhotoStockService _photoStockService;
        private readonly PhotoHelper _photoHelper;

        public CatalogService(HttpClient httpClient, IPhotoStockService photoStockService, PhotoHelper photoHelper)
        {
            _httpClient = httpClient;
            _photoStockService = photoStockService;
            _photoHelper = photoHelper;
        }

        public async Task<List<CategoryViewMdel>> GetAllCategoryAsync()
        {
            // http://localhost:5000/services/catalog/categories
            var response = await _httpClient.GetAsync("categories");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseData = await response.Content.ReadFromJsonAsync<Response<List<CategoryViewMdel>>>();

            return responseData.Data;
        }

        public async Task<List<CourseViewModel>> GetAllCourseAsync()
        {
            // http://localhost:5000/services/catalog/courses
            var response = await _httpClient.GetAsync("courses");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseData = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();

            responseData.Data.ForEach(x =>
            {
                x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
                //x.Picture = _photoHelper.GetPhotoStockUrl(x.Picture);
            });

            return responseData.Data;
        }

        public async Task<List<CourseViewModel>> GetAllCourseByUserIdAsync(string userId)
        {
            //  [controller]/GetAllByUserId/{userId}
            var response = await _httpClient.GetAsync($"courses/GetAllByUserId/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseData = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();

            responseData.Data.ForEach(x => 
            {
                x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
                //x.Picture = _photoHelper.GetPhotoStockUrl(x.Picture);
            });

            return responseData.Data;
        }

        public async Task<CourseViewModel> GetByCourseId(string courseId)
        {
            //  [controller]/GetAllByUserId/{userId}
            var response = await _httpClient.GetAsync($"courses/{courseId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseData = await response.Content.ReadFromJsonAsync<Response<CourseViewModel>>();

            responseData.Data.StockPictureUrl = _photoHelper.GetPhotoStockUrl(responseData.Data.Picture);


            return responseData.Data;
        }

        public async Task<bool> CreateCourseAsync(CourseCreateInput courseCreateInput)
        {
            // Photo create ediliyor
            var resultPhotoService = await _photoStockService.UploadPhoto(courseCreateInput.PhotoFile);
            if (resultPhotoService != null)
            {
                courseCreateInput.Picture = resultPhotoService.Url;
            }

            var response = await _httpClient.PostAsJsonAsync<CourseCreateInput>("courses", courseCreateInput);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCourseAsync(CourseUpdateInput courseUpdateInput)
        {
            // Photo create ediliyor
            var resultPhotoService = await _photoStockService.UploadPhoto(courseUpdateInput.PhotoFile);
            if (resultPhotoService != null)
            {
                await _photoStockService.DeletePhoto(courseUpdateInput.Picture);
                courseUpdateInput.Picture = resultPhotoService.Url;
            }
            var response = await _httpClient.PutAsJsonAsync<CourseUpdateInput>("courses", courseUpdateInput);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCourseAsync(string courseId)
        {
            var response = await _httpClient.DeleteAsync($"courses/{courseId}");

            return response.IsSuccessStatusCode;
        }
    }
}
