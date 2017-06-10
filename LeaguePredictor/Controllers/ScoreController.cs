using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeaguePredictor.Models;

namespace LeaguePredictor.Controllers
{
    public class ScoreController : Controller
    {
        // GET: Score
        public ActionResult Index()
        {
            var model = new GameViewModels();
            
            return View(model.LeagueTable);
        }

        public ActionResult PlayerScores(int? ID)
        {
            if (ID == null)
            {
                return HttpNotFound();
            }

            var model = new GameViewModels();
            var psvm = model.GetPlayerScoresViewModel((int)ID);
            if(psvm == null)
            {
                return HttpNotFound();
            }

            return View(psvm);
        }
    }
}