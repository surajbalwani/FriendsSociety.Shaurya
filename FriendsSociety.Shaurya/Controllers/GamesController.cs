using FriendsSociety.Shaurya.Data;
using FriendsSociety.Shaurya.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FriendsSociety.Shaurya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<GamesController> _logger;

        public GamesController(DataContext context, ILogger<GamesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            try
            {
                var games = await _context.Games
                    .Include(g => g.AbilityType)
                    .Where(g => !g.IsDeleted)
                    .OrderBy(g => g.DisabilityTypeCode)
                    .ThenBy(g => g.AgeCategory)
                    .ThenBy(g => g.GameCodeNumber)
                    .ToListAsync();

                return Ok(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving games");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int id)
        {
            try
            {
                var game = await _context.Games
                    .Include(g => g.AbilityType)
                    .FirstOrDefaultAsync(g => g.GameID == id && !g.IsDeleted);

                if (game == null)
                {
                    return NotFound();
                }

                return Ok(game);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving game with ID {GameId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Games/ByCode/{gameCode}
        [HttpGet("ByCode/{gameCode}")]
        public async Task<ActionResult<Game>> GetGameByCode(string gameCode)
        {
            try
            {
                var game = await _context.Games
                    .Include(g => g.AbilityType)
                    .FirstOrDefaultAsync(g => g.GameCode == gameCode && !g.IsDeleted);

                if (game == null)
                {
                    return NotFound(new { message = $"Game with code {gameCode} not found" });
                }

                return Ok(game);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving game with code {GameCode}", gameCode);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Games/ByDisabilityType/{disabilityTypeCode}
        [HttpGet("ByDisabilityType/{disabilityTypeCode}")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGamesByDisabilityType(int disabilityTypeCode)
        {
            try
            {
                var games = await _context.Games
                    .Include(g => g.AbilityType)
                    .Where(g => g.DisabilityTypeCode == disabilityTypeCode && !g.IsDeleted)
                    .OrderBy(g => g.AgeCategory)
                    .ThenBy(g => g.GameCodeNumber)
                    .ToListAsync();

                return Ok(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving games for disability type {DisabilityTypeCode}", disabilityTypeCode);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Games/ByAgeCategory/{ageCategory}
        [HttpGet("ByAgeCategory/{ageCategory}")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGamesByAgeCategory(string ageCategory)
        {
            try
            {
                var games = await _context.Games
                    .Include(g => g.AbilityType)
                    .Where(g => g.AgeCategory == ageCategory && !g.IsDeleted)
                    .OrderBy(g => g.DisabilityTypeCode)
                    .ThenBy(g => g.GameCodeNumber)
                    .ToListAsync();

                return Ok(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving games for age category {AgeCategory}", ageCategory);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Games/ByAgeAndAbility?age={age}&abilityTypeId={abilityTypeId}
        [HttpGet("ByAgeAndAbility")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGamesByAgeAndAbility([FromQuery] int age, [FromQuery] int abilityTypeId)
        {
            try
            {
                // Determine age category based on age
                string ageCategory;
                if (age >= 8 && age <= 12) ageCategory = "A";
                else if (age >= 13 && age <= 17) ageCategory = "B";
                else if (age >= 18 && age <= 22) ageCategory = "C";
                else if (age >= 23 && age <= 27) ageCategory = "D";
                else
                {
                    return BadRequest("Age must be between 8 and 27 years");
                }

                var games = await _context.Games
                    .Include(g => g.AbilityType)
                    .Where(g => g.AgeCategory == ageCategory && 
                                g.AbilityTypeID == abilityTypeId && 
                                !g.IsDeleted)
                    .OrderBy(g => g.Name)
                    .ToListAsync();

                return Ok(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving games for age {Age} and ability type {AbilityTypeId}", age, abilityTypeId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Games/GroupedByDisability
        [HttpGet("GroupedByDisability")]
        public async Task<ActionResult<object>> GetGamesGroupedByDisability()
        {
            try
            {
                var games = await _context.Games
                    .Include(g => g.AbilityType)
                    .Where(g => !g.IsDeleted)
                    .ToListAsync();

                var grouped = games
                    .GroupBy(g => new 
                    { 
                        g.DisabilityTypeCode, 
                        g.AbilityType!.Name 
                    })
                    .Select(g => new
                    {
                        DisabilityTypeCode = g.Key.DisabilityTypeCode,
                        DisabilityTypeName = g.Key.Name,
                        Games = g.GroupBy(game => game.AgeCategory)
                            .Select(ageGroup => new
                            {
                                AgeCategory = ageGroup.Key,
                                AgeRange = $"{ageGroup.First().AgeRangeStart}-{ageGroup.First().AgeRangeEnd}",
                                Games = ageGroup.Select(gm => new
                                {
                                    gm.GameID,
                                    gm.Name,
                                    gm.GameCode,
                                    gm.Description,
                                    gm.Rules
                                }).OrderBy(gm => gm.GameCode).ToList()
                            })
                            .OrderBy(ag => ag.AgeCategory)
                            .ToList()
                    })
                    .OrderBy(g => g.DisabilityTypeCode)
                    .ToList();

                return Ok(grouped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving grouped games");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Games/Participation
        [HttpGet("Participation")]
        public async Task<ActionResult<object>> GetGameParticipation()
        {
            try
            {
                var games = await _context.Games
                    .Include(g => g.AbilityType)
                    .Where(g => !g.IsDeleted)
                    .ToListAsync();

                var participantGames = await _context.ParticipantGames
                    .Where(pg => !pg.IsDeleted)
                    .GroupBy(pg => pg.GameID)
                    .Select(g => new
                    {
                        GameID = g.Key,
                        ParticipantCount = g.Count()
                    })
                    .ToListAsync();

                var result = games
                    .GroupBy(g => new { g.DisabilityTypeCode, g.AbilityType!.Name })
                    .Select(disabilityGroup => new
                    {
                        DisabilityTypeCode = disabilityGroup.Key.DisabilityTypeCode,
                        DisabilityTypeName = disabilityGroup.Key.Name,
                        TotalParticipants = participantGames
                            .Where(pg => disabilityGroup.Select(g => g.GameID).Contains(pg.GameID))
                            .Sum(pg => pg.ParticipantCount),
                        Games = disabilityGroup
                            .Select(game => new
                            {
                                game.GameID,
                                game.Name,
                                game.GameCode,
                                game.AgeCategory,
                                AgeRange = $"{game.AgeRangeStart}-{game.AgeRangeEnd}",
                                ParticipantCount = participantGames
                                    .FirstOrDefault(pg => pg.GameID == game.GameID)?.ParticipantCount ?? 0
                            })
                            .OrderBy(g => g.AgeCategory)
                            .ThenBy(g => g.Name)
                            .ToList()
                    })
                    .OrderBy(g => g.DisabilityTypeCode)
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving game participation");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Games
        [HttpPost]
        public async Task<ActionResult<Game>> CreateGame(Game game)
        {
            try
            {
                // Validate GameCode format
                if (string.IsNullOrEmpty(game.GameCode) || game.GameCode.Length != 4)
                {
                    return BadRequest("Game code must be 4 characters (e.g., 1A02)");
                }

                // Check if game code already exists
                var existingGame = await _context.Games
                    .FirstOrDefaultAsync(g => g.GameCode == game.GameCode);

                if (existingGame != null)
                {
                    return Conflict(new { message = $"Game with code {game.GameCode} already exists" });
                }

                game.CreatedDate = DateTime.Now;
                _context.Games.Add(game);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetGame), new { id = game.GameID }, game);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating game");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Games/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGame(int id, Game game)
        {
            if (id != game.GameID)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var existingGame = await _context.Games.FindAsync(id);
                if (existingGame == null || existingGame.IsDeleted)
                {
                    return NotFound();
                }

                // Check if new game code conflicts with another game
                if (existingGame.GameCode != game.GameCode)
                {
                    var codeExists = await _context.Games
                        .AnyAsync(g => g.GameCode == game.GameCode && g.GameID != id);
                    
                    if (codeExists)
                    {
                        return Conflict(new { message = $"Game with code {game.GameCode} already exists" });
                    }
                }

                existingGame.Name = game.Name;
                existingGame.GameCode = game.GameCode;
                existingGame.GameCodeNumber = game.GameCodeNumber;
                existingGame.DisabilityTypeCode = game.DisabilityTypeCode;
                existingGame.AgeCategory = game.AgeCategory;
                existingGame.AgeRangeStart = game.AgeRangeStart;
                existingGame.AgeRangeEnd = game.AgeRangeEnd;
                existingGame.AbilityTypeID = game.AbilityTypeID;
                existingGame.Description = game.Description;
                existingGame.Rules = game.Rules;
                existingGame.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating game with ID {GameId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            try
            {
                var game = await _context.Games.FindAsync(id);
                if (game == null || game.IsDeleted)
                {
                    return NotFound();
                }

                game.IsDeleted = true;
                game.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting game with ID {GameId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
