using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Hadouken.Plugins.Events.Http.Models;
using Hadouken.Plugins.Events.Hubs;

namespace Hadouken.Plugins.Events.Http.Controllers
{
    public class EventsController : ApiController
    {
        private readonly IEventHub _eventHub;

        public EventsController(IEventHub eventHub)
        {
            _eventHub = eventHub;
        }

        public HttpResponseMessage Post(PostEventDto dto)
        {
            if (dto == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            if (String.IsNullOrEmpty(dto.Name) || dto.Data == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            _eventHub.Publish(dto.Name, dto.Data);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}
