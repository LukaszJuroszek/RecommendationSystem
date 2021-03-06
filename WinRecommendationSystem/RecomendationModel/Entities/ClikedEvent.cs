﻿using System.Collections.Generic;
using RecomendationModel.DAL.Entities;

namespace RecomendationModel.Entities
{
    public class ClikedEvent 
    {
        public int Id { get; set; }
        public virtual TicketEvent TicketEvent { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<ClikedEventDate> ViewedTicketEventDates { get; set; }
        public ClikedEvent()
        {
            ViewedTicketEventDates = new HashSet<ClikedEventDate>();
        }
    }
}
