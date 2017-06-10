using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace LeaguePredictor.Models
{
    public class GameViewModels
    {
        public List<LeagueTableModel> LeagueTable { get; set; }
        public List<PlayerPicks> PlayerPicks { get; set; }
        private List<LeaguePosition> CurrentLeaguePositions;

        public GameViewModels()
        {
            PlayerPicks = new List<Models.PlayerPicks>();
            LeagueTable = new List<LeagueTableModel>();
            CurrentLeaguePositions = new List<LeaguePosition>();
            PopulateLeagueTable();
        }

        public void PopulatePlayerPicks()
        {
            using (var db = new leaguepredictorEntities())
            {
                var playerSelections = db.players_selections.Select(sel =>
                new { Player = sel.player.name, Team = sel.team.team_name, Position = sel.position, Id = sel.player.id }).ToList();

                foreach (var p in playerSelections.Select(p => p.Player).Distinct())
                {
                    var pp = new PlayerPicks();
                    pp.Player = p;
                    pp.ID = db.players.Where(pl => pl.name.Equals(p)).Single().id;
                    pp.Selections = playerSelections.Where(ps => ps.Player.Equals(pp.Player))
                                                    .Select(ps => new LeaguePosition { Team = ps.Team, Position = ps.Position })
                                                    .ToList();
                    PlayerPicks.Add(pp);
                }
            }
        }
        public void PopulateLeagueTable()
        {

            PopulatePlayerPicks();

            using (var db = new leaguepredictorEntities())
            {

                CurrentLeaguePositions = db.team_rankings.Select(tr => new LeaguePosition { Team = tr.team.team_name, Position = tr.position })
                                                                .ToList();

                //calculate each player scores
                PlayerPicks.ForEach(pp =>
                {
                    var ltm = GenerateLeagueTableModel(pp);
                    ltm.ID = pp.ID;
                    LeagueTable.Add(ltm);
                });

                LeagueTable = LeagueTable.OrderBy(ltm => ltm.Points).ToList();
                foreach(var ltm in LeagueTable)
                {
                    ltm.Position = LeagueTable.IndexOf(ltm) +1;
                }
            }
        }

        public PlayerScoresViewModel GetPlayerScoresViewModel(int id)
        {
            using (var db = new leaguepredictorEntities())
            {
                var psvm = db.players.Where(p => p.id == id).Select(p =>
                new
                {
                    ID = p.id,
                    PlayerPicks = new
                    {
                        Player = p.name,
                        Selections = p.players_selections.Where(ps => ps.player_id == p.id).Select(ps =>
                            new
                            {
                                Team = ps.team.team_name,
                                Position = ps.position,
                                League = ps.team.league.league_name
                            }
                        ).ToList(),
                    }
                }).SingleOrDefault();

                if (psvm == null)
                {
                    return null;
                }

                var ret = new PlayerScoresViewModel
                        {
                            ID = psvm.ID,
                            PlayerPicks = new PlayerPicks
                            {
                                Player = psvm.PlayerPicks.Player,
                                Selections = psvm.PlayerPicks.Selections.Select(a => new LeaguePosition
                                {
                                    League = a.League,
                                    Position = a.Position,
                                    Team = a.Team
                                }).ToList()
                            },
                            CurrentLeaguePosition = CurrentLeaguePositions
                        };
                ret.Points = new Dictionary<LeaguePosition, int>();
                ret.PlayerPicks.Selections.ForEach(sel =>
                {
                    ret.Points.Add(sel, GetPositionDifference(sel));
                });

                return ret;
                
            }

        }

        private int GetCurrentPosition(string team)
        {
            return CurrentLeaguePositions.Single(lp => lp.Team.Equals(team)).Position;
        }

        private int GetPositionDifference(LeaguePosition lp)
        {
            var diff = GetCurrentPosition(lp.Team) - lp.Position;
            return Math.Abs(diff);
        }

        private LeagueTableModel GenerateLeagueTableModel(PlayerPicks playerPicks)
        {
            var ltm = new LeagueTableModel();
            ltm.Player = playerPicks.Player;
            foreach(var lp in playerPicks.Selections)
            {
                var points = GetPositionDifference(lp);
                if (points == 0) ltm.Correct++;
                ltm.Points += points;
            }


            return ltm;
        }
    }

    public class LeagueTableModel
    {
        public int ID { get; set; }
        public string Player { get; set; }
        public int Position { get; set; }
        public int Points { get; set; }
        public int Correct { get; set; }
    }

    public class PlayerPicks
    {
        public int ID { get; set; }
        public string Player { get; set; }
        public List<LeaguePosition> Selections { get; set; }
    }

    public class LeaguePosition
    {
        public string Team { get; set; }
        public int Position { get; set; }
        public string League { get; set; }
    }

    public class PlayerScoresViewModel
    {
        public int ID { get; set; }
        public PlayerPicks PlayerPicks { get; set; }
        public List<LeaguePosition> CurrentLeaguePosition { get; set; }
        public Dictionary<LeaguePosition, int> Points { get; set; }
    }

    public class TeamSelectionsViewModel
    {
        public IEnumerable<SelectListItem> PremTeams { get; }
        public IEnumerable<SelectListItem> ChampTeams { get; }
        public TeamSelectionsViewModel()
        {
            using (var db = new leaguepredictorEntities())
            {
                PremTeams = new List<SelectListItem>();
                PremTeams = db.teams.Where(t => t.league.league_name.Equals("Premier League")).Select(t => 
                new SelectListItem { Text = t.team_name, Value = t.team_name }
                ).OrderBy(t => t.Text).ToList();
                ChampTeams = new List<SelectListItem>();
                ChampTeams = db.teams.Where(t => t.league.league_name.Equals("Championship")).Select(t =>
                new SelectListItem { Text = t.team_name, Value = t.team_name }
                ).OrderBy(t => t.Text).ToList();
            }
        }



        [Required]
        [Display(Name = "Name")]
        
        public string Name { get; set; }

        [Required]
        [Display(Name = "Premier Leauge Winner")]
        [LeaguePositionAttribute(Position = 1, League = "Premier League")]
        public string Prem1 { get; set; }

        [Required]
        [Display(Name = "Premier Leauge 2nd")]
        [LeaguePositionAttribute(Position = 2, League = "Premier League")]
        public string Prem2 { get; set; }

        [Required]
        [Display(Name = "Premier Leauge 3rd")]
        [LeaguePositionAttribute(Position = 3, League = "Premier League")]
        public string Prem3 { get; set; }

        [Required]
        [Display(Name = "Premier Leauge 4th")]
        [LeaguePositionAttribute(Position = 4, League = "Premier League")]
        public string Prem4 { get; set; }

        [Required]
        [Display(Name = "Premier Leauge 5th")]
        [LeaguePositionAttribute(Position = 5, League = "Premier League")]
        public string Prem5 { get; set; }

        [Required]
        [Display(Name = "Premier Leauge 6th")]
        [LeaguePositionAttribute(Position = 6, League = "Premier League")]
        public string Prem6 { get; set; }

        [Required]
        [Display(Name = "Premier Leauge 18th")]
        [LeaguePositionAttribute(Position = 18, League = "Premier League")]
        public string Prem18 { get; set; }

        [Required]
        [Display(Name = "Premier Leauge 19th")]
        [LeaguePositionAttribute(Position = 19, League = "Premier League")]
        public string Prem19 { get; set; }

        [Required]
        [Display(Name = "Premier Leauge 20th")]
        [LeaguePositionAttribute(Position = 20, League = "Premier League")]
        public string Prem20 { get; set; }

        [Required]
        [Display(Name = "Championship Winner")]
        [LeaguePositionAttribute(Position = 1, League = "Championship")]
        public string Champ1 { get; set; }

        [Required]
        [Display(Name = "Championship 2nd")]
        [LeaguePositionAttribute(Position = 2, League = "Championship")]
        public string Champ2 { get; set; }

        [Required]
        [Display(Name = "Championship 3rd")]
        [LeaguePositionAttribute(Position = 3, League = "Championship")]
        public string Champ3 { get; set; }

        [Required]
        [Display(Name = "Championship 4th")]
        [LeaguePositionAttribute(Position = 4, League = "Championship")]
        public string Champ4 { get; set; }

        [Required]
        [Display(Name = "Championship 5th")]
        [LeaguePositionAttribute(Position = 5, League = "Championship")]
        public string Champ5 { get; set; }

        [Required]
        [Display(Name = "Championship 6th")]
        [LeaguePositionAttribute(Position = 6, League = "Championship")]
        public string Champ6 { get; set; }

        [Required]
        [Display(Name = "Championship 22nd")]
        [LeaguePositionAttribute(Position = 22, League = "Championship")]
        public string Champ22 { get; set; }

        [Required]
        [Display(Name = "Championship 23rd")]
        [LeaguePositionAttribute(Position = 23, League = "Championship")]
        public string Champ23 { get; set; }

        [Required]
        [Display(Name = "Championship 24th")]
        [LeaguePositionAttribute(Position = 24, League = "Championship")]
        public string Champ24 { get; set; }
    }

    public class LeaguePositionAttribute : Attribute
    {
        public int Position { get; set; }
        public string League { get; set; }

    }

    public class AttributeHelper
    {
        public static string[] GetPropertyNameByCustomAttribute
          <ClassToAnalyse, AttributeTypeToFind>
          (
           Func<AttributeTypeToFind, bool> attributePredicate
          )
          where AttributeTypeToFind : Attribute
        {
            if (attributePredicate == null)
            {
                throw new ArgumentNullException("attributePredicate");
            }
            else
            {
                List<string> propertyNames = new List<string>();

                foreach
                (
                  PropertyInfo propertyInfo in typeof(ClassToAnalyse).GetProperties()
                )
                {
                    if
                    (
                      propertyInfo.GetCustomAttributes
                      (
                        typeof(AttributeTypeToFind), true
                      ).Any
                      (
                        currentAttribute =>
                        attributePredicate((AttributeTypeToFind)currentAttribute)
                      )
                    )
                    {
                        propertyNames.Add(propertyInfo.Name);
                    }
                }

                return propertyNames.ToArray();
            }
        }

        public static AttributeTypeToFind GetAttributes<ClassToAnalyse, AttributeTypeToFind>(string propertyName)
            where AttributeTypeToFind : Attribute
        {
            var properties = typeof(ClassToAnalyse).GetProperties();
            var propertyInfo = properties.Where(p => p.Name.Equals(propertyName)).Single();
            var attribute = propertyInfo.GetCustomAttribute<AttributeTypeToFind>();
            return attribute;
        }

        public static string[] GetPropertiesWithAttribute<ClassToAnalyse, AttributeTypeToFind>()
            where AttributeTypeToFind : Attribute
        {
            var properties = typeof(ClassToAnalyse).GetProperties();
            var propertiesWithAttribute = properties.Where(p =>
            
                p.CustomAttributes.Where(attr => attr.AttributeType == typeof(AttributeTypeToFind)).Any()

                ).Select(p => p.Name).ToArray();
            return propertiesWithAttribute;
        }

        public static string GetPropertyValue<ClassToAnalyse>(object ob, string propertyName)
        {
            var properties = typeof(ClassToAnalyse).GetProperties();
            var property = properties.Where(p => p.Name.Equals(propertyName)).Single();
            var value = property.GetValue(ob);
            return value as string;
        }
    }
}