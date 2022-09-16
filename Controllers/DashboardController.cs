using ExpenseTracker.Data;
using ExpenseTracker.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ExpenseTrackerContext _context;
        public DashboardController(ExpenseTrackerContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
           
            //showing last 7 days transaction history
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;
            List<Transaction> SelectedTransactions = await _context.Transaction.
                Include(x => x.Category).
                Where(y => y.Date >= StartDate && y.Date<= EndDate ).
                ToListAsync();
            //total income
            int TotalIncome = SelectedTransactions
                .Where(i=>i.Category.Type == "Income")
                .Sum(i=>i.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");
            //total expense
            int TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(i => i.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");
            //balance
            int Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = Balance.ToString("C0");
            return View();
        }
    }
}
