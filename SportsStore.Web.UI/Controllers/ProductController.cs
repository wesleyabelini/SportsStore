﻿using SportsStore.Domain.Abstract;
using SportsStore.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.Web.UI.Controllers
{
    public class ProductController : Controller
    {
        private IProductsRepository repository;
        public int PageSize = 4;

        public ProductController(IProductsRepository productsRepository)
        {
            this.repository = productsRepository;
        }

        // GET: Product
        public ViewResult List(int page =1)
        {
            ProductsListViewModel model = new ProductsListViewModel
            {
                Products = repository.Products.OrderBy(p => p.ProductID).Skip((page - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Products.Count()
                }
            };

            return View(model);
        }
    }
}