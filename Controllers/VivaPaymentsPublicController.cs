﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Data;
using Nop.Plugin.Payments.VivaPayments.Helpers;
using Nop.Plugin.Payments.VivaPayments.Models;
using Nop.Plugin.Payments.VivaPayments.Services;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.VivaPayments.Controllers;
public class VivaPaymentsPublicController : BasePaymentController {
    protected readonly IRepository<Order> _orderRepository;
    protected readonly IOrderService _orderService;
    protected readonly VivaApiService _vivaApiService;
    protected readonly IOrderProcessingService _orderProcessingService;
    public VivaPaymentsPublicController(IRepository<Order> orderRepository, IOrderService orderService, VivaApiService vivaApiService, IOrderProcessingService orderProcessingService) {
        _orderRepository = orderRepository;
        _orderService = orderService;
        _vivaApiService = vivaApiService;
        _orderProcessingService = orderProcessingService;
    }

    public async Task<IActionResult> PaymentSuccess([FromQuery] VivaPaymentRedirection vivaPaymerRedirection) {
        if (vivaPaymerRedirection == null) {
            throw new NopException("Viva redirection result is null");
        }
        var vivaTransactionResponse = await _vivaApiService.GetTransactionDetailsAsync(vivaPaymerRedirection.TransactionId);
        if (vivaTransactionResponse != null) {
            var paymentStatus = Common.GetPaymentStatus(vivaTransactionResponse.StatusId);
            var order = _orderRepository.Table.FirstOrDefault(x => x.AuthorizationTransactionCode == vivaPaymerRedirection.OrderCode);
            order.CaptureTransactionId = vivaPaymerRedirection.TransactionId;
            await _orderService.InsertOrderNoteAsync(new OrderNote {
                OrderId = order.Id,
                Note = JsonSerializer.Serialize(vivaTransactionResponse),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            if (order.OrderTotal != (decimal)vivaTransactionResponse.Amount) {
                var message = $"There is a diference between paid amount ({vivaTransactionResponse.Amount}) and order amount ({order.OrderTotal})";
                order.CaptureTransactionId = vivaPaymerRedirection.TransactionId;
                await _orderService.InsertOrderNoteAsync(new OrderNote {
                    OrderId = order.Id,
                    Note = message,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                return RedirectToAction("Index", "Home", new { area = string.Empty });
            }
            if (paymentStatus == PaymentStatus.Paid) {
                if (!_orderProcessingService.CanMarkOrderAsPaid(order)) {
                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                }
                await _orderProcessingService.MarkOrderAsPaidAsync(order);
            } else if (paymentStatus == PaymentStatus.Authorized) {
                if (!_orderProcessingService.CanMarkOrderAsAuthorized(order)) {
                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                }
                await _orderProcessingService.MarkAsAuthorizedAsync(order);
            }
            await _orderService.UpdateOrderAsync(order);
            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
        } else {
            throw new NopException("Viva transaction result is null");
        }
    }
}
