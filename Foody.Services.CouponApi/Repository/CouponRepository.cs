using AutoMapper;
using Foody.Services.CouponApi.DbContexts;
using Foody.Services.CouponApi.Models;
using Foody.Services.CouponApi.Models.DataTransferObjs;
using Microsoft.EntityFrameworkCore;

namespace Foody.Services.CouponApi.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private ApplicationDbContext _dbContext;
        private IMapper _mapper;

        public CouponRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<CouponDto> GetCouponbyCode(string code)
        {
            Coupon coupon = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.CouponCode == code);
            return _mapper.Map<CouponDto>(coupon);
        }
    }
}
