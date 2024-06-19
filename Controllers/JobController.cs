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
        public async Task<IActionResult> Create([Bind("JobName,JobDescription,CompanyName,Location,FromSalary,ToSalary,JobTypeId,FormFile,CategoryId,Gender,Experiences,Responsibility,Qualifications,Benefits")] Job job)
        {
            if (!ModelState.IsValid)
            {
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
        public async Task<IActionResult> Edit(int? id, [Bind("JobName,JobDescription,CompanyName,Location,FromSalary,ToSalary,Status,JobTypeId,FormFile,CategoryId,Gender,Responsibility,Experiences,Qualifications,Benefits")] Job job)
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
            kq.CategoryId = job.CategoryId;
            kq.Responsibility = job.Responsibility;
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
            var kq = _context.jobs.Where(p => p.Id == id).Include(c => c.JobType).FirstOrDefault();
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
        public IActionResult AllJobFilter(int page = 1)
        {
            int jobPerpages = 4;
            int totalJobs = _context.jobs.ToList().Count;
            int totalPage=(int)Math.Ceiling((double)totalJobs/jobPerpages); 

            var jobFilter = _context.jobs.Include(c => c.JobType).AsQueryable();
            if (page != 0)
            {
                jobFilter = jobFilter.Skip((page - 1) * jobPerpages).Take(jobPerpages);
            }
            ViewData["totalPage"] = totalPage;
            ViewData["currentPage"] = page;
            return View(jobFilter.ToList());
        }
        public IActionResult FilterJob(JobFilter jobFilter, int page = 1)
        {
            var allJobFilter = _context.jobs.Include(c => c.JobType).AsQueryable();
            if (jobFilter.SearchKeyword != null)
            {
                allJobFilter = allJobFilter.Where(c => c.JobName.Contains(jobFilter.SearchKeyword)
                    || c.JobDescription.Contains(jobFilter.SearchKeyword)
                    || c.JobType.JobTitle.Contains(jobFilter.SearchKeyword)
                    || c.Category.CategoryName.Contains(jobFilter.SearchKeyword));
            }
            if (jobFilter.Location != null)
            {
                allJobFilter = allJobFilter.Where(c => c.Location.CompareTo(jobFilter.Location) == 0);
            }
            if (jobFilter.Category != null)
            {
                allJobFilter = allJobFilter.Where(c => c.CategoryId == jobFilter.Category);
            }
            if (jobFilter.JobType != null)
            {
                allJobFilter = allJobFilter.Where(c => c.JobTypeId == jobFilter.JobType);
            }
            if (jobFilter.Gender != null)
            {
                allJobFilter = allJobFilter.Where(c => c.Gender == jobFilter.Gender);
            }

            // Pagination
            int jobPerPages = 4;
            int totalJobs = allJobFilter.Count();
            int totalPage=(int)Math.Ceiling((double)totalJobs/jobPerPages); 

            if (page != 0)
            {
                allJobFilter = allJobFilter.Skip((page - 1) * jobPerPages).Take(jobPerPages);
            }

            ViewData["SearchKeyword"] = jobFilter.SearchKeyword;
            ViewData["Location"] = jobFilter.Location;
            ViewData["Category"] = jobFilter.Category;
            ViewData["JobType"] = jobFilter.JobType;
            ViewData["Gender"] = jobFilter.Gender;
            ViewData["totalPage"] = totalPage;
            ViewData["currentPage"] = page;
            return View("AllJobFilter", allJobFilter.ToList());
        }

    }
}
