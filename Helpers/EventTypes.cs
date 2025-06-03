using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.VivaPayments.Helpers;
public enum EventTypes {
    TransactionPaymentCreated = 1796,
    TransactionFailed = 1798,
    TransactionPriceCalcuated = 1799,
    TransactionReversalCreated = 1797,
    AccountTransactionCreated = 2054,
    CommandBankTransferCreated = 768,
    CommandBankTransferExecuted = 769,
    AccountConnected = 8193,
    AccountVerificationStatusChanged = 8194,
    TransferCreated = 8448,
    TransactionPosEcrSessionCreated = 1802,
    TransactionPosEcrSessionFailed = 1803
}
