using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Portal.Data.Entities;
using Portal.Data.ViewModels.Article;
using Portal.Persistance.Repositories.Interfaces;
using Portal.Web.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Portal.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            ArticleHomeListViewModel HomeList = new ArticleHomeListViewModel();
            HomeList.News = new List<ArticleHomeViewModel>();
            HomeList.Buisness= new List<ArticleHomeViewModel>();
            HomeList.Sport= new List<ArticleHomeViewModel>();
            var NewsList = _unitOfWork.Articles.GetAll().Where(a => a.Active == true && a.Category.Name=="News").OrderByDescending(a => a.CreateOn).Take(15).ToList();
            foreach (var item in NewsList)
            {
                var photo = _unitOfWork.ArticlePhotos.GetAll().Where(p => p.ArticleId == item.Id).Take(1).FirstOrDefault();
                HomeList.News.Add(new ArticleHomeViewModel()
                {
                    Active = item.Active,
                    CategoryId = item.CategoryId,
                    Content = item.Content,
                    CreateOn = item.CreateOn,
                    Id = item.Id,
                    Likes = item.Likes,
                    Title = item.Title,
                    UpdatedOn = item.UpdatedOn,
                    UserId = item.UserId,
                    Photo = photo
                });
            }
            var BuisnessList = _unitOfWork.Articles.GetAll().Where(a => a.Active == true && a.Category.Name == "Business").OrderByDescending(a => a.CreateOn).Take(15).ToList();
            foreach (var item in BuisnessList)
            {
                var photo = _unitOfWork.ArticlePhotos.GetAll().Where(p => p.ArticleId == item.Id).Take(1).FirstOrDefault();
                HomeList.Buisness.Add(new ArticleHomeViewModel()
                {
                    Active = item.Active,
                    CategoryId = item.CategoryId,
                    Content = item.Content,
                    CreateOn = item.CreateOn,
                    Id = item.Id,
                    Likes = item.Likes,
                    Title = item.Title,
                    UpdatedOn = item.UpdatedOn,
                    UserId = item.UserId,
                    Photo = photo
                });
            }
            var SportList = _unitOfWork.Articles.GetAll().Where(a => a.Active == true && a.Category.Name == "Sport").OrderByDescending(a => a.CreateOn).Take(15).ToList();
            foreach (var item in SportList)
            {
                var photo = _unitOfWork.ArticlePhotos.GetAll().Where(p => p.ArticleId == item.Id).Take(1).FirstOrDefault();
                HomeList.Sport.Add(new ArticleHomeViewModel()
                {
                    Active = item.Active,
                    CategoryId = item.CategoryId,
                    Content = item.Content,
                    CreateOn = item.CreateOn,
                    Id = item.Id,
                    Likes = item.Likes,
                    Title = item.Title,
                    UpdatedOn = item.UpdatedOn,
                    UserId = item.UserId,
                    Photo = photo
                });
            }




            return View(HomeList);
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult ArticlePage(string SelectedList)
        {
            ArticleHomeListViewModel ArticleList = new ArticleHomeListViewModel();
            ArticleList.SelectedList = SelectedList;
            ArticleList.News = new List<ArticleHomeViewModel>();
            ArticleList.Buisness = new List<ArticleHomeViewModel>();
            ArticleList.Sport = new List<ArticleHomeViewModel>();
            if (ArticleList.SelectedList == "News")
            {
                var NewsList = _unitOfWork.Articles.GetAll().Where(a => a.Active == true && a.Category.Name == "News").OrderByDescending(a => a.CreateOn).Take(15).ToList();
                foreach (var item in NewsList)
                {
                    var photo = _unitOfWork.ArticlePhotos.GetAll().Where(p => p.ArticleId == item.Id).Take(1).FirstOrDefault();
                    ArticleList.News.Add(new ArticleHomeViewModel()
                    {
                        Active = item.Active,
                        CategoryId = item.CategoryId,
                        Content = item.Content,
                        CreateOn = item.CreateOn,
                        Id = item.Id,
                        Likes = item.Likes,
                        Title = item.Title,
                        UpdatedOn = item.UpdatedOn,
                        UserId = item.UserId,
                        Photo = photo
                    });
                }

            }
            else if (ArticleList.SelectedList == "Business")
            {
                var BuisnessList = _unitOfWork.Articles.GetAll().Where(a => a.Active == true && a.Category.Name == "Business").OrderByDescending(a => a.CreateOn).Take(15).ToList();
                foreach (var item in BuisnessList)
                {
                    var photo = _unitOfWork.ArticlePhotos.GetAll().Where(p => p.ArticleId == item.Id).Take(1).FirstOrDefault();
                    ArticleList.Buisness.Add(new ArticleHomeViewModel()
                    {
                        Active = item.Active,
                        CategoryId = item.CategoryId,
                        Content = item.Content,
                        CreateOn = item.CreateOn,
                        Id = item.Id,
                        Likes = item.Likes,
                        Title = item.Title,
                        UpdatedOn = item.UpdatedOn,
                        UserId = item.UserId,
                        Photo = photo
                    });
                }

            }
            else if (ArticleList.SelectedList == "Sport")
            {
                var SportList = _unitOfWork.Articles.GetAll().Where(a => a.Active == true && a.Category.Name == "Sport").OrderByDescending(a => a.CreateOn).Take(15).ToList();
                foreach (var item in SportList)
                {
                    var photo = _unitOfWork.ArticlePhotos.GetAll().Where(p => p.ArticleId == item.Id).Take(1).FirstOrDefault();
                    ArticleList.Sport.Add(new ArticleHomeViewModel()
                    {
                        Active = item.Active,
                        CategoryId = item.CategoryId,
                        Content = item.Content,
                        CreateOn = item.CreateOn,
                        Id = item.Id,
                        Likes = item.Likes,
                        Title = item.Title,
                        UpdatedOn = item.UpdatedOn,
                        UserId = item.UserId,
                        Photo = photo
                    });
                }
            }
                return View(ArticleList);
        }

    }
}
