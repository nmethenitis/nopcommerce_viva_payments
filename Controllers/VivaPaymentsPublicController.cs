﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentMigrator.Runner;
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

    public async Task<IActionResult> PaymentSuccess([FromQuery] VivaPaymentRedirection vivaPaymentRedirection) {
        if (vivaPaymentRedirection == null) {
            throw new NopException("Viva redirection result is null");
        }
        var vivaTransactionResponse = await _vivaApiService.GetTransactionDetailsAsync(vivaPaymentRedirection.TransactionId);
        if (vivaTransactionResponse != null) {
            var paymentStatus = Common.GetPaymentStatus(vivaTransactionResponse.StatusId);
            var order = _orderRepository.Table.FirstOrDefault(x => x.AuthorizationTransactionCode == vivaPaymentRedirection.OrderCode);
            order.CaptureTransactionId = vivaPaymentRedirection.TransactionId;
            await _orderService.InsertOrderNoteAsync(new OrderNote {
                OrderId = order.Id,
                Note = JsonSerializer.Serialize(vivaTransactionResponse),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            if (order.OrderTotal != (decimal)vivaTransactionResponse.Amount) {
                var message = $"There is a diference between paid amount ({vivaTransactionResponse.Amount}) and order amount ({order.OrderTotal})";
                order.CaptureTransactionId = vivaPaymentRedirection.TransactionId;
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
                    return RedirectToRoute(VivaPaymentsDefaults.OrderCompletedRouteName, new { orderGuid = order.OrderGuid.ToString() });
                }
                await _orderProcessingService.MarkOrderAsPaidAsync(order);
            } else if (paymentStatus == PaymentStatus.Authorized) {
                if (!_orderProcessingService.CanMarkOrderAsAuthorized(order)) {
                    return RedirectToRoute(VivaPaymentsDefaults.OrderCompletedRouteName, new { orderGuid = order.OrderGuid.ToString() });
                }
                await _orderProcessingService.MarkAsAuthorizedAsync(order);
            }
            await _orderService.UpdateOrderAsync(order);            
            return RedirectToRoute(VivaPaymentsDefaults.OrderCompletedRouteName, new { orderGuid = order.OrderGuid.ToString() });
        } else {
            throw new NopException("Viva transaction result is null");
        }
    }

    public async Task<IActionResult> PaymentFail([FromQuery] VivaPaymentRedirection vivaPaymentRedirection) {
        if (vivaPaymentRedirection == null) {
            throw new NopException("Viva redirection result is null");
        }
        var order = _orderRepository.Table.FirstOrDefault(x => x.AuthorizationTransactionCode == vivaPaymentRedirection.OrderCode);
        if (!string.IsNullOrEmpty(vivaPaymentRedirection.TransactionId)) {
            var vivaTransactionResponse = await _vivaApiService.GetTransactionDetailsAsync(vivaPaymentRedirection.TransactionId);
        }
        var message = vivaPaymentRedirection.IsCancelled ? "Payment has been Cancelled by User" : $"Payment failed with event id: {vivaPaymentRedirection.EventId} - {Constants.EventIds[vivaPaymentRedirection.EventId]}";
        await _orderService.InsertOrderNoteAsync(new OrderNote {
            OrderId = order.Id,
            Note = message,
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });
        return RedirectToRoute(VivaPaymentsDefaults.OrderCompletedRouteName, new { orderGuid = order.OrderGuid.ToString() });
    }

    public async Task<IActionResult> PaymentWebhook([FromQuery] VivaPaymentWebhookRequest vivaPaymentWebhookRequest) {
        if (vivaPaymentWebhookRequest == null) {
            throw new NopException("Viva redirection result is null");
        }
        var order = _orderRepository.Table.FirstOrDefault(x => x.AuthorizationTransactionCode == vivaPaymentWebhookRequest.EventData.OrderCode.ToString());
        return RedirectToRoute(VivaPaymentsDefaults.OrderCompletedRouteName, new { orderGuid = order.OrderGuid.ToString() });
    }

    public async Task<IActionResult> OrderCompleted(Guid orderGuid) {
        var order = await _orderService.GetOrderByGuidAsync(orderGuid);
        if (order == null) {
            return NotFound();
        }
        return View("~/Plugins/Payments.VivaPayments/Views/Completed.cshtml", order);
    }
}
