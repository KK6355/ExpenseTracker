using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using System.Linq;

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
                .Where(i => i.Category.Type == "Income")
                .Sum(j=>j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");
            //total expense
            int TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");
            //balance
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture, "{0:c0}", Balance);

            //Doughnut Chart - Expense By Category
            ViewBag.DoughnutChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString(),

                }
                )
                .OrderByDescending(j=>j.amount)
                .ToList() ;
            //spline Chart - Income VS Exoense
            //INCOME
            List<SplineChartData> IncomeSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(j => j.Amount)
                }
                ).ToList();
            //Expense
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(j => j.Amount)
                }
                ).ToList();
            //Combine Income & Expense
            string[] Last7Days = Enumerable.Range(0,7)
                .Select(i=> StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();
            ViewBag.SplineChartData = from day in Last7Days
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };
            //Recent Transactions
            ViewBag.RecentTransactions = await _context.Transaction
                .Include(i => i.Category)
                .OrderByDescending(j => j.Date)
                .Take(5)
                .ToListAsync();



            return View();
        }
    }
    public class SplineChartData
    {
        public string day  { get; set; }
        public int income { get; set; }
        public int expense { get; set; }
    }
}
