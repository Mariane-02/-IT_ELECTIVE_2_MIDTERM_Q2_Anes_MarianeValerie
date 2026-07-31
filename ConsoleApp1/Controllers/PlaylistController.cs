using Microsoft.AspNetCore.Mvc;
using PlaylistApp.Filters;
using PlaylistApp.Models.DTOs;
using PlaylistApp.Models.Entities;
using PlaylistApp.Data;

namespace PlaylistApp.Controllers
{
    [AuthorizeSession]
    public class PlaylistController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("UserSession");
            return View(MockDatabase.Playlists);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var dto = new CreatePlaylistDto();
            dto.Videos.Add(new CreateVideoDto()); // Start with 1 empty row
            return View(dto);
        }

        // Action to ADD a row without JavaScript
        [HttpPost]
        public IActionResult AddVideoRow(CreatePlaylistDto dto)
        {
            dto.Videos.Add(new CreateVideoDto());
            ModelState.Clear();
            return View("Create", dto);
        }

        // Action to REMOVE a row without JavaScript
        [HttpPost]
        public IActionResult RemoveVideoRow(CreatePlaylistDto dto, int index)
        {
            if (index >= 0 && index < dto.Videos.Count)
            {
                dto.Videos.RemoveAt(index);
            }
            ModelState.Clear();
            return View("Create", dto);
        }

        [HttpPost]
        public IActionResult Save(CreatePlaylistDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", dto);
            }

            var playlist = new Playlist
            {
                Title = dto.Title,
                CreatorName = HttpContext.Session.GetString("UserSession") ?? "Anonymous"
            };

            foreach (var v in dto.Videos)
            {
                if (!string.IsNullOrWhiteSpace(v.YouTubeUrl))
                {
                    playlist.Videos.Add(new VideoItem { YouTubeUrl = v.YouTubeUrl, TrackNote = v.TrackNote });
                }
            }

            MockDatabase.Playlists.Add(playlist);
            return RedirectToAction("Index");
        }
    }
}
