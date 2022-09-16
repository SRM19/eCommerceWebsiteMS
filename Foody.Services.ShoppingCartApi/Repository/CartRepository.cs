using AutoMapper;
using Foody.Services.ShoppingCartApi.DbContexts;
using Foody.Services.ShoppingCartApi.Models;
using Foody.Services.ShoppingCartApi.Models.DataTransferObjs;
using Microsoft.EntityFrameworkCore;

namespace Foody.Services.ShoppingCartApi.Repository
{
    public class CartRepository : ICartRepository
    {
        private ApplicationDbContext _dbContext;
        private IMapper _mapper;

        public CartRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> ApplyCoupon(string userId, string code)
        {
            var cartHeaderfromDb = await _dbContext.CartHeaders
                                         .FirstOrDefaultAsync(c => c.UserId == userId);
            if(cartHeaderfromDb!= null)
            {
                cartHeaderfromDb.CouponCode = code;
                _dbContext.CartHeaders.Update(cartHeaderfromDb);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderfromDb = await _dbContext.CartHeaders.FirstOrDefaultAsync(c =>
            c.UserId == userId);
            if (cartHeaderfromDb != null)
            {
                //remove cart details which matches with cart header
                _dbContext.CartDetails.RemoveRange(
                    _dbContext.CartDetails.Where(
                        c => c.CartHeaderId == cartHeaderfromDb.CartHeaderId));
                //remove cart header
                _dbContext.CartHeaders.Remove(cartHeaderfromDb);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            //map cartdto to cart
            Cart cart = _mapper.Map<Cart>(cartDto);
            //check if product exist in database(shopping cart api DB)
            var productfromDb = await _dbContext.Products.FirstOrDefaultAsync(p =>
                                p.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId);
            // if not add product to database
            if (productfromDb == null)
            {
                //foody.web will need to pass the product details 
                _dbContext.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _dbContext.SaveChangesAsync();
            }
            //check if header exists based on user id or is null
            //AsNoTracking to avoid error from EF (as we are not updating existing object but to add new object)
            var cartHeaderfromDb = await _dbContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c =>
            c.UserId == cartDto.CartHeader.UserId);
            if (cartHeaderfromDb == null)
            {
                //create cart header and cart details
                _dbContext.CartHeaders.Add(cart.CartHeader);
                await _dbContext.SaveChangesAsync();
                // assigning newly created cart header id
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                //set product to null as the product is already populated
                cart.CartDetails.FirstOrDefault().Product = null;
                _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                //(else the cart is already present add/update product in it
                //check if product exist in cart
                var cartDetailsfromDb = await _dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    c => c.ProductId == cart.CartDetails.FirstOrDefault().ProductId
                    && c.CartHeaderId == cartHeaderfromDb.CartHeaderId);

                if (cartDetailsfromDb == null)
                {
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderfromDb.CartHeaderId;
                    _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }

                else
                {
                    //update count of product
                    cart.CartDetails.FirstOrDefault().Count += cartDetailsfromDb.Count;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _dbContext.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }


            }
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            Cart cart = new();
            var cartHeaderfromDb = await _dbContext.CartHeaders
                                    .FirstOrDefaultAsync(c => c.UserId == userId);
            cart.CartHeader = cartHeaderfromDb;
            //include product details as well
            cart.CartDetails = _dbContext.CartDetails.Where(c =>
                                c.CartHeaderId == cartHeaderfromDb.CartHeaderId)
                                .Include(c => c.Product);
            return _mapper.Map<CartDto>(cart);

        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cartHeaderfromDb = await _dbContext.CartHeaders
                                         .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cartHeaderfromDb != null)
            {
                cartHeaderfromDb.CouponCode = "";
                _dbContext.CartHeaders.Update(cartHeaderfromDb);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        //Remove product from cart
        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                var cartDetailfromDb = await _dbContext.CartDetails.FirstOrDefaultAsync(c => c.CartDetailsId == cartDetailsId);
                //check if no other item in cart, then remove cart header as well
                int countofItemsinCart = _dbContext.CartDetails
                                        .Where(c => c.CartHeaderId == cartDetailfromDb.CartHeaderId)
                                        .Count();
                //remove cart details
                _dbContext.CartDetails.Remove(cartDetailfromDb);
                //only one item was there in the cart
                if (countofItemsinCart == 1)
                {
                    var cartHeadertoRemove = _dbContext.CartHeaders
                        .FirstOrDefault(c => c.CartHeaderId == cartDetailfromDb.CartHeaderId);
                    _dbContext.CartHeaders.Remove(cartHeadertoRemove);
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }
    }
}
