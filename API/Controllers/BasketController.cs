using System;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly StoreContext _context;
        public BasketController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet(Name="GetBasket")]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await RetriveBasket();
            if (basket == null) return NotFound();
            return MapBasketToDto(basket);
        }

        

        [HttpPost] // api/basket?productId=3&quantity=2
        public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
        {
            //get basket || create basket if no basket
            var basket = await RetriveBasket();
            if (basket == null) basket = CreateBasket();

            //get product
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            //add item
            basket.AddItem(product, quantity);

            //save changes
            var result = await _context.SaveChangesAsync() > 0; // returns the integer of numbers we made to change the data, should be 1
            
            if (result) return CreatedAtRoute("GetBasket", MapBasketToDto(basket)); // to add location to response headers
            
            return BadRequest(new ProblemDetails{Title = "Problem saving item to basket"});
        }   

        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
        {
            //get basekt
            var basket = await RetriveBasket();
            if (basket == null) return NotFound();

            //remove item or reduce quantity
            basket.RemoveItem(productId, quantity);

            //save changes
            var result = await _context.SaveChangesAsync() > 0; // returns the integer of numbers we made to change the data, should be 1
            if (result) return Ok();

            return BadRequest(new ProblemDetails{Title = "Problem deleting item from basket"});
        }

        private async Task<Basket> RetriveBasket()
        {
            return await _context.Baskets
                    .Include(i => i.Items)
                    .ThenInclude(p => p.Product)
                    .FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerId"]);
        }

        private Basket CreateBasket()
        {
            var buyerId = Guid.NewGuid().ToString(); // should be safe to use it to greate id
            var cookieOptions = new CookieOptions{IsEssential = true, Expires = DateTime.Now.AddDays(10)};
            Response.Cookies.Append("buyerId", buyerId, cookieOptions); //send back as response
            var basket = new Basket{BuyerId = buyerId};
            _context.Baskets.Add(basket);
            return basket;
        }

        private BasketDto MapBasketToDto(Basket basket)
        {
            return new BasketDto
            {
                Id = basket.Id,
                BuyerId = basket.BuyerId,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    ProductId = item.ProductId,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    PrictureURL = item.Product.PictureUrl,
                    Type = item.Product.Type,
                    Brand = item.Product.Brand,
                    Quantity = item.Quantity
                }).ToList()
            };
        }

    }
}