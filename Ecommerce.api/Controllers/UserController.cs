using ClassLibrary2.Dtos;
using ClassLibrary2.Model;
using Ecommerce.api.Dbcontext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;

namespace Ecommerce.api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly GiftShopDbContext Dbcontext;

        public UserController(GiftShopDbContext context)
        {
            Dbcontext = context;
        }

        [HttpGet]
        public IActionResult GetAllUser()
        {
            var users = Dbcontext.Users.ToList();
            return Ok(users);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDto userdto)
        {
            var post = new User()
            {
                RoleID = 2,
                Name = userdto.Name,
                Address = userdto.Address,
                Pincode = userdto.Pincode,
                Email = userdto.Email,
                Password = userdto.Password
            };
            Dbcontext.Users.Add(post);
            Dbcontext.SaveChanges();
            return Ok(post);
        }
        [HttpGet("{id:int}")]
        public IActionResult GetUserById(int id)
        {
            var user = Dbcontext.Users.Find(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateUser(int id, [FromBody] UserDto updatedUser)
        {
            var user = Dbcontext.Users.Find(id);
            if (user == null)
                return NotFound();

            user.Name = updatedUser.Name;
            user.Address = updatedUser.Address;
            user.Pincode = updatedUser.Pincode;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;

            Dbcontext.SaveChanges();
            return Ok(user);
        }



        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var user = Dbcontext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            Dbcontext.Users.Remove(user);
            Dbcontext.SaveChanges();

            return NoContent();
        }

        // ====================================== Products -----------------------------------------------------------


        [HttpPost("products")]
        public IActionResult CreateProduct([FromBody] ProductDto userDto)
        {
            var post = new Product()
            {
                ProductName = userDto.ProductName,
                ProductDescription = userDto.ProductDescription,
                ProductPrice = userDto.ProductPrice,
                ProductImageUrl = userDto.ProductImageurl
            };
            Dbcontext.Products.Add(post);
            Dbcontext.SaveChanges();
            return Ok(post);
        }

        [HttpGet("products")]
        public IActionResult GetAllProducts()
        {
            var products = Dbcontext.Products.ToList();
            return Ok(products);
        }

        [HttpGet("products/{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = Dbcontext.Products.Find(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPut("products/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var product = Dbcontext.Products.Find(id);
            if (product == null) return NotFound();

            product.ProductName = updatedProduct.ProductName;
            product.ProductDescription = updatedProduct.ProductDescription;
            product.ProductPrice = updatedProduct.ProductPrice;
            product.ProductImageUrl = updatedProduct.ProductImageUrl;

            Dbcontext.SaveChanges();
            return Ok(product);
        }

        [HttpDelete("products/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = Dbcontext.Products.Find(id);
            if (product == null) return NotFound();

            Dbcontext.Products.Remove(product);
            Dbcontext.SaveChanges();
            return Ok($"Product with id {id} deleted successfully.");
        }

        //===================================Addtocart====================================================
        [HttpPost("cart")]
        public IActionResult AddToCart([FromBody] CartItemDto itemDto)
        {
            var userExists = Dbcontext.Users.Any(u => u.Id == itemDto.UserId);
            var productExists = Dbcontext.Products.Any(p => p.ProductId == itemDto.ProductId);

            if (!userExists)
                return BadRequest(new { message = $"User with ID {itemDto.UserId} does not exist." });

            if (!productExists)
                return BadRequest(new { message = $"Product with ID {itemDto.ProductId} does not exist." });

            var existingItem = Dbcontext.CartItems
                .FirstOrDefault(c => c.UserId == itemDto.UserId && c.ProductId == itemDto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += itemDto.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = itemDto.UserId,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity
                };

                Dbcontext.CartItems.Add(cartItem);
            }

            Dbcontext.SaveChanges();

            return Ok("Item added to cart.");
        }




        [HttpGet("cart/{userId}")]
        public IActionResult GetUserCart(int userId)
        {
            var cartItems = Dbcontext.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToList();

            return Ok(cartItems);
        }

        [HttpDelete("cartitem/{id}")]
        public IActionResult RemoveFromCart(int id)
        {
            var item = Dbcontext.CartItems.Find(id);
            if (item == null) return NotFound();

            Dbcontext.CartItems.Remove(item);
            Dbcontext.SaveChanges();
            return Ok("Removed");
        }

        [HttpDelete("cart/{userId}")]
        public IActionResult ClearCart(int userId)
        {
            var items = Dbcontext.CartItems.Where(c => c.UserId == userId).ToList();
            Dbcontext.CartItems.RemoveRange(items);
            Dbcontext.SaveChanges();
            return Ok("Cart cleared");
        }

        //============================================order========================================================
        [HttpPost("placeorder")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderDto dto)
        {
            try
            {
                var user = await Dbcontext.Users.FindAsync(dto.UserId);
                if (user == null) return NotFound("User not found");

                var order = new Order
                {
                    UserId = dto.UserId,
                    Items = dto.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList(),
                    CustomerName = dto.Name ?? user.Name,
                    CustomerAddress = dto.Address ?? user.Address,
                    Status = "Pending"
                };

                Dbcontext.Orders.Add(order);

                // Clear user's cart
                var cartItems = Dbcontext.CartItems.Where(c => c.UserId == dto.UserId);
                Dbcontext.CartItems.RemoveRange(cartItems);

                await Dbcontext.SaveChangesAsync();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("userorders/{userId}")]
        public IActionResult GetUserOrders(int userId)
        {
            var orders = Dbcontext.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToList();

            return Ok(orders);
        }

        [HttpGet("allorders")] // For admin
        public IActionResult GetAllOrders()
        {
            var orders = Dbcontext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToList();

            return Ok(orders);
        }

        [HttpPut("update-status/{orderId}")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] string newStatus)
        {
            var order = await Dbcontext.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            order.Status = newStatus;
            await Dbcontext.SaveChangesAsync();
            return Ok(order);
        }
        //===========================esearch=================================
        [HttpGet("search")]
        public IActionResult SearchProducts(string query)
        {
            var products = Dbcontext.Products
                .Where(p => p.ProductName.Contains(query) || p.ProductDescription.Contains(query))
                .ToList();

            return Ok(products);
        }

    }
}

