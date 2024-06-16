using JobSeed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobSeed.Controllers
{
    public class JobController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _context;
        private readonly ILogger<JobController> _logger;

        public JobController(AppDbContext context, IWebHostEnvironment environment, ILogger<JobController> logger)
        {
            _environment = environment;
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var allJob = (from p in _context.jobs select p).Include(p => p.JobType).ToList();
            return View(allJob);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("JobName,JobDescription,CompanyName,Location,FromSalary,ToSalary,JobTypeId,FormFile,Gender,Experiences,Qualifications,Benefits")] Job job)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Id cua job type:"+job.JobTypeId);
                LogModelStateErrors();
                return View();
            }
            if (job.FormFile != null)
            {
                var filepath = Path.Combine(_environment.WebRootPath, "uploads", job.FormFile.FileName);
                if (!System.IO.File.Exists(filepath))
                {
                    using FileStream fileStream = new FileStream(filepath, FileMode.Create);
                    job.FormFile.CopyTo(fileStream);
                }
                job.ImageLink = $"uploads/{job.FormFile.FileName}";
            }
            await _context.jobs.AddAsync(job);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            var kq = _context.jobs.Where(c => c.Id == id).FirstOrDefault();
            if (kq == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(kq);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, [Bind("JobName,JobDescription,CompanyName,Location,FromSalary,ToSalary,Status,JobTypeId,FormFile,Gender,Experiences,Qualifications,Benefits")] Job job)
        {
            if (!ModelState.IsValid)
            {
                LogModelStateErrors();
                return View();
            }
            var kq = _context.jobs.Where(c => c.Id == id).FirstOrDefault();
            if (kq == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            if (job.FormFile != null)
            {
                var filepath = Path.Combine(_environment.WebRootPath, "uploads", job.FormFile.FileName);
                if (!System.IO.File.Exists(filepath))
                {
                    using FileStream fileStream = new FileStream(filepath, FileMode.Create);
                    job.FormFile.CopyTo(fileStream);
                }
                job.ImageLink = $"uploads/{job.FormFile.FileName}";
            }

            kq.JobName = job.JobName;
            kq.JobDescription = job.JobDescription;
            kq.CompanyName = job.CompanyName;
            kq.Location = job.Location;
            kq.ImageLink = job.ImageLink;
            kq.FromSalary = job.FromSalary;
            kq.ToSalary = job.ToSalary;
            kq.JobTypeId = job.JobTypeId;
            kq.Gender = job.Gender;
            kq.Experiences = job.Experiences;
            kq.Qualifications = job.Qualifications;
            kq.Benefits = job.Benefits;

            _context.Entry(kq).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            var kq = _context.jobs.Where(c => c.Id == id).FirstOrDefault();
            if (kq == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            _context.jobs.Remove(kq);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Detail(int id)
        {
            var kq = _context.jobs.Where(p => p.Id == id).FirstOrDefault();
            if (kq == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(kq);
        }

        [AllowAnonymous]
        public IActionResult Search(string jobName)
        {
            var kq = _context.jobs.Where(p => p.JobName.Contains(jobName)).ToList();
            if (kq == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View("Index", kq);
        }

        private void LogModelStateErrors()
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            foreach (var error in errors)
            {
                foreach (var subError in error.Errors)
                {
                    _logger.LogError("Field: {Field}, Error: {Error}", error.Key, subError.ErrorMessage);
                }
            }
        }
    }
}
