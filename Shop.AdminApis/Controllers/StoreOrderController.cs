﻿using ENode.Commanding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.AdminApis.Services;
using Shop.Api.Models.Request;
using Shop.Api.Models.Request.StoreOrders;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.StoreOrders;
using Shop.Common.Enums;
using Shop.QueryServices;
using System.Linq;
using Xia.Common.Extensions;

namespace Shop.AdminApis.Controllers
{
    [Route("[controller]")]
    public class StoreOrderController:BaseApiController
    {

        private IStoreOrderQueryService _storeOrderQueryService;//Q 端

        public StoreOrderController(ICommandService commandService, IContextService contextService,
            IStoreOrderQueryService storeOrderQueryService) : base(commandService,contextService)
        {
            _storeOrderQueryService = storeOrderQueryService;
        }
        

        #region 后台管理
        /// <summary>
        /// 店铺列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("ListPage")]
        public ListPageResponse ListPage([FromBody]ListPageRequest request)
        {
            request.CheckNotNull(nameof(request));

            var pageSize = 20;
            var storeOrders = _storeOrderQueryService.StoreOrderList();
            var total = storeOrders.Count();
            //筛选
            if (request.Status != StoreOrderStatus.All)
            {
                storeOrders = storeOrders.Where(x => x.Status == request.Status);
            }
            if (!request.Number.IsNullOrEmpty())
            {
                storeOrders = storeOrders.Where(x => x.Number.Contains(request.Number));
            }
            if (!request.Mobile.IsNullOrEmpty())
            {
                storeOrders = storeOrders.Where(x => x.Mobile.Contains(request.Mobile));
            }
            if (!request.StoreName.IsNullOrEmpty())
            {
                storeOrders = storeOrders.Where(x => x.Name.Contains(request.StoreName));
            }
            total = storeOrders.Count();

            //分页
            storeOrders = storeOrders.OrderByDescending(x => x.CreatedOn).Skip(pageSize * (request.Page - 1)).Take(pageSize);

            return new ListPageResponse
            {
                Total = total,
                StoreOrders = storeOrders.Select(x => new StoreOrderWithInfo
                {
                    Id = x.Id,
                    StoreId = x.StoreId,
                    Name=x.Name,
                    Mobile=x.Mobile,
                    NickName=x.NickName,
                    UserId=x.UserId,
                    Region = x.Region,
                    Number = x.Number,
                    Remark = x.Remark,
                    ExpressRegion = x.ExpressRegion,
                    ExpressAddress = x.ExpressAddress,
                    ExpressName = x.ExpressName,
                    ExpressMobile = x.ExpressMobile,
                    ExpressZip = x.ExpressZip,
                    CreatedOn = x.CreatedOn,
                    Total = x.Total,
                    ShopCash=x.ShopCash,
                    StoreTotal = x.StoreTotal,
                    DeliverExpressName = x.DeliverExpressName,
                    DeliverExpressCode=x.DeliverExpressCode,
                    DeliverExpressNumber=x.DeliverExpressNumber,
                    Status = x.Status.ToString()
                }).ToList()
            };
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Detail")]
        public BaseApiResponse Detail([FromBody]InfoRequest request)
        {
            request.CheckNotNull(nameof(request));
            var storeorder = _storeOrderQueryService.FindOrder(request.Id);
            if (storeorder == null)
            {
                return new BaseApiResponse { Code = 400, Message = "未找到该订单" };
            }
            return new InfoResponse
            {
                Id = storeorder.Id,
                StoreId = storeorder.StoreId,
                Number = storeorder.Number,
                Region = storeorder.Region,
                Remark = storeorder.Remark,
                ExpressRegion = storeorder.ExpressRegion,
                ExpressAddress = storeorder.ExpressAddress,
                ExpressName = storeorder.ExpressName,
                ExpressMobile = storeorder.ExpressMobile,
                ExpressZip = storeorder.ExpressZip,
                CreatedOn = storeorder.CreatedOn,
                Total = storeorder.Total,
                ShopCash=storeorder.ShopCash,
                Status = storeorder.Status.ToDescription(),
                StoreOrderGoodses = storeorder.StoreOrderGoodses.Select(z => new StoreOrderGoods
                {
                    Id = z.Id,
                    GoodsId = z.GoodsId,
                    SpecificationId = z.SpecificationId,
                    SpecificationName = z.SpecificationName,
                    GoodsName = z.GoodsName,
                    GoodsPic = z.GoodsPic,
                    Quantity = z.Quantity,
                    Price = z.Price,
                    OriginalPrice = z.OriginalPrice,
                    Total = z.Total,
                    Benevolence=z.Benevolence,
                    ShopCash=z.ShopCash,
                    StoreTotal = z.StoreTotal
                }).ToList()
            };
        }
        #endregion

        
    }
}