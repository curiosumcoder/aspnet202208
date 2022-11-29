﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using WA90.Data;
using WA90.Models;

namespace WA90.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGetList<int> _num;

        public HomeController(ILogger<HomeController> logger, IGetList<int> num)
        {
            _logger = logger;
            _num = num;
        }

        public IActionResult Index()
        {            
            return View(_num.GetList());            
        }

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