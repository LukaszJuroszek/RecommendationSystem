﻿using System;
using System.Collections.Generic;
using System.Linq;
using WinRecomendationSystem.DAL;
using WinRecomendationSystem.DAL.Entities;
using WinRecomendationSystem.Entities;
using WinRecomendationSystem.Enums;
using WinRecomendationSystem.Model;
using WinRecomendationSystem.RecommendationEngine;

namespace WinRecomendationSystem.ViewModel
{
    public class MainViewModel
    {
        private IUnitOfWork _unitOfWork;
        public IEnumerable<TicketEvent> TicketEvents { get; set; }
        public IEnumerable<User> Users { get; set; }
        public MainViewModel()
        {
            _unitOfWork = new UnitOfWork();
            TicketEvents = _unitOfWork.TicketEventRepository.All().ToList();
            Users = _unitOfWork.UserRepository.All();
        }
        public void AddOpinion(OpinionViewModel model)
        {
            var exsOpinion = _unitOfWork.OpinionRepository.Filter(x => x.TicketEvents.Id == model.TicketEvents.Id).ToList();
            if (exsOpinion.Count() != 0)//if opinion exist
                exsOpinion.First().EventOpinion = model.EventOpinion;
            else
                _unitOfWork.OpinionRepository.Add(new Opinion
                {
                    EventOpinion = model.EventOpinion,
                    TicketEvents = model.TicketEvents,
                    User = model.User
                });
            _unitOfWork.Commit();
        }
        public void AddClikedEvent(ShowClickedTicketViewModel model)
        {
            var exsClickedEvent = _unitOfWork.ClikedEventRepository.Filter(x => x.TicketEvent.Id == model.TicketEvent.Id);
            if (exsClickedEvent.Count() != 0)
                exsClickedEvent.First().ViewedTicketEventDates.Add(new ClikedEventDate { WhenClicked = model.WhenClicked });
            else
                _unitOfWork.ClikedEventRepository.Add(new ClikedEvent
                {
                    TicketEvent = model.TicketEvent,
                    User = model.User,
                    ViewedTicketEventDates = new List<ClikedEventDate>() { new ClikedEventDate { WhenClicked = model.WhenClicked } }
                });
            _unitOfWork.Commit();
        }
        public TicketEvent GetTicketEventById(int id)
        {
            return _unitOfWork.TicketEventRepository.Filter(x => x.Id == id).First();
        }
        public IEnumerable<TicketEvent> GetRemomendedTicketEvents(int count)
        {
            var usrRec = new UserRecommendation(new RecommendationProfile(Users.First()));
            var listOfTicketEvent = usrRec.GetEventsCategoriesBasedOnRecommendCategories(count);
            var result = new List<TicketEvent>();

            for (int i = 0; i < listOfTicketEvent.Count; i++)
            {
                var toAdd = GetTicketEventByEventCategorFromListByI(listOfTicketEvent, i);
                if (!result.Contains(toAdd))
                    if (TicketEvents.Where(x => x.EventCategory == listOfTicketEvent[i]).Count() >= count)
                        do
                        {
                            toAdd = GetTicketEventByEventCategorFromListByI(listOfTicketEvent, i);
                        } while (result.Contains(toAdd));
                if (!result.Contains(toAdd))
                    result.Add(toAdd);
            }
            return result;
        }
        private TicketEvent GetTicketEventByEventCategorFromListByI(IList<EventCategory> listOfTicketEvent, int i)
        {
            return TicketEvents.Where(x => x.EventCategory == listOfTicketEvent[i]).OrderBy(x => Guid.NewGuid()).First();
        }
    }
}

