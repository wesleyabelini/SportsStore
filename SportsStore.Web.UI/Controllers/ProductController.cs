using SportsStore.Domain.Abstract;
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

        public ProductController(IProductsRepository productsRepository)
        {
            this.repository = productsRepository;
        }

        // GET: Product
        public ViewResult List()
        {
            return View(repository.Products);
        }
    }
}