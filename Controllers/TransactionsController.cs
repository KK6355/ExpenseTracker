using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ExpenseTrackerContext _context;

        public TransactionsController(ExpenseTrackerContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var expenseTrackerContext = _context.Transaction.Include(t => t.Category);
            return View(await expenseTrackerContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Transaction == null)
        //    {
        //        return NotFound();
        //    }

        //    var transaction = await _context.Transaction
        //        .Include(t => t.Category)
        //        .FirstOrDefaultAsync(m => m.TransactionId == id);
        //    if (transaction == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(transaction);
        //}

        // GET: Transactions/AddOrEdit
        public IActionResult AddOrEdit(int id=0)
        {
            PopulateCategories();
            if (id == 0)
                return View(new Transaction());
            else
                return View(_context.Transaction.Find(id));
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if(transaction.TransactionId == 0)
                _context.Add(transaction);
                else
                 _context.Update(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryId", transaction.CategoryId);
            PopulateCategories();
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Transaction == null)
        //    {
        //        return NotFound();
        //    }

        //    var transaction = await _context.Transaction.FindAsync(id);
        //    if (transaction == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryId", transaction.CategoryId);
        //    return View(transaction);
        //}

        //// POST: Transactions/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        //{
        //    if (id != transaction.TransactionId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(transaction);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!TransactionExists(transaction.TransactionId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryId", transaction.CategoryId);
        //    return View(transaction);
        //}

        // GET: Transactions/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Transaction == null)
        //    {
        //        return NotFound();
        //    }

        //    var transaction = await _context.Transaction
        //        .Include(t => t.Category)
        //        .FirstOrDefaultAsync(m => m.TransactionId == id);
        //    if (transaction == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(transaction);
        //}

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transaction == null)
            {
                return Problem("Entity set 'ExpenseTrackerContext.Transaction'  is null.");
            }
            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction != null)
            {
                _context.Transaction.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //private bool TransactionExists(int id)
        //{
        //  return (_context.Transaction?.Any(e => e.TransactionId == id)).GetValueOrDefault();
        //}
        [NonAction] //has nothing with form and db
        public void PopulateCategories() //create a collection
        {
            var CategoryCollection = _context.Category.ToList();
            Category DefaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
            CategoryCollection.Insert(0,DefaultCategory);
            ViewBag.Category = CategoryCollection;
            
        }
    }
}
