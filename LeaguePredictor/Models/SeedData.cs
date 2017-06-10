using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaguePredictor.Models
{
    public static class SeedData
    {
        public static void Initialize()
        {
            using (var context = new leaguepredictorEntities())
            {
                context.players.AddRange(new player[] 
                {
                    new player { name = "chris" },
                    new player { name = "john" }
                });

                context.leagues.AddRange(new league[]
                {
                    new league { league_name = "Premier League" },
                    new league { league_name = "Championship" }
                });

                context.SaveChanges();

                context.teams.AddRange(new team[]
                {
                    new team { team_name = "Man Utd", league = context.leagues.Single(l => l.league_name.Equals("Premier League")) },
                    new team { team_name = "Arsenal", league = context.leagues.Single(l => l.league_name.Equals("Championship")) },
                    new team { team_name = "Chelsea", league = context.leagues.Single(l => l.league_name.Equals("Premier League")) },
                    new team { team_name = "Liverpool", league = context.leagues.Single(l => l.league_name.Equals("Premier League")) }
                });

                context.SaveChanges();

                context.team_rankings.AddRange(new team_rankings[]
                {
                    new team_rankings { team = context.teams.Single(t => t.team_name.Equals("Man Utd")), position = 1 },
                    new team_rankings { team = context.teams.Single(t => t.team_name.Equals("Arsenal")), position = 15 },
                    new team_rankings { team = context.teams.Single(t => t.team_name.Equals("Chelsea")), position = 8 },
                    new team_rankings { team = context.teams.Single(t => t.team_name.Equals("Liverpool")), position = 4 }
                });

                context.SaveChanges();

                context.players_selections.AddRange(new players_selections[]
                {
                    new players_selections { player = context.players.Single(p => p.name.Equals("chris")), team = context.teams.Single(t => t.team_name.Equals("Man Utd")), position = 4 },
                    new players_selections { player = context.players.Single(p => p.name.Equals("chris")), team = context.teams.Single(t => t.team_name.Equals("Arsenal")), position = 1 },
                    new players_selections { player = context.players.Single(p => p.name.Equals("chris")), team = context.teams.Single(t => t.team_name.Equals("Chelsea")), position = 2 },
                    new players_selections { player = context.players.Single(p => p.name.Equals("chris")), team = context.teams.Single(t => t.team_name.Equals("Liverpool")), position = 3 },
                    new players_selections { player = context.players.Single(p => p.name.Equals("john")), team = context.teams.Single(t => t.team_name.Equals("Man Utd")), position = 3 },
                    new players_selections { player = context.players.Single(p => p.name.Equals("john")), team = context.teams.Single(t => t.team_name.Equals("Arsenal")), position = 4 },
                    new players_selections { player = context.players.Single(p => p.name.Equals("john")), team = context.teams.Single(t => t.team_name.Equals("Chelsea")), position = 1 },
                    new players_selections { player = context.players.Single(p => p.name.Equals("john")), team = context.teams.Single(t => t.team_name.Equals("Liverpool")), position = 2 }
                });

                context.SaveChanges();


            }
        }
    }
}