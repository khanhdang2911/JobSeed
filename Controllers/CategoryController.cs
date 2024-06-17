// using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using JobSeed.Models;

namespace JobSeed.Controllers;

public class CategoryController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<CategoryController> _logger;
    public CategoryController(ILogger<CategoryController> logger,AppDbContext context)
    {
        _logger = logger;
        _context=context;
    }

    public IActionResult Index()
    {
        var allCategory=(from c in _context.categories select c).ToList();
        return View(allCategory);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create([Bind("CategoryName")] Category category)
    {

        if(!ModelState.IsValid)
        {
            return View();
        }
        
        await _context.categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int? id)
    {
        
        if(id==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var kq=_context.categories.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int? id,[Bind("CategoryName")] Category category)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.categories.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }

        kq.CategoryName=category.CategoryName;
        _context.Entry(kq).State=EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");

    }
    public IActionResult Delete(int? id)
    {
        var kq=_context.categories.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        _context.categories.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
