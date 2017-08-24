using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ngUICalender.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            //pega o DB e percorre ela (data context) 
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var v = dc.Events.OrderBy(a => a.StartAt).ToList();
                return new JsonResult { Data = v, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        //Salva o evento
        [HttpPost]
        public JsonResult SaveEvent(Event evt)
        {
            bool status = false;
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                if (evt.EndAt != null && evt.StartAt.TimeOfDay == new TimeSpan(0,0,0) && 
                    evt.EndAt.Value.TimeOfDay == new TimeSpan(0,0,0))
                {
                    evt.IsFullDay = true;
                }
                else
                {
                    evt.IsFullDay = false;
                }

                if (evt.EventID > 0)
                {
                    var v = dc.Events.Where(a => a.EventID.Equals(evt.EventID)).FirstOrDefault();
                    if (v != null)
                    {
                        v.Title = evt.Title;
                        v.Description = evt.Description;
                        v.StartAt = evt.StartAt;
                        v.EndAt = evt.EndAt;
                        v.IsFullDay = evt.IsFullDay;
                        //v.User = evt.User;  
                    }
                }
                else
                {
                    dc.Events.Add(evt);
                }
                dc.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status} };
        }

        // deleta o evento
        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            bool status = false;
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var v = dc.Events.Where(a => a.EventID.Equals(eventID)).FirstOrDefault();
                if (v != null)
                {
                    dc.Events.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
	}
}