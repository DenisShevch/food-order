﻿using System;
using System.Linq;
using Core;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebUI.Areas.Admin.Models;

namespace WebUI.Areas.Admin.Controllers
{
    [Route("admin/order")]
    public class OrderController : BaseController
    {
        private readonly IRepository _repository;
        private IConfiguration _configuration;

        private decimal _glagolPrice;
        private decimal _cafePrice;

        public OrderController(IRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;

            decimal.TryParse(_configuration["OrderPrice:Glagol"], out _glagolPrice);
            decimal.TryParse(_configuration["OrderPrice:Cafe"], out _cafePrice);
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("statistic")]
        public IActionResult Statistic()
        {
            var higherPrice = _cafePrice > _glagolPrice ? _cafePrice : _glagolPrice;
            var orders = _repository.All<Order>()
                .Where(x => x.Price > higherPrice && x.Date.Month == DateTime.Today.AddMonths(-1).Month)
                .Include(x => x.OrderItems).ThenInclude(x => x.DishItem).ThenInclude(x => x.Supplier)
                .Include(x => x.User).OrderBy(x => x.User.FirstName).ToArray();

            var models = from order in orders
                let suppId = order.OrderItems.First().DishItem.Supplier.Id
                let processingPrice = suppId == (int) FoodSupplier.Cafe ? _cafePrice : _glagolPrice
                where order.Price > processingPrice
                select new OverpriceViewModel
                {
                    UserName = order.User.FirstName,
                    Price = order.Price,
                    MaxPrice = processingPrice,
                    PriceDifference = order.Price - processingPrice,
                    Date = order.Date,
                };
            
            return View(models.ToArray());
        }
    }
}