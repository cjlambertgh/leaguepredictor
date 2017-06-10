using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeaguePredictor.Models;
using Microsoft.AspNet.Identity;

namespace LeaguePredictor.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index()
        {
            var game = new GameViewModels();

            return View(game.LeagueTable);
        }

        // GET: Player Selections
        public ActionResult PlayerSelections()
        {
            return View();
        }

        // GET: TeamSelections
        [Authorize]
        public ActionResult TeamSelections()
        {
            var model = new TeamSelectionsViewModel();
            var email = User.Identity.GetUserName();
            using (var db = new leaguepredictorEntities())
            {
                var player = db.players.Where(p => p.email.Equals(email)).SingleOrDefault();
                if(player != null)
                {
                    var type = model.GetType();
                    var selections = player.players_selections.Select(ps => 
                    new {
                        Team = ps.team.team_name,
                        Position = ps.position,
                        League = ps.team.league.league_name
                    }).ToList();
                    var name = player.name;
                    model.Name = name;
                    selections.ForEach(sel =>
                    {
                        var property = AttributeHelper.GetPropertyNameByCustomAttribute<TeamSelectionsViewModel, LeaguePositionAttribute>(
                            attr => (attr.League.Equals(sel.League) && attr.Position == sel.Position)
                            ).First();

                        
                        var prop = type.GetProperty(property);
                        prop.SetValue(model, sel.Team, null);
                    });
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult TeamSelections(TeamSelectionsViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //get league positions
            var selections = new List<LeaguePosition>();
            var properties = AttributeHelper.GetPropertiesWithAttribute<TeamSelectionsViewModel, LeaguePositionAttribute>();
            foreach(var prop in properties)
            {
                var lp = new LeaguePosition();
                var attr = AttributeHelper.GetAttributes<TeamSelectionsViewModel, LeaguePositionAttribute>(prop);
                var league = attr.League;
                lp.Team = AttributeHelper.GetPropertyValue<TeamSelectionsViewModel>(model, prop);
                lp.Position = attr.Position;
                selections.Add(lp);
            }

            var email = User.Identity.GetUserName();
            using (var db = new leaguepredictorEntities())
            {
                var player = db.players.Where(p => p.email.Equals(email)).SingleOrDefault();
                if (player == null)
                {
                    player = new player { email = email, name = model.Name };
                    db.players.Add(player);
                    db.SaveChanges();
                }
                else
                {
                    //remove previous selections
                    foreach (var ps in player.players_selections.ToList())
                    {
                        db.players_selections.Remove(ps);
                    }
                }

                foreach(var sel in selections)
                {
                    var team = db.teams.Where(t => t.team_name == sel.Team).Single();
                    player.players_selections.Add(new players_selections
                    {
                        team = team,
                        position = sel.Position
                    });
                }

                db.SaveChanges();
            }

            return View(model);
           
        }
    }
}