using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePro.Data;
using MoviePro.Models;
using MoviePro.Models.ViewModels;
using MoviePro.Services;
using MoviePro.Services.Interfaces;
using System.Diagnostics;

namespace MoviePro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IRemoteMovieService _tmdbMovieService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IRemoteMovieService tmdbMovieService)
        {
            _logger = logger;
            _context = context;
            _tmdbMovieService = tmdbMovieService;
        }


        public async Task<IActionResult> Index()
        {
            const int count = 18;

            var data = new LandingPageVM()
            {
                CustomCollections = await _context.Collection
                                .Include(c => c.MovieCollections)
                .ThenInclude(mc => mc.Movie)
                                .ToListAsync(),
                NowPlaying = await _tmdbMovieService.SearchMoviesAsync(Enums.MovieCategory.now_playing, count),
                Popular = await _tmdbMovieService.SearchMoviesAsync(Enums.MovieCategory.popular, count),
                TopRated = await _tmdbMovieService.SearchMoviesAsync(Enums.MovieCategory.top_rated, count),
                Upcoming = await _tmdbMovieService.SearchMoviesAsync(Enums.MovieCategory.upcoming, count)
            };
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}