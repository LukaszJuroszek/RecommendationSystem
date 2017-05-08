﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinRecomendationSystem.DAL;
using WinRecomendationSystem.Entities;

namespace WinRecomendationSystem.RecommendationEngine
{
    public class WatchedEventStorage
    {
        public User User { get; set; }
        public TicketEvent TicketEvent { get; set; }
        public Dictionary<DateTime, int> WatchedTicketEventCountsPerDay { get; set; }
        public WatchedEventStorage(User user, TicketEvent ticketEvent, IUnitOfWork uow)
        {
            User = user;
            TicketEvent = ticketEvent;
            WatchedTicketEventCountsPerDay = GetClickedEventsFromDate(DateTime.Now.Add(TimeSpan.FromDays(-7)), uow);
        }
        public int SumAllClickedTicketEvents()
        {
            return WatchedTicketEventCountsPerDay.Values.Sum();
        }
        private Dictionary<DateTime, int> GetClickedEventsFromDate(DateTime fromDate, IUnitOfWork uow)
        {
            ClikedEvent clickedEvent = GetUserClickedEvents(uow);
            return clickedEvent.ViewedTicketEventDates
                .Where(day => day.WhenClicked > fromDate)
                .GroupBy(x => x.WhenClicked.Date)
                .ToDictionary(days => days.Key, clicked => clicked.Count());
        }
        private ClikedEvent GetUserClickedEvents(IUnitOfWork uow)
        {
            return uow.ClikedEventRepository
                            .Filter(ev => ev.TicketEvent.Id == TicketEvent.Id && ev.User.Id == User.Id)
                            .First();
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"User Name: {User.Name} ");
            sb.AppendLine($"Ticket Title:{TicketEvent.Title}, Category: {TicketEvent.EventCategory}");
            sb.AppendLine($"Watched Times:{SumAllClickedTicketEvents()}");
            foreach (KeyValuePair<DateTime, int> item in WatchedTicketEventCountsPerDay)
            {
                sb.AppendLine($"Watched in {item.Key.ToShortDateString()} {item.Value} Times");
            }
            return sb.ToString();
        }
    }
}