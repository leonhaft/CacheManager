using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CacheManagerApp.Models.EntityframeWork;
using CacheManager.Core;
using CacheManager.Serialization.Json;
using CacheManagerApp.Models;
using NLog;

namespace CacheManagerApp.Controllers
{
    public class ProductsController : Controller
    {
        private AdventureModel db = new AdventureModel();
        private static readonly ICacheManager<object> Cache;
        private readonly ILogger Log = LogManager.GetLogger("Product");

        static ProductsController()
        {
            Cache = CacheFactory.Build("Product", settings => settings
            .WithSystemRuntimeCacheHandle("runtimeCache")
            .And
            .WithSerializer(typeof(JsonCacheSerializer))
            .WithRedisConfiguration("redisConfig", config =>
                config.WithEndpoint("localhost", 6379))
            .WithRedisBackPlate("redisConfig")
            .WithRedisCacheHandle("redisConfig", true));
        }
        // GET: Products
        public async Task<ActionResult> Index()
        {
            return View(await db.Product.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productInfo = Cache.Get<ProductDto>($"Product:{id.Value}");
            if (productInfo == null)
            {
                Product product = await db.Product.FindAsync(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                productInfo = new ProductDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Price = product.ListPrice
                };
                Cache.Add($"Product:{id.Value}", productInfo);
                Log.Info("数据库中获取");
            }
            else
            {
                Log.Info("缓存中获取");
            }

            return View(productInfo);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProductId,Name,ListPrice")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Product.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Product.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProductId,Name,ListPrice")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Product.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Product.FindAsync(id);
            db.Product.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
