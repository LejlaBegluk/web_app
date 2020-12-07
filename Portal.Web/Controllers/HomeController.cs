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

            var List = _unitOfWork.Articles.GetAll().Where(a=>a.Active==true).OrderByDescending(a => a.CreateOn).Take(15).ToList();
            List<ArticleHomeViewModel> HomeList = new List<ArticleHomeViewModel>();
            foreach (var item in List)
            {
                var photo= _unitOfWork.ArticlePhotos.GetAll().Where(p => p.ArticleId == item.Id).Take(1).FirstOrDefault();
                HomeList.Add(new ArticleHomeViewModel()
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
                    Photo= photo
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

    }
}
