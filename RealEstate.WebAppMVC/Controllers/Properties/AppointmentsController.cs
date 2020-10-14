using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Ninject;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.WebAppMVC.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppointmentServices appointmentsManager;

        [Inject]
        public AppointmentsController(AppointmentServices appointmentsManager)
        {
            this.appointmentsManager = appointmentsManager;
        }

        //Used for users to see the events of the Agent
        //
        //GET: /Appointments/GetAppointments
        [HttpGet]
        public async Task<ActionResult> GetAppointments(string agentId, DateTime? start = null, DateTime? end = null)
        {
            var appointments = (await appointmentsManager.GetAgentAppointmentsForClient(agentId, start, end))
                .Select(a => new
                {
                    id = a.Id,
                    start = a.StartDateInMilliseconds,
                    end = a.EndDateInMilliseconds,
                    title = a.Title
                })
                .ToList();

            return Json(appointments, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "Agent,Client,Admin")]
        public async Task<ActionResult> GetOwnAppointments(DateTime? from = null, DateTime? to = null)
        {
            if (User.IsInRole("Agent"))
            {
                var agentId = User.Identity.GetUserId();
                var appointments = await appointmentsManager.GetAgentOwnAppointments(agentId, from, to);
                return View("GetAgentAppointments", appointments);
            }

            if (User.IsInRole("Client"))
            {
                var clientId = User.Identity.GetUserId();
                var appointments = await appointmentsManager.GetClientAppointments(clientId);
                return View("GetClientAppointments", appointments);
            }

            if (User.IsInRole("Admin"))
            {
                var appointments = await appointmentsManager.GetAgentOwnAppointments(null);
                return View("GetAgentAppointments", appointments);
            }

            return HttpNotFound();
        }

       

        /// <summary>
        /// used in /Appointments/GetOwnAppointments
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Agent,Admininstrator")]
        public async Task<ActionResult> ApproveAppointment(int appointmentId)
        {
            await appointmentsManager.ApproveAppointment(appointmentId, User.Identity.GetUserId());

            return Json("STATUS_OK");
        }

        /// <summary>
        /// used in /Appointments/GetOwnAppointments
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <param name="newDate"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Agent")]
        public async Task<ActionResult> ChangeDate(int? appointmentId, DateTime? newDate)
        {
            if (appointmentId == null || newDate == null)
            {
                return HttpNotFound();
            }

            if (newDate < DateTime.Now)
            {
                ModelState.AddModelError("newDate", "Въведете дата след сегашният момент!");
                return Json(ModelState.ToDictionary());
            }

            await appointmentsManager.ChangeAppointmentDate((int)appointmentId, User.Identity.GetUserId(), (DateTime)newDate);

            return Json("STATUS_OK");
        }

        //
        //POST: /Appointments/Create
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AppointmentCreateViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                ModelState.Remove(nameof(model.ClientName));
                ModelState.Remove(nameof(model.ClientPhoneNumber));
                ModelState.Remove(nameof(model.ClientEmail));
            }

            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            int appointmentId;
            if (User.Identity.IsAuthenticated)
            {
                appointmentId = await appointmentsManager.Create(new AppointmentCreateDTO(model), User.Identity.GetUserId());
            }
            else
            {
                appointmentId = await appointmentsManager.Create(new AppointmentCreateDTO(model), new NonRegisteredUserCreateDTO(model));
            }

            return Json(appointmentId);
        }


        //
        //POST: /Appointments/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {           
            await appointmentsManager.Delete(id, User.Identity.GetUserId());

            return Json("STATUS_OK");
        }
    }
}