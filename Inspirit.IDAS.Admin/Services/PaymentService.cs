using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{
    public class PaymentService
    {
        IDASDbContext _dbContext;
        public PaymentService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ActionResult<PaymentResponse>> PaymentList(PaymentDataTableRequest request)
        {
            PaymentResponse response = new PaymentResponse();
            try
            {
                DateTime FilterToDate = new DateTime(Convert.ToDateTime(request.todate).Year, Convert.ToDateTime(request.todate).Month, Convert.ToDateTime(request.todate).Day, 23, 59, 59);

                var lst = _dbContext.Payments.Include(c => c.Customer).Include(i => i.Invoice).AsQueryable();
                int cnt = _dbContext.Payments.Count();
                var flt = lst;
                if (request.customerId != Guid.Empty)
                {
                    lst = lst.Where(c => c.CustomerId == request.customerId);
                }
                flt = lst.Where(t => t.Date <= Convert.ToDateTime(FilterToDate)
                && t.Date >= Convert.ToDateTime(request.fromdate));

                var data = (from s in flt
                            select new PaymentVm
                            {
                                Id = s.Id,
                                PaymentReceivedAmount = s.PaymentAmountReceive,
                                Comments = s.Comments,
                                CustomerName = s.Customer.TradingName,
                                CustomerCode = s.Customer.Code,
                                InvoiceDate = s.Invoice.InvoiceDate,
                                PaymentReceivedDate = s.PaymentReceivedDate.Value,
                                InvoiceNumber = s.Invoice.InvoiceDisplayNumber,
                                Date = s.Date,
                                Number = s.Invoice.InvoiceDisplayNumber.Replace("INV","PMT"),
                                Reference = s.Reference
                            }).ToList();
                response.data = data;
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public async Task<CrudResponsePayment> Insert(Payment data)
        {
            CrudResponsePayment response = new CrudResponsePayment();

            try
            {
                data.Id = Guid.NewGuid();
                Customer cus = _dbContext.Customers.FirstOrDefault();
                data.CustomerId = cus.Id;
                Invoice invoice = _dbContext.Invoices.FirstOrDefault();
                data.InvoiceId = invoice.ID;
                // data.CustomerId = invoice.CustomerId;
                _dbContext.Payments.Add(data);
                await _dbContext.SaveChangesAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }
        public CrudResponsePayment Update(Payment data)
        {
            CrudResponsePayment response = new CrudResponsePayment();
            try
            {
                _dbContext.Payments.Update(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }
        public async Task<List<Customer>> GetCustomers()
        {
            List<Customer> list = new List<Customer>();
            list = await _dbContext.Customers.ToListAsync();
            return list;
        }
        public async Task<Customer> GetCustomerById(Guid customerid)
        {
            Customer customer = new Customer();
            try
            {
                customer = await _dbContext.Customers
                    .FirstOrDefaultAsync(m => m.Status == "Active" && m.Id == customerid);
            }
            catch (Exception ex) { }
            return customer;
        }
        public async Task<List<LookupData>> GetPaymentLookupvalues()
        {
            List<LookupData> list = new List<LookupData>();
            list = await _dbContext.LookupDatas
                .Where(l => l.Type == "Payment Type" && l.IsActive == true)
                .ToListAsync();
            return list;
        }
        public CrudResponsePayment Delete(Guid id)
        {
            CrudResponsePayment response = new CrudResponsePayment();
            try
            {
                var data = _dbContext.Payments.Include(t=>t.Invoice).FirstOrDefault(t => t.Id == id);
                _dbContext.Payments.Remove(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public Payment View(Guid id)
        {
            Payment data = new Payment();
            try
            {
                data = (from s in _dbContext.Payments
                        where s.Id == id
                        select new Payment
                        {
                            Id = s.Id,
                            InvoiceId = s.InvoiceId,
                            CustomerId = s.CustomerId,
                            Amount = s.Amount,
                            PaymentAmountReceive = s.PaymentAmountReceive,
                            Comments = s.Comments,
                            Customer = s.Customer,
                            Date = s.Date,
                            PaymentReceivedDate = s.PaymentReceivedDate,
                            Invoice = s.Invoice,
                            Number = s.Number,
                            PaymentType = s.PaymentType,
                            Reference = s.Reference
                        }).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {

            }
            return data;
        }
    }
    public class PaymentDataTableRequest
    {
        public string fromdate { get; set; }
        public string todate { get; set; }
        public Guid customerId { get; set; }
        public DataTableRequest dtRequest { get; set; }
    }
    public class PaymentResponse : DataTableResponse<PaymentVm>
    {

    }
    public class PaymentVm
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public Guid CustomerId { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime PaymentReceivedDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public decimal? PaymentReceivedAmount { get; set; }
        public string Comments { get; set; }
    }
    public class CrudResponsePayment
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}

