﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinRecomendationSystem.DAL;
using WinRecomendationSystem.Entities;
using WinRecomendationSystem.Enums;

namespace WinRecomendationSystem.RecommendationEngine
{
    public class RecommendationProfile
    {
        public User User { get; set; }
        public OpinionsStorage Opinions { get; private set; }
        public List<WatchedEventStorage> WatchedEvents { get; private set; }
        private IUnitOfWork unitOfWork;
        public RecommendationProfile()
        {
            unitOfWork = new UnitOfWork();
            User = unitOfWork.UserRepository.All().First();
            SetOpinions();
            SetWatchedEvents();
        }
        public void SetOpinions()
        {
            var op = new Dictionary<TicketEvent, EventOpinion>();
            var userOpinions = unitOfWork.OpinionRepository
                .All()
                .Where(x => x.User.Id == User.Id)
                .ToList();
            foreach (var opinion in userOpinions)
            {
                op.Add(opinion.TicketEvents, opinion.EventOpinion);
            }
            Opinions = new OpinionsStorage
            {
                User = User,
                EventOpinions = op
            };
        }
        public void SetWatchedEvents()
        {
            WatchedEvents = new List<WatchedEventStorage>();
            var userEventis = unitOfWork.ClikedEventRepository
                .Filter(x => x.User.Id == User.Id)
                .GroupBy(x => x.TicketEvent)
                .Select(x => x.Key)
                .ToList();
            foreach (var ticketEvent in userEventis)
            {
                WatchedEvents.Add(new WatchedEventStorage(User, ticketEvent, unitOfWork));
            }
        }
        public double GetPercentTicketEventsParticipationFromAllTicketEvents(TicketEvent te)
        {
            if (WatchedEvents.FindIndex(item => item.TicketEvent.Id == te.Id) != 0)
            {
                var teViewsCount = WatchedEvents
                    .Where(x => x.TicketEvent.Id == te.Id)
                    .First().SumAllClickedTicketEvents();
                var allViews = WatchedEvents.Sum(x => x.SumAllClickedTicketEvents());
                return ((double)teViewsCount / allViews) * 100;
            }
            return 0.0;
        }
        public string AnalysisToString()
        {
            var sb = new StringBuilder();
            sb.Append(Opinions.ToString());
            foreach (var item in WatchedEvents)
            {
                sb.Append(item.ToString());
            }
            return sb.ToString();
        }
    }
}
