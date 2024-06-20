using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JobSeed.Models;

namespace JobSeed.Controllers;
[Authorize(Roles ="Admin")]
public class AdminController : Controller
{

    private readonly AppDbContext _context;
    public AdminController(AppDbContext context)
    {
        _context=context;
    }

    public IActionResult Index()
    {
        var kq=(from u in _context.users select u).ToList();
        foreach(var item in kq)
        {
            _context.Entry(item).Collection(r=>r.usersRoles).Load();
        }
        return View(kq);
    }
    public IActionResult RoleManage()
    {
        var kq=(from r in _context.roles select r).ToList();
        return View(kq);
    }

    public IActionResult AddUser()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> AddUser([Bind("Email,Password,Name,Phone")] Users users) 
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var checkUserExists1=_context.users.Where(u=>u.Email==users.Email).FirstOrDefault();
        if(checkUserExists1!=null)
        {
            ModelState.AddModelError("","Email already exists");
            return View();
        }
        var checkUserExists2=_context.users.Where(u=>u.Phone==users.Phone).FirstOrDefault();
        if(checkUserExists2!=null)
        {
            ModelState.AddModelError("","Phone already exists");
            return View();
        }
        _context.users.Add(users);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    
    public IActionResult DeleteUser([FromRoute]int? id)
    {
        if(id==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var kq=_context.users.Where(u=>u.Id==id).FirstOrDefault();
        Console.WriteLine(id+"  sdasdsa here");
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var deletejob=_context.jobs.Where(c=>c.EmployerId==id).ToList();
        _context.jobs.RemoveRange(deletejob);
        _context.users.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    public IActionResult AddRole()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> AddRole([Bind("RoleName")] Role role)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var checkRoleExists=_context.roles.Any(r=>r.RoleName==role.RoleName);
        if(checkRoleExists==true)
        {
            ModelState.AddModelError("","Role already exists");
            return View();
        }
        await _context.roles.AddAsync(role);
        await _context.SaveChangesAsync();
        return RedirectToAction("RoleManage");
    }
    public IActionResult DeleteRole(int id)
    {
        if(id==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var kq=_context.roles.Where(r=>r.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        _context.roles.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("RoleManage");
    }
    public IActionResult EditRole(int id)
    {
        var kq=_context.roles.Where(r=>r.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        
        return View(kq);
    }
    [HttpPost]
    public IActionResult EditRole(int id,[Bind("RoleName")] Role role)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var checkRoleExists=_context.roles.Any(r=>r.RoleName==role.RoleName);
        if(checkRoleExists==true)
        {
            ModelState.AddModelError("","Role already exists");
            return View();
        }
        var kq=_context.roles.Where(r=>r.Id==id).FirstOrDefault();
        _context.Entry(kq).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
        kq.RoleName=role.RoleName;
        _context.SaveChanges();
        return RedirectToAction("RoleManage");
    }
    [HttpGet]
    public IActionResult AddRoleForUser(int id)
    {
        List<RoleCheckbox> RoleList=new List<RoleCheckbox>();
        foreach(var item in _context.roles)
        {
                RoleList.Add(new RoleCheckbox(){Id=item.Id,RoleName=item.RoleName,IsChecked=false});
        }
        Users user=_context.users.Where(u=>u.Id==id).FirstOrDefault();
        if(user==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        ViewData["user"]=user;
        return View(RoleList);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRoleForUser(int id,[Bind]List<RoleCheckbox> RoleList)
    {
        var kq=_context.users.Where(u=>u.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        RoleList=RoleList.Where(r=>r.IsChecked==true).ToList();
        
        var userRoleList=_context.usersRoles.Where(r=>r.UsersId==id).ToList();
        foreach(var deleteItem in userRoleList)
        {
            if(RoleList.Any(r=>r.Id==deleteItem.RoleId)==false)
            {
                _context.usersRoles.Remove(deleteItem);
            }
        }
        await _context.SaveChangesAsync();
        foreach(var addItem in RoleList)
        {
            if(userRoleList.Any(ur=>ur.RoleId==addItem.Id)==false)
            {
                UsersRole usersRole=new UsersRole(){UsersId=id,RoleId=addItem.Id};
                await _context.usersRoles.AddAsync(usersRole);
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    public IActionResult UserManage()
    {
        return RedirectToAction("Index");
    }
    public IActionResult CategoryManage()
    {
        return RedirectToAction("Index","Category");
    }
    [HttpPost]
    public IActionResult SearchUser(string email)
    {
        var kq=_context.users.Where(u=>u.Email.Contains(email)).ToList();
        foreach(var item in kq)
        {
            _context.Entry(item).Collection(r=>r.usersRoles).Load();
        }
        return View("Index",kq);        
    }
}
